using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 活动系统数据类型
    //===========================================================

    /// <summary>
    /// 活动类型
    /// </summary>
    public enum ActivityType
    {
        /// <summary>限时活动</summary>
        LimitedTime = 0,

        /// <summary>每日活动</summary>
        Daily = 1,

        /// <summary>周期性活动(每周/每月)</summary>
        Periodic = 2,

        /// <summary>永久活动</summary>
        Permanent = 3,
    }

    /// <summary>
    /// 活动状态
    /// </summary>
    public enum ActivityState
    {
        /// <summary>未开始</summary>
        NotStarted = 0,

        /// <summary>进行中</summary>
        Active = 1,

        /// <summary>已结束</summary>
        Ended = 2,

        /// <summary>已领奖</summary>
        Rewarded = 3,
    }

    //===========================================================
    // 活动配置与数据
    //===========================================================

    /// <summary>
    /// 活动奖励配置
    /// </summary>
    [Serializable]
    public class ActivityRewardConfig
    {
        /// <summary>奖励ID</summary>
        public string RewardId;

        /// <summary>奖励类型</summary>
        public RewardType RewardType;

        /// <summary>奖励数量</summary>
        public int RewardCount;

        /// <summary>条件值(如累计充值金额、累计登录天数)</summary>
        public int ConditionValue;
    }

    /// <summary>
    /// 活动配置
    /// </summary>
    [Serializable]
    public class ActivityConfig
    {
        /// <summary>活动ID</summary>
        public string Id;

        /// <summary>活动名称</summary>
        public string Name;

        /// <summary>活动描述</summary>
        public string Description;

        /// <summary>活动类型</summary>
        public ActivityType Type;

        /// <summary>开始时间戳</summary>
        public long StartTime;

        /// <summary>结束时间戳</summary>
        public long EndTime;

        /// <summary>奖励列表</summary>
        public List<ActivityRewardConfig> Rewards = new List<ActivityRewardConfig>();

        /// <summary>活动图标</summary>
        public string Icon;

        /// <summary>是否已隐藏</summary>
        public bool IsHidden;
    }

    /// <summary>
    /// 玩家活动数据
    /// </summary>
    [Serializable]
    public class PlayerActivityData
    {
        /// <summary>活动ID</summary>
        public string ActivityId;

        /// <summary>活动状态</summary>
        public ActivityState State;

        /// <summary>当前进度</summary>
        public int CurrentProgress;

        /// <summary>最大进度</summary>
        public int MaxProgress;

        /// <summary>已领取奖励列表</summary>
        public List<string> ClaimedRewardIds = new List<string>();

        /// <summary>参与时间</summary>
        public long JoinTime;
    }

    //===========================================================
    // 活动系统接口
    //===========================================================

    /// <summary>
    /// 活动系统接口
    /// </summary>
    public interface IActivitySystem
    {
        /// <summary>获取活动列表</summary>

        /// <summary>初始化系统</summary>
        void Initialize();

        /// <summary>获取活动列表</summary>
        /// <param name="type">活动类型</param>
        /// <returns>活动列表</returns>
        List<ActivityConfig> GetActivityList(ActivityType type);

        /// <summary>获取进行中的活动</summary>
        /// <returns>活动列表</returns>
        List<ActivityConfig> GetActiveActivities();

        /// <summary>获取活动数据</summary>
        /// <param name="activityId">活动ID</param>
        /// <returns>活动数据</returns>
        PlayerActivityData GetActivityData(string activityId);

        /// <summary>更新活动进度</summary>
        /// <param name="activityId">活动ID</param>
        /// <param name="progress">增加进度</param>
        void UpdateActivityProgress(string activityId, int progress);

        /// <summary>领取活动奖励</summary>
        /// <param name="activityId">活动ID</param>
        /// <param name="rewardId">奖励ID</param>
        /// <returns>是否成功</returns>
        bool ClaimActivityReward(string activityId, string rewardId);

        /// <summary>检查活动状态</summary>
        void CheckActivityStates();

        /// <summary>保存数据</summary>
        void SaveData();

        /// <summary>加载数据</summary>
        void LoadData();
    }

    //===========================================================
    // 活动系统实现
    //===========================================================

    /// <summary>
    /// 活动系统
    /// 职责：
    /// 1. 管理各类活动(限时/每日/周期/永久)
    /// 2. 活动状态追踪
    /// 3. 活动进度更新
    /// 4. 活动奖励发放
    /// 
    /// 扩展点：
    /// 1. 可配置活动类型
    /// 2. 支持活动模板
    /// 3. 支持多条件活动
    /// </summary>
    public class ActivitySystem : BaseLogicSys<ActivitySystem>, IActivitySystem
    {
        //========================== 常量 ==========================

        private const string DATA_KEY = "ActivityData";

        //========================== 私有变量 ==========================

        /// <summary>活动配置</summary>
        private Dictionary<string, ActivityConfig> m_activityConfigs = new Dictionary<string, ActivityConfig>();

        /// <summary>玩家活动数据</summary>
        private Dictionary<string, PlayerActivityData> m_playerActivities = new Dictionary<string, PlayerActivityData>();

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

        public override bool OnInit()
        {
            base.OnInit();

            LoadActivityConfigs();
            LoadData();
            CheckActivityStates();

            m_isInitialized = true;
            Debug.Log($"[ActivitySystem] Initialized with {m_activityConfigs.Count} activities");
            return true;
        }

        public override void OnDestroy()
        {
            SaveData();
            m_activityConfigs.Clear();
            m_playerActivities.Clear();
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 获取活动列表
        /// </summary>
        public List<ActivityConfig> GetActivityList(ActivityType type)
        {
            var result = new List<ActivityConfig>();
            foreach (var config in m_activityConfigs.Values)
            {
                if (config.Type == type)
                    result.Add(config);
            }
            return result;
        }

        /// <summary>
        /// 获取进行中的活动
        /// </summary>
        public List<ActivityConfig> GetActiveActivities()
        {
            var result = new List<ActivityConfig>();
            var now = TimeUtil.GetCurrentTimeMillis();

            foreach (var config in m_activityConfigs.Values)
            {
                if (now >= config.StartTime && now <= config.EndTime)
                {
                    result.Add(config);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取活动数据
        /// </summary>
        public PlayerActivityData GetActivityData(string activityId)
        {
            if (m_playerActivities.TryGetValue(activityId, out var data))
                return data;
            return null;
        }

        /// <summary>
        /// 更新活动进度
        /// </summary>
        public void UpdateActivityProgress(string activityId, int progress)
        {
            if (!m_isInitialized) return;

            var data = GetActivityData(activityId);
            if (data == null) return;

            var config = GetActivityConfig(activityId);
            if (config == null) return;

            // 更新进度
            data.CurrentProgress = Mathf.Min(data.CurrentProgress + progress, data.MaxProgress);

            Debug.Log($"[ActivitySystem] Activity {activityId} progress: {data.CurrentProgress}/{data.MaxProgress}");
        }

        /// <summary>
        /// 领取活动奖励
        /// </summary>
        public bool ClaimActivityReward(string activityId, string rewardId)
        {
            var data = GetActivityData(activityId);
            if (data == null)
            {
                Debug.LogWarning($"[ActivitySystem] Activity not found: {activityId}");
                return false;
            }

            var config = GetActivityConfig(activityId);
            if (config == null)
            {
                Debug.LogWarning($"[ActivitySystem] Activity config not found: {activityId}");
                return false;
            }

            // 检查是否已领取
            if (data.ClaimedRewardIds.Contains(rewardId))
            {
                Debug.LogWarning($"[ActivitySystem] Reward already claimed: {rewardId}");
                return false;
            }

            // 检查进度是否满足
            var reward = config.Rewards.Find(r => r.RewardId == rewardId);
            if (reward == null)
            {
                Debug.LogWarning($"[ActivitySystem] Reward not found: {rewardId}");
                return false;
            }

            if (data.CurrentProgress < reward.ConditionValue)
            {
                Debug.LogWarning($"[ActivitySystem] Progress not enough: {data.CurrentProgress}/{reward.ConditionValue}");
                return false;
            }

            // 发放奖励
            GrantReward(reward);

            // 记录
            data.ClaimedRewardIds.Add(rewardId);

            // 检查是否已领取所有奖励
            if (data.ClaimedRewardIds.Count >= config.Rewards.Count)
            {
                data.State = ActivityState.Rewarded;
            }

            SaveData();
            Debug.Log($"[ActivitySystem] Activity reward claimed: {activityId}, {rewardId}");
            return true;
        }

        /// <summary>
        /// 检查活动状态
        /// </summary>
        public void CheckActivityStates()
        {
            var now = TimeUtil.GetCurrentTimeMillis();

            foreach (var config in m_activityConfigs.Values)
            {
                var data = GetActivityData(config.Id);

                if (data == null)
                {
                    // 创建新活动数据
                    data = new PlayerActivityData
                    {
                        State = ActivityState.NotStarted,
                        ActivityId = config.Id,
                        MaxProgress = 100, // 默认值
                        ClaimedRewardIds = new List<string>(),
                        JoinTime = now
                    };

                    // 根据时间设置状态
                    if (now >= config.StartTime && now <= config.EndTime)
                    {
                        data.State = ActivityState.Active;
                    }
                    else if (now > config.EndTime)
                    {
                        data.State = ActivityState.Ended;
                    }

                    m_playerActivities[config.Id] = data;
                }
                else
                {
                    // 更新状态
                    if (now >= config.StartTime && now <= config.EndTime && data.State == ActivityState.NotStarted)
                    {
                        data.State = ActivityState.Active;
                    }
                    else if (now > config.EndTime && data.State == ActivityState.Active)
                    {
                        data.State = ActivityState.Ended;
                    }
                }
            }
        }

        //========================== 数据持久化 ==========================

        public void SaveData()
        {
            var wrapper = new ActivityDataWrapper
            {
                Activities = m_playerActivities
            };
            var json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(DATA_KEY, json);
            Debug.Log("[ActivitySystem] Data saved");
        }

        public void LoadData()
        {
            if (!PlayerPrefs.HasKey(DATA_KEY))
            {
                return;
            }

            try
            {
                var json = PlayerPrefs.GetString(DATA_KEY);
                var wrapper = JsonUtility.FromJson<ActivityDataWrapper>(json);
                if (wrapper != null)
                {
                    m_playerActivities = wrapper.Activities ?? new Dictionary<string, PlayerActivityData>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[ActivitySystem] Failed to load data: {e.Message}");
            }
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 加载活动配置
        /// </summary>
        private void LoadActivityConfigs()
        {
            m_activityConfigs.Clear();

            // 示例活动配置
            CreateSampleActivities();
        }

        /// <summary>
        /// 创建示例活动
        /// </summary>
        private void CreateSampleActivities()
        {
            var now = TimeUtil.GetCurrentTimeMillis();
            var day = TimeSpan.FromMilliseconds(now).Days;

            // 限时活动
            AddActivityConfig(new ActivityConfig
            {
                Id = "activity_spring",
                Name = "新春活动",
                Description = "春节期间限定活动",
                Type = ActivityType.LimitedTime,
                StartTime = now - 7 * 24 * 60 * 60 * 1000,
                EndTime = now + 7 * 24 * 60 * 60 * 1000,
                Icon = "activity_spring.png",
                IsHidden = false,
                Rewards = new List<ActivityRewardConfig>
                {
                    new ActivityRewardConfig { RewardId = "reward_1", RewardType = RewardType.Diamond, RewardCount = 50, ConditionValue = 100 },
                    new ActivityRewardConfig { RewardId = "reward_2", RewardType = RewardType.Clothes, RewardCount = 1, ConditionValue = 500 },
                }
            });

            // 每日活动
            AddActivityConfig(new ActivityConfig
            {
                Id = "activity_daily_1",
                Name = "今日任务",
                Description = "完成每日任务获得奖励",
                Type = ActivityType.Daily,
                StartTime = now,
                EndTime = now + 24 * 60 * 60 * 1000,
                Icon = "activity_daily.png",
                IsHidden = false,
                Rewards = new List<ActivityRewardConfig>
                {
                    new ActivityRewardConfig { RewardId = "daily_reward_1", RewardType = RewardType.Diamond, RewardCount = 10, ConditionValue = 10 },
                    new ActivityRewardConfig { RewardId = "daily_reward_2", RewardType = RewardType.Star, RewardCount = 5, ConditionValue = 30 },
                }
            });

            // 永久活动
            AddActivityConfig(new ActivityConfig
            {
                Id = "activity_permanent_1",
                Name = "收集达人",
                Description = "收集指定数量服装",
                Type = ActivityType.Permanent,
                StartTime = 0,
                EndTime = long.MaxValue,
                Icon = "activity_collect.png",
                IsHidden = false,
                Rewards = new List<ActivityRewardConfig>
                {
                    new ActivityRewardConfig { RewardId = "collect_10", RewardType = RewardType.Diamond, RewardCount = 100, ConditionValue = 10 },
                    new ActivityRewardConfig { RewardId = "collect_50", RewardType = RewardType.Clothes, RewardCount = 1, ConditionValue = 50 },
                }
            });
        }

        private void AddActivityConfig(ActivityConfig config)
        {
            m_activityConfigs[config.Id] = config;
        }

        private ActivityConfig GetActivityConfig(string activityId)
        {
            if (m_activityConfigs.TryGetValue(activityId, out var config))
                return config;
            return null;
        }

        private void GrantReward(ActivityRewardConfig reward)
        {
            switch (reward.RewardType)
            {
                case RewardType.Diamond:
                    Debug.Log($"[ActivitySystem] Grant Diamond: {reward.RewardCount}");
                    break;
                case RewardType.Star:
                    Debug.Log($"[ActivitySystem] Grant Star: {reward.RewardCount}");
                    break;
                case RewardType.Clothes:
                    Debug.Log($"[ActivitySystem] Grant Clothes: {reward.RewardId}");
                    break;
                default:
                    Debug.Log($"[ActivitySystem] Grant: {reward.RewardType}");
                    break;
            }
        }

        //========================== 测试接口 ==========================

        public void Test_ResetActivities()
        {
            m_playerActivities.Clear();
            SaveData();
        }
    }

    //===========================================================
    // 数据包装类
    //===========================================================

    [Serializable]
    public class ActivityDataWrapper
    {
        public Dictionary<string, PlayerActivityData> Activities;
    }
}
