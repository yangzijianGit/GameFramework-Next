//========================================================================================
//  ShopSystem - 商店系统
//========================================================================================
//  功能：处理游戏内商城购买逻辑
//  职责：商品管理、购买验证、货币扣除、奖励发放、特惠刷新
//  扩展点：接入真实支付SDK、活动商品配置、数据统计
//========================================================================================

using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 商店数据类型
    //===========================================================
    
    /// <summary>
    /// 商店类型
    /// </summary>
    public enum ShopType
    {
        /// <summary>货币商店</summary>
        Currency = 0,
        
        /// <summary>道具商店</summary>
        Prop = 1,
        
        /// <summary>服装商店</summary>
        Clothes = 2,
        
        /// <summary>特惠商店</summary>
        LimitedOffer = 3,
        
        /// <summary>月卡商店</summary>
        Subscription = 4,
    }

    /// <summary>
    /// 商品类型
    /// </summary>
    public enum GoodsType
    {
        /// <summary>钻石</summary>
        Diamond = 1,
        
        /// <summary>幸福星</summary>
        Star = 2,
        
        /// <summary>体力</summary>
        Energy = 3,
        
        /// <summary>道具</summary>
        Item = 4,
        
        /// <summary>服装</summary>
        Clothes = 5,
        
        /// <summary>皮肤</summary>
        Skin = 6,
    }

    /// <summary>
    /// 购买限制类型
    /// </summary>
    public enum PurchaseLimitType
    {
        /// <summary>无限制</summary>
        None = 0,
        
        /// <summary>每日限购</summary>
        Daily = 1,
        
        /// <summary>每周限购</summary>
        Weekly = 2,
        
        /// <summary>终身限购</summary>
        Lifetime = 3,
    }

    /// <summary>
    /// 货币类型
    /// </summary>
    public enum CurrencyType
    {
        /// <summary>金币</summary>
        Coin = 1,
        
        /// <summary>钻石</summary>
        Diamond = 2,
        
        /// <summary>代币(活动)</summary>
        Token = 3,
    }

    //===========================================================
    // 配置数据结构 (从配置表加载)
    //===========================================================

    /// <summary>
    /// 商品配置数据
    /// </summary>
    [Serializable]
    public class ShopGoodsConfig
    {
        /// <summary>商品ID</summary>
        public string Id;
        
        /// <summary>商品名称</summary>
        public string Name;
        
        /// <summary>商品描述</summary>
        public string Description;
        
        /// <summary>商店类型</summary>
        public ShopType ShopType;
        
        /// <summary>商品类型</summary>
        public GoodsType GoodsType;
        
        /// <summary>商品图标</summary>
        public string Icon;
        
        /// <summary>原价(原价不为0则显示折扣)</summary>
        public int OriginalPrice;
        
        /// <summary>现价</summary>
        public int Price;
        
        /// <summary>货币类型</summary>
        public CurrencyType CurrencyType;
        
        /// <summary>购买获得的商品数量</summary>
        public int GoodsCount;
        
        /// <summary>赠送的商品数量(首充/活动)</summary>
        public int BonusCount;
        
        /// <summary>限购类型</summary>
        public PurchaseLimitType LimitType;
        
        /// <summary>限购数量</summary>
        public int LimitCount;
        
        /// <summary>是否首充商品</summary>
        public bool IsFirstPurchase;
        
        /// <summary>排序权重</summary>
        public int SortWeight;
        
        /// <summary>是否启用</summary>
        public bool IsEnabled;
        
        /// <summary>关联商品ID(套装/组合)</summary>
        public string BundleId;
    }

    /// <summary>
    /// 特惠商品配置(限时)
    /// </summary>
    [Serializable]
    public class LimitedOfferConfig
    {
        /// <summary>特惠ID</summary>
        public string Id;
        
        /// <summary>特惠名称</summary>
        public string Name;
        
        /// <summary>商品列表</summary>
        public List<ShopGoodsConfig> GoodsList;
        
        /// <summary>原价总和</summary>
        public int OriginalTotalPrice;
        
        /// <summary>现价</summary>
        public int Price;
        
        /// <summary>货币类型</summary>
        public CurrencyType CurrencyType;
        
        /// <summary>开始时间戳</summary>
        public long StartTime;
        
        /// <summary>结束时间戳</summary>
        public long EndTime;
        
        /// <summary>刷新类型(不刷新/每日/每周)</summary>
        public string RefreshType;
        
        /// <summary>是否已购买</summary>
        public bool IsPurchased;
    }

    /// <summary>
    /// 月卡配置
    /// </summary>
    [Serializable]
    public class SubscriptionConfig
    {
        /// <summary>月卡ID</summary>
        public string Id;
        
        /// <summary>月卡名称</summary>
        public string Name;
        
        /// <summary>月卡描述</summary>
        public string Description;
        
        /// <summary>价格</summary>
        public int Price;
        
        /// <summary>货币类型</summary>
        public CurrencyType CurrencyType;
        
        /// <summary>持续天数</summary>
        public int DurationDays;
        
        /// <summary>每日赠送商品</summary>
        public GoodsType DailyRewardType;
        
        /// <summary>每日赠送数量</summary>
        public int DailyRewardCount;
        
        /// <summary>是否首次购买赠送额外</summary>
        public bool HasFirstBonus;
        
        /// <summary>首次购买额外奖励</summary>
        public int FirstBonusCount;
        
        /// <summary>图标</summary>
        public string Icon;
    }

    //===========================================================
    // 运行时数据结构
    //===========================================================

    /// <summary>
    /// 玩家购买记录
    /// </summary>
    [Serializable]
    public class PurchaseRecord
    {
        /// <summary>商品ID</summary>
        public string GoodsId;
        
        /// <summary>已购买数量</summary>
        public int PurchasedCount;
        
        /// <summary>首次购买时间戳</summary>
        public long FirstPurchaseTime;
        
        /// <summary>最后购买时间戳</summary>
        public long LastPurchaseTime;
    }

    /// <summary>
    /// 玩家月卡状态
    /// </summary>
    [Serializable]
    public class SubscriptionState
    {
        /// <summary>月卡ID</summary>
        public string SubscriptionId;
        
        /// <summary>开始时间戳</summary>
        public long StartTime;
        
        /// <summary>结束时间戳</summary>
        public long EndTime;
        
        /// <summary>是否已领取今日奖励</summary>
        public bool HasClaimedToday;
        
        /// <summary>今日领取时间戳</summary>
        public long TodayClaimTime;
    }

    //===========================================================
    // 商店系统
    //===========================================================

    /// <summary>
    /// 商店系统
    /// 处理游戏内商城购买逻辑
    /// 
    /// 职责：
    /// - 商品数据管理(从配置表加载)
    /// - 购买验证(货币是否足够、是否达到限购)
    /// - 货币扣除与商品发放
    /// - 特惠商品刷新逻辑
    /// - 月卡状态管理与奖励领取
    /// 
    /// 扩展点：
    /// - 接入真实支付SDK(第三方支付渠道)
    /// - 商品数据从服务器拉取
    /// - 购买行为数据统计上报
    /// - 活动期间折扣配置
    /// 
    /// 使用方式：
    /// <code>
    /// // 打开商店界面
    /// ShopSystem.Instance.OpenShop(ShopType.Currency);
    /// 
    /// // 购买商品
    /// var result = ShopSystem.Instance.PurchaseGoods("goods_001", 1);
    /// if (result.Success) { /* 购买成功 */ }
    /// 
    /// // 领取月卡奖励
    /// ShopSystem.Instance.ClaimSubscriptionReward("monthly_card");
    /// </code>
    /// </summary>
    public class ShopSystem : BaseLogicSys<ShopSystem>
    {
        // 常量定义
        private const string PLAYER_DATA_KEY = "ShopData";
        
        // 运行时数据
        private Dictionary<string, ShopGoodsConfig> _allGoods = new Dictionary<string, ShopGoodsConfig>();
        private Dictionary<ShopType, List<ShopGoodsConfig>> _shopGoodsMap = new Dictionary<ShopType, List<ShopGoodsConfig>>();
        private Dictionary<string, PurchaseRecord> _purchaseRecords = new Dictionary<string, PurchaseRecord>();
        private Dictionary<string, SubscriptionState> _subscriptionStates = new Dictionary<string, SubscriptionState>();
        private List<LimitedOfferConfig> _limitedOffers = new List<LimitedOfferConfig>();
        
        // 事件回调
        /// <summary>商品数据加载完成回调(商店类型)</summary>
        public event Action<ShopType> OnShopDataLoaded;
        
        /// <summary>购买成功回调(商品ID, 数量)</summary>
        public event Action<string, int> OnPurchaseSuccess;
        
        /// <summary>购买失败回调(商品ID, 错误信息)</summary>
        public event Action<string, string> OnPurchaseFailed;
        
        /// <summary>月卡奖励领取回调(月卡ID)</summary>
        public event Action<string> OnSubscriptionClaimed;
        
        /// <summary>特惠刷新回调</summary>
        public event Action OnLimitedOfferRefreshed;

        //===========================================================
        // 生命周期方法
        //===========================================================

        /// <summary>
        /// 初始化
        /// </summary>
        public override bool OnInit()
        {
            base.OnInit();
            LoadShopData();
            return true;
        }

        /// <summary>
        /// 开始(可进行数据刷新)
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();
            RefreshLimitedOffers();
            CheckSubscriptionExpired();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void OnDestroy()
        {
            SaveShopData();
            base.OnDestroy();
        }

        //===========================================================
        // 公共接口
        //===========================================================

        /// <summary>
        /// 打开商店界面
        /// </summary>
        /// <param name="shopType">商店类型</param>
        public void OpenShop(ShopType shopType)
        {
            Debug.Log(string.Format("[ShopSystem] Open shop: {0}", shopType));
            
            // 检查是否需要刷新
            if (shopType == ShopType.LimitedOffer)
            {
                RefreshLimitedOffers();
            }
            
            // 通知UI加载商品数据
            OnShopDataLoaded?.Invoke(shopType);
        }

        /// <summary>
        /// 获取商店商品列表
        /// </summary>
        /// <param name="shopType">商店类型</param>
        /// <returns>商品配置列表</returns>
        public List<ShopGoodsConfig> GetShopGoodsList(ShopType shopType)
        {
            if (_shopGoodsMap.TryGetValue(shopType, out var goodsList))
            {
                return goodsList;
            }
            return new List<ShopGoodsConfig>();
        }

        /// <summary>
        /// 获取特惠商品列表
        /// </summary>
        /// <returns>特惠商品列表</returns>
        public List<LimitedOfferConfig> GetLimitedOffers()
        {
            return _limitedOffers;
        }

        /// <summary>
        /// 获取月卡列表
        /// </summary>
        /// <returns>月卡配置列表(实际项目中从配置表加载)</returns>
        public List<SubscriptionConfig> GetSubscriptionList()
        {
            // 实际项目中从配置表加载
            // 这里返回模拟数据
            return new List<SubscriptionConfig>
            {
                new SubscriptionConfig
                {
                    Id = "monthly_card",
                    Name = "月卡",
                    Description = "每日领取60钻石",
                    Price = 30,
                    CurrencyType = CurrencyType.Diamond,
                    DurationDays = 30,
                    DailyRewardType = GoodsType.Diamond,
                    DailyRewardCount = 60,
                    HasFirstBonus = true,
                    FirstBonusCount = 300,
                    Icon = "Icon/MonthlyCard"
                },
                new SubscriptionConfig
                {
                    Id = "weekly_card",
                    Name = "周卡",
                    Description = "每日领取30钻石",
                    Price = 8,
                    CurrencyType = CurrencyType.Diamond,
                    DurationDays = 7,
                    DailyRewardType = GoodsType.Diamond,
                    DailyRewardCount = 30,
                    HasFirstBonus = true,
                    FirstBonusCount = 50,
                    Icon = "Icon/WeeklyCard"
                }
            };
        }

        /// <summary>
        /// 购买商品
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="count">购买数量</param>
        /// <returns>购买结果</returns>
        public PurchaseResult PurchaseGoods(string goodsId, int count = 1)
        {
            // 参数验证
            if (string.IsNullOrEmpty(goodsId))
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = "商品ID不能为空" };
                OnPurchaseFailed?.Invoke(goodsId, result.ErrorMessage);
                return result;
            }

            if (count <= 0)
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = "购买数量必须大于0" };
                OnPurchaseFailed?.Invoke(goodsId, result.ErrorMessage);
                return result;
            }

            // 获取商品配置
            if (!_allGoods.TryGetValue(goodsId, out var goodsConfig))
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = "商品不存在" };
                OnPurchaseFailed?.Invoke(goodsId, result.ErrorMessage);
                return result;
            }

            if (!goodsConfig.IsEnabled)
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = "商品已下架" };
                OnPurchaseFailed?.Invoke(goodsId, result.ErrorMessage);
                return result;
            }

            // 检查限购
            var limitResult = CheckPurchaseLimit(goodsConfig, count);
            if (!limitResult.CanPurchase)
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = limitResult.ErrorMessage };
                OnPurchaseFailed?.Invoke(goodsId, result.ErrorMessage);
                return result;
            }

            // 计算价格
            int totalPrice = goodsConfig.Price * count;
            
            // 检查货币是否足够
            var currencyResult = CheckCurrency(goodsConfig.CurrencyType, totalPrice);
            if (!currencyResult.HasEnough)
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = $"货币不足，需要{GetCurrencyName(goodsConfig.CurrencyType)}{totalPrice}" };
                OnPurchaseFailed?.Invoke(goodsId, result.ErrorMessage);
                return result;
            }

            // 扣除货币
            DeductCurrency(goodsConfig.CurrencyType, totalPrice);
            
            // 发放商品(包含赠送)
            int totalGoodsCount = (goodsConfig.GoodsCount + goodsConfig.BonusCount) * count;
            GrantGoods(goodsConfig.GoodsType, goodsConfig.Id, totalGoodsCount);
            
            // 记录购买
            RecordPurchase(goodsId, count);
            
            // 保存数据
            SaveShopData();
            
            // 事件通知
            OnPurchaseSuccess?.Invoke(goodsId, count);
            
            Debug.Log(string.Format("[ShopSystem] Purchase success: goodsId={0}, count={1}, totalPrice={2}", goodsId, count, totalPrice));
            
            return new PurchaseResult 
            { 
                Success = true, 
                GoodsConfig = goodsConfig, 
                PurchaseCount = count,
                TotalGoodsCount = totalGoodsCount 
            };
        }

        /// <summary>
        /// 购买特惠商品
        /// </summary>
        /// <param name="offerId">特惠ID</param>
        /// <returns>购买结果</returns>
        public PurchaseResult PurchaseLimitedOffer(string offerId)
        {
            var offer = _limitedOffers.Find(o => o.Id == offerId);
            if (offer == null)
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = "特惠不存在" };
                OnPurchaseFailed?.Invoke(offerId, result.ErrorMessage);
                return result;
            }

            if (offer.IsPurchased)
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = "特惠已购买" };
                OnPurchaseFailed?.Invoke(offerId, result.ErrorMessage);
                return result;
            }

            // 检查特惠是否在有效期内
            long currentTime = GetCurrentTime();
            if (currentTime < offer.StartTime || currentTime > offer.EndTime)
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = "特惠已过期" };
                OnPurchaseFailed?.Invoke(offerId, result.ErrorMessage);
                return result;
            }

            // 检查货币
            var currencyResult = CheckCurrency(offer.CurrencyType, offer.Price);
            if (!currencyResult.HasEnough)
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = $"货币不足" };
                OnPurchaseFailed?.Invoke(offerId, result.ErrorMessage);
                return result;
            }

            // 扣除货币
            DeductCurrency(offer.CurrencyType, offer.Price);
            
            // 发放商品
            foreach (var goods in offer.GoodsList)
            {
                GrantGoods(goods.GoodsType, goods.Id, goods.GoodsCount);
            }
            
            // 标记已购买
            offer.IsPurchased = true;
            
            // 保存
            SaveShopData();
            
            // 事件
            OnPurchaseSuccess?.Invoke(offerId, 1);
            
            Debug.Log(string.Format("[ShopSystem] Purchase limited offer success: {0}", offerId));
            
            return new PurchaseResult { Success = true };
        }

        /// <summary>
        /// 购买月卡
        /// </summary>
        /// <param name="subscriptionId">月卡ID</param>
        /// <returns>购买结果</returns>
        public PurchaseResult PurchaseSubscription(string subscriptionId)
        {
            var subscriptionList = GetSubscriptionList();
            var subscription = subscriptionList.Find(s => s.Id == subscriptionId);
            if (subscription == null)
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = "月卡不存在" };
                OnPurchaseFailed?.Invoke(subscriptionId, result.ErrorMessage);
                return result;
            }

            // 检查是否已购买
            if (_subscriptionStates.TryGetValue(subscriptionId, out var state))
            {
                if (state.EndTime > GetCurrentTime())
                {
                    var result = new PurchaseResult { Success = false, ErrorMessage = "月卡已在有效期内" };
                    OnPurchaseFailed?.Invoke(subscriptionId, result.ErrorMessage);
                    return result;
                }
            }

            // 检查货币
            var currencyResult = CheckCurrency(subscription.CurrencyType, subscription.Price);
            if (!currencyResult.HasEnough)
            {
                var result = new PurchaseResult { Success = false, ErrorMessage = "货币不足" };
                OnPurchaseFailed?.Invoke(subscriptionId, result.ErrorMessage);
                return result;
            }

            // 扣除货币
            DeductCurrency(subscription.CurrencyType, subscription.Price);
            
            // 激活月卡
            long currentTime = GetCurrentTime();
            var newState = new SubscriptionState
            {
                SubscriptionId = subscriptionId,
                StartTime = currentTime,
                EndTime = currentTime + subscription.DurationDays * 24 * 3600,
                HasClaimedToday = false,
                TodayClaimTime = 0
            };
            _subscriptionStates[subscriptionId] = newState;
            
            // 首次购买额外奖励
            if (subscription.HasFirstBonus)
            {
                GrantGoods(subscription.DailyRewardType, subscriptionId, subscription.FirstBonusCount);
            }
            
            // 保存
            SaveShopData();
            
            // 事件
            OnPurchaseSuccess?.Invoke(subscriptionId, 1);
            
            Debug.Log(string.Format("[ShopSystem] Purchase subscription success: {0}", subscriptionId));
            
            return new PurchaseResult { Success = true };
        }

        /// <summary>
        /// 领取月卡每日奖励
        /// </summary>
        /// <param name="subscriptionId">月卡ID</param>
        /// <returns>领取结果</returns>
        public ClaimResult ClaimSubscriptionReward(string subscriptionId)
        {
            if (!_subscriptionStates.TryGetValue(subscriptionId, out var state))
            {
                return new ClaimResult { Success = false, ErrorMessage = "未购买该月卡" };
            }

            long currentTime = GetCurrentTime();
            if (currentTime > state.EndTime)
            {
                return new ClaimResult { Success = false, ErrorMessage = "月卡已过期" };
            }

            // 检查今日是否已领取
            if (IsSameDay(state.TodayClaimTime, currentTime) && state.HasClaimedToday)
            {
                return new ClaimResult { Success = false, ErrorMessage = "今日奖励已领取" };
            }

            // 获取配置
            var subscriptionList = GetSubscriptionList();
            var config = subscriptionList.Find(s => s.Id == subscriptionId);
            if (config == null)
            {
                return new ClaimResult { Success = false, ErrorMessage = "月卡配置不存在" };
            }

            // 发放奖励
            GrantGoods(config.DailyRewardType, subscriptionId, config.DailyRewardCount);
            
            // 更新状态
            state.HasClaimedToday = true;
            state.TodayClaimTime = currentTime;
            
            // 保存
            SaveShopData();
            
            // 事件
            OnSubscriptionClaimed?.Invoke(subscriptionId);
            
            Debug.Log(string.Format("[ShopSystem] Claim subscription reward: {0}", subscriptionId));
            
            return new ClaimResult 
            { 
                Success = true, 
                RewardType = config.DailyRewardType, 
                RewardCount = config.DailyRewardCount 
            };
        }

        /// <summary>
        /// 检查月卡是否在有效期内
        /// </summary>
        /// <param name="subscriptionId">月卡ID</param>
        /// <returns>是否有效</returns>
        public bool IsSubscriptionActive(string subscriptionId)
        {
            if (!_subscriptionStates.TryGetValue(subscriptionId, out var state))
            {
                return false;
            }
            return state.EndTime > GetCurrentTime();
        }

        /// <summary>
        /// 获取月卡剩余天数
        /// </summary>
        /// <param name="subscriptionId">月卡ID</param>
        /// <returns>剩余天数，-1表示已过期或未购买</returns>
        public int GetSubscriptionRemainingDays(string subscriptionId)
        {
            if (!_subscriptionStates.TryGetValue(subscriptionId, out var state))
            {
                return -1;
            }
            
            long currentTime = GetCurrentTime();
            if (currentTime >= state.EndTime)
            {
                return -1;
            }
            
            return (int)((state.EndTime - currentTime) / (24 * 3600)) + 1;
        }

        /// <summary>
        /// 获取剩余购买次数
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <returns>剩余次数，-1表示不限购</returns>
        public int GetRemainingPurchaseCount(string goodsId)
        {
            if (!_allGoods.TryGetValue(goodsId, out var config))
            {
                return 0;
            }

            if (config.LimitType == PurchaseLimitType.None)
            {
                return -1;
            }

            if (!_purchaseRecords.TryGetValue(goodsId, out var record))
            {
                return config.LimitCount;
            }

            int purchased = GetActualPurchasedCount(record, config.LimitType);
            return Math.Max(0, config.LimitCount - purchased);
        }

        /// <summary>
        /// 刷新特惠商品
        /// </summary>
        public void RefreshLimitedOffers()
        {
            // 检查是否需要刷新
            // 实际项目中根据刷新类型和上次刷新时间判断
            // 这里简化处理
            
            // 模拟刷新逻辑
            if (_limitedOffers.Count == 0)
            {
                LoadLimitedOffers();
            }
            
            // 检查过期
            long currentTime = GetCurrentTime();
            _limitedOffers.RemoveAll(o => currentTime > o.EndTime);
            
            OnLimitedOfferRefreshed?.Invoke();
        }

        //===========================================================
        // 私有方法
        //===========================================================

        /// <summary>
        /// 加载商店数据
        /// </summary>
        private void LoadShopData()
        {
            // 从本地存储加载购买记录
            string json = PlayerPrefs.GetString(PLAYER_DATA_KEY, "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var data = JsonUtility.FromJson<ShopData>(json);
                    if (data != null)
                    {
                        LoadPurchaseRecords(data.PurchaseRecords);
                        LoadSubscriptionStates(data.SubscriptionStates);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("[ShopSystem] Load shop data failed: {0}", e.Message));
                }
            }
            
            // 加载商品配置
            LoadGoodsConfig();
            
            // 加载特惠
            LoadLimitedOffers();
        }

        /// <summary>
        /// 保存商店数据
        /// </summary>
        private void SaveShopData()
        {
            var data = new ShopData
            {
                PurchaseRecords = GetPurchaseRecordsJson(),
                SubscriptionStates = GetSubscriptionStatesJson()
            };
            
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(PLAYER_DATA_KEY, json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 加载商品配置
        /// </summary>
        private void LoadGoodsConfig()
        {
            // 实际项目中从配置表加载
            // 这里使用模拟数据
            var allGoodsList = new List<ShopGoodsConfig>
            {
                // 货币商店
                new ShopGoodsConfig
                {
                    Id = "diamond_1", Name = "60钻石", Description = "购买60钻石", 
                    ShopType = ShopType.Currency, GoodsType = GoodsType.Diamond, 
                    Price = 60, CurrencyType = CurrencyType.Coin, GoodsCount = 60,
                    SortWeight = 1, IsEnabled = true
                },
                new ShopGoodsConfig
                {
                    Id = "diamond_2", Name = "300钻石", Description = "购买300钻石", 
                    ShopType = ShopType.Currency, GoodsType = GoodsType.Diamond, 
                    Price = 300, CurrencyType = CurrencyType.Coin, GoodsCount = 300,
                    BonusCount = 30, SortWeight = 2, IsEnabled = true
                },
                new ShopGoodsConfig
                {
                    Id = "diamond_3", Name = "980钻石", Description = "购买980钻石", 
                    ShopType = ShopType.Currency, GoodsType = GoodsType.Diamond, 
                    Price = 980, CurrencyType = CurrencyType.Coin, GoodsCount = 980,
                    BonusCount = 128, SortWeight = 3, IsEnabled = true
                },
                new ShopGoodsConfig
                {
                    Id = "diamond_4", Name = "1980钻石", Description = "购买1980钻石", 
                    ShopType = ShopType.Currency, GoodsType = GoodsType.Diamond, 
                    Price = 1980, CurrencyType = CurrencyType.Coin, GoodsCount = 1980,
                    BonusCount = 328, SortWeight = 4, IsEnabled = true
                },
                new ShopGoodsConfig
                {
                    Id = "diamond_5", Name = "3280钻石", Description = "购买3280钻石", 
                    ShopType = ShopType.Currency, GoodsType = GoodsType.Diamond, 
                    Price = 3280, CurrencyType = CurrencyType.Coin, GoodsCount = 3280,
                    BonusCount = 680, SortWeight = 5, IsEnabled = true
                },
                new ShopGoodsConfig
                {
                    Id = "diamond_6", Name = "6480钻石", Description = "购买6480钻石", 
                    ShopType = ShopType.Currency, GoodsType = GoodsType.Diamond, 
                    Price = 6480, CurrencyType = CurrencyType.Coin, GoodsCount = 6480,
                    BonusCount = 1680, SortWeight = 6, IsEnabled = true
                },
                
                // 道具商店
                new ShopGoodsConfig
                {
                    Id = "prop_hammer", Name = "锤子", Description = "消除单个元素", 
                    ShopType = ShopType.Prop, GoodsType = GoodsType.Item, 
                    Price = 50, CurrencyType = CurrencyType.Diamond, GoodsCount = 1,
                    LimitType = PurchaseLimitType.Daily, LimitCount = 10,
                    SortWeight = 1, IsEnabled = true
                },
                new ShopGoodsConfig
                {
                    Id = "prop_bomb", Name = "炸弹", Description = "消除3x3区域", 
                    ShopType = ShopType.Prop, GoodsType = GoodsType.Item, 
                    Price = 100, CurrencyType = CurrencyType.Diamond, GoodsCount = 1,
                    LimitType = PurchaseLimitType.Daily, LimitCount = 5,
                    SortWeight = 2, IsEnabled = true
                },
                new ShopGoodsConfig
                {
                    Id = "prop_rocket", Name = "火箭", Description = "消除整行或整列", 
                    ShopType = ShopType.Prop, GoodsType = GoodsType.Item, 
                    Price = 150, CurrencyType = CurrencyType.Diamond, GoodsCount = 1,
                    LimitType = PurchaseLimitType.Daily, LimitCount = 5,
                    SortWeight = 3, IsEnabled = true
                },
                new ShopGoodsConfig
                {
                    Id = "prop_rainbow", Name = "彩虹球", Description = "消除所有同色元素", 
                    ShopType = ShopType.Prop, GoodsType = GoodsType.Item, 
                    Price = 200, CurrencyType = CurrencyType.Diamond, GoodsCount = 1,
                    LimitType = PurchaseLimitType.Weekly, LimitCount = 3,
                    SortWeight = 4, IsEnabled = true
                },
                new ShopGoodsConfig
                {
                    Id = "prop_energy", Name = "体力", Description = "恢复30体力", 
                    ShopType = ShopType.Prop, GoodsType = GoodsType.Energy, 
                    Price = 10, CurrencyType = CurrencyType.Diamond, GoodsCount = 30,
                    LimitType = PurchaseLimitType.Daily, LimitCount = 10,
                    SortWeight = 5, IsEnabled = true
                },
                
                // 服装商店
                new ShopGoodsConfig
                {
                    Id = "clothes_1", Name = "萌新套装", Description = "新手服装", 
                    ShopType = ShopType.Clothes, GoodsType = GoodsType.Clothes, 
                    Price = 999, CurrencyType = CurrencyType.Diamond, GoodsCount = 1,
                    SortWeight = 1, IsEnabled = true
                },
                new ShopGoodsConfig
                {
                    Id = "clothes_2", Name = "达人套装", Description = "进阶服装", 
                    ShopType = ShopType.Clothes, GoodsType = GoodsType.Clothes, 
                    Price = 2999, CurrencyType = CurrencyType.Diamond, GoodsCount = 1,
                    SortWeight = 2, IsEnabled = true
                },
            };

            // 构建映射
            _allGoods.Clear();
            _shopGoodsMap.Clear();
            
            foreach (var goods in allGoodsList)
            {
                _allGoods[goods.Id] = goods;
                
                if (!_shopGoodsMap.ContainsKey(goods.ShopType))
                {
                    _shopGoodsMap[goods.ShopType] = new List<ShopGoodsConfig>();
                }
                _shopGoodsMap[goods.ShopType].Add(goods);
            }
            
            // 排序
            foreach (var list in _shopGoodsMap.Values)
            {
                list.Sort((a, b) => b.SortWeight.CompareTo(a.SortWeight));
            }
            
            Debug.Log(string.Format("[ShopSystem] Loaded {0} goods", allGoodsList.Count));
        }

        /// <summary>
        /// 加载特惠商品
        /// </summary>
        private void LoadLimitedOffers()
        {
            // 实际项目中从配置表加载
            long currentTime = GetCurrentTime();
            
            _limitedOffers = new List<LimitedOfferConfig>
            {
                new LimitedOfferConfig
                {
                    Id = "limited_1",
                    Name = "特惠礼包",
                    OriginalTotalPrice = 500,
                    Price = 99,
                    CurrencyType = CurrencyType.Diamond,
                    StartTime = currentTime,
                    EndTime = currentTime + 24 * 3600,
                    RefreshType = "daily",
                    GoodsList = new List<ShopGoodsConfig>
                    {
                        new ShopGoodsConfig { Id = "diamond_pack", GoodsType = GoodsType.Diamond, GoodsCount = 500 },
                        new ShopGoodsConfig { Id = "prop_hammer", GoodsType = GoodsType.Item, GoodsCount = 5 }
                    }
                }
            };
        }

        /// <summary>
        /// 检查购买限制
        /// </summary>
        private LimitCheckResult CheckPurchaseLimit(ShopGoodsConfig config, int count)
        {
            if (config.LimitType == PurchaseLimitType.None)
            {
                return new LimitCheckResult { CanPurchase = true };
            }

            if (!_purchaseRecords.TryGetValue(config.Id, out var record))
            {
                if (count > config.LimitCount)
                {
                    return new LimitCheckResult { CanPurchase = false, ErrorMessage = $"限购{config.LimitCount}个" };
                }
                return new LimitCheckResult { CanPurchase = true };
            }

            int purchased = GetActualPurchasedCount(record, config.LimitType);
            int remaining = config.LimitCount - purchased;
            
            if (count > remaining)
            {
                return new LimitCheckResult { CanPurchase = false, ErrorMessage = $"已达到购买上限，剩余{remaining}个" };
            }
            
            return new LimitCheckResult { CanPurchase = true };
        }

        /// <summary>
        /// 获取实际购买数量(根据限购类型)
        /// </summary>
        private int GetActualPurchasedCount(PurchaseRecord record, PurchaseLimitType limitType)
        {
            if (limitType == PurchaseLimitType.Lifetime)
            {
                return record.PurchasedCount;
            }
            
            long currentTime = GetCurrentTime();
            
            if (limitType == PurchaseLimitType.Daily)
            {
                // 检查是否是同一天
                if (!IsSameDay(record.LastPurchaseTime, currentTime))
                {
                    return 0;
                }
            }
            else if (limitType == PurchaseLimitType.Weekly)
            {
                // 检查是否是同一周
                if (!IsSameWeek(record.LastPurchaseTime, currentTime))
                {
                    return 0;
                }
            }
            
            return record.PurchasedCount;
        }

        /// <summary>
        /// 检查货币
        /// </summary>
        private CurrencyCheckResult CheckCurrency(CurrencyType currencyType, int amount)
        {
            int currentAmount = GetCurrencyAmount(currencyType);
            return new CurrencyCheckResult 
            { 
                HasEnough = currentAmount >= amount,
                CurrentAmount = currentAmount,
                RequiredAmount = amount
            };
        }

        /// <summary>
        /// 扣除货币
        /// </summary>
        private void DeductCurrency(CurrencyType currencyType, int amount)
        {
            // 实际项目中调用货币系统
            // 这里简化处理
            Debug.Log(string.Format("[ShopSystem] Deduct currency: type={0}, amount={1}", currencyType, amount));
            
            // 模拟扣除
            // PlayerData.Currency -= amount;
        }

        /// <summary>
        /// 发放商品
        /// </summary>
        private void GrantGoods(GoodsType goodsType, string goodsId, int count)
        {
            // 实际项目中根据商品类型调用对应系统
            Debug.Log(string.Format("[ShopSystem] Grant goods: type={0}, id={1}, count={2}", goodsType, goodsId, count));
            
            switch (goodsType)
            {
                case GoodsType.Diamond:
                    // DiamondSystem.Instance.AddDiamond(count);
                    break;
                case GoodsType.Star:
                    // StarSystem.Instance.AddStar(count);
                    break;
                case GoodsType.Energy:
                    // EnergySystem.Instance.AddEnergy(count);
                    break;
                case GoodsType.Item:
                    // ItemSystem.Instance.AddItem(goodsId, count);
                    break;
                case GoodsType.Clothes:
                    // CollectionSystem.Instance.UnlockClothes(goodsId);
                    break;
                case GoodsType.Skin:
                    // CollectionSystem.Instance.UnlockSkin(goodsId);
                    break;
            }
        }

        /// <summary>
        /// 记录购买
        /// </summary>
        private void RecordPurchase(string goodsId, int count)
        {
            long currentTime = GetCurrentTime();
            
            if (!_purchaseRecords.TryGetValue(goodsId, out var record))
            {
                record = new PurchaseRecord
                {
                    GoodsId = goodsId,
                    PurchasedCount = count,
                    FirstPurchaseTime = currentTime,
                    LastPurchaseTime = currentTime
                };
                _purchaseRecords[goodsId] = record;
            }
            else
            {
                record.PurchasedCount += count;
                record.LastPurchaseTime = currentTime;
            }
        }

        /// <summary>
        /// 检查月卡过期
        /// </summary>
        private void CheckSubscriptionExpired()
        {
            long currentTime = GetCurrentTime();
            var expiredList = new List<string>();
            
            foreach (var kvp in _subscriptionStates)
            {
                if (kvp.Value.EndTime < currentTime)
                {
                    expiredList.Add(kvp.Key);
                }
            }
            
            foreach (var id in expiredList)
            {
                _subscriptionStates.Remove(id);
            }
            
            if (expiredList.Count > 0)
            {
                SaveShopData();
            }
        }

        /// <summary>
        /// 获取货币数量
        /// </summary>
        private int GetCurrencyAmount(CurrencyType currencyType)
        {
            // 实际项目中从玩家数据获取
            // 这里返回模拟值
            switch (currencyType)
            {
                case CurrencyType.Coin:
                    return PlayerPrefs.GetInt("PlayerCoin", 10000);
                case CurrencyType.Diamond:
                    return PlayerPrefs.GetInt("PlayerDiamond", 100);
                case CurrencyType.Token:
                    return PlayerPrefs.GetInt("PlayerToken", 0);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 获取货币名称
        /// </summary>
        private string GetCurrencyName(CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.Coin: return "金币";
                case CurrencyType.Diamond: return "钻石";
                case CurrencyType.Token: return "代币";
                default: return "货币";
            }
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        private long GetCurrentTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 判断是否同一天
        /// </summary>
        private bool IsSameDay(long time1, long time2)
        {
            var date1 = DateTimeOffset.FromUnixTimeSeconds(time1).DateTime.Date;
            var date2 = DateTimeOffset.FromUnixTimeSeconds(time2).DateTime.Date;
            return date1 == date2;
        }

        /// <summary>
        /// 判断是否同一周
        /// </summary>
        private bool IsSameWeek(long time1, long time2)
        {
            var date1 = DateTimeOffset.FromUnixTimeSeconds(time1).DateTime;
            var date2 = DateTimeOffset.FromUnixTimeSeconds(time2).DateTime;
            
            // 获取周一
            int days1 = (int)date1.DayOfWeek;
            int days2 = (int)date2.DayOfWeek;
            if (days1 == 0) days1 = 7;
            if (days2 == 0) days2 = 7;
            
            var monday1 = date1.AddDays(-days1 + 1);
            var monday2 = date2.AddDays(-days2 + 1);
            
            return monday1 == monday2;
        }

        /// <summary>
        /// 加载购买记录
        /// </summary>
        private void LoadPurchaseRecords(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            
            try
            {
                var records = JsonUtility.FromJson<PurchaseRecordsData>(json);
                if (records != null && records.Records != null)
                {
                    foreach (var record in records.Records)
                    {
                        _purchaseRecords[record.GoodsId] = record;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("[ShopSystem] Load purchase records failed: {0}", e.Message));
            }
        }

        /// <summary>
        /// 加载月卡状态
        /// </summary>
        private void LoadSubscriptionStates(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            
            try
            {
                var states = JsonUtility.FromJson<SubscriptionStatesData>(json);
                if (states != null && states.States != null)
                {
                    foreach (var state in states.States)
                    {
                        _subscriptionStates[state.SubscriptionId] = state;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("[ShopSystem] Load subscription states failed: {0}", e.Message));
            }
        }

        /// <summary>
        /// 获取购买记录JSON
        /// </summary>
        private string GetPurchaseRecordsJson()
        {
            var data = new PurchaseRecordsData
            {
                Records = new List<PurchaseRecord>(_purchaseRecords.Values)
            };
            return JsonUtility.ToJson(data);
        }

        /// <summary>
        /// 获取月卡状态JSON
        /// </summary>
        private string GetSubscriptionStatesJson()
        {
            var data = new SubscriptionStatesData
            {
                States = new List<SubscriptionState>(_subscriptionStates.Values)
            };
            return JsonUtility.ToJson(data);
        }
    }

    //===========================================================
    // 结果数据结构
    //===========================================================

    /// <summary>
    /// 购买结果
    /// </summary>
    public class PurchaseResult
    {
        public bool Success;
        public string ErrorMessage;
        public ShopGoodsConfig GoodsConfig;
        public int PurchaseCount;
        public int TotalGoodsCount;
    }

    /// <summary>
    /// 限购检查结果
    /// </summary>
    public class LimitCheckResult
    {
        public bool CanPurchase;
        public string ErrorMessage;
    }

    /// <summary>
    /// 货币检查结果
    /// </summary>
    public class CurrencyCheckResult
    {
        public bool HasEnough;
        public int CurrentAmount;
        public int RequiredAmount;
    }

    /// <summary>
    /// 领取结果
    /// </summary>
    public class ClaimResult
    {
        public bool Success;
        public string ErrorMessage;
        public GoodsType RewardType;
        public int RewardCount;
    }

    //===========================================================
    // 数据结构(序列化用)
    //===========================================================

    [Serializable]
    public class ShopData
    {
        public string PurchaseRecords;
        public string SubscriptionStates;
    }

    [Serializable]
    public class PurchaseRecordsData
    {
        public List<PurchaseRecord> Records;
    }

    [Serializable]
    public class SubscriptionStatesData
    {
        public List<SubscriptionState> States;
    }
}
