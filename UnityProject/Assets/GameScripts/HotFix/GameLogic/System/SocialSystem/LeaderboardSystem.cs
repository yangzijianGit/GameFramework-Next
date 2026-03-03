using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 排行榜数据类型
    //===========================================================

    /// <summary>
    /// 排行榜类型
    /// </summary>
    public enum LeaderboardType
    {
        /// <summary>总榜</summary>
        Total = 0,
        
        /// <summary>周榜</summary>
        Weekly = 1,
        
        /// <summary>好友榜</summary>
        Friends = 2,
        
        /// <summary>区域榜</summary>
        Region = 3,
    }

    /// <summary>
    /// 排行榜条目
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        /// <summary>排名</summary>
        public int Rank;
        
        /// <summary>玩家ID</summary>
        public string PlayerId;
        
        /// <summary>玩家名称</summary>
        public string PlayerName;
        
        /// <summary>玩家等级</summary>
        public int Level;
        
        /// <summary>头像</summary>
        public string HeadIcon;
        
        /// <summary>分数/战力</summary>
        public long Score;
        
        /// <summary>排名变化(+涨 -跌 =不变)</summary>
        public int RankChange;
        
        /// <summary>是否为当前玩家</summary>
        public bool IsCurrentPlayer;
    }

    //===========================================================
    // 排行榜系统接口
    //===========================================================

    /// <summary>
    /// 排行榜系统接口
    /// </summary>
    public interface ILeaderboardSystem
    {

        /// <summary>获取排行榜</summary>
        /// <param name="type">排行榜类型</param>
        /// <param name="offset">起始位置</param>
        /// <param name="count">获取数量</param>
        /// <returns>排行榜条目列表</returns>
        List<LeaderboardEntry> GetLeaderboard(LeaderboardType type, int offset = 0, int count = 20);

        /// <summary>获取玩家排名</summary>
        /// <param name="type">排行榜类型</param>
        /// <param name="playerId">玩家ID</param>
        /// <returns>排名(未上榜返回-1)</returns>
        int GetPlayerRank(LeaderboardType type, string playerId);

        /// <summary>上传分数</summary>
        /// <param name="type">排行榜类型</param>
        /// <param name="score">分数</param>
        /// <returns>是否成功</returns>
        bool SubmitScore(LeaderboardType type, long score);

        /// <summary>刷新排行榜</summary>
        /// <param name="type">排行榜类型</param>
        void RefreshLeaderboard(LeaderboardType type);

        /// <summary>获取我的排名信息</summary>
        /// <param name="type">排行榜类型</param>
        /// <returns>排名信息</returns>
        LeaderboardEntry GetMyRankInfo(LeaderboardType type);

        /// <summary>保存数据</summary>
        void SaveData();

        /// <summary>加载数据</summary>
        void LoadData();
    }

    //===========================================================
    // 排行榜系统实现
    //===========================================================

    /// <summary>
    /// 排行榜系统
    /// 职责：
    /// 1. 管理各类排行榜
    /// 2. 分数提交和排名更新
    /// 3. 排行榜数据缓存
    /// 4. 排名变化追踪
    /// 
    /// 扩展点：
    /// 1. 可接入服务器排行榜
    /// 2. 支持自定义排行榜类型
    /// 3. 支持多维度排序
    /// </summary>
    public class LeaderboardSystem : BaseLogicSys<LeaderboardSystem>, ILeaderboardSystem
    {
        //========================== 常量 ==========================

        private const string DATA_KEY = "LeaderboardData";
        private const int CACHE_EXPIRE_MINUTES = 5;

        //========================== 私有变量 ==========================

        /// <summary>排行榜缓存</summary>
        private Dictionary<LeaderboardType, List<LeaderboardEntry>> m_leaderboards = 
            new Dictionary<LeaderboardType, List<LeaderboardEntry>>();

        /// <summary>玩家排名缓存</summary>
        private Dictionary<LeaderboardType, int> m_playerRanks = 
            new Dictionary<LeaderboardType, int>();

        /// <summary>缓存时间</summary>
        private Dictionary<LeaderboardType, long> m_cacheTimes = 
            new Dictionary<LeaderboardType, long>();

        /// <summary>当前玩家ID</summary>
        private string m_currentPlayerId = "player_001";

        /// <summary>当前玩家名称</summary>
        private string m_currentPlayerName = "玩家";

        /// <summary>当前玩家分数</summary>
        private long m_currentPlayerScore = 0;

        /// <summary>是否已初始化</summary>
        private bool m_isInitialized = false;

        //========================== 生命周期 ==========================

        public override bool OnInit()
        {
            base.OnInit();

            LoadData();
            InitializeLeaderboards();

            m_isInitialized = true;
            Debug.Log("[LeaderboardSystem] Initialized");
            return true;
        }

        public override void OnDestroy()
        {
            SaveData();
            m_leaderboards.Clear();
            m_playerRanks.Clear();
            m_cacheTimes.Clear();
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 获取排行榜
        /// </summary>
        public List<LeaderboardEntry> GetLeaderboard(LeaderboardType type, int offset = 0, int count = 20)
        {
            // 检查缓存是否有效
            if (!IsCacheValid(type))
            {
                RefreshLeaderboard(type);
            }

            if (m_leaderboards.TryGetValue(type, out var list))
            {
                int endIndex = Mathf.Min(offset + count, list.Count);
                if (offset < list.Count)
                {
                    return list.GetRange(offset, endIndex - offset);
                }
            }

            return new List<LeaderboardEntry>();
        }

        /// <summary>
        /// 获取玩家排名
        /// </summary>
        public int GetPlayerRank(LeaderboardType type, string playerId)
        {
            if (!IsCacheValid(type))
            {
                RefreshLeaderboard(type);
            }

            if (m_leaderboards.TryGetValue(type, out var list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].PlayerId == playerId)
                    {
                        return i + 1;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// 上传分数
        /// </summary>
        public bool SubmitScore(LeaderboardType type, long score)
        {
            // 更新当前玩家分数
            m_currentPlayerScore = (long)Mathf.Max(m_currentPlayerScore, score);

            // 实际项目中应发送到服务器
            // 这里简化处理，直接更新本地排名

            Debug.Log($"[LeaderboardSystem] Score submitted: type={type}, score={score}");

            // 刷新排行榜
            RefreshLeaderboard(type);

            return true;
        }

        /// <summary>
        /// 刷新排行榜
        /// </summary>
        public void RefreshLeaderboard(LeaderboardType type)
        {
            // 实际项目中应从服务器获取
            // 这里生成模拟数据
            GenerateSampleLeaderboard(type);

            // 更新缓存时间
            m_cacheTimes[type] = TimeUtil.GetCurrentTimeMillis();

            Debug.Log($"[LeaderboardSystem] Leaderboard refreshed: {type}");
        }

        /// <summary>
        /// 获取我的排名信息
        /// </summary>
        public LeaderboardEntry GetMyRankInfo(LeaderboardType type)
        {
            int rank = GetPlayerRank(type, m_currentPlayerId);
            if (rank <= 0) return null;

            return new LeaderboardEntry
            {
                Rank = rank,
                PlayerId = m_currentPlayerId,
                PlayerName = m_currentPlayerName,
                Level = 1,
                Score = m_currentPlayerScore,
                RankChange = 0,
                IsCurrentPlayer = true
            };
        }

        //========================== 数据持久化 ==========================

        public void SaveData()
        {
            var wrapper = new LeaderboardDataWrapper
            {
                CurrentPlayerId = m_currentPlayerId,
                CurrentPlayerName = m_currentPlayerName,
                CurrentPlayerScore = m_currentPlayerScore
            };
            var json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(DATA_KEY, json);
            Debug.Log("[LeaderboardSystem] Data saved");
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
                var wrapper = JsonUtility.FromJson<LeaderboardDataWrapper>(json);
                if (wrapper != null)
                {
                    m_currentPlayerId = wrapper.CurrentPlayerId;
                    m_currentPlayerName = wrapper.CurrentPlayerName;
                    m_currentPlayerScore = wrapper.CurrentPlayerScore;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[LeaderboardSystem] Failed to load data: {e.Message}");
            }
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 初始化排行榜
        /// </summary>
        private void InitializeLeaderboards()
        {
            foreach (LeaderboardType type in Enum.GetValues(typeof(LeaderboardType)))
            {
                GenerateSampleLeaderboard(type);
                m_cacheTimes[type] = TimeUtil.GetCurrentTimeMillis();
            }
        }

        /// <summary>
        /// 生成示例排行榜
        /// </summary>
        private void GenerateSampleLeaderboard(LeaderboardType type)
        {
            var entries = new List<LeaderboardEntry>();

            // 根据类型生成不同的数据
            string[] names = { "玩家A", "玩家B", "玩家C", "玩家D", "玩家E", 
                               "玩家F", "玩家G", "玩家H", "玩家I", "玩家J" };
            
            int baseScore = 100000;
            switch (type)
            {
                case LeaderboardType.Total:
                    baseScore = 100000;
                    break;
                case LeaderboardType.Weekly:
                    baseScore = 10000;
                    break;
                case LeaderboardType.Friends:
                    baseScore = 5000;
                    break;
                case LeaderboardType.Region:
                    baseScore = 80000;
                    break;
            }

            for (int i = 0; i < names.Length; i++)
            {
                // 生成排名变化
                int rankChange = UnityEngine.Random.Range(-3, 4);

                entries.Add(new LeaderboardEntry
                {
                    Rank = i + 1,
                    PlayerId = $"player_{i + 1}",
                    PlayerName = names[i],
                    Level = UnityEngine.Random.Range(20, 60),
                    Score = baseScore - i * UnityEngine.Random.Range(500, 2000),
                    RankChange = rankChange,
                    IsCurrentPlayer = false
                });
            }

            // 确保当前玩家在榜单中
            int playerRank = UnityEngine.Random.Range(3, 8);
            var myEntry = new LeaderboardEntry
            {
                Rank = playerRank,
                PlayerId = m_currentPlayerId,
                PlayerName = m_currentPlayerName,
                Level = 30,
                Score = m_currentPlayerScore > 0 ? m_currentPlayerScore : baseScore - playerRank * 1000,
                RankChange = UnityEngine.Random.Range(-2, 3),
                IsCurrentPlayer = true
            };
            entries.Insert(playerRank - 1, myEntry);


            // 重新排序
            entries.Sort((a, b) => b.Score.CompareTo(a.Score));

            // 重新设置排名
            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].Rank = i + 1;
            }

            m_leaderboards[type] = entries;
            m_playerRanks[type] = playerRank;
        }

        /// <summary>
        /// 检查缓存是否有效
        /// </summary>
        private bool IsCacheValid(LeaderboardType type)
        {
            if (!m_cacheTimes.TryGetValue(type, out var cacheTime))
                return false;

            var now = TimeUtil.GetCurrentTimeMillis();
            var elapsedMinutes = (now - cacheTime) / (1000 * 60);

            return elapsedMinutes < CACHE_EXPIRE_MINUTES;
        }

        //========================== 测试接口 ==========================

        /// <summary>
        /// 测试：强制刷新所有排行榜
        /// </summary>
        public void Test_RefreshAll()
        {
            foreach (LeaderboardType type in Enum.GetValues(typeof(LeaderboardType)))
            {
                RefreshLeaderboard(type);
            }
        }

        /// <summary>
        /// 测试：上传测试分数
        /// </summary>
        public void Test_SubmitScore(long score)
        {
            SubmitScore(LeaderboardType.Total, score);
        }
    }

    //===========================================================
    // 数据包装类
    //===========================================================

    [Serializable]
    public class LeaderboardDataWrapper
    {
        public string CurrentPlayerId;
        public string CurrentPlayerName;
        public long CurrentPlayerScore;
    }
}
