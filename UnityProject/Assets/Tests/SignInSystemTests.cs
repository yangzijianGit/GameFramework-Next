//===========================================================
// SignInSystem 单元测试
// TDD格式：Arrange - Act - Assert
//===========================================================

#if UNITY_TESTS || UNITY_WEBGL && !UNITY_EDITOR

using System;
using UnityEngine;
using NUnit.Framework;

namespace GameMain.Tests
{
    /// <summary>
    /// 签到系统单元测试
    /// 测试要点：
    /// 1. 签到系统初始化
    /// 2. 签到功能
    /// 3. 重复签到检测
    /// 4. 累积奖励
    /// 5. 签到状态查询
    /// </summary>
    [TestFixture]
    public class SignInSystemTests
    {
        private SignInSystem m_signInSystem;

        //========================== 测试初始化 ==========================

        [SetUp]
        public void SetUp()
        {
            m_signInSystem = SignInSystem.Instance;
            m_signInSystem.OnInit();
        }

        [TearDown]
        public void TearDown()
        {
            m_signInSystem.Test_ResetSignInData();
            m_signInSystem.SaveData();
        }

        //========================== 初始化测试 ==========================

        /// <summary>
        /// 测试：签到系统初始化
        /// 预期：正确加载奖励配置
        /// </summary>
        [Test]
        public void Test_Initialization_LoadsRewardConfigs()
        {
            // Act
            var rewards = m_signInSystem.GetRewardList();

            // Assert
            Assert.IsNotNull(rewards, "奖励列表不应为空");
            Assert.AreEqual(7, rewards.Count, "应有7天签到奖励");
            
            Debug.Log($"签到奖励天数: {rewards.Count}");
        }

        //========================== 签到测试 ==========================

        /// <summary>
        /// 测试：签到成功
        /// 预期：签到成功，返回正确奖励
        /// </summary>
        [Test]
        public void Test_SignIn_Success()
        {
            // Act
            var result = m_signInSystem.SignIn();

            // Assert
            Assert.IsTrue(result.Success, "签到应成功");
            Assert.IsNotNull(result.Reward, "应有奖励");
            Assert.Greater(result.CumulativeDays, 0, "累积天数应大于0");
            
            Debug.Log($"签到成功: Day={result.CumulativeDays}, Reward={result.Reward.RewardType}");
        }

        /// <summary>
        /// 测试：重复签到
        /// 预期：签到失败
        /// </summary>
        [Test]
        public void Test_SignIn_AlreadySignedIn_Fails()
        {
            // 第一次签到
            var firstResult = m_signInSystem.SignIn();
            Assert.IsTrue(firstResult.Success, "第一次签到应成功");

            // 第二次签到
            var secondResult = m_signInSystem.SignIn();

            // Assert
            Assert.IsFalse(secondResult.Success, "重复签到应失败");
            Assert.AreEqual("Already signed in today", secondResult.ErrorMessage);
        }

        /// <summary>
        /// 测试：签到后状态更新
        /// 预期：签到后今日已签到
        /// </summary>
        [Test]
        public void Test_SignIn_UpdatesStatus()
        {
            // Act
            m_signInSystem.SignIn();

            // Assert
            Assert.IsTrue(m_signInSystem.IsSignedInToday(), "今日应已签到");
        }

        //========================== 累积奖励测试 ==========================

        /// <summary>
        /// 测试：累积签到7天
        /// 预期：可以领取累积奖励
        /// </summary>
        [Test]
        public void Test_CumulativeReward_7Days_CanClaim()
        {
            // 模拟签到7天
            for (int i = 1; i <= 7; i++)
            {
                m_signInSystem.Test_SignIn(i);
            }

            // 检查状态
            var status = m_signInSystem.GetSignInStatus();
            
            // Assert
            Assert.AreEqual(7, status.CumulativeDays, "累积天数应为7");
            Assert.IsTrue(status.CanClaimCumulativeReward, "应可领取累积奖励");
        }

        /// <summary>
        /// 测试：领取累积奖励
        /// 预期：领取成功
        /// </summary>
        [Test]
        public void Test_ClaimCumulativeReward_Success()
        {
            // 模拟签到7天
            for (int i = 1; i <= 7; i++)
            {
                m_signInSystem.Test_SignIn(i);
            }

            // Act
            bool result = m_signInSystem.ClaimCumulativeReward();

            // Assert
            Assert.IsTrue(result, "领取应成功");
        }

        /// <summary>
        /// 测试：未满足条件领取累积奖励
        /// 预期：领取失败
        /// </summary>
        [Test]
        public void Test_ClaimCumulativeReward_NotEnough_Fails()
        {
            // 只签到3天
            m_signInSystem.Test_SignIn(1);
            m_signInSystem.Test_SignIn(2);
            m_signInSystem.Test_SignIn(3);

            // Act
            bool result = m_signInSystem.ClaimCumulativeReward();

            // Assert
            Assert.IsFalse(result, "未满足条件应领取失败");
        }

        /// <summary>
        /// 测试：重复领取累积奖励
        /// 预期：领取失败
        /// </summary>
        [Test]
        public void Test_ClaimCumulativeReward_AlreadyClaimed_Fails()
        {
            // 签到7天并领取
            for (int i = 1; i <= 7; i++)
            {
                m_signInSystem.Test_SignIn(i);
            }
            m_signInSystem.ClaimCumulativeReward();

            // Act
            bool result = m_signInSystem.ClaimCumulativeReward();

            // Assert
            Assert.IsFalse(result, "重复领取应失败");
        }

        //========================== 状态查询测试 ==========================

        /// <summary>
        /// 测试：获取签到状态
        /// 预期：返回正确状态
        /// </summary>
        [Test]
        public void Test_GetSignInStatus_ReturnsCorrectStatus()
        {
            // Act
            var status = m_signInSystem.GetSignInStatus();

            // Assert
            Assert.IsNotNull(status, "状态不应为空");
            Assert.IsFalse(status.IsSignedInToday, "初始应未签到");
            Assert.AreEqual(0, status.CumulativeDays, "初始累积天数应为0");
            Assert.GreaterOrEqual(status.RemainingDays, 0, "剩余天数应>=0");
        }

        /// <summary>
        /// 测试：签到后天数更新
        /// 预期：签到后天数正确
        /// </summary>
        [Test]
        public void Test_SignIn_UpdatesDaysCorrectly()
        {
            // Act
            m_signInSystem.SignIn();

            // Assert
            var status = m_signInSystem.GetSignInStatus();
            Assert.AreEqual(1, status.MonthSignInDays, "本月签到天数应为1");
            Assert.AreEqual(1, status.CumulativeDays, "累积天数应为1");
        }

        //========================== 数据持久化测试 ==========================

        /// <summary>
        /// 测试：数据保存和加载
        /// 预期：签到数据正确保存和恢复
        /// </summary>
        [Test]
        public void Test_SaveAndLoadData_PreservesSignIn()
        {
            // 签到
            m_signInSystem.SignIn();
            m_signInSystem.SaveData();

            // 重新初始化
            var newSystem = SignInSystem.Instance;
            newSystem.OnInit();

            // Assert
            Assert.IsTrue(newSystem.IsSignedInToday(), "签到状态应正确保存");
        }

        //========================== 边界测试 ==========================

        /// <summary>
        /// 测试：签到奖励配置完整性
        /// 预期：每天都有对应奖励
        /// </summary>
        [Test]
        public void Test_RewardConfigs_Complete()
        {
            // Arrange
            var rewards = m_signInSystem.GetRewardList();

            // Assert
            for (int day = 1; day <= 7; day++)
            {
                var reward = rewards.Find(r => r.Day == day);
                Assert.IsNotNull(reward, $"第{day}天应有奖励配置");
                Assert.Greater(reward.RewardCount, 0, $"第{day}天奖励数量应>0");
            }
        }

        /// <summary>
        /// 测试：第7天为累积奖励
        /// 预期：第7天标记为累积奖励
        /// </summary>
        [Test]
        public void Test_Day7_IsCumulativeReward()
        {
            // Arrange
            var rewards = m_signInSystem.GetRewardList();
            var day7Reward = rewards.Find(r => r.Day == 7);

            // Assert
            Assert.IsNotNull(day7Reward, "第7天应有奖励");
            Assert.IsTrue(day7Reward.IsCumulative, "第7天应为累积奖励");
            Assert.AreEqual(RewardType.Clothes, day7Reward.RewardType, "第7天应为服装奖励");
        }
    }
}

#endif
