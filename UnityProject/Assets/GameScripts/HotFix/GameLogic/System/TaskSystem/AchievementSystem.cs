using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 成就系统数据类型
    //===========================================================

    /// <summary>
    /// 成就分类
    /// </summary>
    public enum AchievementCategory
    {
        /// <summary>通关成就</summary>
        PassLevel = 0,
        
        /// <summary>收集成就</summary>
        Collect = 1,
        
        /// <summary>社交成就</summary>
        Social = 2,
        
        /// <summary>付费成就</summary>
        Payment = 3,
        
        /// <summary>隐藏成就</summary>
        Secret = 4,
        
        /// <summary> misc成就</summary>
        Misc = 5,
    }

    /// <summary>
    /// 成就状态
    /// </summary>
    public enum AchievementState
    {
        /// <summary>未解锁</summary>
        Locked = 0,
        
        /// <summary>进行中</summary>
        InProgress = 1,
        
        /// <summary>已完成(未领取)</summary>
        Completed = 2,
        
        /// <summary>已领取</summary>
        Claimed = 3,
    }

    //===========================================================
    // 成就配置数据
    //===========================================================

    /// <summary>
    /// 成就配置数据
    /// </summary>
    [Serializable]
    public class AchievementConfig
    {
        /// <summary>成就ID</summary>
        public string Id;
        
        /// <summary>成就名称</summary>
        public string Name;
        
        /// <summary>成就描述</summary>
        public string Description;
        
        /// <summary>成就分类</summary>
        public AchievementCategory Category;
        
        /// <summary>条件类型(同TaskConditionType)</summary>
        public TaskConditionType ConditionType;
        
        /// <summary>目标数量</summary>
        public int TargetCount;
        
        /// <summary>奖励类型</summary>
        public RewardType RewardType;
        
        /// <summary>奖励ID</summary>
        public string RewardId;
        
        /// <summary>奖励数量</summary>
        public int RewardCount;
        
        /// <summary>图标</summary>
        public string Icon;
        
        /// <summary>是否隐藏</summary>
        public bool IsHidden;
        
        /// <summary>前置成就ID</summary>
        public string PreAchievementId;
        
        /// <summary>隐藏条件(如达成某条件后显示)</summary>
        public string UnlockCondition;
    }

    /// <summary>
    /// 玩家成就数据
    /// </summary>
    [Serializable]
    public class PlayerAchievementData
    {
        /// <summary>成就ID</summary>
        public string AchievementId;
        
        /// <summary>当前进度</summary>
        public int CurrentProgress;
        
        /// <summary>成就状态</summary>
        public AchievementState State;
        
        /// <summary>是否已领取奖励</summary>
        public bool IsRewardClaimed;
        
        /// <summary>解锁时间</summary>
        public long UnlockTime;
        
        /// <summary>完成时间</summary>
        public long CompleteTime;
    }

    //===========================================================
    // 成就系统接口
    //===========================================================

    /// <summary>
    /// 成就系统接口
    /// </summary>
    public interface IAchievementSystem
    {
        /// <summary>
        /// 更新成就进度
        /// </summary>

        /// <summary>
        /// 初始化系统
        /// </summary>
        void Initialize();

        /// <summary>
        /// 更新成就进度
        /// </summary>
        /// <param name="conditionType">条件类型</param>
        /// <param name="count">增加的数量</param>
        void UpdateAchievementProgress(TaskConditionType conditionType, int count = 1);

        /// <summary>
        /// 领取成就奖励
        /// </summary>
        /// <param name="achievementId">成就ID</param>
        /// <returns>是否成功</returns>
        bool ClaimAchievementReward(string achievementId);

        /// <summary>
        /// 获取成就数据
        /// </summary>
        /// <param name="achievementId">成就ID</param>
        /// <returns>成就数据</returns>
        PlayerAchievementData GetAchievementData(string achievementId);

        /// <summary>
        /// 获取成就列表
        /// </summary>
        /// <param name="category">分类</param>
        /// <returns>成就列表</returns>
        List<PlayerAchievementData> GetAchievementList(AchievementCategory category);

        /// <summary>
        /// 获取成就列表(所有)
        /// </summary>
        /// <returns>所有成就列表</returns>
        List<PlayerAchievementData> GetAllAchievements();

        /// <summary>
        /// 获取成就完成度
        /// </summary>
        /// <returns>完成度(0-1)</returns>
        float GetCompletionRate();

        /// <summary>
        /// 获取已完成成就数
        /// </summary>
        /// <returns>已完成数</returns>
        int GetCompletedCount();

        /// <summary>
        /// 保存数据
        /// </summary>
        void SaveData();

        /// <summary>
        /// 加载数据
        /// </summary>
        void LoadData();
    }

    //===========================================================
    // 成就系统实现
    //===========================================================

    /// <summary>
    /// 成就系统
    /// 职责：
    /// 1. 管理所有成就(分类展示)
    /// 2. 成就进度追踪和更新
    /// 3. 成就奖励发放
    /// 4. 成就解锁条件判定
    /// 5. 成就数据持久化
    /// 
    /// 扩展点：
    /// 1. 可通过配置扩展成就类型
    /// 2. 可通过继承重写奖励发放
    /// 3. 支持隐藏成就解锁条件
    /// 4. 支持成就链(前置成就)
    /// </summary>
    public class AchievementSystem : BaseLogicSys<AchievementSystem>, IAchievementSystem
    {
        //========================== 常量定义 ==========================

        /// <summary>数据存储Key</summary>
        private const string DATA_KEY = "PlayerAchievementData";

        //========================== 私有变量 ==========================

        /// <summary>成就配置表</summary>
        private Dictionary<string, AchievementConfig> m_achievementConfigs = new Dictionary<string, AchievementConfig>();

        /// <summary>玩家成就数据</summary>
        private Dictionary<string, PlayerAchievementData> m_playerAchievements = new Dictionary<string, PlayerAchievementData>();

        /// <summary>成就分类索引</summary>
        private Dictionary<AchievementCategory, List<string>> m_categoryIndex = new Dictionary<AchievementCategory, List<string>>();

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

            // 加载成就配置
            LoadAchievementConfigs();

            // 加载玩家数据
            LoadData();

            m_isInitialized = true;
            Debug.Log($"[AchievementSystem] Initialized with {m_achievementConfigs.Count} achievements");
            return true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void OnDestroy()
        {
            SaveData();
            m_achievementConfigs.Clear();
            m_playerAchievements.Clear();
            m_categoryIndex.Clear();
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 更新成就进度
        /// 核心逻辑：与任务系统类似，但增加成就链支持
        /// </summary>
        public void UpdateAchievementProgress(TaskConditionType conditionType, int count = 1)
        {
            if (!m_isInitialized) return;

            // 找出所有匹配条件类型的成就
            foreach (var achievementId in GetAchievementIdsByConditionType(conditionType))
            {
                var achievementData = GetAchievementData(achievementId);
                if (achievementData == null) continue;

                // 跳过未解锁和已领取的成就
                if (achievementData.State == AchievementState.Locked || 
                    achievementData.State == AchievementState.Claimed)
                    continue;

                var config = GetAchievementConfig(achievementId);
                if (config == null) continue;

                // 更新进度
                achievementData.CurrentProgress = Mathf.Min(
                    achievementData.CurrentProgress + count, 
                    config.TargetCount);

                // 检查是否完成
                if (achievementData.CurrentProgress >= config.TargetCount && 
                    achievementData.State == AchievementState.InProgress)
                {
                    achievementData.State = AchievementState.Completed;
                    achievementData.CompleteTime = TimeUtil.GetCurrentTimeMillis();
                    OnAchievementCompleted(achievementId);
                }

                // 通知更新
                NotifyAchievementUpdate(achievementId);
            }
        }

        /// <summary>
        /// 领取成就奖励
        /// </summary>
        public bool ClaimAchievementReward(string achievementId)
        {
            var achievementData = GetAchievementData(achievementId);
            if (achievementData == null)
            {
                Debug.LogWarning($"[AchievementSystem] Achievement not found: {achievementId}");
                return false;
            }

            var config = GetAchievementConfig(achievementId);
            if (config == null)
            {
                Debug.LogWarning($"[AchievementSystem] Achievement config not found: {achievementId}");
                return false;
            }

            // 验证状态
            if (achievementData.State != AchievementState.Completed)
            {
                Debug.LogWarning($"[AchievementSystem] Achievement not completed: {achievementId}");
                return false;
            }

            if (achievementData.IsRewardClaimed)
            {
                Debug.LogWarning($"[AchievementSystem] Reward already claimed: {achievementId}");
                return false;
            }

            // 发放奖励
            GrantReward(config.RewardType, config.RewardId, config.RewardCount);

            // 更新状态
            achievementData.State = AchievementState.Claimed;
            achievementData.IsRewardClaimed = true;

            // 解锁下一个成就(成就链)
            UnlockNextAchievement(achievementId);

            // 通知
            NotifyAchievementRewardClaimed(achievementId);

            // 保存
            SaveData();

            Debug.Log($"[AchievementSystem] Achievement reward claimed: {achievementId}");
            return true;
        }

        /// <summary>
        /// 获取成就数据
        /// </summary>
        public PlayerAchievementData GetAchievementData(string achievementId)
        {
            if (m_playerAchievements.TryGetValue(achievementId, out var data))
                return data;
            return null;
        }

        /// <summary>
        /// 获取成就列表
        /// </summary>
        public List<PlayerAchievementData> GetAchievementList(AchievementCategory category)
        {
            var result = new List<PlayerAchievementData>();
            if (m_categoryIndex.TryGetValue(category, out var achievementIds))
            {
                foreach (var achievementId in achievementIds)
                {
                    var data = GetAchievementData(achievementId);
                    if (data != null)
                        result.Add(data);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取所有成就
        /// </summary>
        public List<PlayerAchievementData> GetAllAchievements()
        {
            var result = new List<PlayerAchievementData>();
            foreach (var data in m_playerAchievements.Values)
            {
                result.Add(data);
            }
            return result;
        }

        /// <summary>
        /// 获取完成度
        /// </summary>
        public float GetCompletionRate()
        {
            if (m_achievementConfigs.Count == 0) return 0f;

            int completedCount = 0;
            foreach (var achievementData in m_playerAchievements.Values)
            {
                if (achievementData.State == AchievementState.Completed ||
                    achievementData.State == AchievementState.Claimed)
                {
                    completedCount++;
                }
            }

            return (float)completedCount / m_achievementConfigs.Count;
        }

        /// <summary>
        /// 获取已完成成就数
        /// </summary>
        public int GetCompletedCount()
        {
            int count = 0;
            foreach (var achievementData in m_playerAchievements.Values)
            {
                if (achievementData.State == AchievementState.Completed ||
                    achievementData.State == AchievementState.Claimed)
                {
                    count++;
                }
            }
            return count;
        }

        //========================== 数据持久化 ==========================

        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveData()
        {
            var json = JsonUtility.ToJson(new AchievementDataWrapper
            {
                Achievements = m_playerAchievements
            });

            PlayerPrefs.SetString(DATA_KEY, json);
            Debug.Log("[AchievementSystem] Data saved");
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public void LoadData()
        {
            if (!PlayerPrefs.HasKey(DATA_KEY))
            {
                InitializeAllAchievements();
                return;
            }

            try
            {
                var json = PlayerPrefs.GetString(DATA_KEY);
                var wrapper = JsonUtility.FromJson<AchievementDataWrapper>(json);

                if (wrapper != null)
                {
                    m_playerAchievements = wrapper.Achievements ?? new Dictionary<string, PlayerAchievementData>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[AchievementSystem] Failed to load data: {e.Message}");
                InitializeAllAchievements();
            }
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 加载成就配置
        /// </summary>
        private void LoadAchievementConfigs()
        {
            m_achievementConfigs.Clear();
            m_categoryIndex.Clear();

            // 初始化分类索引
            foreach (AchievementCategory category in Enum.GetValues(typeof(AchievementCategory)))
            {
                m_categoryIndex[category] = new List<string>();
            }

            // 创建示例配置
            CreateSampleAchievementConfigs();
        }

        /// <summary>
        /// 创建示例成就配置
        /// </summary>
        private void CreateSampleAchievementConfigs()
        {
            // 通关成就
            AddAchievementConfig(new AchievementConfig
            {
                Id = "ach_pass_10",
                Name = "小试牛刀",
                Description = "累计通关10关",
                Category = AchievementCategory.PassLevel,
                ConditionType = TaskConditionType.PassLevel,
                TargetCount = 10,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 50,
                Icon = "icon_ach_pass.png",
                IsHidden = false
            });

            AddAchievementConfig(new AchievementConfig
            {
                Id = "ach_pass_50",
                Name = "身经百战",
                Description = "累计通关50关",
                Category = AchievementCategory.PassLevel,
                ConditionType = TaskConditionType.PassLevel,
                TargetCount = 50,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 200,
                Icon = "icon_ach_pass.png",
                IsHidden = false,
                PreAchievementId = "ach_pass_10"
            });

            AddAchievementConfig(new AchievementConfig
            {
                Id = "ach_pass_100",
                Name = "大神",
                Description = "累计通关100关",
                Category = AchievementCategory.PassLevel,
                ConditionType = TaskConditionType.PassLevel,
                TargetCount = 100,
                RewardType = RewardType.Clothes,
                RewardId = "clothes_legend",
                RewardCount = 1,
                Icon = "icon_ach_pass.png",
                IsHidden = false,
                PreAchievementId = "ach_pass_50"
            });

            AddAchievementConfig(new AchievementConfig
            {
                Id = "ach_pass_500",
                Name = "传奇",
                Description = "累计通关500关",
                Category = AchievementCategory.PassLevel,
                ConditionType = TaskConditionType.PassLevel,
                TargetCount = 500,
                RewardType = RewardType.Skin,
                RewardId = "skin_legend",
                RewardCount = 1,
                Icon = "icon_ach_pass.png",
                IsHidden = false,
                PreAchievementId = "ach_pass_100"
            });

            // 收集成就
            AddAchievementConfig(new AchievementConfig
            {
                Id = "ach_collect_10",
                Name = "收藏家",
                Description = "收集10套服装",
                Category = AchievementCategory.Collect,
                ConditionType = TaskConditionType.CollectItem,
                TargetCount = 10,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 300,
                Icon = "icon_ach_collect.png",
                IsHidden = false
            });

            AddAchievementConfig(new AchievementConfig
            {
                Id = "ach_collect_50",
                Name = "收藏大师",
                Description = "收集50套服装",
                Category = AchievementCategory.Collect,
                ConditionType = TaskConditionType.CollectItem,
                TargetCount = 50,
                RewardType = RewardType.Clothes,
                RewardId = "clothes_master",
                RewardCount = 1,
                Icon = "icon_ach_collect.png",
                IsHidden = false,
                PreAchievementId = "ach_collect_10"
            });

            // 社交成就
            AddAchievementConfig(new AchievementConfig
            {
                Id = "ach_friend_10",
                Name = "社交达人",
                Description = "添加10个好友",
                Category = AchievementCategory.Social,
                ConditionType = TaskConditionType.AddFriend,
                TargetCount = 10,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 100,
                Icon = "icon_ach_friend.png",
                IsHidden = false
            });

            // 隐藏成就
            AddAchievementConfig(new AchievementConfig
            {
                Id = "ach_secret_rainbow",
                Name = "🌈 彩虹缔造者",
                Description = "使用彩虹球消除1000个元素",
                Category = AchievementCategory.Secret,
                ConditionType = TaskConditionType.EliminateElement,
                TargetCount = 1000,
                RewardType = RewardType.Skin,
                RewardId = "skin_rainbow",
                RewardCount = 1,
                Icon = "icon_ach_secret.png",
                IsHidden = true,
                UnlockCondition = "eliminate_rainbow_10_times"
            });

            // 付费成就
            AddAchievementConfig(new AchievementConfig
            {
                Id = "ach_pay_1",
                Name = "首次充值",
                Description = "首次购买任意钻石",
                Category = AchievementCategory.Payment,
                ConditionType = TaskConditionType.PassLevel, // 暂用
                TargetCount = 1,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 100,
                Icon = "icon_ach_pay.png",
                IsHidden = false
            });
        }

        /// <summary>
        /// 添加成就配置
        /// </summary>
        private void AddAchievementConfig(AchievementConfig config)
        {
            m_achievementConfigs[config.Id] = config;
            m_categoryIndex[config.Category].Add(config.Id);
        }

        /// <summary>
        /// 获取成就配置
        /// </summary>
        private AchievementConfig GetAchievementConfig(string achievementId)
        {
            if (m_achievementConfigs.TryGetValue(achievementId, out var config))
                return config;
            return null;
        }

        /// <summary>
        /// 初始化所有成就
        /// </summary>
        private void InitializeAllAchievements()
        {
            m_playerAchievements.Clear();

            foreach (var config in m_achievementConfigs.Values)
            {
                // 检查是否有前置成就
                bool isLocked = !string.IsNullOrEmpty(config.PreAchievementId);

                var achievementData = new PlayerAchievementData
                {
                    AchievementId = config.Id,
                    CurrentProgress = 0,
                    State = isLocked ? AchievementState.Locked : AchievementState.InProgress,
                    IsRewardClaimed = false,
                    UnlockTime = TimeUtil.GetCurrentTimeMillis(),
                    CompleteTime = 0
                };

                m_playerAchievements[config.Id] = achievementData;
            }
        }

        /// <summary>
        /// 根据条件类型获取成就ID列表
        /// </summary>
        private List<string> GetAchievementIdsByConditionType(TaskConditionType conditionType)
        {
            var result = new List<string>();

            foreach (var config in m_achievementConfigs.Values)
            {
                if (config.ConditionType == conditionType)
                {
                    result.Add(config.Id);
                }
            }

            return result;
        }

        /// <summary>
        /// 成就完成回调
        /// </summary>
        private void OnAchievementCompleted(string achievementId)
        {
            Debug.Log($"[AchievementSystem] Achievement completed: {achievementId}");
            // 可扩展：播放特效、弹出成就解锁界面等
        }

        /// <summary>
        /// 成就更新通知
        /// </summary>
        private void NotifyAchievementUpdate(string achievementId)
        {
            // EventManager.Instance.NoticeEvent(EVENT_ACHIEVEMENT_UPDATE, achievementId);
        }

        /// <summary>
        /// 成就奖励领取通知
        /// </summary>
        private void NotifyAchievementRewardClaimed(string achievementId)
        {
            // EventManager.Instance.NoticeEvent(EVENT_ACHIEVEMENT_REWARD_CLAIMED, achievementId);
        }

        /// <summary>
        /// 发放奖励
        /// </summary>
        private void GrantReward(RewardType type, string rewardId, int count)
        {
            switch (type)
            {
                case RewardType.Diamond:
                    Debug.Log($"[AchievementSystem] Grant Diamond: {count}");
                    break;
                case RewardType.Star:
                    Debug.Log($"[AchievementSystem] Grant Star: {count}");
                    break;
                case RewardType.Energy:
                    Debug.Log($"[AchievementSystem] Grant Energy: {count}");
                    break;
                case RewardType.Item:
                    Debug.Log($"[AchievementSystem] Grant Item: {rewardId} x {count}");
                    break;
                case RewardType.Clothes:
                    Debug.Log($"[AchievementSystem] Grant Clothes: {rewardId}");
                    break;
                case RewardType.Skin:
                    Debug.Log($"[AchievementSystem] Grant Skin: {rewardId}");
                    break;
            }
        }

        /// <summary>
        /// 解锁下一个成就
        /// </summary>
        private void UnlockNextAchievement(string currentAchievementId)
        {
            var config = GetAchievementConfig(currentAchievementId);
            if (config == null) return;

            // 查找所有以当前成就作为前置的成就
            foreach (var achConfig in m_achievementConfigs.Values)
            {
                if (achConfig.PreAchievementId == currentAchievementId)
                {
                    var nextData = GetAchievementData(achConfig.Id);
                    if (nextData != null && nextData.State == AchievementState.Locked)
                    {
                        nextData.State = AchievementState.InProgress;
                        nextData.UnlockTime = TimeUtil.GetCurrentTimeMillis();
                        Debug.Log($"[AchievementSystem] Achievement unlocked: {achConfig.Id}");
                    }
                }
            }
        }

        /// <summary>
        /// 解锁隐藏成就(根据条件)
        /// </summary>
        public void UnlockHiddenAchievement(string unlockCondition)
        {
            foreach (var config in m_achievementConfigs.Values)
            {
                if (config.IsHidden && config.UnlockCondition == unlockCondition)
                {
                    var data = GetAchievementData(config.Id);
                    if (data != null && data.State == AchievementState.Locked)
                    {
                        data.State = AchievementState.InProgress;
                        Debug.Log($"[AchievementSystem] Hidden achievement unlocked: {config.Id}");
                    }
                }
            }
        }

        //========================== 测试接口 ==========================

        /// <summary>
        /// 测试：直接完成成就
        /// </summary>
        public void Test_CompleteAchievement(string achievementId)
        {
            var config = GetAchievementConfig(achievementId);
            if (config == null) return;

            var achievementData = GetAchievementData(achievementId);
            if (achievementData == null) return;

            achievementData.CurrentProgress = config.TargetCount;
            achievementData.State = AchievementState.Completed;
            NotifyAchievementUpdate(achievementId);
        }

        /// <summary>
        /// 测试：重置所有成就
        /// </summary>
        public void Test_ResetAllAchievements()
        {
            InitializeAllAchievements();
            SaveData();
        }
    }

    //===========================================================
    // 数据包装类
    //===========================================================

    /// <summary>
    /// 成就数据包装类
    /// </summary>
    [Serializable]
    public class AchievementDataWrapper
    {
        public Dictionary<string, PlayerAchievementData> Achievements;
    }
}
