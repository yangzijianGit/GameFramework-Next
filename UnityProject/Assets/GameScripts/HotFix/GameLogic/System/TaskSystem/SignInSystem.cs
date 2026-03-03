using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 签到系统数据类型
    //===========================================================

    /// <summary>
    /// 签到奖励配置
    /// </summary>
    [Serializable]
    public class SignInRewardConfig
    {
        /// <summary>天数(1-7)</summary>
        public int Day;
        
        /// <summary>奖励类型</summary>
        public RewardType RewardType;
        
        /// <summary>奖励ID</summary>
        public string RewardId;
        
        /// <summary>奖励数量</summary>
        public int RewardCount;
        
        /// <summary>是否为累积奖励(最后一天)</summary>
        public bool IsCumulative;
    }

    /// <summary>
    /// 签到周期数据
    /// </summary>
    [Serializable]
    public class SignInCycleData
    {
        /// <summary>周期ID</summary>
        public string CycleId;
        
        /// <summary>开始时间戳</summary>
        public long StartTime;
        
        /// <summary>结束时间戳</summary>
        public long EndTime;
        
        /// <summary>签到记录(天数->是否已签到)</summary>
        public Dictionary<int, bool> SignInRecords = new Dictionary<int, bool>();
        
        /// <summary>累积签到天数</summary>
        public int CumulativeDays;
        
        /// <summary>是否已领取累积奖励</summary>
        public bool HasClaimedCumulativeReward;
    }

    //===========================================================
    // 签到系统接口
    //===========================================================

    /// <summary>
    /// 签到系统接口
    /// </summary>
    public interface ISignInSystem
    {
        /// <summary>
        /// 签到
        /// </summary>

        /// <summary>
        /// 初始化系统
        /// </summary>
        void Initialize();

        /// <summary>
        /// 签到
        /// </summary>
        /// <returns>签到结果</returns>
        SignInResult SignIn();

        /// <summary>
        /// 领取累积奖励
        /// </summary>
        /// <returns>是否成功</returns>
        bool ClaimCumulativeReward();

        /// <summary>
        /// 今日是否已签到
        /// </summary>
        /// <returns>是否已签到</returns>
        bool IsSignedInToday();

        /// <summary>
        /// 本月签到天数
        /// </summary>
        /// <returns>签到天数</returns>
        int GetCurrentMonthSignInDays();

        /// <summary>
        /// 本月累积签到天数
        /// </summary>
        /// <returns>累积天数</returns>
        int GetCumulativeDays();

        /// <summary>
        /// 获取签到奖励列表
        /// </summary>
        /// <returns>奖励列表</returns>
        List<SignInRewardConfig> GetRewardList();

        /// <summary>
        /// 获取签到状态
        /// </summary>
        /// <returns>签到状态</returns>
        SignInStatus GetSignInStatus();

        /// <summary>
        /// 保存数据
        /// </summary>
        void SaveData();

        /// <summary>
        /// 加载数据
        /// </summary>
        void LoadData();
    }

    /// <summary>
    /// 签到结果
    /// </summary>
    public class SignInResult
    {
        /// <summary>是否成功</summary>
        public bool Success;
        
        /// <summary>错误信息</summary>
        public string ErrorMessage;
        
        /// <summary>获得的奖励</summary>
        public SignInRewardConfig Reward;
        
        /// <summary>签到后的累积天数</summary>
        public int CumulativeDays;
        
        /// <summary>本月签到总天数</summary>
        public int TotalSignInDays;

        public bool CanClaimCumulativeReward { get; internal set; }

    }

    /// <summary>
    /// 签到状态
    /// </summary>
    public class SignInStatus
    {
        /// <summary>今日是否已签到</summary>
        public bool IsSignedInToday;
        
        /// <summary>本月签到天数</summary>
        public int MonthSignInDays;
        
        /// <summary>累积签到天数</summary>
        public int CumulativeDays;
        
        /// <summary>是否可领取累积奖励</summary>
        public bool CanClaimCumulativeReward;
        
        /// <summary>本月剩余签到天数</summary>
        public int RemainingDays;
    }

    //===========================================================
    // 签到系统实现
    //===========================================================

    /// <summary>
    /// 签到系统
    /// 职责：
    /// 1. 管理7天签到周期
    /// 2. 签到状态追踪
    /// 3. 累积奖励发放
    /// 4. 断签检测和处理
    /// 5. 补签功能(可选)
    /// 
    /// 扩展点：
    /// 1. 可配置签到周期长度
    /// 2. 可扩展补签规则
    /// 3. 可支持多周期签到
    /// </summary>
    public class SignInSystem : BaseLogicSys<SignInSystem>, ISignInSystem
    {
        //========================== 常量定义 ==========================

        /// <summary>数据存储Key</summary>
        private const string DATA_KEY = "SignInData";

        /// <summary>签到周期天数</summary>
        private const int CYCLE_DAYS = 7;

        /// <summary>补签卡道具ID</summary>
        private const string REPAIR_SIGN_ITEM_ID = "item_repair_sign";

        /// <summary>断签判定时间(小时)</summary>
        private const int BREAK_HOURS = 48;

        //========================== 私有变量 ==========================

        /// <summary>签到奖励配置列表</summary>
        private List<SignInRewardConfig> m_rewardConfigs = new List<SignInRewardConfig>();

        /// <summary>当前签到周期数据</summary>
        private SignInCycleData m_currentCycle;

        /// <summary>是否已初始化</summary>
        private bool m_isInitialized = false;

        //========================== 生命周期 ==========================

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            OnInit();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override bool OnInit()
        {
            base.OnInit();

            // 加载奖励配置
            LoadRewardConfigs();

            // 加载玩家数据
            LoadData();

            // 检查是否需要创建新周期
            CheckAndCreateNewCycle();

            m_isInitialized = true;
            Debug.Log($"[SignInSystem] Initialized. Current cycle: {m_currentCycle?.CycleId}");
            return true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void OnDestroy()
        {
            SaveData();
            m_rewardConfigs.Clear();
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 签到
        /// </summary>
        public SignInResult SignIn()
        {
            var result = new SignInResult();

            // 检查初始化
            if (!m_isInitialized)
            {
                result.Success = false;
                result.ErrorMessage = "System not initialized";
                return result;
            }

            // 检查周期有效性
            if (m_currentCycle == null)
            {
                result.Success = false;
                result.ErrorMessage = "No valid cycle";
                return result;
            }

            // 检查是否已签到
            if (IsSignedInToday())
            {
                result.Success = false;
                result.ErrorMessage = "Already signed in today";
                return result;
            }

            // 获取今天是第几天
            int dayOfCycle = GetDayOfCycle();
            if (dayOfCycle < 1 || dayOfCycle > CYCLE_DAYS)
            {
                result.Success = false;
                result.ErrorMessage = "Invalid day of cycle";
                return result;
            }

            // 记录签到
            m_currentCycle.SignInRecords[dayOfCycle] = true;
            m_currentCycle.CumulativeDays = dayOfCycle;

            // 获取奖励
            var reward = GetRewardForDay(dayOfCycle);
            result.Reward = reward;
            result.CumulativeDays = m_currentCycle.CumulativeDays;
            result.TotalSignInDays = GetCurrentMonthSignInDays();

            // 发放奖励
            if (reward != null)
            {
                GrantReward(reward);
            }

            // 检查是否可以领取累积奖励
            if (m_currentCycle.CumulativeDays >= CYCLE_DAYS && !m_currentCycle.HasClaimedCumulativeReward)
            {
                result.CanClaimCumulativeReward = true;
            }

            // 保存
            SaveData();

            result.Success = true;
            Debug.Log($"[SignInSystem] Sign in success. Day: {dayOfCycle}, Reward: {reward?.RewardType}");

            return result;
        }

        /// <summary>
        /// 领取累积奖励
        /// </summary>
        public bool ClaimCumulativeReward()
        {
            if (m_currentCycle == null)
            {
                Debug.LogWarning("[SignInSystem] No valid cycle");
                return false;
            }

            if (m_currentCycle.CumulativeDays < CYCLE_DAYS)
            {
                Debug.LogWarning("[SignInSystem] Not enough cumulative days");
                return false;
            }

            if (m_currentCycle.HasClaimedCumulativeReward)
            {
                Debug.LogWarning("[SignInSystem] Cumulative reward already claimed");
                return false;
            }

            // 获取累积奖励(最后一天的奖励)
            var cumulativeReward = GetRewardForDay(CYCLE_DAYS);
            if (cumulativeReward == null)
            {
                Debug.LogWarning("[SignInSystem] No cumulative reward config");
                return false;
            }

            // 发放奖励
            GrantReward(cumulativeReward);

            // 标记已领取
            m_currentCycle.HasClaimedCumulativeReward = true;

            // 保存
            SaveData();

            Debug.Log("[SignInSystem] Cumulative reward claimed");
            return true;
        }

        /// <summary>
        /// 今日是否已签到
        /// </summary>
        public bool IsSignedInToday()
        {
            if (m_currentCycle == null) return false;

            int dayOfCycle = GetDayOfCycle();
            return m_currentCycle.SignInRecords.ContainsKey(dayOfCycle) && 
                   m_currentCycle.SignInRecords[dayOfCycle];
        }

        /// <summary>
        /// 本月签到天数
        /// </summary>
        public int GetCurrentMonthSignInDays()
        {
            if (m_currentCycle == null) return 0;

            int count = 0;
            foreach (var record in m_currentCycle.SignInRecords)
            {
                if (record.Value) count++;
            }
            return count;
        }

        /// <summary>
        /// 累积签到天数
        /// </summary>
        public int GetCumulativeDays()
        {
            return m_currentCycle?.CumulativeDays ?? 0;
        }

        /// <summary>
        /// 获取签到奖励列表
        /// </summary>
        public List<SignInRewardConfig> GetRewardList()
        {
            return m_rewardConfigs;
        }

        /// <summary>
        /// 获取签到状态
        /// </summary>
        public SignInStatus GetSignInStatus()
        {
            var status = new SignInStatus
            {
                IsSignedInToday = IsSignedInToday(),
                MonthSignInDays = GetCurrentMonthSignInDays(),
                CumulativeDays = GetCumulativeDays(),
                RemainingDays = CYCLE_DAYS - GetCurrentMonthSignInDays()
            };

            // 检查是否可以领取累积奖励
            if (m_currentCycle != null && 
                m_currentCycle.CumulativeDays >= CYCLE_DAYS && 
                !m_currentCycle.HasClaimedCumulativeReward)
            {
                status.CanClaimCumulativeReward = true;
            }

            return status;
        }

        //========================== 数据持久化 ==========================

        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveData()
        {
            if (m_currentCycle == null) return;

            var json = JsonUtility.ToJson(m_currentCycle);
            PlayerPrefs.SetString(DATA_KEY, json);
            Debug.Log("[SignInSystem] Data saved");
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public void LoadData()
        {
            if (!PlayerPrefs.HasKey(DATA_KEY))
            {
                CreateNewCycle();
                return;
            }

            try
            {
                var json = PlayerPrefs.GetString(DATA_KEY);
                m_currentCycle = JsonUtility.FromJson<SignInCycleData>(json);

                if (m_currentCycle == null)
                {
                    CreateNewCycle();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SignInSystem] Failed to load data: {e.Message}");
                CreateNewCycle();
            }
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 加载奖励配置
        /// </summary>
        private void LoadRewardConfigs()
        {
            m_rewardConfigs.Clear();

            // 创建7天签到奖励配置
            m_rewardConfigs.Add(new SignInRewardConfig
            {
                Day = 1,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 10,
                IsCumulative = false
            });

            m_rewardConfigs.Add(new SignInRewardConfig
            {
                Day = 2,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 15,
                IsCumulative = false
            });

            m_rewardConfigs.Add(new SignInRewardConfig
            {
                Day = 3,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 20,
                IsCumulative = false
            });

            m_rewardConfigs.Add(new SignInRewardConfig
            {
                Day = 4,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 25,
                IsCumulative = false
            });

            m_rewardConfigs.Add(new SignInRewardConfig
            {
                Day = 5,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 30,
                IsCumulative = false
            });

            m_rewardConfigs.Add(new SignInRewardConfig
            {
                Day = 6,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 50,
                IsCumulative = false
            });

            // 第7天为累积奖励
            m_rewardConfigs.Add(new SignInRewardConfig
            {
                Day = 7,
                RewardType = RewardType.Clothes,
                RewardId = "clothes_001",
                RewardCount = 1,
                IsCumulative = true
            });
        }

        /// <summary>
        /// 创建新周期
        /// </summary>
        private void CreateNewCycle()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            m_currentCycle = new SignInCycleData
            {
                CycleId = $"{now.Year}_{now.Month}",
                StartTime = TimeUtil.ToUnixTimeMillis(startOfMonth),
                EndTime = TimeUtil.ToUnixTimeMillis(endOfMonth),
                SignInRecords = new Dictionary<int, bool>(),
                CumulativeDays = 0,
                HasClaimedCumulativeReward = false
            };

            Debug.Log($"[SignInSystem] New cycle created: {m_currentCycle.CycleId}");
        }

        /// <summary>
        /// 检查并创建新周期
        /// </summary>
        private void CheckAndCreateNewCycle()
        {
            if (m_currentCycle == null)
            {
                CreateNewCycle();
                return;
            }

            var now = DateTime.Now;
            var currentCycleId = $"{now.Year}_{now.Month}";

            // 如果到了新月份，创建新周期
            if (m_currentCycle.CycleId != currentCycleId)
            {
                // 保存旧周期数据(可选)
                CreateNewCycle();
            }
        }

        /// <summary>
        /// 获取今天是周期的第几天
        /// </summary>
        private int GetDayOfCycle()
        {
            if (m_currentCycle == null) return -1;

            var now = DateTime.Now;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            return now.Day;
        }

        /// <summary>
        /// 获取指定天数的奖励
        /// </summary>
        private SignInRewardConfig GetRewardForDay(int day)
        {
            foreach (var config in m_rewardConfigs)
            {
                if (config.Day == day)
                    return config;
            }
            return null;
        }

        /// <summary>
        /// 发放奖励
        /// </summary>
        private void GrantReward(SignInRewardConfig reward)
        {
            if (reward == null) return;

            switch (reward.RewardType)
            {
                case RewardType.Diamond:
                    Debug.Log($"[SignInSystem] Grant Diamond: {reward.RewardCount}");
                    break;
                case RewardType.Star:
                    Debug.Log($"[SignInSystem] Grant Star: {reward.RewardCount}");
                    break;
                case RewardType.Energy:
                    Debug.Log($"[SignInSystem] Grant Energy: {reward.RewardCount}");
                    break;
                case RewardType.Item:
                    Debug.Log($"[SignInSystem] Grant Item: {reward.RewardId} x {reward.RewardCount}");
                    break;
                case RewardType.Clothes:
                    Debug.Log($"[SignInSystem] Grant Clothes: {reward.RewardId}");
                    break;
                case RewardType.Skin:
                    Debug.Log($"[SignInSystem] Grant Skin: {reward.RewardId}");
                    break;
            }
        }

        /// <summary>
        /// 检查是否断签
        /// </summary>
        private bool IsBreakSign()
        {
            if (m_currentCycle == null || m_currentCycle.SignInRecords.Count == 0)
                return false;

            // 找出最后一次签到时间
            long lastSignTime = 0;
            foreach (var record in m_currentCycle.SignInRecords)
            {
                if (record.Value)
                {
                    // 这里简化处理，实际应该记录每次签到的时间
                    lastSignTime = m_currentCycle.StartTime;
                }
            }

            if (lastSignTime == 0) return false;

            var now = TimeUtil.GetCurrentTimeMillis();
            var hoursSinceLastSign = (now - lastSignTime) / (1000 * 60 * 60);

            return hoursSinceLastSign > BREAK_HOURS;
        }

        /// <summary>
        /// 补签
        /// </summary>
        /// <param name="useItem">是否使用补签卡</param>
        /// <returns>补签结果</returns>
        public SignInResult RepairSignIn(bool useItem)
        {
            var result = new SignInResult();

            // 检查是否有补签卡
            if (useItem)
            {
                // 检查道具数量
                // var itemCount = PlayerData.Instance.GetItemCount(REPAIR_SIGN_ITEM_ID);
                // if (itemCount <= 0)
                // {
                //     result.Success = false;
                //     result.ErrorMessage = "No repair sign item";
                //     return result;
                // }
                
                // 消耗补签卡
                // PlayerData.Instance.UseItem(REPAIR_SIGN_ITEM_ID, 1);
            }

            // 执行补签(这里简化处理，实际需要记录每天是否签到)
            // 这里仅模拟补签逻辑
            result.Success = true;
            Debug.Log("[SignInSystem] Repair sign in success");

            return result;
        }

        //========================== 测试接口 ==========================

        /// <summary>
        /// 测试：重置签到数据
        /// </summary>
        public void Test_ResetSignInData()
        {
            CreateNewCycle();
            SaveData();
        }

        /// <summary>
        /// 测试：模拟签到
        /// </summary>
        public void Test_SignIn(int day)
        {
            if (m_currentCycle == null) return;
            m_currentCycle.SignInRecords[day] = true;
            m_currentCycle.CumulativeDays = Mathf.Max(m_currentCycle.CumulativeDays, day);
            SaveData();
        }
    }
}
