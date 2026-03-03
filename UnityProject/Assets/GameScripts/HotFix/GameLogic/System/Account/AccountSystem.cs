using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 账户系统数据类型
    //===========================================================

    /// <summary>
    /// 登录方式
    /// </summary>
    public enum LoginType
    {
        /// <summary>游客</summary>
        Guest = 0,
        
        /// <summary>Google</summary>
        Google = 1,
        
        /// <summary>Apple</summary>
        Apple = 2,
        
        /// <summary>Facebook</summary>
        Facebook = 3,
    }

    /// <summary>
    /// 账户数据
    /// </summary>
    [Serializable]
    public class AccountData
    {
        /// <summary>账户ID</summary>
        public string AccountId;
        
        /// <summary>登录类型</summary>
        public LoginType LoginType;
        
        /// <summary>OpenID(第三方)</summary>
        public string OpenId;
        
        /// <summary>Token</summary>
        public string Token;
        
        /// <summary>创建时间</summary>
        public long CreateTime;
        
        /// <summary>最后登录时间</summary>
        public long LastLoginTime;
        
        /// <summary>是否绑定</summary>
        public bool IsBinded;
        
        /// <summary>绑定时间</summary>
        public long BindTime;
    }

    /// <summary>
    /// 玩家基础数据
    /// </summary>
    [Serializable]
    public class PlayerBasicData
    {
        /// <summary>玩家ID</summary>
        public string PlayerId;
        
        /// <summary>玩家名称</summary>
        public string PlayerName;
        
        /// <summary>玩家等级</summary>
        public int Level;
        
        /// <summary>经验</summary>
        public long Exp;
        
        /// <summary>头像ID</summary>
        public string HeadIconId;
        
        /// <summary>头像框ID</summary>
        public string FrameId;
        
        /// <summary>地区</summary>
        public string Region;
        
        /// <summary>注册时间</summary>
        public long RegisterTime;
        
        /// <summary>最后在线时间</summary>
        public long LastOnlineTime;
    }

    //===========================================================
    // 账户系统接口
    //===========================================================

    /// <summary>
    /// 账户系统接口
    /// </summary>
    public interface IAccountSystem
    {
        /// <summary>登录</summary>

        /// <summary>初始化系统</summary>
        void Initialize();

        /// <summary>登录</summary>
        /// <param name="type">登录类型</param>
        /// <param name="callback">回调</param>
        void Login(LoginType type, Action<bool, string> callback);

        /// <summary>绑定账户</summary>
        /// <param name="type">登录类型</param>
        /// <param name="callback">回调</param>
        void BindAccount(LoginType type, Action<bool, string> callback);

        /// <summary>获取账户数据</summary>
        /// <returns>账户数据</returns>
        AccountData GetAccountData();

        /// <summary>获取玩家数据</summary>
        /// <returns>玩家数据</returns>
        PlayerBasicData GetPlayerData();

        /// <summary>修改玩家名称</summary>
        /// <param name="name">新名称</param>
        /// <returns>是否成功</returns>
        bool ChangePlayerName(string name);

        /// <summary>修改头像</summary>
        /// <param name="headIconId">头像ID</param>
        /// <returns>是否成功</returns>
        bool ChangeHeadIcon(string headIconId);

        /// <summary>修改头像框</summary>
        /// <param name="frameId">头像框ID</param>
        /// <returns>是否成功</returns>
        bool ChangeFrame(string frameId);

        /// <summary>同步数据到服务器</summary>
        void SyncToServer();

        /// <summary>从服务器同步数据</summary>
        void SyncFromServer();

        /// <summary>保存数据</summary>
        void SaveData();

        /// <summary>加载数据</summary>
        void LoadData();
    }

    //===========================================================
    // 账户系统实现
    //===========================================================

    /// <summary>
    /// 账户系统
    /// 职责：
    /// 1. 多平台登录(游客/Google/Apple/Facebook)
    /// 2. 账户绑定和数据同步
    /// 3. 玩家数据管理
    /// 4. 数据持久化
    /// 
    /// 扩展点：
    /// 1. 可扩展登录平台
    /// 2. 可自定义数据同步策略
    /// </summary>
    public class AccountSystem : BaseLogicSys<AccountSystem>, IAccountSystem
    {
        //========================== 常量 ==========================

        private const string ACCOUNT_KEY = "AccountData";
        private const string PLAYER_KEY = "PlayerBasicData";

        //========================== 私有变量 ==========================

        /// <summary>账户数据</summary>
        private AccountData m_accountData;

        /// <summary>玩家数据</summary>
        private PlayerBasicData m_playerData;

        /// <summary>是否已登录</summary>
        private bool m_isLoggedIn = false;

        /// <summary>是否已初始化</summary>
        private bool m_isInitialized = false;

        //========================== 回调 ==========================

        /// <summary>登录成功回调</summary>
        public Action<AccountData> OnLoginSuccess;

        /// <summary>登录失败回调</summary>
        public Action<string> OnLoginFailed;

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

            LoadData();

            // 检查是否已登录
            if (m_accountData != null && !string.IsNullOrEmpty(m_accountData.Token))
            {
                m_isLoggedIn = true;
            }

            m_isInitialized = true;
            Debug.Log($"[AccountSystem] Initialized, loggedIn: {m_isLoggedIn}");
            return true;
        }

        public override void OnDestroy()
        {
            SaveData();
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 登录
        /// </summary>
        public void Login(LoginType type, Action<bool, string> callback)
        {
            // 实际项目中应调用对应平台的SDK
            // 这里简化处理，直接创建本地账户

            if (m_accountData == null)
            {
                // 创建新账户
                m_accountData = new AccountData
                {
                    AccountId = GenerateAccountId(),
                    LoginType = type,
                    CreateTime = TimeUtil.GetCurrentTimeMillis(),
                    LastLoginTime = TimeUtil.GetCurrentTimeMillis(),
                    IsBinded = false
                };
            }

            if (m_playerData == null)
            {
                // 创建玩家数据
                m_playerData = new PlayerBasicData
                {
                    PlayerId = m_accountData.AccountId,
                    PlayerName = $"玩家{UnityEngine.Random.Range(1000, 9999)}",
                    Level = 1,
                    Exp = 0,
                    HeadIconId = "default",
                    FrameId = "default",
                    Region = "CN",
                    RegisterTime = TimeUtil.GetCurrentTimeMillis(),
                    LastOnlineTime = TimeUtil.GetCurrentTimeMillis()
                };
            }

            m_accountData.LastLoginTime = TimeUtil.GetCurrentTimeMillis();
            m_playerData.LastOnlineTime = TimeUtil.GetCurrentTimeMillis();

            SaveData();
            m_isLoggedIn = true;

            Debug.Log($"[AccountSystem] Login success: {type}");
            callback?.Invoke(true, "");
        }

        /// <summary>
        /// 绑定账户
        /// </summary>
        public void BindAccount(LoginType type, Action<bool, string> callback)
        {
            if (m_accountData == null)
            {
                callback?.Invoke(false, "No account");
                return;
            }

            if (m_accountData.IsBinded)
            {
                callback?.Invoke(false, "Already binded");
                return;
            }

            // 实际项目中应调用对应平台的SDK进行绑定
            m_accountData.LoginType = type;
            m_accountData.IsBinded = true;
            m_accountData.BindTime = TimeUtil.GetCurrentTimeMillis();

            SaveData();

            Debug.Log($"[AccountSystem] Account binded: {type}");
            callback?.Invoke(true, "");
        }

        /// <summary>
        /// 获取账户数据
        /// </summary>
        public AccountData GetAccountData()
        {
            return m_accountData;
        }

        /// <summary>
        /// 获取玩家数据
        /// </summary>
        public PlayerBasicData GetPlayerData()
        {
            return m_playerData;
        }

        /// <summary>
        /// 修改玩家名称
        /// </summary>
        public bool ChangePlayerName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 2 || name.Length > 12)
            {
                Debug.LogWarning("[AccountSystem] Invalid name");
                return false;
            }

            if (m_playerData == null)
            {
                return false;
            }

            m_playerData.PlayerName = name;
            SaveData();

            Debug.Log($"[AccountSystem] Name changed: {name}");
            return true;
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        public bool ChangeHeadIcon(string headIconId)
        {
            if (m_playerData == null)
            {
                return false;
            }

            m_playerData.HeadIconId = headIconId;
            SaveData();

            Debug.Log($"[AccountSystem] Head icon changed: {headIconId}");
            return true;
        }

        /// <summary>
        /// 修改头像框
        /// </summary>
        public bool ChangeFrame(string frameId)
        {
            if (m_playerData == null)
            {
                return false;
            }

            m_playerData.FrameId = frameId;
            SaveData();

            Debug.Log($"[AccountSystem] Frame changed: {frameId}");
            return true;
        }

        /// <summary>
        /// 同步到服务器
        /// </summary>
        public void SyncToServer()
        {
            // 实际项目中应发送到服务器
            Debug.Log("[AccountSystem] Sync to server");
        }

        /// <summary>
        /// 从服务器同步
        /// </summary>
        public void SyncFromServer()
        {
            // 实际项目中应从服务器拉取
            Debug.Log("[AccountSystem] Sync from server");
        }

        //========================== 数据持久化 ==========================

        public void SaveData()
        {
            if (m_accountData != null)
            {
                var json = JsonUtility.ToJson(m_accountData);
                PlayerPrefs.SetString(ACCOUNT_KEY, json);
            }

            if (m_playerData != null)
            {
                var json = JsonUtility.ToJson(m_playerData);
                PlayerPrefs.SetString(PLAYER_KEY, json);
            }

            Debug.Log("[AccountSystem] Data saved");
        }

        public void LoadData()
        {
            // 加载账户数据
            if (PlayerPrefs.HasKey(ACCOUNT_KEY))
            {
                try
                {
                    var json = PlayerPrefs.GetString(ACCOUNT_KEY);
                    m_accountData = JsonUtility.FromJson<AccountData>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[AccountSystem] Failed to load account: {e.Message}");
                }
            }

            // 加载玩家数据
            if (PlayerPrefs.HasKey(PLAYER_KEY))
            {
                try
                {
                    var json = PlayerPrefs.GetString(PLAYER_KEY);
                    m_playerData = JsonUtility.FromJson<PlayerBasicData>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[AccountSystem] Failed to load player: {e.Message}");
                }
            }
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 生成账户ID
        /// </summary>
        private string GenerateAccountId()
        {
            return $"player_{TimeUtil.GetCurrentTimeMillis()}_{UnityEngine.Random.Range(1000, 9999)}";
        }

        /// <summary>
        /// 是否已登录
        /// </summary>
        public bool IsLoggedIn()
        {
            return m_isLoggedIn;
        }
    }
}
