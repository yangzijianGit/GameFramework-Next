using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 关卡选择数据类型
    //===========================================================

    /// <summary>
    /// 关卡状态
    /// </summary>
    public enum LevelState
    {
        /// <summary>未解锁</summary>
        Locked = 0,
        
        /// <summary>可挑战</summary>
        Available = 1,
        
        /// <summary>已通关(1星)</summary>
        Passed_1Star = 2,
        
        /// <summary>已通关(2星)</summary>
        Passed_2Star = 3,
        
        /// <summary>已通关(3星)</summary>
        Passed_3Star = 4,
        
        /// <summary>今日已玩</summary>
        PlayedToday = 5,
    }

    /// <summary>
    /// 关卡难度
    /// </summary>
    public enum LevelDifficulty
    {
        /// <summary>普通</summary>
        Normal = 0,
        
        /// <summary>精英</summary>
        Elite = 1,
        
        /// <summary>Boss</summary>
        Boss = 2,
    }

    /// <summary>
    /// 关卡数据
    /// </summary>
    [Serializable]
    public class LevelData
    {
        /// <summary>关卡ID</summary>
        public string LevelId;
        
        /// <summary>关卡名称</summary>
        public string Name;
        
        /// <summary>关卡难度</summary>
        public LevelDifficulty Difficulty;
        
        /// <summary>前置关卡ID</summary>
        public string PreLevelId;
        
        /// <summary>解锁条件(玩家等级)</summary>
        public int UnlockPlayerLevel;
        
        /// <summary>解锁消耗(道具ID和数量)</summary>
        public Dictionary<string, int> UnlockCost = new Dictionary<string, int>();
        
        /// <summary>最大步数</summary>
        public int MaxMoves;
        
        /// <summary>目标类型</summary>
        public string TargetType;
        
        /// <summary>目标值</summary>
        public int TargetValue;
        
        /// <summary>1星条件</summary>
        public int Star1Value;
        
        /// <summary>2星条件</summary>
        public int Star2Value;
        
        /// <summary>3星条件</summary>
        public int Star3Value;
        
        /// <summary>首次通关奖励</summary>
        public Dictionary<string, int> FirstPassReward = new Dictionary<string, int>();
        
        /// <summary>星级奖励</summary>
        public Dictionary<int, Dictionary<string, int>> StarRewards = new Dictionary<int, Dictionary<string, int>>();
        
        /// <summary>每日挑战次数限制</summary>
        public int DailyPlayLimit = 999;
        
        /// <summary>是否隐藏</summary>
        public bool IsHidden;
    }

    /// <summary>
    /// 玩家关卡数据
    /// </summary>
    [Serializable]
    public class PlayerLevelData
    {
        /// <summary>关卡ID</summary>
        public string LevelId;
        
        /// <summary>关卡状态</summary>
        public LevelState State;
        
        /// <summary>最高星级</summary>
        public int BestStar;
        
        /// <summary>最高分数</summary>
        public long BestScore;
        
        /// <summary>通关次数</summary>
        public int PassCount;
        
        /// <summary>今日已玩次数</summary>
        public int TodayPlayCount;
        
        /// <summary>最后挑战时间</summary>
        public long LastPlayTime;
    }

    //===========================================================
    // 关卡选择系统接口
    //===========================================================

    /// <summary>
    /// 关卡选择系统接口
    /// </summary>
    public interface ILevelSelectSystem
    {
        /// <summary>显示关卡选择</summary>

        /// <summary>初始化系统</summary>
        void Initialize();

        /// <summary>显示关卡选择</summary>
        void Show();

        /// <summary>隐藏关卡选择</summary>
        void Hide();

        /// <summary>获取关卡数据</summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>关卡数据</returns>
        LevelData GetLevelData(string levelId);

        /// <summary>获取玩家关卡数据</summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>玩家数据</returns>
        PlayerLevelData GetPlayerLevelData(string levelId);

        /// <summary>获取关卡列表</summary>
        /// <param name="difficulty">难度筛选</param>
        /// <returns>关卡列表</returns>
        List<LevelData> GetLevelList(LevelDifficulty? difficulty = null);

        /// <summary>获取当前可挑战关卡</summary>
        /// <returns>关卡列表</returns>
        List<LevelData> GetAvailableLevels();

        /// <summary>进入关卡</summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>是否成功</returns>
        bool EnterLevel(string levelId);

        /// <summary>扫荡关卡</summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>扫荡结果</returns>
        SweepResult SweepLevel(string levelId);

        /// <summary>检查关卡是否解锁</summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>是否解锁</returns>
        bool IsLevelUnlocked(string levelId);

        /// <summary>保存数据</summary>
        void SaveData();

        /// <summary>加载数据</summary>
        void LoadData();
    }

    /// <summary>
    /// 扫荡结果
    /// </summary>
    public class SweepResult
    {
        /// <summary>是否成功</summary>
        public bool Success;
        
        /// <summary>错误信息</summary>
        public string ErrorMessage;
        
        /// <summary>获得奖励</summary>
        public Dictionary<string, int> Rewards;
        
        /// <summary>扫荡次数</summary>
        public int SweepCount;
    }

    //===========================================================
    // 关卡选择系统实现
    //===========================================================

    /// <summary>
    /// 关卡选择系统
    /// 职责：
    /// 1. 管理关卡数据
    /// 2. 关卡解锁状态管理
    /// 3. 关卡进入和扫荡
    /// 4. 关卡进度追踪
    /// 5. 星级评价
    /// 
    /// 扩展点：
    /// 1. 可配置关卡解锁条件
    /// 2. 支持扫荡功能
    /// 3. 支持章节目录
    /// </summary>
    public class LevelSelectSystem : BaseLogicSys<LevelSelectSystem>, ILevelSelectSystem
    {
        //========================== 常量 ==========================

        private const string DATA_KEY = "LevelSelectData";

        //========================== 私有变量 ==========================

        /// <summary>关卡配置</summary>
        private Dictionary<string, LevelData> m_levelConfigs = new Dictionary<string, LevelData>();

        /// <summary>玩家关卡数据</summary>
        private Dictionary<string, PlayerLevelData> m_playerLevels = new Dictionary<string, PlayerLevelData>();

        /// <summary>当前状态</summary>
        private bool m_isShown = false;

        /// <summary>是否已初始化</summary>
        private bool m_isInitialized = false;

        //========================== 回调 ==========================

        /// <summary>显示回调</summary>
        public Action OnShow;

        /// <summary>隐藏回调</summary>
        public Action OnHide;

        /// <summary>进入关卡回调</summary>
        public Action<string> OnEnterLevel;

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

            LoadLevelConfigs();
            LoadData();
            UpdateLevelStates();

            m_isInitialized = true;
            Debug.Log($"[LevelSelectSystem] Initialized with {m_levelConfigs.Count} levels");
            return true;
        }

        public override void OnDestroy()
        {
            SaveData();
            m_levelConfigs.Clear();
            m_playerLevels.Clear();
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 显示关卡选择
        /// </summary>
        public void Show()
        {
            if (m_isShown) return;

            m_isShown = true;
            UpdateLevelStates();
            OnShow?.Invoke();
            Debug.Log("[LevelSelectSystem] Level select shown");
        }

        /// <summary>
        /// 隐藏关卡选择
        /// </summary>
        public void Hide()
        {
            if (!m_isShown) return;

            m_isShown = false;
            OnHide?.Invoke();
            Debug.Log("[LevelSelectSystem] Level select hidden");
        }

        /// <summary>
        /// 获取关卡数据
        /// </summary>
        public LevelData GetLevelData(string levelId)
        {
            if (m_levelConfigs.TryGetValue(levelId, out var data))
                return data;
            return null;
        }

        /// <summary>
        /// 获取玩家关卡数据
        /// </summary>
        public PlayerLevelData GetPlayerLevelData(string levelId)
        {
            if (m_playerLevels.TryGetValue(levelId, out var data))
                return data;
            return null;
        }

        /// <summary>
        /// 获取关卡列表
        /// </summary>
        public List<LevelData> GetLevelList(LevelDifficulty? difficulty = null)
        {
            var result = new List<LevelData>();

            foreach (var level in m_levelConfigs.Values)
            {
                if (difficulty.HasValue && level.Difficulty != difficulty.Value)
                    continue;
                result.Add(level);
            }

            // 按ID排序
            result.Sort((a, b) => string.Compare(a.LevelId, b.LevelId, StringComparison.Ordinal));
            return result;
        }

        /// <summary>
        /// 获取可挑战关卡
        /// </summary>
        public List<LevelData> GetAvailableLevels()
        {
            var result = new List<LevelData>();

            foreach (var level in m_levelConfigs.Values)
            {
                var playerData = GetPlayerLevelData(level.LevelId);
                if (playerData == null)
                {
                    // 未挑战过，检查解锁条件
                    if (IsLevelUnlocked(level.LevelId))
                    {
                        result.Add(level);
                    }
                }
                else if (playerData.State == LevelState.Available || 
                         playerData.State == LevelState.Passed_1Star ||
                         playerData.State == LevelState.Passed_2Star ||
                         playerData.State == LevelState.Passed_3Star)
                {
                    result.Add(level);
                }
            }

            return result;
        }

        /// <summary>
        /// 进入关卡
        /// </summary>
        public bool EnterLevel(string levelId)
        {
            var level = GetLevelData(levelId);
            if (level == null)
            {
                Debug.LogWarning($"[LevelSelectSystem] Level not found: {levelId}");
                return false;
            }

            // 检查解锁
            if (!IsLevelUnlocked(levelId))
            {
                Debug.LogWarning($"[LevelSelectSystem] Level locked: {levelId}");
                return false;
            }

            // 检查体力
            // var energy = PlayerData.Instance.GetCurrencyCount("energy");
            // if (energy < 1)
            // {
            //     Debug.LogWarning("[LevelSelectSystem] Not enough energy");
            //     return false;
            // }

            // 消耗体力
            // PlayerData.Instance.AddCurrency("energy", -1);

            // 记录开始时间
            var playerData = GetPlayerLevelData(levelId);
            if (playerData == null)
            {
                playerData = new PlayerLevelData
                {
                    LevelId = levelId,
                    State = LevelState.Available
                };
                m_playerLevels[levelId] = playerData;
            }

            playerData.TodayPlayCount++;
            playerData.LastPlayTime = TimeUtil.GetCurrentTimeMillis();

            SaveData();

            // 触发回调
            OnEnterLevel?.Invoke(levelId);

            Debug.Log($"[LevelSelectSystem] Enter level: {levelId}");
            return true;
        }

        /// <summary>
        /// 扫荡关卡
        /// </summary>
        public SweepResult SweepLevel(string levelId)
        {
            var result = new SweepResult();

            var level = GetLevelData(levelId);
            if (level == null)
            {
                result.Success = false;
                result.ErrorMessage = "Level not found";
                return result;
            }

            var playerData = GetPlayerLevelData(levelId);
            if (playerData == null || playerData.BestStar < 1)
            {
                result.Success = false;
                result.ErrorMessage = "Level not passed";
                return result;
            }

            // 扫荡次数(根据VIP等级或道具)
            result.SweepCount = 1;
            result.Rewards = new Dictionary<string, int>();

            // 根据星级发放奖励
            if (playerData.BestStar >= 1 && level.StarRewards.TryGetValue(1, out var star1Rewards))
            {
                foreach (var reward in star1Rewards)
                {
                    result.Rewards[reward.Key] = reward.Value * result.SweepCount;
                }
            }

            // 发放奖励
            foreach (var reward in result.Rewards)
            {
                Debug.Log($"[LevelSelectSystem] Sweep reward: {reward.Key} x {reward.Value}");
            }

            result.Success = true;
            Debug.Log($"[LevelSelectSystem] Level swept: {levelId}, count: {result.SweepCount}");
            return result;
        }

        /// <summary>
        /// 检查关卡是否解锁
        /// </summary>
        public bool IsLevelUnlocked(string levelId)
        {
            var level = GetLevelData(levelId);
            if (level == null) return false;

            // 检查玩家等级
            // if (PlayerData.Instance.Level < level.UnlockPlayerLevel)
            //     return false;

            // 检查前置关卡
            if (!string.IsNullOrEmpty(level.PreLevelId))
            {
                var prePlayerData = GetPlayerLevelData(level.PreLevelId);
                if (prePlayerData == null || prePlayerData.BestStar < 1)
                    return false;
            }

            return true;
        }

        //========================== 数据持久化 ==========================

        public void SaveData()
        {
            var wrapper = new LevelSelectDataWrapper
            {
                PlayerLevels = m_playerLevels
            };
            var json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(DATA_KEY, json);
            Debug.Log("[LevelSelectSystem] Data saved");
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
                var wrapper = JsonUtility.FromJson<LevelSelectDataWrapper>(json);
                if (wrapper != null)
                {
                    m_playerLevels = wrapper.PlayerLevels ?? new Dictionary<string, PlayerLevelData>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[LevelSelectSystem] Failed to load data: {e.Message}");
            }
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 加载关卡配置
        /// </summary>
        private void LoadLevelConfigs()
        {
            m_levelConfigs.Clear();

            // 创建示例关卡配置
            CreateSampleLevels();
        }

        /// <summary>
        /// 创建示例关卡
        /// </summary>
        private void CreateSampleLevels()
        {
            // 第1章
            for (int i = 1; i <= 15; i++)
            {
                string levelId = $"level_{i:D3}";
                string preLevelId = i > 1 ? $"level_{i - 1:D3}" : "";

                var level = new LevelData
                {
                    LevelId = levelId,
                    Name = $"第{i}关",
                    Difficulty = LevelDifficulty.Normal,
                    PreLevelId = preLevelId,
                    UnlockPlayerLevel = 1,
                    MaxMoves = 15 + (i / 3),
                    TargetType = "score",
                    TargetValue = 1000 + i * 500,
                    Star1Value = 1000 + i * 500,
                    Star2Value = 1500 + i * 700,
                    Star3Value = 2000 + i * 1000,
                    FirstPassReward = new Dictionary<string, int>
                    {
                        { "diamond", 10 + i },
                        { "star", 5 }
                    },
                    StarRewards = new Dictionary<int, Dictionary<string, int>>
                    {
                        { 1, new Dictionary<string, int> { { "diamond", 5 } } },
                        { 2, new Dictionary<string, int> { { "diamond", 10 }, { "star", 2 } } },
                        { 3, new Dictionary<string, int> { { "diamond", 15 }, { "star", 5 } } }
                    },
                    DailyPlayLimit = 999,
                    IsHidden = false
                };

                m_levelConfigs[levelId] = level;
            }

            // Boss关卡
            m_levelConfigs["level_boss_1"] = new LevelData
            {
                LevelId = "level_boss_1",
                Name = "Boss战-章鱼博士",
                Difficulty = LevelDifficulty.Boss,
                PreLevelId = "level_015",
                UnlockPlayerLevel = 5,
                MaxMoves = 25,
                TargetType = "damage",
                TargetValue = 10000,
                Star1Value = 10000,
                Star2Value = 15000,
                Star3Value = 20000,
                FirstPassReward = new Dictionary<string, int>
                {
                    { "diamond", 100 },
                    { "clothes", 1 }
                },
                StarRewards = new Dictionary<int, Dictionary<string, int>>
                {
                    { 1, new Dictionary<string, int> { { "diamond", 50 } } },
                    { 2, new Dictionary<string, int> { { "diamond", 80 } } },
                    { 3, new Dictionary<string, int> { { "diamond", 100 }, { "clothes", 1 } } }
                },
                DailyPlayLimit = 3,
                IsHidden = false
            };

            Debug.Log($"[LevelSelectSystem] Created {m_levelConfigs.Count} levels");
        }

        /// <summary>
        /// 更新关卡状态
        /// </summary>
        private void UpdateLevelStates()
        {
            foreach (var level in m_levelConfigs.Values)
            {
                var playerData = GetPlayerLevelData(level.LevelId);

                if (playerData == null)
                {
                    // 未挑战过
                    if (IsLevelUnlocked(level.LevelId))
                    {
                        // 已解锁
                        playerData = new PlayerLevelData
                        {
                            LevelId = level.LevelId,
                            State = LevelState.Available
                        };
                        m_playerLevels[level.LevelId] = playerData;
                    }
                }
                else
                {
                    // 已挑战过，更新状态
                    if (playerData.BestStar >= 3)
                        playerData.State = LevelState.Passed_3Star;
                    else if (playerData.BestStar >= 2)
                        playerData.State = LevelState.Passed_2Star;
                    else if (playerData.BestStar >= 1)
                        playerData.State = LevelState.Passed_1Star;

                    // 检查今日次数
                    if (playerData.TodayPlayCount >= level.DailyPlayLimit)
                    {
                        playerData.State = LevelState.PlayedToday;
                    }
                }
            }
        }

        /// <summary>
        /// 通关关卡记录
        /// </summary>
        public void OnLevelPassed(string levelId, int star, long score)
        {
            var playerData = GetPlayerLevelData(levelId);
            if (playerData == null)
            {
                playerData = new PlayerLevelData { LevelId = levelId };
                m_playerLevels[levelId] = playerData;
            }

            // 更新数据
            playerData.PassCount++;
            playerData.LastPlayTime = TimeUtil.GetCurrentTimeMillis();

            if (star > playerData.BestStar)
                playerData.BestStar = star;

            if (score > playerData.BestScore)
                playerData.BestScore = score;

            // 更新状态
            if (star >= 3)
                playerData.State = LevelState.Passed_3Star;
            else if (star >= 2)
                playerData.State = LevelState.Passed_2Star;
            else
                playerData.State = LevelState.Passed_1Star;

            // 检查下一关解锁
            var level = GetLevelData(levelId);
            if (level != null)
            {
                // 这里可以添加自动解锁下一关的逻辑
            }

            SaveData();
            Debug.Log($"[LevelSelectSystem] Level passed: {levelId}, star: {star}, score: {score}");
        }
    }

    //===========================================================
    // 数据包装类
    //===========================================================

    [Serializable]
    public class LevelSelectDataWrapper
    {
        public Dictionary<string, PlayerLevelData> PlayerLevels;
    }
}
