using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 任务数据类型
    //===========================================================
    
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum TaskType
    {
        /// <summary>每日任务</summary>
        Daily = 0,
        
        /// <summary>每周任务</summary>
        Weekly = 1,
        
        /// <summary>成就任务</summary>
        Achievement = 2,
        
        /// <summary>活动任务</summary>
        Activity = 3,
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskState
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

    /// <summary>
    /// 任务条件类型
    /// </summary>
    public enum TaskConditionType
    {
        PassLevel = 1,          // 通关关卡
        EliminateElement = 2,   // 消除元素
        UseSkill = 3,          // 使用技能
        ShareGame = 4,         // 分享游戏
        CollectItem = 5,       // 收集道具
        SpendEnergy = 6,       // 消耗体力
        EarnScore = 7,         // 获得分数
        AddFriend = 8,         // 添加好友
        WinMatch = 9,          // 赢得对战
    }

    /// <summary>
    /// 奖励类型
    /// </summary>
    public enum RewardType
    {
        Diamond = 1,   // 钻石
        Star = 2,      // 幸福星
        Energy = 3,    // 体力
        Item = 4,      // 道具
        Clothes = 5,   // 服装
        Skin = 6,      // 皮肤
    }

    //===========================================================
    // 任务配置数据 (从配置表加载)
    //===========================================================

    /// <summary>
    /// 任务配置数据
    /// </summary>
    [Serializable]
    public class TaskConfig
    {
        /// <summary>任务ID</summary>
        public string Id;
        
        /// <summary>任务名称</summary>
        public string Name;
        
        /// <summary>任务描述</summary>
        public string Description;
        
        /// <summary>任务类型</summary>
        public TaskType Type;
        
        /// <summary>条件类型</summary>
        public TaskConditionType ConditionType;
        
        /// <summary>目标数量</summary>
        public int TargetCount;
        
        /// <summary>奖励类型</summary>
        public RewardType RewardType;
        
        /// <summary>奖励ID(如item_id)</summary>
        public string RewardId;
        
        /// <summary>奖励数量</summary>
        public int RewardCount;
        
        /// <summary>图标</summary>
        public string Icon;
        
        /// <summary>是否隐藏</summary>
        public bool IsHidden;
        
        /// <summary>前置任务ID</summary>
        public string PreTaskId;
    }

    /// <summary>
    /// 玩家任务数据
    /// </summary>
    [Serializable]
    public class PlayerTaskData
    {
        /// <summary>任务ID</summary>
        public string TaskId;
        
        /// <summary>当前进度</summary>
        public int CurrentProgress;
        
        /// <summary>任务状态</summary>
        public TaskState State;
        
        /// <summary>是否已领取奖励</summary>
        public bool IsRewardClaimed;
        
        /// <summary>解锁时间戳</summary>
        public long UnlockTime;
    }

    //===========================================================
    // 任务系统接口
    //===========================================================

    /// <summary>
    /// 任务系统接口
    /// </summary>
    public interface ITaskSystem
    {
        /// <summary>
        /// 更新任务进度
        /// </summary>

        /// <summary>
        /// 初始化系统
        /// </summary>
        void Initialize();

        /// <summary>
        /// 更新任务进度
        /// </summary>
        /// <param name="conditionType">条件类型</param>
        /// <param name="count">增加的数量</param>
        void UpdateTaskProgress(TaskConditionType conditionType, int count = 1);

        /// <summary>
        /// 领取任务奖励
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否成功</returns>
        bool ClaimTaskReward(string taskId);

        /// <summary>
        /// 获取任务数据
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>任务数据</returns>
        PlayerTaskData GetTaskData(string taskId);

        /// <summary>
        /// 获取所有任务列表
        /// </summary>
        /// <param name="type">任务类型</param>
        /// <returns>任务列表</returns>
        List<PlayerTaskData> GetTaskList(TaskType type);

        /// <summary>
        /// 刷新每日任务
        /// </summary>
        void RefreshDailyTasks();

        /// <summary>
        /// 刷新每周任务
        /// </summary>
        void RefreshWeeklyTasks();

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
    // 任务系统实现 (单例模式，继承BaseLogicSys)
    //===========================================================

    /// <summary>
    /// 任务系统
    /// 职责：
    /// 1. 管理所有任务(每日/每周/成就/活动)
    /// 2. 任务进度追踪和更新
    /// 3. 任务奖励发放
    /// 4. 任务数据持久化
    /// 
    /// 扩展点：
    /// 1. 可通过配置文件扩展任务类型
    /// 2. 可通过继承重写奖励发放逻辑
    /// 3. 可通过事件扩展任务触发条件
    /// </summary>
    public class TaskSystem : BaseLogicSys<TaskSystem>, ITaskSystem
    {
        //========================== 常量定义 ==========================

        /// <summary>数据存储Key</summary>
        private const string DATA_KEY = "PlayerTaskData";

        /// <summary>每日任务刷新时间(小时)</summary>
        private const int DAILY_REFRESH_HOUR = 0;

        /// <summary>每周任务刷新时间(周一0点)</summary>
        private const int WEEKLY_REFRESH_DAY = 1;

        //========================== 私有变量 ==========================

        /// <summary>任务配置表(从配置表加载)</summary>
        private Dictionary<string, TaskConfig> m_taskConfigs = new Dictionary<string, TaskConfig>();

        /// <summary>玩家任务数据(按任务ID索引)</summary>
        private Dictionary<string, PlayerTaskData> m_playerTasks = new Dictionary<string, PlayerTaskData>();

        /// <summary>任务类型索引(加速查询)</summary>
        private Dictionary<TaskType, List<string>> m_taskTypeIndex = new Dictionary<TaskType, List<string>>();

        /// <summary>上次刷新时间</summary>
        private long m_lastRefreshTime;

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

            // 加载任务配置(实际项目中从配置表加载)
            LoadTaskConfigs();

            // 加载玩家数据
            LoadData();

            // 检查是否需要刷新任务
            CheckAndRefreshTasks();

            m_isInitialized = true;
            Debug.Log($"[TaskSystem] Initialized with {m_taskConfigs.Count} task configs");
            return true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void OnDestroy()
        {
            SaveData();
            m_taskConfigs.Clear();
            m_playerTasks.Clear();
            m_taskTypeIndex.Clear();
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 更新任务进度
        /// 核心逻辑：当满足特定条件时，触发任务进度更新
        /// </summary>
        /// <param name="conditionType">条件类型</param>
        /// <param name="count">增加的数量</param>
        public void UpdateTaskProgress(TaskConditionType conditionType, int count = 1)
        {
            if (!m_isInitialized) return;

            // 找出所有匹配条件类型的未完成任务
            foreach (var taskId in GetTaskIdsByConditionType(conditionType))
            {
                var taskData = GetTaskData(taskId);
                if (taskData == null) continue;

                // 跳过已锁定和已领取的任务
                if (taskData.State == TaskState.Locked || taskData.State == TaskState.Claimed)
                    continue;

                var config = GetTaskConfig(taskId);
                if (config == null) continue;

                // 更新进度
                taskData.CurrentProgress = Mathf.Min(taskData.CurrentProgress + count, config.TargetCount);

                // 检查是否完成
                if (taskData.CurrentProgress >= config.TargetCount && taskData.State == TaskState.InProgress)
                {
                    taskData.State = TaskState.Completed;
                    OnTaskCompleted(taskId);
                }

                // 通知UI更新
                NotifyTaskUpdate(taskId);
            }
        }

        /// <summary>
        /// 领取任务奖励
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否成功</returns>
        public bool ClaimTaskReward(string taskId)
        {
            var taskData = GetTaskData(taskId);
            if (taskData == null)
            {
                Debug.LogWarning($"[TaskSystem] Task not found: {taskId}");
                return false;
            }

            var config = GetTaskConfig(taskId);
            if (config == null)
            {
                Debug.LogWarning($"[TaskSystem] Task config not found: {taskId}");
                return false;
            }

            // 检查任务状态
            if (taskData.State != TaskState.Completed)
            {
                Debug.LogWarning($"[TaskSystem] Task not completed: {taskId}, state: {taskData.State}");
                return false;
            }

            // 检查是否已领取
            if (taskData.IsRewardClaimed)
            {
                Debug.LogWarning($"[TaskSystem] Reward already claimed: {taskId}");
                return false;
            }

            // 发放奖励(可扩展：此处可接入货币系统、道具系统等)
            if (!string.IsNullOrEmpty(config.RewardId))
            {
                GrantReward(config.RewardType, config.RewardId, config.RewardCount);
            }

            // 更新状态
            taskData.State = TaskState.Claimed;
            taskData.IsRewardClaimed = true;

            // 触发后续任务(如果是成就任务)
            if (config.Type == TaskType.Achievement)
            {
                UnlockNextAchievement(config.Id);
            }

            // 通知
            NotifyTaskRewardClaimed(taskId);

            // 保存
            SaveData();

            Debug.Log($"[TaskSystem] Reward claimed: {taskId}");
            return true;
        }

        /// <summary>
        /// 获取任务数据
        /// </summary>
        public PlayerTaskData GetTaskData(string taskId)
        {
            if (m_playerTasks.TryGetValue(taskId, out var data))
                return data;
            return null;
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        public List<PlayerTaskData> GetTaskList(TaskType type)
        {
            var result = new List<PlayerTaskData>();
            if (m_taskTypeIndex.TryGetValue(type, out var taskIds))
            {
                foreach (var taskId in taskIds)
                {
                    var data = GetTaskData(taskId);
                    if (data != null)
                        result.Add(data);
                }
            }
            return result;
        }

        /// <summary>
        /// 刷新每日任务
        /// </summary>
        public void RefreshDailyTasks()
        {
            RefreshTasksByType(TaskType.Daily);
            SaveData();
            Debug.Log("[TaskSystem] Daily tasks refreshed");
        }

        /// <summary>
        /// 刷新每周任务
        /// </summary>
        public void RefreshWeeklyTasks()
        {
            RefreshTasksByType(TaskType.Weekly);
            SaveData();
            Debug.Log("[TaskSystem] Weekly tasks refreshed");
        }

        //========================== 数据持久化 ==========================

        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveData()
        {
            // 序列化数据
            var json = JsonUtility.ToJson(new TaskDataWrapper
            {
                Tasks = m_playerTasks,
                LastRefreshTime = m_lastRefreshTime
            });

            // 存储(实际项目中应使用更安全的存储方式)
            PlayerPrefs.SetString(DATA_KEY, json);
            Debug.Log("[TaskSystem] Data saved");
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public void LoadData()
        {
            if (!PlayerPrefs.HasKey(DATA_KEY))
            {
                // 首次创建，初始化所有任务
                InitializeAllTasks();
                return;
            }

            try
            {
                var json = PlayerPrefs.GetString(DATA_KEY);
                var wrapper = JsonUtility.FromJson<TaskDataWrapper>(json);
                
                if (wrapper != null)
                {
                    m_playerTasks = wrapper.Tasks ?? new Dictionary<string, PlayerTaskData>();
                    m_lastRefreshTime = wrapper.LastRefreshTime;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[TaskSystem] Failed to load data: {e.Message}");
                InitializeAllTasks();
            }
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 加载任务配置
        /// 实际项目中应从Luban配置表加载
        /// </summary>
        private void LoadTaskConfigs()
        {
            // 清空
            m_taskConfigs.Clear();
            m_taskTypeIndex.Clear();

            // 初始化索引
            foreach (TaskType type in Enum.GetValues(typeof(TaskType)))
            {
                m_taskTypeIndex[type] = new List<string>();
            }

            // 创建示例配置(实际项目中从配置表加载)
            CreateSampleTaskConfigs();
        }

        /// <summary>
        /// 创建示例任务配置
        /// </summary>
        private void CreateSampleTaskConfigs()
        {
            // 每日任务
            AddTaskConfig(new TaskConfig
            {
                Id = "daily_001",
                Name = "初出茅庐",
                Description = "通关1个关卡",
                Type = TaskType.Daily,
                ConditionType = TaskConditionType.PassLevel,
                TargetCount = 1,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 10,
                Icon = "icon_level.png"
            });

            AddTaskConfig(new TaskConfig
            {
                Id = "daily_002",
                Name = "消除达人",
                Description = "消除50个元素",
                Type = TaskType.Daily,
                ConditionType = TaskConditionType.EliminateElement,
                TargetCount = 50,
                RewardType = RewardType.Star,
                RewardId = "star",
                RewardCount = 5,
                Icon = "icon_eliminate.png"
            });

            AddTaskConfig(new TaskConfig
            {
                Id = "daily_003",
                Name = "分享喜悦",
                Description = "分享游戏1次",
                Type = TaskType.Daily,
                ConditionType = TaskConditionType.ShareGame,
                TargetCount = 1,
                RewardType = RewardType.Energy,
                RewardId = "energy",
                RewardCount = 20,
                Icon = "icon_share.png"
            });

            // 每周任务
            AddTaskConfig(new TaskConfig
            {
                Id = "weekly_001",
                Name = "周冠军",
                Description = "累计获得50000分",
                Type = TaskType.Weekly,
                ConditionType = TaskConditionType.EarnScore,
                TargetCount = 50000,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 100,
                Icon = "icon_score.png"
            });

            AddTaskConfig(new TaskConfig
            {
                Id = "weekly_002",
                Name = "社交达人",
                Description = "添加5个好友",
                Type = TaskType.Weekly,
                ConditionType = TaskConditionType.AddFriend,
                TargetCount = 5,
                RewardType = RewardType.Clothes,
                RewardId = "clothes_001",
                RewardCount = 1,
                Icon = "icon_friend.png"
            });

            // 成就任务
            AddTaskConfig(new TaskConfig
            {
                Id = "ach_pass_10",
                Name = "小试牛刀",
                Description = "累计通关10关",
                Type = TaskType.Achievement,
                ConditionType = TaskConditionType.PassLevel,
                TargetCount = 10,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 50,
                Icon = "icon_ach_pass.png",
                IsHidden = false
            });

            AddTaskConfig(new TaskConfig
            {
                Id = "ach_pass_50",
                Name = "高手在民间",
                Description = "累计通关50关",
                Type = TaskType.Achievement,
                ConditionType = TaskConditionType.PassLevel,
                TargetCount = 50,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 200,
                Icon = "icon_ach_pass.png",
                IsHidden = false,
                PreTaskId = "ach_pass_10"
            });

            AddTaskConfig(new TaskConfig
            {
                Id = "ach_pass_100",
                Name = "大神",
                Description = "累计通关100关",
                Type = TaskType.Achievement,
                ConditionType = TaskConditionType.PassLevel,
                TargetCount = 100,
                RewardType = RewardType.Clothes,
                RewardId = "clothes_legend",
                RewardCount = 1,
                Icon = "icon_ach_pass.png",
                IsHidden = false,
                PreTaskId = "ach_pass_50"
            });

            AddTaskConfig(new TaskConfig
            {
                Id = "ach_collect_10",
                Name = "收藏家",
                Description = "收集10套服装",
                Type = TaskType.Achievement,
                ConditionType = TaskConditionType.CollectItem,
                TargetCount = 10,
                RewardType = RewardType.Diamond,
                RewardId = "diamond",
                RewardCount = 300,
                Icon = "icon_ach_collect.png",
                IsHidden = false
            });
        }

        /// <summary>
        /// 添加任务配置
        /// </summary>
        private void AddTaskConfig(TaskConfig config)
        {
            m_taskConfigs[config.Id] = config;
            m_taskTypeIndex[config.Type].Add(config.Id);
        }

        /// <summary>
        /// 获取任务配置
        /// </summary>
        private TaskConfig GetTaskConfig(string taskId)
        {
            if (m_taskConfigs.TryGetValue(taskId, out var config))
                return config;
            return null;
        }

        /// <summary>
        /// 初始化所有任务
        /// </summary>
        private void InitializeAllTasks()
        {
            m_playerTasks.Clear();

            foreach (var config in m_taskConfigs.Values)
            {
                var taskData = new PlayerTaskData
                {
                    TaskId = config.Id,
                    CurrentProgress = 0,
                    State = TaskState.InProgress,
                    IsRewardClaimed = false,
                    UnlockTime = TimeUtil.GetCurrentTimeMillis()
                };

                m_playerTasks[config.Id] = taskData;
            }

            m_lastRefreshTime = TimeUtil.GetCurrentTimeMillis();
        }

        /// <summary>
        /// 检查并刷新任务
        /// </summary>
        private void CheckAndRefreshTasks()
        {
            var now = TimeUtil.GetCurrentTimeMillis();
            var lastRefresh = TimeUtil.GetDateTime(m_lastRefreshTime);
            var today = TimeUtil.GetDateTime(now);

            // 检查是否需要刷新每日任务
            if (today.Day != lastRefresh.Day || today.Month != lastRefresh.Month)
            {
                RefreshDailyTasks();
            }

            // 检查是否需要刷新每周任务
            if ((int)today.DayOfWeek != WEEKLY_REFRESH_DAY || 
                today.Day / 7 != lastRefresh.Day / 7)
            {
                RefreshWeeklyTasks();
            }
        }

        /// <summary>
        /// 按类型刷新任务
        /// </summary>
        private void RefreshTasksByType(TaskType type)
        {
            if (!m_taskTypeIndex.TryGetValue(type, out var taskIds))
                return;

            foreach (var taskId in taskIds)
            {
                if (m_playerTasks.TryGetValue(taskId, out var taskData))
                {
                    // 重置进度和状态
                    taskData.CurrentProgress = 0;
                    taskData.State = TaskState.InProgress;
                    taskData.IsRewardClaimed = false;
                    taskData.UnlockTime = TimeUtil.GetCurrentTimeMillis();
                }
            }

            m_lastRefreshTime = TimeUtil.GetCurrentTimeMillis();
        }

        /// <summary>
        /// 根据条件类型获取任务ID列表
        /// </summary>
        private List<string> GetTaskIdsByConditionType(TaskConditionType conditionType)
        {
            var result = new List<string>();

            foreach (var config in m_taskConfigs.Values)
            {
                if (config.ConditionType == conditionType)
                {
                    result.Add(config.Id);
                }
            }

            return result;
        }

        /// <summary>
        /// 任务完成回调
        /// </summary>
        private void OnTaskCompleted(string taskId)
        {
            Debug.Log($"[TaskSystem] Task completed: {taskId}");
            // 可扩展：播放特效、弹出庆祝界面等
        }

        /// <summary>
        /// 任务更新通知
        /// </summary>
        private void NotifyTaskUpdate(string taskId)
        {
            // 通过事件系统通知UI更新
            // EventManager.Instance.NoticeEvent(EVENT_TASK_UPDATE, taskId);
        }

        /// <summary>
        /// 奖励领取通知
        /// </summary>
        private void NotifyTaskRewardClaimed(string taskId)
        {
            // 通过事件系统通知
            // EventManager.Instance.NoticeEvent(EVENT_TASK_REWARD_CLAIMED, taskId);
        }

        /// <summary>
        /// 发放奖励
        /// 扩展点：可接入货币系统、道具系统等
        /// </summary>
        private void GrantReward(RewardType type, string rewardId, int count)
        {
            switch (type)
            {
                case RewardType.Diamond:
                    // PlayerData.Instance.AddDiamond(count);
                    Debug.Log($"[TaskSystem] Grant Diamond: {count}");
                    break;

                case RewardType.Star:
                    // PlayerData.Instance.AddStar(count);
                    Debug.Log($"[TaskSystem] Grant Star: {count}");
                    break;

                case RewardType.Energy:
                    // PlayerData.Instance.AddEnergy(count);
                    Debug.Log($"[TaskSystem] Grant Energy: {count}");
                    break;

                case RewardType.Item:
                    // PlayerData.Instance.AddItem(rewardId, count);
                    Debug.Log($"[TaskSystem] Grant Item: {rewardId} x {count}");
                    break;

                case RewardType.Clothes:
                    // PlayerData.Instance.AddCloth(rewardId, 1, 0);
                    Debug.Log($"[TaskSystem] Grant Clothes: {rewardId}");
                    break;

                case RewardType.Skin:
                    Debug.Log($"[TaskSystem] Grant Skin: {rewardId}");
                    break;
            }
        }

        /// <summary>
        /// 解锁下一个成就
        /// </summary>
        private void UnlockNextAchievement(string currentTaskId)
        {
            var config = GetTaskConfig(currentTaskId);
            if (config == null || string.IsNullOrEmpty(config.PreTaskId))
                return;

            // 解锁前置任务
            if (m_playerTasks.TryGetValue(config.PreTaskId, out var preTaskData))
            {
                if (preTaskData.State == TaskState.Locked)
                {
                    preTaskData.State = TaskState.InProgress;
                    Debug.Log($"[TaskSystem] Achievement unlocked: {config.PreTaskId}");
                }
            }
        }

        //========================== 测试接口 ==========================

        /// <summary>
        /// 测试：直接完成任务
        /// </summary>
        public void Test_CompleteTask(string taskId)
        {
            var config = GetTaskConfig(taskId);
            if (config == null) return;

            var taskData = GetTaskData(taskId);
            if (taskData == null) return;

            taskData.CurrentProgress = config.TargetCount;
            taskData.State = TaskState.Completed;
            NotifyTaskUpdate(taskId);
        }

        /// <summary>
        /// 测试：重置所有任务
        /// </summary>
        public void Test_ResetAllTasks()
        {
            InitializeAllTasks();
            SaveData();
        }
    }

    //===========================================================
    // 数据包装类(用于JSON序列化)
    //===========================================================

    /// <summary>
    /// 任务数据包装类
    /// </summary>
    [Serializable]
    public class TaskDataWrapper
    {
        public Dictionary<string, PlayerTaskData> Tasks;
        public long LastRefreshTime;
    }

    //===========================================================
    // 时间工具类(示例，实际项目中应有统一的时间工具)
    //===========================================================

    /// <summary>
    /// 时间工具类
    /// </summary>
    public static class TimeUtil
    {
        /// <summary>
        /// 获取当前时间戳(毫秒)
        /// </summary>
        public static long GetCurrentTimeMillis()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 时间戳转DateTime
        /// </summary>
        public static DateTime GetDateTime(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
        }

        /// <summary>
        /// DateTime转时间戳(毫秒)
        /// </summary>
        public static long ToUnixTimeMillis(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }
    }
}
