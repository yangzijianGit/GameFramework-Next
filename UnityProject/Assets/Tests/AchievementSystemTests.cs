//===========================================================
// AchievementSystem 单元测试
// TDD格式：Arrange - Act - Assert
//===========================================================

#if UNITY_TESTS || UNITY_WEBGL && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace GameMain.Tests
{
    /// <summary>
    /// 成就系统单元测试
    /// 测试要点：
    /// 1. 成就初始化
    /// 2. 成就进度更新
    /// 3. 成就完成判定
    /// 4. 成就奖励领取
    /// 5. 成就链解锁
    /// 6. 隐藏成就解锁
    /// </summary>
    [TestFixture]
    public class AchievementSystemTests
    {
        private AchievementSystem m_achievementSystem;

        //========================== 测试初始化 ==========================

        [SetUp]
        public void SetUp()
        {
            m_achievementSystem = AchievementSystem.Instance;
            m_achievementSystem.OnInit();
        }

        [TearDown]
        public void TearDown()
        {
            m_achievementSystem.Test_ResetAllAchievements();
            m_achievementSystem.SaveData();
        }

        //========================== 初始化测试 ==========================

        /// <summary>
        /// 测试：成就系统初始化
        /// 预期：正确加载成就配置
        /// </summary>
        [Test]
        public void Test_Initialization_LoadsAchievementConfigs()
        {
            // Act
            var passLevelAchievements = m_achievementSystem.GetAchievementList(AchievementCategory.PassLevel);
            var collectAchievements = m_achievementSystem.GetAchievementList(AchievementCategory.Collect);
            var allAchievements = m_achievementSystem.GetAllAchievements();

            // Assert
            Assert.IsNotNull(passLevelAchievements);
            Assert.IsNotNull(collectAchievements);
            Assert.IsNotNull(allAchievements);
            
            Assert.Greater(passLevelAchievements.Count, 0, "应有通关成就");
            Assert.Greater(collectAchievements.Count, 0, "应有收集成就");
            Assert.Greater(allAchievements.Count, 0, "应有成就在系统中");
            
            Debug.Log($"成就数 - 通关:{passLevelAchievements.Count}, 收集:{collectAchievements.Count}, 总计:{allAchievements.Count}");
        }

        //========================== 进度更新测试 ==========================

        /// <summary>
        /// 测试：成就进度更新
        /// 预期：进度正确累加
        /// </summary>
        [Test]
        public void Test_UpdateProgress_IncreasesProgress()
        {
            // Arrange
            var achievementId = "ach_pass_10";
            var achievement = m_achievementSystem.GetAchievementData(achievementId);
            Assert.IsNotNull(achievement, $"成就{achievementId}应存在");
            
            int initialProgress = achievement.CurrentProgress;

            // Act
            m_achievementSystem.UpdateAchievementProgress(TaskConditionType.PassLevel, 5);

            // Assert
            var updatedAchievement = m_achievementSystem.GetAchievementData(achievementId);
            Assert.AreEqual(initialProgress + 5, updatedAchievement.CurrentProgress, 
                "进度应增加5");
        }

        /// <summary>
        /// 测试：成就进度上限
        /// 预期：进度不超过目标值
        /// </summary>
        [Test]
        public void Test_UpdateProgress_CappedAtTarget()
        {
            // Arrange
            var achievementId = "ach_pass_10";
            var targetCount = 10; // 从配置可知

            // Act
            m_achievementSystem.UpdateAchievementProgress(TaskConditionType.PassLevel, 15);

            // Assert
            var achievement = m_achievementSystem.GetAchievementData(achievementId);
            Assert.LessOrEqual(achievement.CurrentProgress, targetCount, "进度不应超过目标");
        }

        //========================== 成就完成测试 ==========================

        /// <summary>
        /// 测试：成就完成判定
        /// 预期：达到目标后状态变为Completed
        /// </summary>
        [Test]
        public void Test_AchievementCompletion_ReachesTarget_ChangesStateToCompleted()
        {
            // Arrange
            var achievementId = "ach_pass_10";

            // Act
            m_achievementSystem.UpdateAchievementProgress(TaskConditionType.PassLevel, 10);

            // Assert
            var achievement = m_achievementSystem.GetAchievementData(achievementId);
            Assert.AreEqual(AchievementState.Completed, achievement.State, 
                "达到目标后状态应为Completed");
        }

        /// <summary>
        /// 测试：成就完成判定 - 未达目标
        /// 预期：状态保持InProgress
        /// </summary>
        [Test]
        public void Test_AchievementCompletion_BelowTarget_StaysInProgress()
        {
            // Arrange
            var achievementId = "ach_pass_10";

            // Act
            m_achievementSystem.UpdateAchievementProgress(TaskConditionType.PassLevel, 9);

            // Assert
            var achievement = m_achievementSystem.GetAchievementData(achievementId);
            Assert.AreEqual(AchievementState.InProgress, achievement.State, 
                "未达目标应保持InProgress");
        }

        //========================== 奖励领取测试 ==========================

        /// <summary>
        /// 测试：领取成就奖励 - 成功
        /// 预期：奖励成功，状态变为Claimed
        /// </summary>
        [Test]
        public void Test_ClaimReward_CompletedAchievement_Succeeds()
        {
            // Arrange
            var achievementId = "ach_pass_10";
            
            // 完成成就
            m_achievementSystem.Test_CompleteAchievement(achievementId);
            
            var achievementBeforeClaim = m_achievementSystem.GetAchievementData(achievementId);
            Assert.AreEqual(AchievementState.Completed, achievementBeforeClaim.State);

            // Act
            bool result = m_achievementSystem.ClaimAchievementReward(achievementId);

            // Assert
            Assert.IsTrue(result, "领取应成功");
            
            var achievementAfterClaim = m_achievementSystem.GetAchievementData(achievementId);
            Assert.AreEqual(AchievementState.Claimed, achievementAfterClaim.State, 
                "领取后状态应为Claimed");
            Assert.IsTrue(achievementAfterClaim.IsRewardClaimed);
        }

        /// <summary>
        /// 测试：领取成就奖励 - 未完成
        /// 预期：领取失败
        /// </summary>
        [Test]
        public void Test_ClaimReward_IncompleteAchievement_Fails()
        {
            // Arrange
            var achievementId = "ach_pass_10";
            var achievement = m_achievementSystem.GetAchievementData(achievementId);
            Assert.AreEqual(AchievementState.InProgress, achievement.State);

            // Act
            bool result = m_achievementSystem.ClaimAchievementReward(achievementId);

            // Assert
            Assert.IsFalse(result, "未完成不应领取成功");
        }

        /// <summary>
        /// 测试：领取成就奖励 - 已领取
        /// 预期：重复领取失败
        /// </summary>
        [Test]
        public void Test_ClaimReward_AlreadyClaimed_Fails()
        {
            // Arrange
            var achievementId = "ach_pass_10";
            
            // 完成并领取
            m_achievementSystem.Test_CompleteAchievement(achievementId);
            m_achievementSystem.ClaimAchievementReward(achievementId);

            // Act
            bool result = m_achievementSystem.ClaimAchievementReward(achievementId);

            // Assert
            Assert.IsFalse(result, "已领取不应重复领取");
        }

        //========================== 成就链测试 ==========================

        /// <summary>
        /// 测试：成就链解锁
        /// 预期：完成前置成就后解锁下一个成就
        /// </summary>
        [Test]
        public void Test_AchievementChain_NextUnlocksAfterPreCompleted()
        {
            // Arrange
            var preAchievementId = "ach_pass_10"; // 10关成就是50关的前置
            var nextAchievementId = "ach_pass_50";
            
            var nextAchievement = m_achievementSystem.GetAchievementData(nextAchievementId);
            Assert.AreEqual(AchievementState.Locked, nextAchievement.State, 
                "初始状态应为Locked");

            // Act
            m_achievementSystem.Test_CompleteAchievement(preAchievementId);
            m_achievementSystem.ClaimAchievementReward(preAchievementId);

            // Assert
            var updatedNextAchievement = m_achievementSystem.GetAchievementData(nextAchievementId);
            Assert.AreEqual(AchievementState.InProgress, updatedNextAchievement.State, 
                "前置完成后应解锁下一个成就");
        }

        //========================== 完成度测试 ==========================

        /// <summary>
        /// 测试：完成度计算
        /// 预期：正确计算已完成的成就比例
        /// </summary>
        [Test]
        public void Test_CompletionRate_CalculatesCorrectly()
        {
            // Arrange
            var allAchievements = m_achievementSystem.GetAllAchievements();
            int totalCount = allAchievements.Count;
            
            // Act
            float initialRate = m_achievementSystem.GetCompletionRate();
            int initialCompleted = m_achievementSystem.GetCompletedCount();

            // Assert
            Assert.Greater(totalCount, 0, "应有成就总数");
            Assert.AreEqual(0, initialCompleted, "初始应无完成");
            Assert.AreEqual(0f, initialRate, "初始完成度应为0");
        }

        /// <summary>
        /// 测试：完成度计算 - 部分完成
        /// 预期：正确计算比例
        /// </summary>
        [Test]
        public void Test_CompletionRate_PartialCompletion()
        {
            // Arrange
            var achievementId = "ach_pass_10";
            
            // 完成1个成就
            m_achievementSystem.Test_CompleteAchievement(achievementId);
            m_achievementSystem.ClaimAchievementReward(achievementId);

            // Act
            float rate = m_achievementSystem.GetCompletionRate();
            int completedCount = m_achievementSystem.GetCompletedCount();

            // Assert
            Assert.Greater(rate, 0f, "完成度应大于0");
            Assert.Greater(completedCount, 0, "完成数应大于0");
            Debug.Log($"完成度: {rate}, 已完成: {completedCount}");
        }

        //========================== 分类测试 ==========================

        /// <summary>
        /// 测试：获取分类成就
        /// 预期：返回正确分类的成就列表
        /// </summary>
        [Test]
        public void Test_GetAchievementList_ByCategory()
        {
            // Act
            var passLevelList = m_achievementSystem.GetAchievementList(AchievementCategory.PassLevel);
            var collectList = m_achievementSystem.GetAchievementList(AchievementCategory.Collect);
            var secretList = m_achievementSystem.GetAchievementList(AchievementCategory.Secret);

            // Assert
            Assert.IsNotNull(passLevelList);
            Assert.IsNotNull(collectList);
            Assert.IsNotNull(secretList);
            
            Assert.Greater(passLevelList.Count, 0, "应有通关成就");
            Assert.Greater(collectList.Count, 0, "应有收集成就");
        }

        //========================== 数据持久化测试 ==========================

        /// <summary>
        /// 测试：数据保存和加载
        /// 预期：数据正确保存和恢复
        /// </summary>
        [Test]
        public void Test_SaveAndLoadData_PreservesProgress()
        {
            // Arrange
            var achievementId = "ach_pass_10";
            
            // 完成部分进度
            m_achievementSystem.UpdateAchievementProgress(TaskConditionType.PassLevel, 5);
            m_achievementSystem.SaveData();

            // 重新初始化
            var newSystem = AchievementSystem.Instance;
            newSystem.OnInit();

            // Assert
            var achievement = newSystem.GetAchievementData(achievementId);
            Assert.AreEqual(5, achievement.CurrentProgress, "进度应正确保存");
        }
    }
}

#endif
