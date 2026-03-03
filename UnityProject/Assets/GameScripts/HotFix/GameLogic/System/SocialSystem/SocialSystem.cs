using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 社交系统数据类型
    //===========================================================

    /// <summary>
    /// 好友数据
    /// </summary>
    [Serializable]
    public class FriendData
    {
        /// <summary>玩家ID</summary>
        public string PlayerId;
        
        /// <summary>玩家名称</summary>
        public string PlayerName;
        
        /// <summary>玩家等级</summary>
        public int Level;
        
        /// <summary>战力</summary>
        public int Power;
        
        /// <summary>胜率</summary>
        public float WinRate;
        
        /// <summary>头像</summary>
        public string HeadIcon;
        
        /// <summary>最后在线时间</summary>
        public long LastOnlineTime;
        
        /// <summary>好友状态</summary>
        public FriendState State;
    }

    /// <summary>
    /// 好友状态
    /// </summary>
    public enum FriendState
    {
        /// <summary>待同意</summary>
        Pending = 0,
        
        /// <summary>已是好友</summary>
        Friend = 1,
        
        /// <summary>已拉黑</summary>
        Blocked = 2,
    }

    /// <summary>
    /// 社交动作类型
    /// </summary>
    public enum SocialActionType
    {
        /// <summary>送体力</summary>
        SendEnergy = 0,
        
        /// <summary>领取体力</summary>
        ReceiveEnergy = 1,
        
        /// <summary>点赞</summary>
        Like = 2,
        
        /// <summary>复仇</summary>
        Revenge = 3,
    }

    //===========================================================
    // 社交系统接口
    //===========================================================

    /// <summary>
    /// 社交系统接口
    /// </summary>
    public interface ISocialSystem
    {
        /// <summary>获取好友列表</summary>

        /// <summary>初始化系统</summary>
        void Initialize();

        /// <summary>获取好友列表</summary>
        /// <returns>好友列表</returns>
        List<FriendData> GetFriendList();

        /// <summary>获取好友请求列表</summary>
        /// <returns>请求列表</returns>
        List<FriendData> GetFriendRequestList();

        /// <summary>获取推荐好友列表</summary>
        /// <returns>推荐列表</returns>
        List<FriendData> GetRecommendedFriends();

        /// <summary>发送好友请求</summary>
        /// <param name="playerId">玩家ID</param>
        /// <returns>是否成功</returns>
        bool SendFriendRequest(string playerId);

        /// <summary>处理好友请求</summary>
        /// <param name="playerId">玩家ID</param>
        /// <param name="accept">是否接受</param>
        /// <returns>是否成功</returns>
        bool HandleFriendRequest(string playerId, bool accept);

        /// <summary>删除好友</summary>
        /// <param name="playerId">玩家ID</param>
        /// <returns>是否成功</returns>
        bool RemoveFriend(string playerId);

        /// <summary>执行社交动作</summary>
        /// <param name="playerId">玩家ID</param>
        /// <param name="action">动作类型</param>
        /// <returns>是否成功</returns>
        bool DoSocialAction(string playerId, SocialActionType action);

        /// <summary>获取可赠送体力的好友</summary>
        /// <returns>好友列表</returns>
        List<FriendData> GetFriendsCanSendEnergy();

        /// <summary>获取可领取体力的好友</summary>
        /// <returns>好友列表</returns>
        List<FriendData> GetFriendsCanReceiveEnergy();

        /// <summary>保存数据</summary>
        void SaveData();

        /// <summary>加载数据</summary>
        void LoadData();
    }

    //===========================================================
    // 社交系统实现
    //===========================================================

    /// <summary>
    /// 社交系统
    /// 职责：
    /// 1. 好友管理(添加/删除/拉黑)
    /// 2. 好友请求处理
    /// 3. 社交互动(送体力/点赞/复仇)
    /// 4. 推荐好友
    /// 
    /// 扩展点：
    /// 1. 可扩展社交动作类型
    /// 2. 可接入真实社交API
    /// </summary>
    public class SocialSystem : BaseLogicSys<SocialSystem>, ISocialSystem
    {
        //========================== 常量 ==========================

        private const string DATA_KEY = "SocialData";
        private const int MAX_FRIENDS = 100;
        private const int ENERGY_SEND_AMOUNT = 5;
        private const int ENERGY_RECEIVE_AMOUNT = 5;

        //========================== 私有变量 ==========================

        /// <summary>好友列表</summary>
        private List<FriendData> m_friends = new List<FriendData>();

        /// <summary>好友请求列表</summary>
        private List<FriendData> m_friendRequests = new List<FriendData>();

        /// <summary>今日已赠送体力列表</summary>
        private HashSet<string> m_sentEnergyToday = new HashSet<string>();

        /// <summary>今日已领取体力列表</summary>
        private HashSet<string> m_receivedEnergyToday = new HashSet<string>();

        /// <summary>是否已初始化</summary>
        private bool m_isInitialized = false;

        //========================== 生命周期 ==========================

        /// <summary>
        /// 初始化系统
        /// </summary>
        public void Initialize()
        {
            OnInit();
        }

        public override bool OnInit()
        {
            base.OnInit();

            LoadData();
            
            // 检查并重置每日数据
            CheckAndResetDailyData();

            m_isInitialized = true;
            Debug.Log($"[SocialSystem] Initialized with {m_friends.Count} friends");
            return true;
        }

        public override void OnDestroy()
        {
            SaveData();
            m_friends.Clear();
            m_friendRequests.Clear();
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 获取好友列表
        /// </summary>
        public List<FriendData> GetFriendList()
        {
            return m_friends.FindAll(f => f.State == FriendState.Friend);
        }

        /// <summary>
        /// 获取好友请求列表
        /// </summary>
        public List<FriendData> GetFriendRequestList()
        {
            return m_friendRequests;
        }

        /// <summary>
        /// 获取推荐好友列表
        /// </summary>
        public List<FriendData> GetRecommendedFriends()
        {
            // 实际项目中应从服务器获取推荐列表
            // 这里返回模拟数据
            var recommended = new List<FriendData>
            {
                new FriendData { PlayerId = "rec_1", PlayerName = "推荐玩家1", Level = 30, Power = 50000 },
                new FriendData { PlayerId = "rec_2", PlayerName = "推荐玩家2", Level = 25, Power = 40000 },
                new FriendData { PlayerId = "rec_3", PlayerName = "推荐玩家3", Level = 28, Power = 45000 },
            };
            return recommended;
        }

        /// <summary>
        /// 发送好友请求
        /// </summary>
        public bool SendFriendRequest(string playerId)
        {
            if (m_friends.Count >= MAX_FRIENDS)
            {
                Debug.LogWarning("[SocialSystem] Friend list is full");
                return false;
            }

            // 检查是否已经是好友
            if (m_friends.Exists(f => f.PlayerId == playerId))
            {
                Debug.LogWarning("[SocialSystem] Already a friend");
                return false;
            }

            // 实际项目中应发送请求到服务器
            Debug.Log($"[SocialSystem] Friend request sent to: {playerId}");
            return true;
        }

        /// <summary>
        /// 处理好友请求
        /// </summary>
        public bool HandleFriendRequest(string playerId, bool accept)
        {
            var request = m_friendRequests.Find(r => r.PlayerId == playerId);
            if (request == null)
            {
                Debug.LogWarning("[SocialSystem] Friend request not found");
                return false;
            }

            if (accept)
            {
                // 添加为好友
                request.State = FriendState.Friend;
                m_friends.Add(request);
                Debug.Log($"[SocialSystem] Friend request accepted: {playerId}");
            }

            // 移除请求
            m_friendRequests.Remove(request);
            SaveData();
            return true;
        }

        /// <summary>
        /// 删除好友
        /// </summary>
        public bool RemoveFriend(string playerId)
        {
            var friend = m_friends.Find(f => f.PlayerId == playerId && f.State == FriendState.Friend);
            if (friend == null)
            {
                Debug.LogWarning("[SocialSystem] Friend not found");
                return false;
            }

            m_friends.Remove(friend);
            SaveData();
            Debug.Log($"[SocialSystem] Friend removed: {playerId}");
            return true;
        }

        /// <summary>
        /// 执行社交动作
        /// </summary>
        public bool DoSocialAction(string playerId, SocialActionType action)
        {
            var friend = m_friends.Find(f => f.PlayerId == playerId && f.State == FriendState.Friend);
            if (friend == null)
            {
                Debug.LogWarning("[SocialSystem] Friend not found");
                return false;
            }

            switch (action)
            {
                case SocialActionType.SendEnergy:
                    return SendEnergy(playerId);

                case SocialActionType.ReceiveEnergy:
                    return ReceiveEnergy(playerId);

                case SocialActionType.Like:
                    return LikeFriend(playerId);

                case SocialActionType.Revenge:
                    return RevengeFriend(playerId);

                default:
                    return false;
            }
        }

        /// <summary>
        /// 获取可赠送体力的好友
        /// </summary>
        public List<FriendData> GetFriendsCanSendEnergy()
        {
            return m_friends.FindAll(f => 
                f.State == FriendState.Friend && 
                !m_sentEnergyToday.Contains(f.PlayerId));
        }

        /// <summary>
        /// 获取可领取体力的好友
        /// </summary>
        public List<FriendData> GetFriendsCanReceiveEnergy()
        {
            return m_friends.FindAll(f => 
                f.State == FriendState.Friend && 
                m_sentEnergyToday.Contains(f.PlayerId) &&
                !m_receivedEnergyToday.Contains(f.PlayerId));
        }

        //========================== 社交动作实现 ==========================

        /// <summary>
        /// 送体力
        /// </summary>
        private bool SendEnergy(string playerId)
        {
            if (m_sentEnergyToday.Contains(playerId))
            {
                Debug.LogWarning("[SocialSystem] Already sent energy today");
                return false;
            }

            m_sentEnergyToday.Add(playerId);
            
            // 实际项目中应通知服务器
            Debug.Log($"[SocialSystem] Energy sent to: {playerId}");
            SaveData();
            return true;
        }

        /// <summary>
        /// 领体力
        /// </summary>
        private bool ReceiveEnergy(string playerId)
        {
            if (!m_sentEnergyToday.Contains(playerId))
            {
                Debug.LogWarning("[SocialSystem] No energy to receive");
                return false;
            }

            if (m_receivedEnergyToday.Contains(playerId))
            {
                Debug.LogWarning("[SocialSystem] Already received energy today");
                return false;
            }

            m_receivedEnergyToday.Add(playerId);

            // 发放体力
            // PlayerData.Instance.AddEnergy(ENERGY_RECEIVE_AMOUNT);
            Debug.Log($"[SocialSystem] Energy received from: {playerId}, amount: {ENERGY_RECEIVE_AMOUNT}");
            
            SaveData();
            return true;
        }

        /// <summary>
        /// 点赞
        /// </summary>
        private bool LikeFriend(string playerId)
        {
            // 实际项目中应发送点赞到服务器
            Debug.Log($"[SocialSystem] Liked friend: {playerId}");
            return true;
        }

        /// <summary>
        /// 复仇
        /// </summary>
        private bool RevengeFriend(string playerId)
        {
            // 复仇逻辑：进入对战场景
            Debug.Log($"[SocialSystem] Revenge against: {playerId}");
            return true;
        }

        //========================== 数据持久化 ==========================

        public void SaveData()
        {
            var wrapper = new SocialDataWrapper
            {
                Friends = m_friends,
                FriendRequests = m_friendRequests
            };
            var json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(DATA_KEY, json);
            Debug.Log("[SocialSystem] Data saved");
        }

        public void LoadData()
        {
            if (!PlayerPrefs.HasKey(DATA_KEY))
            {
                CreateSampleData();
                return;
            }

            try
            {
                var json = PlayerPrefs.GetString(DATA_KEY);
                var wrapper = JsonUtility.FromJson<SocialDataWrapper>(json);
                if (wrapper != null)
                {
                    m_friends = wrapper.Friends ?? new List<FriendData>();
                    m_friendRequests = wrapper.FriendRequests ?? new List<FriendData>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SocialSystem] Failed to load data: {e.Message}");
                CreateSampleData();
            }
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 创建示例数据
        /// </summary>
        private void CreateSampleData()
        {
            m_friends = new List<FriendData>
            {
                new FriendData { PlayerId = "friend_1", PlayerName = "张三", Level = 45, Power = 80000, WinRate = 0.75f, State = FriendState.Friend },
                new FriendData { PlayerId = "friend_2", PlayerName = "李四", Level = 38, Power = 65000, WinRate = 0.68f, State = FriendState.Friend },
            };
        }

        /// <summary>
        /// 检查并重置每日数据
        /// </summary>
        private void CheckAndResetDailyData()
        {
            // 实际项目中应根据日期重置
            // 这里简化处理
            if (m_sentEnergyToday.Count > 0 || m_receivedEnergyToday.Count > 0)
            {
                // 重置每日数据
                m_sentEnergyToday.Clear();
                m_receivedEnergyToday.Clear();
                Debug.Log("[SocialSystem] Daily data reset");
            }
        }

        //========================== 测试接口 ==========================

        public void Test_AddFriend(string playerId, string playerName)
        {
            m_friends.Add(new FriendData
            {
                PlayerId = playerId,
                PlayerName = playerName,
                Level = 1,
                Power = 0,
                WinRate = 0,
                State = FriendState.Friend
            });
            SaveData();
        }
    }

    //===========================================================
    // 数据包装类
    //===========================================================

    [Serializable]
    public class SocialDataWrapper
    {
        public List<FriendData> Friends;
        public List<FriendData> FriendRequests;
    }
}
