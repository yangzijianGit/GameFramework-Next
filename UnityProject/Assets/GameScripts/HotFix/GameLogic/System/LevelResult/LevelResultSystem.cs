using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 战斗结算数据类型
    //===========================================================

    /// <summary>
    /// 战斗结果类型
    /// </summary>
    public enum BattleResultType
    {
        /// <summary>胜利</summary>
        Win = 0,
        
        /// <summary>失败</summary>
        Lose = 1,
        
        /// <summary>放弃</summary>
        GiveUp = 2,
    }

    /// <summary>
    /// 战斗结果数据
    /// </summary>
    [Serializable]
    public class BattleResultData
    {
        /// <summary>关卡ID</summary>
        public string LevelId;
        
        /// <summary>结果类型</summary>
        public BattleResultType ResultType;
        
        /// <summary>获得星级</summary>
        public int Star;
        
        /// <summary>获得分数</summary>
        public long Score;
        
        /// <summary>剩余步数</summary>
        public int RemainingMoves;
        
        /// <summary>消除数量</summary>
        public int EliminateCount;
        
        /// <summary>连锁次数</summary>
        public int ComboCount;
        
        /// <summary>最大连锁</summary>
        public int MaxCombo;
        
        /// <summary>使用技能次数</summary>
        public int SkillUseCount;
        
        /// <summary>狂热次数</summary>
        public int FeverCount;
        
        /// <summary>战斗时长(秒)</summary>
        public int BattleDuration;
        
        /// <summary>获得货币</summary>
        public Dictionary<string, int> EarnedCurrency = new Dictionary<string, int>();
        
        /// <summary>获得道具</summary>
        public Dictionary<string, int> EarnedItems = new Dictionary<string, int>();
        
        /// <summary>首次通关奖励</summary>
        public bool IsFirstPass;
        
        /// <summary>是否刷新最高分</summary>
        public bool IsNewHighScore;
    }

    /// <summary>
    /// 奖励详情
    /// </summary>
    [Serializable]
    public class RewardDetail
    {
        /// <summary>奖励类型</summary>
        public RewardType Type;
        
        /// <summary>奖励ID</summary>
        public string RewardId;
        
        /// <summary>数量</summary>
        public int Count;
        
        /// <summary>图标</summary>
        public string Icon;
    }

    //===========================================================
    // 战斗结算系统接口
    //===========================================================

    /// <summary>
    /// 战斗结算系统接口
    /// </summary>
    public interface ILevelResultSystem
    {
        /// <summary>显示结算界面</summary>

        /// <summary>初始化系统</summary>
        void Initialize();

        /// <summary>显示结算界面</summary>
        /// <param name="result">战斗结果</param>
        void ShowResult(BattleResultData result);

        /// <summary>隐藏结算界面</summary>
        void HideResult();

        /// <summary>重新挑战</summary>
        void Retry();

        /// <summary>下一关</summary>
        void NextLevel();

        /// <summary>分享结果</summary>
        /// <returns>分享数据</returns>
        string Share();

        /// <summary>获取当前结算数据</summary>
        /// <returns>结算数据</returns>
        BattleResultData GetCurrentResult();
    }

    //===========================================================
    // 战斗结算系统实现
    //===========================================================

    /// <summary>
    /// 战斗结算系统
    /// 职责：
    /// 1. 战斗结果计算
    /// 2. 奖励发放
    /// 3. 星级判定
    /// 4. 结算界面显示控制
    /// 5. 结算动画播放
    /// 
    /// 扩展点：
    /// 1. 可自定义奖励计算公式
    /// 2. 支持视频广告增加奖励
    /// 3. 支持分享奖励
    /// </summary>
    public class LevelResultSystem : BaseLogicSys<LevelResultSystem>, ILevelResultSystem
    {
        //========================== 常量 ==========================

        /// <summary>首次通关额外奖励倍率</summary>
        private const float FIRST_PASS_MULTIPLIER = 2.0f;

        /// <summary>分享奖励</summary>
        private const int SHARE_REWARD_DIAMOND = 5;

        //========================== 私有变量 ==========================

        /// <summary>当前战斗结果</summary>
        private BattleResultData m_currentResult;

        /// <summary>是否已初始化</summary>
        private bool m_isInitialized = false;

        //========================== 回调 ==========================

        /// <summary>显示结算回调</summary>
        public Action<BattleResultData> OnShowResult;

        /// <summary>隐藏结算回调</summary>
        public Action OnHideResult;

        /// <summary>重新挑战回调</summary>
        public Action OnRetry;

        /// <summary>下一关回调</summary>
        public Action<string> OnNextLevel;

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

            m_isInitialized = true;
            Debug.Log("[LevelResultSystem] Initialized");
            return true;
        }

        public override void OnDestroy()
        {
            m_currentResult = null;
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 显示结算界面
        /// </summary>
        public void ShowResult(BattleResultData result)
        {
            if (result == null)
            {
                Debug.LogError("[LevelResultSystem] Result is null");
                return;
            }

            m_currentResult = result;

            // 计算最终奖励
            CalculateRewards(result);

            // 发放奖励
            GrantRewards(result);

            // 更新关卡数据
            UpdateLevelData(result);

            // 触发回调
            OnShowResult?.Invoke(result);

            Debug.Log($"[LevelResultSystem] Show result: {result.ResultType}, star: {result.Star}, score: {result.Score}");
        }

        /// <summary>
        /// 隐藏结算界面
        /// </summary>
        public void HideResult()
        {
            m_currentResult = null;
            OnHideResult?.Invoke();
            Debug.Log("[LevelResultSystem] Hide result");
        }

        /// <summary>
        /// 重新挑战
        /// </summary>
        public void Retry()
        {
            if (m_currentResult == null)
            {
                Debug.LogWarning("[LevelResultSystem] No current result");
                return;
            }

            var levelId = m_currentResult.LevelId;
            
            // 隐藏结算界面
            HideResult();

            // 触发重新挑战
            OnRetry?.Invoke();

            Debug.Log($"[LevelResultSystem] Retry level: {levelId}");
        }

        /// <summary>
        /// 下一关
        /// </summary>
        public void NextLevel()
        {
            if (m_currentResult == null)
            {
                Debug.LogWarning("[LevelResultSystem] No current result");
                return;
            }

            if (m_currentResult.ResultType != BattleResultType.Win)
            {
                Debug.LogWarning("[LevelResultSystem] Cannot go to next level: not win");
                return;
            }

            // 获取下一关ID
            string currentLevelId = m_currentResult.LevelId;
            string nextLevelId = GetNextLevelId(currentLevelId);

            // 隐藏结算界面
            HideResult();

            // 触发进入下一关
            if (!string.IsNullOrEmpty(nextLevelId))
            {
                OnNextLevel?.Invoke(nextLevelId);
            }

            Debug.Log($"[LevelResultSystem] Go to next level: {nextLevelId}");
        }

        /// <summary>
        /// 分享
        /// </summary>
        public string Share()
        {
            if (m_currentResult == null)
            {
                Debug.LogWarning("[LevelResultSystem] No current result");
                return "";
            }

            // 生成分享文本
            string shareText = GenerateShareText(m_currentResult);

            // 发放分享奖励
            // PlayerData.Instance.AddCurrency("diamond", SHARE_REWARD_DIAMOND);
            Debug.Log($"[LevelResultSystem] Share reward: {SHARE_REWARD_DIAMOND} diamond");

            return shareText;
        }

        /// <summary>
        /// 获取当前结果
        /// </summary>
        public BattleResultData GetCurrentResult()
        {
            return m_currentResult;
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 计算奖励
        /// </summary>
        private void CalculateRewards(BattleResultData result)
        {
            result.EarnedCurrency = new Dictionary<string, int>();
            result.EarnedItems = new Dictionary<string, int>();

            if (result.ResultType != BattleResultType.Win)
            {
                // 失败没有奖励
                return;
            }

            // 获取关卡配置
            var levelData = LevelSelectSystem.Instance.GetLevelData(result.LevelId);
            if (levelData == null) return;

            // 基础奖励
            int baseDiamond = 10;
            int baseStar = 5;

            // 首次通关奖励
            result.IsFirstPass = IsFirstPass(result.LevelId);
            if (result.IsFirstPass)
            {
                if (levelData.FirstPassReward != null)
                {
                    foreach (var reward in levelData.FirstPassReward)
                    {
                        AddReward(result.EarnedCurrency, reward.Key, reward.Value);
                    }
                }
            }

            // 星级奖励
            if (levelData.StarRewards.TryGetValue(result.Star, out var starRewards))
            {
                foreach (var reward in starRewards)
                {
                    AddReward(result.EarnedCurrency, reward.Key, reward.Value);
                }
            }

            // 步数奖励
            if (result.RemainingMoves > 0)
            {
                int moveBonus = result.RemainingMoves * 2;
                AddReward(result.EarnedCurrency, "diamond", moveBonus);
            }

            // 连锁奖励
            if (result.MaxCombo > 3)
            {
                int comboBonus = (result.MaxCombo - 3) * 5;
                AddReward(result.EarnedCurrency, "diamond", comboBonus);
            }

            // 狂热奖励
            if (result.FeverCount > 0)
            {
                int feverBonus = result.FeverCount * 10;
                AddReward(result.EarnedCurrency, "diamond", feverBonus);
            }

            Debug.Log($"[LevelResultSystem] Rewards calculated: diamond={GetRewardCount(result.EarnedCurrency, "diamond")}, star={GetRewardCount(result.EarnedCurrency, "star")}");
        }

        /// <summary>
        /// 发放奖励
        /// </summary>
        private void GrantRewards(BattleResultData result)
        {
            if (result.EarnedCurrency == null) return;

            foreach (var currency in result.EarnedCurrency)
            {
                // PlayerData.Instance.AddCurrency(currency.Key, currency.Value);
                Debug.Log($"[LevelResultSystem] Grant currency: {currency.Key} x {currency.Value}");
            }

            foreach (var item in result.EarnedItems)
            {
                // PlayerData.Instance.AddItem(item.Key, item.Value);
                Debug.Log($"[LevelResultSystem] Grant item: {item.Key} x {item.Value}");
            }
        }

        /// <summary>
        /// 更新关卡数据
        /// </summary>
        private void UpdateLevelData(BattleResultData result)
        {
            // 检查是否刷新最高分
            var playerData = LevelSelectSystem.Instance.GetPlayerLevelData(result.LevelId);
            result.IsNewHighScore = playerData == null || result.Score > playerData.BestScore;

            // 记录通关
            if (result.ResultType == BattleResultType.Win)
            {
                LevelSelectSystem.Instance.OnLevelPassed(result.LevelId, result.Star, result.Score);
            }
        }

        /// <summary>
        /// 是否首次通关
        /// </summary>
        private bool IsFirstPass(string levelId)
        {
            var playerData = LevelSelectSystem.Instance.GetPlayerLevelData(levelId);
            return playerData == null || playerData.PassCount == 0;
        }

        /// <summary>
        /// 获取下一关ID
        /// </summary>
        private string GetNextLevelId(string currentLevelId)
        {
            // 从关卡配置中获取下一关
            var levels = LevelSelectSystem.Instance.GetLevelList();
            
            for (int i = 0; i < levels.Count - 1; i++)
            {
                if (levels[i].LevelId == currentLevelId)
                {
                    return levels[i + 1].LevelId;
                }
            }

            return null;
        }

        /// <summary>
        /// 添加奖励
        /// </summary>
        private void AddReward(Dictionary<string, int> rewards, string id, int count)
        {
            if (count <= 0) return;

            if (rewards.ContainsKey(id))
            {
                rewards[id] += count;
            }
            else
            {
                rewards[id] = count;
            }
        }

        /// <summary>
        /// 获取奖励数量
        /// </summary>
        private int GetRewardCount(Dictionary<string, int> rewards, string id)
        {
            if (rewards == null || !rewards.ContainsKey(id))
                return 0;
            return rewards[id];
        }

        /// <summary>
        /// 生成分享文本
        /// </summary>
        private string GenerateShareText(BattleResultData result)
        {
            string starStr = "";
            for (int i = 0; i < result.Star; i++)
            {
                starStr += "⭐";
            }

            string text = $"我在{result.LevelId}获得了{starStr}评价！\n" +
                         $"得分: {result.Score}\n" +
                         $"连锁: {result.MaxCombo}连击\n" +
                         $"#三消游戏";

            return text;
        }

        //========================== 静态方法 ==========================

        /// <summary>
        /// 计算战斗结果
        /// </summary>
        public static BattleResultData CalculateBattleResult(
            string levelId,
            int usedMoves,
            int maxMoves,
            long score,
            int eliminateCount,
            int maxCombo,
            int skillUseCount,
            int feverCount,
            int battleDuration)
        {
            var result = new BattleResultData
            {
                LevelId = levelId,
                RemainingMoves = maxMoves - usedMoves,
                EliminateCount = eliminateCount,
                MaxCombo = maxCombo,
                SkillUseCount = skillUseCount,
                FeverCount = feverCount,
                BattleDuration = battleDuration
            };

            // 获取关卡配置
            var levelData = LevelSelectSystem.Instance.GetLevelData(levelId);
            if (levelData == null)
            {
                result.ResultType = BattleResultType.Lose;
                result.Star = 0;
                result.Score = 0;
                return result;
            }

            // 判定胜负
            // 这里简化处理，实际需要根据目标类型判定
            bool isWin = score >= levelData.TargetValue;
            result.ResultType = isWin ? BattleResultType.Win : BattleResultType.Lose;
            result.Score = score;

            // 计算星级
            if (isWin)
            {
                if (score >= levelData.Star3Value)
                    result.Star = 3;
                else if (score >= levelData.Star2Value)
                    result.Star = 2;
                else
                    result.Star = 1;
            }
            else
            {
                result.Star = 0;
            }

            // 计算Combo
            result.ComboCount = maxCombo;

            return result;
        }
    }
}
