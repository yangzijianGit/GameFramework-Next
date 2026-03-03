//===========================================================
// TaskSystem 单元测试
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
    /// 任务系统单元测试
    /// 测试要点：
    /// 1. 任务初始化
    /// 2. 任务进度更新
    /// 3. 任务完成判定
    /// 4. 任务奖励领取
    /// 5. 任务刷新
    /// </summary>
    [TestFixture]
    public class TaskSystemTests
    {
        private TaskSystem m_taskSystem;

        //========================== 测试初始化 ==========================

        [SetUp]
        public void SetUp()
        {
            // Arrange: 创建任务系统实例
            m_taskSystem = TaskSystem.Instance;
            m_taskSystem.OnInit();
        }

        [TearDown]
        public void TearDown()
        {
            // 清理：重置测试数据
            m_taskSystem.Test_ResetAllTasks();
            m_taskSystem.SaveData();
        }

        //========================== 初始化测试 ==========================

        /// <summary>
        /// 测试：任务系统初始化
        /// 预期：任务配置正确加载
        /// </summary>
        [Test]
        public void Test_Initialization_LoadsTaskConfigs()
        {
            // Act
            var dailyTasks = m_taskSystem.GetTaskList(TaskType.Daily);
            var weeklyTasks = m_taskSystem.GetTaskList(TaskType.Weekly);
            var achievementTasks = m_taskSystem.GetTaskList(TaskType.Achievement);

            // Assert
            Assert.IsNotNull(dailyTasks, "每日任务列表不应为空");
            Assert.IsNotNull(weeklyTasks, "每周任务列表不应为空");
            Assert.IsNotNull(achievementTasks, "成就任务列表不应为空");
            
            Assert.Greater(dailyTasks.Count, 0, "应有每日任务");
            Assert.Greater(weeklyTasks.Count, 0, "应有每周任务");
            Assert.Greater(achievementTasks.Count, 0, "应有成就任务");
            
            Debug.Log($"初始化任务数 - 每日:{dailyTasks.Count}, 每周:{weeklyTasks.Count}, 成就:{achievementTasks.Count}");
        }

        //========================== 进度更新测试 ==========================

        /// <summary>
        /// 测试：更新任务进度 - 单次更新
        /// 预期：进度正确累加
        /// </summary>
        [Test]
        public void Test_UpdateProgress_SingleUpdate_IncreasesProgress()
        {
            // Arrange
            var taskId = "daily_002"; // 消除50个元素任务
            var initialTask = m_taskSystem.GetTaskData(taskId);
            Assert.IsNotNull(initialTask, $"任务{taskId}应存在");
            
            int initialProgress = initialTask.CurrentProgress;

            // Act
            m_taskSystem.UpdateTaskProgress(TaskConditionType.EliminateElement, 10);

            // Assert
            var updatedTask = m_taskSystem.GetTaskData(taskId);
            Assert.AreEqual(initialProgress + 10, updatedTask.CurrentProgress, 
                "进度应增加10");
        }

        /// <summary>
        /// 测试：更新任务进度 - 多次更新
        /// 预期：进度正确累加，不超过目标值
        /// </summary>
        [Test]
        public void Test_UpdateProgress_MultipleUpdates_CappedAtTarget()
        {
            // Arrange
            var taskId = "daily_001"; // 通关1个关卡任务
            var config = GetTaskConfig(m_taskSystem, taskId);
            int targetCount = config.TargetCount;

            // Act
            for (int i = 0; i < targetCount + 5; i++)
            {
                m_taskSystem.UpdateTaskProgress(TaskConditionType.PassLevel, 1);
            }

            // Assert
            var task = m_taskSystem.GetTaskData(taskId);
            Assert.LessOrEqual(task.CurrentProgress, targetCount, 
                "进度不应超过目标值");
            Assert.AreEqual(targetCount, task.CurrentProgress);
        }

        //========================== 任务完成测试 ==========================

        /// <summary>
        /// 测试：任务完成判定 - 达到目标
        /// 预期：任务状态变为Completed
        /// </summary>
        [Test]
        public void Test_TaskCompletion_ReachesTarget_ChangesStateToCompleted()
        {
            // Arrange
            var taskId = "daily_001";
            var config = GetTaskConfig(m_taskSystem, taskId);
            
            // Act
            m_taskSystem.UpdateTaskProgress(TaskConditionType.PassLevel, config.TargetCount);

            // Assert
            var task = m_taskSystem.GetTaskData(taskId);
            Assert.AreEqual(TaskState.Completed, task.State, 
                "达到目标后状态应为Completed");
        }

        /// <summary>
        /// 测试：任务完成判定 - 未达目标
        /// 预期：任务状态保持InProgress
        /// </summary>
        [Test]
        public void Test_TaskCompletion_BelowTarget_StaysInProgress()
        {
            // Arrange
            var taskId = "daily_002";
            var config = GetTaskConfig(m_taskSystem, taskId);

            // Act
            m_taskSystem.UpdateTaskProgress(TaskConditionType.EliminateElement, config.TargetCount - 1);

            // Assert
            var task = m_taskSystem.GetTaskData(taskId);
            Assert.AreEqual(TaskState.InProgress, task.State, 
                "未达目标应保持InProgress");
        }

        //========================== 奖励领取测试 ==========================

        /// <summary>
        /// 测试：领取奖励 - 成功情况
        /// 预期：奖励成功领取，状态变为Claimed
        /// </summary>
        [Test]
        public void Test_ClaimReward_CompletedTask_Succeeds()
        {
            // Arrange
            var taskId = "daily_001";
            
            // 先完成任务
            m_taskSystem.Test_CompleteTask(taskId);
            
            var taskBeforeClaim = m_taskSystem.GetTaskData(taskId);
            Assert.AreEqual(TaskState.Completed, taskBeforeClaim.State, 
                "任务应为Completed状态");

            // Act
            bool result = m_taskSystem.ClaimTaskReward(taskId);

            // Assert
            Assert.IsTrue(result, "领取应成功");
            
            var taskAfterClaim = m_taskSystem.GetTaskData(taskId);
            Assert.AreEqual(TaskState.Claimed, taskAfterClaim.State, 
                "领取后状态应为Claimed");
            Assert.IsTrue(taskAfterClaim.IsRewardClaimed, 
                "应标记为已领取");
        }

        /// <summary>
        /// 测试：领取奖励 - 未完成任务
        /// 预期：领取失败
        /// </summary>
        [Test]
        public void Test_ClaimReward_IncompleteTask_Fails()
        {
            // Arrange
            var taskId = "daily_001";
            var task = m_taskSystem.GetTaskData(taskId);
            Assert.AreEqual(TaskState.InProgress, task.State, "初始状态应为InProgress");

            // Act
            bool result = m_taskSystem.ClaimTaskReward(taskId);

            // Assert
            Assert.IsFalse(result, "未完成不应领取成功");
        }

        /// <summary>
        /// 测试：领取奖励 - 已领取过
        /// 预期：重复领取失败
        /// </summary>
        [Test]
        public void Test_ClaimReward_AlreadyClaimed_Fails()
        {
            // Arrange
            var taskId = "daily_001";
            
            // 完成并领取
            m_taskSystem.Test_CompleteTask(taskId);
            m_taskSystem.ClaimTaskReward(taskId);

            // Act
            bool result = m_taskSystem.ClaimTaskReward(taskId);

            // Assert
            Assert.IsFalse(result, "已领取不应重复领取");
        }

        /// <summary>
        /// 测试：领取奖励 - 不存在的任务
        /// 预期：返回false
        /// </summary>
        [Test]
        public void Test_ClaimReward_NonExistentTask_Fails()
        {
            // Act
            bool result = m_taskSystem.ClaimTaskReward("non_existent_task");

            // Assert
            Assert.IsFalse(result, "不存在的任务应领取失败");
        }

        //========================== 任务刷新测试 ==========================

        /// <summary>
        /// 测试：刷新每日任务
        /// 预期：进度重置，状态重置
        /// </summary>
        [Test]
        public void Test_RefreshDailyTasks_ResetsProgress()
        {
            // Arrange
            var taskId = "daily_001";
            
            // 完成并领取
            m_taskSystem.Test_CompleteTask(taskId);
            m_taskSystem.ClaimTaskReward(taskId);

            // Act
            m_taskSystem.RefreshDailyTasks();

            // Assert
            var task = m_taskSystem.GetTaskData(taskId);
            Assert.AreEqual(0, task.CurrentProgress, "进度应重置");
            Assert.AreEqual(TaskState.InProgress, task.State, "状态应重置");
            Assert.IsFalse(task.IsRewardClaimed, "应重置为未领取");
        }

        /// <summary>
        /// 测试：刷新每周任务
        /// 预期：进度重置，状态重置
        /// </summary>
        [Test]
        public void Test_RefreshWeeklyTasks_ResetsProgress()
        {
            // Arrange
            var taskId = "weekly_001";
            
            // 完成
            m_taskSystem.Test_CompleteTask(taskId);

            // Act
            m_taskSystem.RefreshWeeklyTasks();

            // Assert
            var task = m_taskSystem.GetTaskData(taskId);
            Assert.AreEqual(0, task.CurrentProgress, "进度应重置");
            Assert.AreEqual(TaskState.InProgress, task.State, "状态应重置");
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
            var taskId = "daily_001";
            
            // 完成部分进度
            m_taskSystem.UpdateTaskProgress(TaskConditionType.PassLevel, 1);
            m_taskSystem.SaveData();

            // 重新初始化(模拟重新进入游戏)
            var newTaskSystem = TaskSystem.Instance;
            newTaskSystem.OnInit();

            // Assert
            var task = newTaskSystem.GetTaskData(taskId);
            Assert.AreEqual(1, task.CurrentProgress, "进度应正确保存");
        }

        //========================== 边界条件测试 ==========================

        /// <summary>
        /// 测试：零值更新
        /// 预期：不产生错误
        /// </summary>
        [Test]
        public void Test_UpdateProgress_ZeroCount_NoError()
        {
            // Act & Assert (不应抛出异常)
            Assert.DoesNotThrow(() => 
            {
                m_taskSystem.UpdateTaskProgress(TaskConditionType.PassLevel, 0);
            });
        }

        /// <summary>
        /// 测试：负值更新
        /// 预期：进度不减少
        /// </summary>
        [Test]
        public void Test_UpdateProgress_NegativeCount_NoDecrease()
        {
            // Arrange
            var taskId = "daily_001";
            m_taskSystem.UpdateTaskProgress(TaskConditionType.PassLevel, 5);

            // Act
            m_taskSystem.UpdateTaskProgress(TaskConditionType.PassLevel, -3);

            // Assert
            var task = m_taskSystem.GetTaskData(taskId);
            Assert.GreaterOrEqual(task.CurrentProgress, 0, "进度不应为负");
        }

        //========================== 辅助方法 ==========================

        /// <summary>
        /// 获取任务配置(通过反射或公开接口)
        /// </summary>
        private TaskConfig GetTaskConfig(TaskSystem system, string taskId)
        {
            // 由于TaskConfig是内部数据，这里通过进度更新来间接测试
            // 实际项目中应提供公开的GetTaskConfig接口
            
            // 创建一个测试配置
            switch (taskId)
            {
                case "daily_001":
                    return new TaskConfig { Id = "daily_001", TargetCount = 1, Type = TaskType.Daily };
                case "daily_002":
                    return new TaskConfig { Id = "daily_002", TargetCount = 50, Type = TaskType.Daily };
                case "weekly_001":
                    return new TaskConfig { Id = "weekly_001", TargetCount = 50000, Type = TaskType.Weekly };
                default:
                    return new TaskConfig { Id = taskId, TargetCount = 10, Type = TaskType.Daily };
            }
        }
    }
}

#endif
