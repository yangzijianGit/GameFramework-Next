using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 主界面数据类型
    //===========================================================

    /// <summary>
    /// 主界面功能入口类型
    /// </summary>
    public enum MainMenuEntryType
    {
        /// <summary>开始游戏</summary>
        StartGame = 0,
        
        /// <summary>关卡选择</summary>
        LevelSelect = 1,
        
        /// <summary>乐园/建造</summary>
        Park = 2,
        
        /// <summary>活动中心</summary>
        Activity = 3,
        
        /// <summary>商店</summary>
        Shop = 4,
        
        /// <summary>仓库/背包</summary>
        Warehouse = 5,
        
        /// <summary>设置</summary>
        Settings = 6,
    }

    /// <summary>
    /// 顶部货币显示配置
    /// </summary>
    [Serializable]
    public class CurrencyDisplayConfig
    {
        /// <summary>货币ID</summary>
        public string CurrencyId;
        
        /// <summary>图标</summary>
        public string Icon;
        
        /// <summary>是否显示数量</summary>
        public bool ShowAmount;
    }

    /// <summary>
    /// 主界面配置
    /// </summary>
    [Serializable]
    public class MainMenuConfig
    {
        /// <summary>是否显示顶部状态栏</summary>
        public bool ShowTopBar = true;
        
        /// <summary>顶部货币显示</summary>
        public List<CurrencyDisplayConfig> TopCurrencies = new List<CurrencyDisplayConfig>();
        
        /// <summary>快捷入口</summary>
        public List<MainMenuEntryType> QuickEntries = new List<MainMenuEntryType>();
        
        /// <summary>签到按钮是否可见</summary>
        public bool ShowSignIn = true;
        
        /// <summary>任务按钮是否可见</summary>
        public bool ShowTask = true;
        
        /// <summary>排行榜按钮是否可见</summary>
        public bool ShowLeaderboard = true;
        
        /// <summary>Banner广告刷新间隔(秒)</summary>
        public float BannerRefreshInterval = 60f;
    }

    /// <summary>
    /// 主界面状态
    /// </summary>
    public enum MainMenuState
    {
        /// <summary>初始化</summary>
        Init = 0,
        
        /// <summary>显示中</summary>
        Shown = 1,
        
        /// <summary>隐藏中</summary>
        Hidden = 2,
    }

    //===========================================================
    // 主界面系统接口
    //===========================================================

    /// <summary>
    /// 主界面系统接口
    /// </summary>
    public interface IMainMenuSystem
    {
        /// <summary>显示主界面</summary>
        void Show();

        /// <summary>初始化系统</summary>
        void Initialize();

        /// <summary>隐藏主界面</summary>
        void Hide();

        /// <summary>进入功能</summary>
        /// <param name="entryType">入口类型</param>
        void EnterEntry(MainMenuEntryType entryType);

        /// <summary>获取主界面配置</summary>
        /// <returns>配置</returns>
        MainMenuConfig GetConfig();

        /// <summary>获取当前状态</summary>
        /// <returns>状态</returns>
        MainMenuState GetState();

        /// <summary>刷新货币显示</summary>
        void RefreshCurrencyDisplay();

        /// <summary>显示签到提示</summary>
        void ShowSignInTip();

        /// <summary>显示任务提示</summary>
        void ShowTaskTip();

        /// <summary>更新玩家信息</summary>
        void UpdatePlayerInfo();
    }

    //===========================================================
    // 主界面系统实现
    //===========================================================

    /// <summary>
    /// 主界面系统
    /// 职责：
    /// 1. 管理主界面UI显示
    /// 2. 功能入口导航
    /// 3. 货币显示更新
    /// 4. 快捷入口管理
    /// 5. 签到/任务红点提示
    /// 
    /// 扩展点：
    /// 1. 可配置入口按钮
    /// 2. 可自定义入口跳转逻辑
    /// </summary>
    public class MainMenuSystem : BaseLogicSys<MainMenuSystem>, IMainMenuSystem
    {
        //========================== 常量 ==========================

        /// <summary>签到提示Key</summary>
        private const string SIGN_IN_TIP_KEY = "MainMenu_ShowSignInTip";

        //========================== 私有变量 ==========================

        /// <summary>主界面配置</summary>
        private MainMenuConfig m_config;

        /// <summary>当前状态</summary>
        private MainMenuState m_state = MainMenuState.Init;

        /// <summary>是否显示签到提示</summary>
        private bool m_showSignInTip = false;

        /// <summary>是否显示任务提示</summary>
        private bool m_showTaskTip = false;

        /// <summary>是否已初始化</summary>
        private bool m_isInitialized = false;

        //========================== 回调事件 ==========================

        /// <summary>进入入口回调</summary>
        public Action<MainMenuEntryType> OnEnterEntry;

        /// <summary>显示主界面回调</summary>
        public Action OnShow;

        /// <summary>隐藏主界面回调</summary>
        public Action OnHide;

        //========================== 生命周期 ==========================

        /// </summary>
        public void Initialize()
        {
            OnInit();
        }


        public override bool OnInit()
        {
            base.OnInit();

            // 加载配置
            LoadConfig();

            // 检查签到提示
            CheckSignInTip();

            // 检查任务提示
            CheckTaskTip();

            m_isInitialized = true;
            Debug.Log("[MainMenuSystem] Initialized");
            return true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void OnDestroy()
        {
            m_config = null;
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 显示主界面
        /// </summary>
        public void Show()
        {
            if (m_state == MainMenuState.Shown)
                return;

            m_state = MainMenuState.Shown;

            // 刷新货币显示
            RefreshCurrencyDisplay();

            // 更新玩家信息
            UpdatePlayerInfo();

            // 触发回调
            OnShow?.Invoke();

            Debug.Log("[MainMenuSystem] Main menu shown");
        }

        /// <summary>
        /// 隐藏主界面
        /// </summary>
        public void Hide()
        {
            if (m_state == MainMenuState.Hidden)
                return;

            m_state = MainMenuState.Hidden;

            // 触发回调
            OnHide?.Invoke();

            Debug.Log("[MainMenuSystem] Main menu hidden");
        }

        /// <summary>
        /// 进入功能入口
        /// </summary>
        public void EnterEntry(MainMenuEntryType entryType)
        {
            Debug.Log($"[MainMenuSystem] Enter entry: {entryType}");

            // 隐藏主界面
            Hide();

            // 触发回调
            OnEnterEntry?.Invoke(entryType);

            // 根据入口类型执行不同逻辑
            switch (entryType)
            {
                case MainMenuEntryType.StartGame:
                case MainMenuEntryType.LevelSelect:
                    // 进入关卡选择
                    // LevelSelectSystem.Instance.Show();
                    break;

                case MainMenuEntryType.Park:
                    // 进入乐园系统
                    // ParkSystem.Instance.Show();
                    break;

                case MainMenuEntryType.Activity:
                    // 进入活动中心
                    // ActivitySystem.Instance.Show();
                    break;

                case MainMenuEntryType.Shop:
                    // 进入商店
                    // ShopSystem.Instance.Show();
                    break;

                case MainMenuEntryType.Warehouse:
                    // 进入仓库
                    // WarehouseSystem.Instance.Show();
                    break;

                case MainMenuEntryType.Settings:
                    // 进入设置
                    // SettingsSystem.Instance.Show();
                    break;
            }
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        public MainMenuConfig GetConfig()
        {
            return m_config;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        public MainMenuState GetState()
        {
            return m_state;
        }

        /// <summary>
        /// 刷新货币显示
        /// </summary>
        public void RefreshCurrencyDisplay()
        {
            if (m_config == null || !m_config.ShowTopBar)
                return;

            foreach (var currency in m_config.TopCurrencies)
            {
                // 获取货币数量
                // var amount = PlayerData.Instance.GetCurrencyCount(currency.CurrencyId);
                // 更新UI显示
                Debug.Log($"[MainMenuSystem] Refresh currency: {currency.CurrencyId}");
            }
        }

        /// <summary>
        /// 显示签到提示
        /// </summary>
        public void ShowSignInTip()
        {
            m_showSignInTip = true;
            // 通知UI显示红点
            Debug.Log("[MainMenuSystem] Show sign in tip");
        }

        /// <summary>
        /// 显示任务提示
        /// </summary>
        public void ShowTaskTip()
        {
            m_showTaskTip = true;
            // 通知UI显示红点
            Debug.Log("[MainMenuSystem] Show task tip");
        }

        /// <summary>
        /// 更新玩家信息
        /// </summary>
        public void UpdatePlayerInfo()
        {
            // 获取玩家数据
            // var level = PlayerData.Instance.Level;
            // var name = PlayerData.Instance.Name;
            // 更新UI显示
            Debug.Log("[MainMenuSystem] Player info updated");
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 加载配置
        /// </summary>
        private void LoadConfig()
        {
            m_config = new MainMenuConfig
            {
                ShowTopBar = true,
                TopCurrencies = new List<CurrencyDisplayConfig>
                {
                    new CurrencyDisplayConfig { CurrencyId = "diamond", Icon = "icon_diamond", ShowAmount = true },
                    new CurrencyDisplayConfig { CurrencyId = "star", Icon = "icon_star", ShowAmount = true },
                    new CurrencyDisplayConfig { CurrencyId = "energy", Icon = "icon_energy", ShowAmount = true }
                },
                QuickEntries = new List<MainMenuEntryType>
                {
                    MainMenuEntryType.StartGame,
                    MainMenuEntryType.LevelSelect,
                    MainMenuEntryType.Park,
                    MainMenuEntryType.Activity,
                    MainMenuEntryType.Shop,
                    MainMenuEntryType.Warehouse,
                    MainMenuEntryType.Settings
                },
                ShowSignIn = true,
                ShowTask = true,
                ShowLeaderboard = true,
                BannerRefreshInterval = 60f
            };
        }

        /// <summary>
        /// 检查签到提示
        /// </summary>
        private void CheckSignInTip()
        {
            // 检查今天是否签到
            // if (!SignInSystem.Instance.IsSignedInToday())
            // {
            //     m_showSignInTip = true;
            // }
        }

        /// <summary>
        /// 检查任务提示
        /// </summary>
        private void CheckTaskTip()
        {
            // 检查是否有可领取的任务
            // var dailyTasks = TaskSystem.Instance.GetTaskList(TaskType.Daily);
            // foreach (var task in dailyTasks)
            // {
            //     if (task.State == TaskState.Completed)
            //     {
            //         m_showTaskTip = true;
            //         break;
            //     }
            // }
        }

        //========================== 测试接口 ==========================

        /// <summary>
        /// 测试：进入关卡
        /// </summary>
        public void Test_EnterLevel(string levelId)
        {
            Debug.Log($"[MainMenuSystem] Test enter level: {levelId}");
            // LevelSelectSystem.Instance.EnterLevel(levelId);
        }
    }
}
