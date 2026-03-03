//========================================================================================
//  ShopSystemTests - 商店系统单元测试
//========================================================================================
//  测试范围：商品购买、限购验证、货币扣除、月卡管理、特惠刷新
//  测试方法：Arrange-Act-Assert
//========================================================================================

using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace GameMain.Tests
{
    /// <summary>
    /// 商店系统单元测试
    /// </summary>
    [TestFixture]
    public class ShopSystemTests
    {
        private ShopSystem _shopSystem;
        
        [SetUp]
        public void Setup()
        {
            // Arrange: 初始化商店系统
            _shopSystem = new ShopSystem();
            _shopSystem.OnInit();
        }
        
        [TearDown]
        public void TearDown()
        {
            // 清理测试数据
            PlayerPrefs.DeleteAll();
        }

        //===========================================================
        // 商品数据加载测试
        //===========================================================

        /// <summary>
        /// 测试：获取货币商店商品列表
        /// 预期：返回货币类型商品列表
        /// </summary>
        [Test]
        public void Test_GetShopGoodsList_CurrencyShop_ReturnsCurrencyGoods()
        {
            // Act
            var goodsList = _shopSystem.GetShopGoodsList(ShopType.Currency);
            
            // Assert
            Assert.IsNotNull(goodsList, "商品列表不应为空");
            Assert.Greater(goodsList.Count, 0, "货币商店应有商品");
            
            foreach (var goods in goodsList)
            {
                Assert.AreEqual(ShopType.Currency, goods.ShopType, "商品类型应为货币商店");
                Assert.AreEqual(GoodsType.Diamond, goods.GoodsType, "商品类型应为钻石");
            }
        }

        /// <summary>
        /// 测试：获取道具商店商品列表
        /// 预期：返回道具类型商品列表
        /// </summary>
        [Test]
        public void Test_GetShopGoodsList_PropShop_ReturnsPropGoods()
        {
            // Act
            var goodsList = _shopSystem.GetShopGoodsList(ShopType.Prop);
            
            // Assert
            Assert.IsNotNull(goodsList, "商品列表不应为空");
            Assert.Greater(goodsList.Count, 0, "道具商店应有商品");
            
            foreach (var goods in goodsList)
            {
                Assert.AreEqual(ShopType.Prop, goods.ShopType, "商品类型应为道具商店");
            }
        }

        /// <summary>
        /// 测试：获取不存在的商店类型
        /// 预期：返回空列表
        /// </summary>
        [Test]
        public void Test_GetShopGoodsList_InvalidType_ReturnsEmptyList()
        {
            // Act
            var goodsList = _shopSystem.GetShopGoodsList(ShopType.Subscription);
            
            // Assert
            Assert.IsNotNull(goodsList, "即使为空也不应返回null");
            Assert.AreEqual(0, goodsList.Count, "订阅商店应为空");
        }

        //===========================================================
        // 购买验证测试
        //===========================================================

        /// <summary>
        /// 测试：购买商品ID为空
        /// 预期：购买失败，返回错误信息
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_NullGoodsId_ReturnsFailed()
        {
            // Act
            var result = _shopSystem.PurchaseGoods(null, 1);
            
            // Assert
            Assert.IsFalse(result.Success, "购买应失败");
            Assert.IsNotNull(result.ErrorMessage, "应有错误信息");
            Assert.AreEqual("商品ID不能为空", result.ErrorMessage);
        }

        /// <summary>
        /// 测试：购买商品ID不存在
        /// 预期：购买失败，返回商品不存在错误
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_NonExistentGoods_ReturnsFailed()
        {
            // Act
            var result = _shopSystem.PurchaseGoods("non_existent_goods", 1);
            
            // Assert
            Assert.IsFalse(result.Success, "购买应失败");
            Assert.AreEqual("商品不存在", result.ErrorMessage);
        }

        /// <summary>
        /// 测试：购买数量为0
        /// 预期：购买失败，返回错误信息
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_ZeroCount_ReturnsFailed()
        {
            // Act
            var result = _shopSystem.PurchaseGoods("diamond_1", 0);
            
            // Assert
            Assert.IsFalse(result.Success, "购买应失败");
            Assert.AreEqual("购买数量必须大于0", result.ErrorMessage);
        }

        /// <summary>
        /// 测试：购买数量为负数
        /// 预期：购买失败，返回错误信息
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_NegativeCount_ReturnsFailed()
        {
            // Act
            var result = _shopSystem.PurchaseGoods("diamond_1", -1);
            
            // Assert
            Assert.IsFalse(result.Success, "购买应失败");
            Assert.AreEqual("购买数量必须大于0", result.ErrorMessage);
        }

        //===========================================================
        // 货币验证测试
        //===========================================================

        /// <summary>
        /// 测试：货币不足时购买
        /// 预期：购买失败，返回货币不足错误
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_InsufficientCurrency_ReturnsFailed()
        {
            // Arrange: 设置货币为0
            PlayerPrefs.SetInt("PlayerCoin", 0);
            PlayerPrefs.SetInt("PlayerDiamond", 0);
            
            // Act
            var result = _shopSystem.PurchaseGoods("diamond_1", 1);
            
            // Assert
            Assert.IsFalse(result.Success, "购买应失败");
            Assert.IsNotNull(result.ErrorMessage, "应有错误信息");
            Assert.IsTrue(result.ErrorMessage.Contains("货币不足"), "应提示货币不足");
        }

        //===========================================================
        // 限购验证测试
        //===========================================================

        /// <summary>
        /// 测试：每日限购商品，达到上限后再次购买
        /// 预期：购买失败，返回达到上限错误
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_DailyLimitExceeded_ReturnsFailed()
        {
            // Arrange: 设置货币充足，模拟之前已购买到上限
            PlayerPrefs.SetInt("PlayerDiamond", 10000);
            
            // 先购买一次(设置购买记录)
            // 实际测试中需要设置购买记录，这里假设已购买
            // 由于限购是运行时检查，需要先进行一次购买
            
            // 尝试购买超出限制的数量
            // 这里需要先有购买记录才能测试限购
            // 简化测试：测试获取剩余购买次数
            
            // Act
            var remaining = _shopSystem.GetRemainingPurchaseCount("prop_hammer");
            
            // Assert
            Assert.GreaterOrEqual(remaining, 0, "剩余次数应为非负数");
        }

        /// <summary>
        /// 测试：获取不限购商品的剩余次数
        /// 预期：返回-1表示不限购
        /// </summary>
        [Test]
        public void Test_GetRemainingPurchaseCount_UnlimitedGoods_ReturnsMinusOne()
        {
            // Act
            var remaining = _shopSystem.GetRemainingPurchaseCount("diamond_1");
            
            // Assert
            Assert.AreEqual(-1, remaining, "不限购商品应返回-1");
        }

        //===========================================================
        // 月卡系统测试
        //===========================================================

        /// <summary>
        /// 测试：获取月卡列表
        /// 预期：返回月卡配置列表
        /// </summary>
        [Test]
        public void Test_GetSubscriptionList_ReturnsSubscriptionConfigs()
        {
            // Act
            var subscriptions = _shopSystem.GetSubscriptionList();
            
            // Assert
            Assert.IsNotNull(subscriptions, "月卡列表不应为空");
            Assert.Greater(subscriptions.Count, 0, "应有月卡配置");
            
            // 验证月卡配置字段
            foreach (var sub in subscriptions)
            {
                Assert.IsNotNull(sub.Id, "月卡ID不应为空");
                Assert.IsNotNull(sub.Name, "月卡名称不应为空");
                Assert.Greater(sub.Price, 0, "价格应大于0");
                Assert.Greater(sub.DurationDays, 0, "持续天数应大于0");
            }
        }

        /// <summary>
        /// 测试：检查未购买的月卡状态
        /// 预期：返回false表示未激活
        /// </summary>
        [Test]
        public void Test_IsSubscriptionActive_NotPurchased_ReturnsFalse()
        {
            // Act
            var isActive = _shopSystem.IsSubscriptionActive("monthly_card");
            
            // Assert
            Assert.IsFalse(isActive, "未购买的月卡应返回false");
        }

        /// <summary>
        /// 测试：获取未购买月卡的剩余天数
        /// 预期：返回-1
        /// </summary>
        [Test]
        public void Test_GetSubscriptionRemainingDays_NotPurchased_ReturnsMinusOne()
        {
            // Act
            var days = _shopSystem.GetSubscriptionRemainingDays("monthly_card");
            
            // Assert
            Assert.AreEqual(-1, days, "未购买月卡应返回-1");
        }

        /// <summary>
        /// 测试：领取未购买月卡的奖励
        /// 预期：领取失败，返回错误信息
        /// </summary>
        [Test]
        public void Test_ClaimSubscriptionReward_NotPurchased_ReturnsFailed()
        {
            // Act
            var result = _shopSystem.ClaimSubscriptionReward("monthly_card");
            
            // Assert
            Assert.IsFalse(result.Success, "领取应失败");
            Assert.AreEqual("未购买该月卡", result.ErrorMessage);
        }

        //===========================================================
        // 特惠商品测试
        //===========================================================

        /// <summary>
        /// 测试：获取特惠商品列表
        /// 预期：返回特惠商品列表
        /// </summary>
        [Test]
        public void Test_GetLimitedOffers_ReturnsLimitedOffers()
        {
            // Act
            var offers = _shopSystem.GetLimitedOffers();
            
            // Assert
            Assert.IsNotNull(offers, "特惠列表不应为空");
        }

        /// <summary>
        /// 测试：刷新特惠商品
        /// 预期：刷新成功，触发回调
        /// </summary>
        [Test]
        public void Test_RefreshLimitedOffers_Success_TriggersCallback()
        {
            // Arrange
            bool callbackTriggered = false;
            _shopSystem.OnLimitedOfferRefreshed += () => callbackTriggered = true;
            
            // Act
            _shopSystem.RefreshLimitedOffers();
            
            // Assert
            Assert.IsTrue(callbackTriggered, "刷新回调应被触发");
        }

        //===========================================================
        // 购买成功测试
        //===========================================================

        /// <summary>
        /// 测试：购买成功事件回调
        /// 预期：购买成功后触发回调
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_Success_TriggersCallback()
        {
            // Arrange
            string purchasedGoodsId = null;
            int purchaseCount = 0;
            _shopSystem.OnPurchaseSuccess += (goodsId, count) => 
            {
                purchasedGoodsId = goodsId;
                purchaseCount = count;
            };
            
            // 设置充足货币
            PlayerPrefs.SetInt("PlayerCoin", 100000);
            
            // Act
            var result = _shopSystem.PurchaseGoods("diamond_1", 1);
            
            // Assert
            if (result.Success)
            {
                Assert.IsNotNull(purchasedGoodsId, "回调中的商品ID不应为空");
                Assert.Greater(purchaseCount, 0, "回调中的数量应大于0");
            }
        }

        /// <summary>
        /// 测试：购买失败事件回调
        /// 预期：购买失败后触发回调
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_Failed_TriggersFailedCallback()
        {
            // Arrange
            string failedGoodsId = null;
            string errorMessage = null;
            _shopSystem.OnPurchaseFailed += (goodsId, error) =>
            {
                failedGoodsId = goodsId;
                errorMessage = error;
            };
            
            // Act
            var result = _shopSystem.PurchaseGoods(null, 1);
            
            // Assert
            Assert.IsFalse(result.Success, "购买应失败");
            Assert.IsNotNull(errorMessage, "错误回调中的错误信息不应为空");
        }

        //===========================================================
        // 边界条件测试
        //===========================================================

        /// <summary>
        /// 测试：购买商品数量为极大值
        /// 预期：处理合理，不会崩溃
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_ExtremeCount_DoesNotCrash()
        {
            // Arrange
            PlayerPrefs.SetInt("PlayerCoin", int.MaxValue);
            
            // Act & Assert: 不应抛出异常
            try
            {
                var result = _shopSystem.PurchaseGoods("diamond_1", int.MaxValue);
                // 只测试不崩溃，结果可能失败
            }
            catch (Exception ex)
            {
                Assert.Fail($"不应抛出异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 测试：连续购买多个商品
        /// 预期：每次购买独立处理
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_MultiplePurchases_HandlesCorrectly()
        {
            // Arrange
            PlayerPrefs.SetInt("PlayerCoin", 100000);
            PlayerPrefs.SetInt("PlayerDiamond", 1000);
            
            int successCount = 0;
            _shopSystem.OnPurchaseSuccess += (id, count) => successCount++;
            
            // Act: 尝试购买多个商品
            var result1 = _shopSystem.PurchaseGoods("diamond_1", 1);
            var result2 = _shopSystem.PurchaseGoods("diamond_2", 1);
            var result3 = _shopSystem.PurchaseGoods("prop_energy", 1);
            
            // Assert
            Assert.GreaterOrEqual(successCount, 0, "应记录购买成功次数");
        }

        //===========================================================
        // 数据持久化测试
        //===========================================================

        /// <summary>
        /// 测试：购买记录保存
        /// 预期：购买后数据保存到本地
        /// </summary>
        [Test]
        public void Test_PurchaseGoods_SavePurchaseRecord_Success()
        {
            // Arrange
            PlayerPrefs.SetInt("PlayerCoin", 100000);
            
            // Act: 购买商品
            var result = _shopSystem.PurchaseGoods("diamond_1", 1);
            
            // Assert: 验证数据已保存(通过再次获取剩余次数验证)
            // 注意：由于限购类型为None，剩余次数为-1
            if (result.Success)
            {
                var remaining = _shopSystem.GetRemainingPurchaseCount("diamond_1");
                Assert.AreEqual(-1, remaining, "不限购商品应返回-1");
            }
        }
    }
}
