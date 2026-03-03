//========================================================================================
//  BattleSystem - 战斗系统
//========================================================================================
//  功能：三消战斗核心逻辑，管理棋盘、元素、消除、回合、胜负
//  职责：关卡流程控制、元素交互、消除检测、技能释放、胜负判定
//  扩展点：接入真实关卡配置、动画系统、音效系统、战斗事件统计
//========================================================================================

using System;
using System.Collections.Generic;
using GameBase;
using GameFramework;
using UnityEngine;

namespace ENate
{
    //===========================================================
    // 战斗数据类型
    //===========================================================

    /// <summary>
    /// 战斗状态
    /// </summary>
    public enum BattleState
    {
        /// <summary>未开始</summary>
        None = 0,
        
        /// <summary>初始化</summary>
        Init = 1,
        
        /// <summary>等待玩家操作</summary>
        WaitInput = 2,
        
        /// <summary>元素交换中</summary>
        Swapping = 3,
        
        /// <summary>消除检测中</summary>
        CheckingMatch = 4,
        
        /// <summary>消除动画中</summary>
        Eliminating = 5,
        
        /// <summary>元素下落中</summary>
        Dropping = 6,
        
        /// <summary>生成新元素</summary>
        Spawning = 7,
        
        /// <summary>技能释放中</summary>
        UsingSkill = 8,
        
        /// <summary>结算中</summary>
        Settling = 9,
        
        /// <summary>已结束</summary>
        Ended = 10,
    }

    /// <summary>
    /// 消除结果
    /// </summary>
    public enum EliminateResult
    {
        /// <summary>无消除</summary>
        None = 0,
        
        /// <summary>普通消除</summary>
        Normal = 1,
        
        /// <summary>技能消除</summary>
        Skill = 2,
        
        /// <summary>连消</summary>
        Combo = 3,
        
        /// <summary>大范围消除</summary>
        Massive = 4,
    }

    /// <summary>
    /// 胜负结果
    /// </summary>
    public enum BattleResult
    {
        /// <summary>进行中</summary>
        Processing = 0,
        
        /// <summary>胜利</summary>
        Victory = 1,
        
        /// <summary>失败</summary>
        Defeat = 2,
        
        /// <summary>平局</summary>
        Draw = 3,
    }

    /// <summary>
    /// 技能类型
    /// </summary>
    public enum SkillType
    {
        /// <summary>锤子 - 消除单个</summary>
        Hammer = 1,
        
        /// <summary>炸弹 - 消除3x3</summary>
        Bomb = 2,
        
        /// <summary>火箭 - 消除整行</summary>
        Rocket = 3,
        
        /// <summary>彩虹球 - 消除同色</summary>
        Rainbow = 4,
        
        /// <summary>时间延长</summary>
        TimeExt = 5,
    }

    //===========================================================
    // 配置数据结构
    //===========================================================

    /// <summary>
    /// 关卡配置
    /// </summary>
    [Serializable]
    public class LevelConfig
    {
        /// <summary>关卡ID</summary>
        public int LevelId;
        
        /// <summary>关卡名称</summary>
        public string Name;
        
        /// <summary>目标类型</summary>
        public int TargetType;
        
        /// <summary>目标数量</summary>
        public int TargetCount;
        
        /// <summary>步数限制</summary>
        public int MoveLimit;
        
        /// <summary>时间限制(秒，0表示不限)</summary>
        public int TimeLimit;
        
        /// <summary>棋盘宽度</summary>
        public int BoardWidth;
        
        /// <summary>棋盘高度</summary>
        public int BoardHeight;
        
        /// <summary>元素类型列表</summary>
        public List<string> ElementTypes;
        
        /// <summary>初始棋子配置</summary>
        public string InitialBoard;
        
        /// <summary>是否启用技能</summary>
        public bool EnableSkill;
        
        /// <summary>难度等级</summary>
        public int Difficulty;
        
        /// <summary>星级分数要求</summary>
        public int[] StarScores;
    }

    /// <summary>
    /// 战斗结果数据
    /// </summary>
    [Serializable]
    public class BattleResultData
    {
        /// <summary>关卡ID</summary>
        public int LevelId;
        
        /// <summary>胜负结果</summary>
        public BattleResult Result;
        
        /// <summary>获得分数</summary>
        public int Score;
        
        /// <summary>剩余步数</summary>
        public int RemainingMoves;
        
        /// <summary>剩余时间</summary>
        public int RemainingTime;
        
        /// <summary>消除次数</summary>
        public int EliminateCount;
        
        /// <summary>连击次数</summary>
        public int ComboCount;
        
        /// <summary>使用技能次数</summary>
        public int SkillUseCount;
        
        /// <summary>获得星星数</summary>
        public int StarCount;
        
        /// <summary>获得奖励</summary>
        public Dictionary<string, int> Rewards;
    }

    /// <summary>
    /// 技能使用数据
    /// </summary>
    [Serializable]
    public class SkillUseData
    {
        /// <summary>技能类型</summary>
        public SkillType SkillType;
        
        /// <summary>目标位置X</summary>
        public int TargetX;
        
        /// <summary>目标位置Y</summary>
        public int TargetY;
        
        /// <summary>使用时间</summary>
        public long UseTime;
    }

    //===========================================================
    // 战斗系统
    //===========================================================

    /// <summary>
    /// 战斗系统
    /// 管理三消战斗的核心逻辑
    /// 
    /// 职责：
    /// - 关卡初始化和流程控制
    /// - 玩家输入处理和元素交换
    /// - 消除检测和连锁反应
    /// - 元素下落和生成
    /// - 技能系统管理
    /// - 胜负判定和结算
    /// - 与MatchCore集成
    /// 
    /// 扩展点：
    /// - 接入真实关卡配置表
    /// - 动画系统集成
    /// - 音效系统集成
    /// - 战斗事件数据上报
    /// - 好友对战模式
    /// 
    /// 使用方式：
    /// <code>
    /// // 开始战斗
    /// BattleSystem.Instance.StartBattle(levelId);
    /// 
    /// // 玩家交换元素
    /// BattleSystem.Instance.SwapElements(x1, y1, x2, y2);
    /// 
    /// // 使用技能
    /// BattleSystem.Instance.UseSkill(SkillType.Hammer, targetX, targetY);
    /// </code>
    /// </summary>
    public class BattleSystem : BaseLogicSys<BattleSystem>
    {
        // 常量
        private const int DEFAULT_BOARD_WIDTH = 8;
        private const int DEFAULT_BOARD_HEIGHT = 8;
        private const int MIN_MATCH_COUNT = 3;
        
        // 运行时数据
        private LevelConfig _currentLevel;
        private BattleState _battleState;
        private BattleResult _battleResult;
        
        private int _currentMoves;
        private int _currentScore;
        private int _currentCombo;
        private int _maxCombo;
        private long _battleStartTime;
        
        private int[,] _boardData;
        private List<GridCoord> _eliminateList;
        private List<SkillUseData> _skillHistory;
        
        // 统计
        private int _totalEliminateCount;
        private int _normalEliminateCount;
        private int _skillEliminateCount;
        
        // 事件回调
        /// <summary>战斗开始回调(关卡ID)</summary>
        public event Action<int> OnBattleStart;
        
        /// <summary>战斗状态变化回调(旧状态, 新状态)</summary>
        public event Action<BattleState, BattleState> OnStateChanged;
        
        /// <summary>元素交换回调(x1, y1, x2, y2)</summary>
        public event Action<int, int, int, int> OnElementSwapped;
        
        /// <summary>消除开始回调(消除列表)</summary>
        public event Action<List<GridCoord>> OnEliminateStart;
        
        /// <summary>消除结束回调(消除结果)</summary>
        public event Action<EliminateResult> OnEliminateEnd;
        
        /// <summary>元素下落回调</summary>
        public event Action OnElementDrop;
        
        /// <summary>分数变化回调(当前分数, 增加分数)</summary>
        public event Action<int, int> OnScoreChanged;
        
        /// <summary>步数变化回调(剩余步数)</summary>
        public event Action<int> OnMovesChanged;
        
        /// <summary>技能使用回调(技能类型)</summary>
        public event Action<SkillType> OnSkillUsed;
        
        /// <summary>连击回调(连击数)</summary>
        public event Action<int> OnComboTriggered;
        
        /// <summary>战斗结束回调(结果数据)</summary>
        public event Action<BattleResultData> OnBattleEnd;

        //===========================================================
        // 生命周期
        //===========================================================

        /// <summary>
        /// 初始化
        /// </summary>
        public override bool OnInit()
        {
            base.OnInit();
            _battleState = BattleState.None;
            _battleResult = BattleResult.Processing;
            _eliminateList = new List<GridCoord>();
            _skillHistory = new List<SkillUseData>();
            return true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void OnDestroy()
        {
            ClearBattleData();
            base.OnDestroy();
        }

        //===========================================================
        // 公共接口
        //===========================================================

        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        public void StartBattle(int levelId)
        {
            Debug.Log(string.Format("[BattleSystem] Start battle: level {0}", levelId));
            
            // 加载关卡配置
            _currentLevel = LoadLevelConfig(levelId);
            if (_currentLevel == null)
            {
                Debug.LogError(string.Format("[BattleSystem] Level config not found: {0}", levelId));
                return;
            }
            
            // 初始化战斗数据
            InitBattleData();
            
            // 设置状态
            SetBattleState(BattleState.Init);
            
            // 生成棋盘
            GenerateBoard();
            
            // 开始战斗
            _battleStartTime = GetCurrentTime();
            SetBattleState(BattleState.WaitInput);
            
            // 事件
            OnBattleStart?.Invoke(levelId);
            
            Debug.Log(string.Format("[BattleSystem] Battle started: moves={0}, target={1}", _currentMoves, _currentLevel.TargetCount));
        }

        /// <summary>
        /// 交换元素
        /// </summary>
        /// <param name="x1">第一个元素X坐标</param>
        /// <param name="y1">第一个元素Y坐标</param>
        /// <param name="x2">第二个元素X坐标</param>
        /// <param name="y2">第二个元素Y坐标</param>
        /// <returns>是否交换成功</returns>
        public bool SwapElements(int x1, int y1, int x2, int y2)
        {
            // 状态检查
            if (_battleState != BattleState.WaitInput)
            {
                Debug.LogWarning(string.Format("[BattleSystem] Cannot swap in current state: {0}", _battleState));
                return false;
            }
            
            // 范围检查
            if (!IsValidCoord(x1, y1) || !IsValidCoord(x2, y2))
            {
                Debug.LogWarning("[BattleSystem] Invalid coordinates");
                return false;
            }
            
            // 不能交换自己
            if (x1 == x2 && y1 == y2)
            {
                return false;
            }
            
            // 相邻检查(只允许相邻交换)
            if (!IsAdjacent(x1, y1, x2, y2))
            {
                Debug.LogWarning("[BattleSystem] Elements not adjacent");
                return false;
            }
            
            // 执行交换
            SetBattleState(BattleState.Swapping);
            SwapInBoard(x1, y1, x2, y2);
            
            // 事件
            OnElementSwapped?.Invoke(x1, y1, x2, y2);
            
            // 检查是否有消除
            bool hasMatch = CheckAndEliminate();
            
            if (!hasMatch)
            {
                // 交换后无消除，交换回来
                SwapInBoard(x1, y1, x2, y2);
                SetBattleState(BattleState.WaitInput);
                Debug.Log("[BattleSystem] Swap cancelled - no match");
                return false;
            }
            
            // 消耗步数
            ConsumeMove();
            
            // 开始消除流程
            return true;
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="skillType">技能类型</param>
        /// <param name="targetX">目标X坐标</param>
        /// <param name="targetY">目标Y坐标</param>
        /// <returns>是否使用成功</returns>
        public bool UseSkill(SkillType skillType, int targetX, int targetY)
        {
            // 状态检查
            if (_battleState != BattleState.WaitInput)
            {
                Debug.LogWarning(string.Format("[BattleSystem] Cannot use skill in current state: {0}", _battleState));
                return false;
            }
            
            // 检查关卡是否启用技能
            if (_currentLevel != null && !_currentLevel.EnableSkill)
            {
                Debug.LogWarning("[BattleSystem] Skill not enabled in this level");
                return false;
            }
            
            // 范围检查
            if (!IsValidCoord(targetX, targetY))
            {
                Debug.LogWarning("[BattleSystem] Invalid skill target");
                return false;
            }
            
            // 记录技能使用
            var skillData = new SkillUseData
            {
                SkillType = skillType,
                TargetX = targetX,
                TargetY = targetY,
                UseTime = GetCurrentTime()
            };
            _skillHistory.Add(skillData);
            
            // 执行技能效果
            SetBattleState(BattleState.UsingSkill);
            ExecuteSkillEffect(skillType, targetX, targetY);
            
            // 事件
            OnSkillUsed?.Invoke(skillType);
            
            return true;
        }

        /// <summary>
        /// 跳过当前动画(调试用)
        /// </summary>
        public void SkipAnimation()
        {
            // 直接进入下一状态
            if (_battleState == BattleState.Eliminating)
            {
                SetBattleState(BattleState.Dropping);
            }
            else if (_battleState == BattleState.Dropping)
            {
                SetBattleState(BattleState.Spawning);
            }
            else if (_battleState == BattleState.Spawning)
            {
                // 检查是否有新的消除
                if (CheckAndEliminate())
                {
                    SetBattleState(BattleState.CheckingMatch);
                }
                else
                {
                    CheckBattleEnd();
                    SetBattleState(BattleState.WaitInput);
                }
            }
        }

        /// <summary>
        /// 获取当前战斗状态
        /// </summary>
        public BattleState GetBattleState()
        {
            return _battleState;
        }

        /// <summary>
        /// 获取剩余步数
        /// </summary>
        public int GetRemainingMoves()
        {
            return _currentMoves;
        }

        /// <summary>
        /// 获取当前分数
        /// </summary>
        public int GetCurrentScore()
        {
            return _currentScore;
        }

        /// <summary>
        /// 获取当前连击数
        /// </summary>
        public int GetCurrentCombo()
        {
            return _currentCombo;
        }

        /// <summary>
        /// 获取棋盘数据
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>元素类型，-1表示空</returns>
        public int GetElementAt(int x, int y)
        {
            if (!IsValidCoord(x, y))
                return -1;
            return _boardData[x, y];
        }

        /// <summary>
        /// 获取棋盘宽度
        /// </summary>
        public int GetBoardWidth()
        {
            return _currentLevel != null ? _currentLevel.BoardWidth : DEFAULT_BOARD_WIDTH;
        }

        /// <summary>
        /// 获取棋盘高度
        /// </summary>
        public int GetBoardHeight()
        {
            return _currentLevel != null ? _currentLevel.BoardHeight : DEFAULT_BOARD_HEIGHT;
        }

        /// <summary>
        /// 获取战斗结果
        /// </summary>
        public BattleResult GetBattleResult()
        {
            return _battleResult;
        }

        /// <summary>
        /// 获取战斗结果数据
        /// </summary>
        public BattleResultData GetBattleResultData()
        {
            return new BattleResultData
            {
                LevelId = _currentLevel != null ? _currentLevel.LevelId : 0,
                Result = _battleResult,
                Score = _currentScore,
                RemainingMoves = _currentMoves,
                RemainingTime = GetRemainingTime(),
                EliminateCount = _totalEliminateCount,
                ComboCount = _maxCombo,
                SkillUseCount = _skillHistory.Count,
                StarCount = CalculateStarCount()
            };
        }

        /// <summary>
        /// 获取剩余时间
        /// </summary>
        public int GetRemainingTime()
        {
            if (_currentLevel == null || _currentLevel.TimeLimit == 0)
                return 0;
            
            long elapsed = GetCurrentTime() - _battleStartTime;
            return Math.Max(0, _currentLevel.TimeLimit - (int)elapsed);
        }

        /// <summary>
        /// 结束战斗
        /// </summary>
        public void EndBattle()
        {
            SetBattleState(BattleState.Ended);
            
            var resultData = GetBattleResultData();
            OnBattleEnd?.Invoke(resultData);
            
            Debug.Log(string.Format("[BattleSystem] Battle ended: result={0}, score={1}, stars={2}", 
                _battleResult, _currentScore, resultData.StarCount));
        }

        //===========================================================
        // 私有方法 - 战斗流程
        //===========================================================

        /// <summary>
        /// 初始化战斗数据
        /// </summary>
        private void InitBattleData()
        {
            _battleState = BattleState.None;
            _battleResult = BattleResult.Processing;
            _currentMoves = _currentLevel != null ? _currentLevel.MoveLimit : 30;
            _currentScore = 0;
            _currentCombo = 0;
            _maxCombo = 0;
            _totalEliminateCount = 0;
            _normalEliminateCount = 0;
            _skillEliminateCount = 0;
            _skillHistory.Clear();
            
            int width = GetBoardWidth();
            int height = GetBoardHeight();
            _boardData = new int[width, height];
        }

        /// <summary>
        /// 生成棋盘
        /// </summary>
        private void GenerateBoard()
        {
            int width = GetBoardWidth();
            int height = GetBoardHeight();
            
            // 获取可用元素类型
            List<int> elementTypes = GetAvailableElementTypes();
            
            // 随机生成，确保开局无消除
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int elementType;
                    do
                    {
                        elementType = elementTypes[UnityEngine.Random.Range(0, elementTypes.Count)];
                    } while (WouldCauseMatch(x, y, elementType));
                    
                    _boardData[x, y] = elementType;
                }
            }
            
            Debug.Log(string.Format("[BattleSystem] Board generated: {0}x{1}", width, height));
        }

        /// <summary>
        /// 检查并执行消除
        /// </summary>
        private bool CheckAndEliminate()
        {
            SetBattleState(BattleState.CheckingMatch);
            
            // 查找所有可消除的位置
            _eliminateList.Clear();
            FindMatches(_eliminateList);
            
            if (_eliminateList.Count == 0)
            {
                return false;
            }
            
            // 执行消除
            SetBattleState(BattleState.Eliminating);
            
            // 增加连击
            _currentCombo++;
            if (_currentCombo > _maxCombo)
            {
                _maxCombo = _currentCombo;
            }
            
            // 事件
            OnEliminateStart?.Invoke(_eliminateList);
            OnComboTriggered?.Invoke(_currentCombo);
            
            // 计算分数
            int eliminateScore = CalculateEliminateScore(_eliminateList.Count, _currentCombo);
            AddScore(eliminateScore);
            
            // 移除元素
            foreach (var coord in _eliminateList)
            {
                _boardData[coord.Line, coord.Col] = -1;
            }
            
            _totalEliminateCount += _eliminateList.Count;
            _normalEliminateCount += _eliminateList.Count;
            
            // 消除完成事件
            OnEliminateEnd?.Invoke(EliminateResult.Normal);
            
            // 进入下落阶段
            ProcessElementDrop();
            
            return true;
        }

        /// <summary>
        /// 处理元素下落
        /// </summary>
        private void ProcessElementDrop()
        {
            SetBattleState(BattleState.Dropping);
            
            int width = GetBoardWidth();
            int height = GetBoardHeight();
            
            // 列处理，从下往上
            for (int x = 0; x < width; x++)
            {
                int emptyCount = 0;
                for (int y = 0; y < height; y++)
                {
                    if (_boardData[x, y] == -1)
                    {
                        emptyCount++;
                    }
                    else if (emptyCount > 0)
                    {
                        // 下落
                        _boardData[x, y - emptyCount] = _boardData[x, y];
                        _boardData[x, y] = -1;
                    }
                }
            }
            
            OnElementDrop?.Invoke();
            
            // 进入生成阶段
            ProcessElementSpawn();
        }

        /// <summary>
        /// 处理元素生成
        /// </summary>
        private void ProcessElementSpawn()
        {
            SetBattleState(BattleState.Spawning);
            
            int width = GetBoardWidth();
            int height = GetBoardHeight();
            List<int> elementTypes = GetAvailableElementTypes();
            
            // 填充空位
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_boardData[x, y] == -1)
                    {
                        _boardData[x, y] = elementTypes[UnityEngine.Random.Range(0, elementTypes.Count)];
                    }
                }
            }
            
            // 检查是否有新的消除(连锁)
            if (CheckAndEliminate())
            {
                // 继续连锁
                SetBattleState(BattleState.CheckingMatch);
            }
            else
            {
                // 连锁结束
                _currentCombo = 0;
                CheckBattleEnd();
                SetBattleState(BattleState.WaitInput);
            }
        }

        /// <summary>
        /// 检查战斗结束
        /// </summary>
        private void CheckBattleEnd()
        {
            if (_battleResult != BattleResult.Processing)
                return;
            
            // 检查胜利条件
            if (IsVictory())
            {
                _battleResult = BattleResult.Victory;
                SetBattleState(BattleState.Settling);
                EndBattle();
                return;
            }
            
            // 检查失败条件
            if (IsDefeat())
            {
                _battleResult = BattleResult.Defeat;
                SetBattleState(BattleState.Settling);
                EndBattle();
                return;
            }
        }

        /// <summary>
        /// 是否胜利
        /// </summary>
        private bool IsVictory()
        {
            if (_currentLevel == null)
                return false;
            
            // 目标类型判断
            switch (_currentLevel.TargetType)
            {
                case 1: // 消除指定数量
                    return _totalEliminateCount >= _currentLevel.TargetCount;
                    
                case 2: // 达到指定分数
                    return _currentScore >= _currentLevel.TargetCount;
                    
                default:
                    return false;
            }
        }

        /// <summary>
        /// 是否失败
        /// </summary>
        private bool IsDefeat()
        {
            if (_currentLevel == null)
                return false;
            
            // 步数用尽
            if (_currentMoves <= 0)
                return true;
            
            // 时间用尽
            if (_currentLevel.TimeLimit > 0 && GetRemainingTime() <= 0)
                return true;
            
            // 无法移动(棋盘无可消除)
            if (!CanMakeMove())
                return true;
            
            return false;
        }

        /// <summary>
        /// 消耗步数
        /// </summary>
        private void ConsumeMove()
        {
            _currentMoves--;
            OnMovesChanged?.Invoke(_currentMoves);
            Debug.Log(string.Format("[BattleSystem] Move consumed: remaining={0}", _currentMoves));
        }

        //===========================================================
        // 私有方法 - 消除检测
        //===========================================================

        /// <summary>
        /// 查找所有匹配
        /// </summary>
        private void FindMatches(List<GridCoord> matches)
        {
            int width = GetBoardWidth();
            int height = GetBoardHeight();
            bool[,] visited = new bool[width, height];
            
            // 水平检测
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width - 2; x++)
                {
                    int type = _boardData[x, y];
                    if (type < 0) continue;
                    
                    int matchLength = 1;
                    while (x + matchLength < width && _boardData[x + matchLength, y] == type)
                    {
                        matchLength++;
                    }
                    
                    if (matchLength >= MIN_MATCH_COUNT)
                    {
                        for (int i = 0; i < matchLength; i++)
                        {
                            if (!visited[x + i, y])
                            {
                                visited[x + i, y] = true;
                                matches.Add(new GridCoord(0, x + i, y));
                            }
                        }
                        x += matchLength - 1;
                    }
                }
            }
            
            // 垂直检测
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height - 2; y++)
                {
                    int type = _boardData[x, y];
                    if (type < 0) continue;
                    
                    int matchLength = 1;
                    while (y + matchLength < height && _boardData[x, y + matchLength] == type)
                    {
                        matchLength++;
                    }
                    
                    if (matchLength >= MIN_MATCH_COUNT)
                    {
                        for (int i = 0; i < matchLength; i++)
                        {
                            if (!visited[x, y + i])
                            {
                                visited[x, y + i] = true;
                                matches.Add(new GridCoord(0, x, y + i));
                            }
                        }
                        y += matchLength - 1;
                    }
                }
            }
        }

        /// <summary>
        /// 是否会导致消除(用于开局生成)
        /// </summary>
        private bool WouldCauseMatch(int x, int y, int elementType)
        {
            int width = GetBoardWidth();
            int height = GetBoardHeight();
            
            // 水平检查
            if (x >= 2)
            {
                if (_boardData[x - 1, y] == elementType && _boardData[x - 2, y] == elementType)
                    return true;
            }
            
            // 垂直检查
            if (y >= 2)
            {
                if (_boardData[x, y - 1] == elementType && _boardData[x, y - 2] == elementType)
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// 是否可以移动
        /// </summary>
        private bool CanMakeMove()
        {
            int width = GetBoardWidth();
            int height = GetBoardHeight();
            
            // 尝试每个位置的交换
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // 尝试向右交换
                    if (x < width - 1)
                    {
                        SwapInBoard(x, y, x + 1, y);
                        if (HasPossibleMatch())
                        {
                            SwapInBoard(x, y, x + 1, y);
                            return true;
                        }
                        SwapInBoard(x, y, x + 1, y);
                    }
                    
                    // 尝试向下交换
                    if (y < height - 1)
                    {
                        SwapInBoard(x, y, x, y + 1);
                        if (HasPossibleMatch())
                        {
                            SwapInBoard(x, y, x, y + 1);
                            return true;
                        }
                        SwapInBoard(x, y, x, y + 1);
                    }
                }
            }
            
            return false;
        }

        /// <summary>
        /// 是否有可能的消除
        /// </summary>
        private bool HasPossibleMatch()
        {
            var tempMatches = new List<GridCoord>();
            FindMatches(tempMatches);
            return tempMatches.Count > 0;
        }

        //===========================================================
        // 私有方法 - 技能系统
        //===========================================================

        /// <summary>
        /// 执行技能效果
        /// </summary>
        private void ExecuteSkillEffect(SkillType skillType, int targetX, int targetY)
        {
            int width = GetBoardWidth();
            int height = GetBoardHeight();
            List<GridCoord> skillTargets = new List<GridCoord>();
            
            switch (skillType)
            {
                case SkillType.Hammer:
                    // 消除单个
                    skillTargets.Add(new GridCoord(0, targetX, targetY));
                    break;
                    
                case SkillType.Bomb:
                    // 消除3x3区域
                    for (int x = targetX - 1; x <= targetX + 1; x++)
                    {
                        for (int y = targetY - 1; y <= targetY + 1; y++)
                        {
                            if (IsValidCoord(x, y))
                            {
                                skillTargets.Add(new GridCoord(0, x, y));
                            }
                        }
                    }
                    break;
                    
                case SkillType.Rocket:
                    // 消除整行
                    for (int x = 0; x < width; x++)
                    {
                        skillTargets.Add(new GridCoord(0, x, targetY));
                    }
                    // 消除整列
                    for (int y = 0; y < height; y++)
                    {
                        if (y != targetY)
                        {
                            skillTargets.Add(new GridCoord(0, targetX, y));
                        }
                    }
                    break;
                    
                case SkillType.Rainbow:
                    // 消除所有同色
                    int targetType = _boardData[targetX, targetY];
                    if (targetType >= 0)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                if (_boardData[x, y] == targetType)
                                {
                                    skillTargets.Add(new GridCoord(0, x, y));
                                }
                            }
                        }
                    }
                    break;
            }
            
            // 执行消除
            if (skillTargets.Count > 0)
            {
                _skillEliminateCount += skillTargets.Count;
                
                foreach (var coord in skillTargets)
                {
                    if (IsValidCoord(coord.Line, coord.Col))
                    {
                        _boardData[coord.Line, coord.Col] = -1;
                    }
                }
                
                // 事件
                OnEliminateStart?.Invoke(skillTargets);
                OnEliminateEnd?.Invoke(EliminateResult.Skill);
                
                // 继续下落
                ProcessElementDrop();
            }
            else
            {
                SetBattleState(BattleState.WaitInput);
            }
        }

        //===========================================================
        // 私有方法 - 工具函数
        //===========================================================

        /// <summary>
        /// 设置战斗状态
        /// </summary>
        private void SetBattleState(BattleState newState)
        {
            if (_battleState == newState)
                return;
            
            BattleState oldState = _battleState;
            _battleState = newState;
            
            OnStateChanged?.Invoke(oldState, newState);
            Debug.Log(string.Format("[BattleSystem] State changed: {0} -> {1}", oldState, newState));
        }

        /// <summary>
        /// 加载关卡配置
        /// </summary>
        private LevelConfig LoadLevelConfig(int levelId)
        {
            // 实际项目中从配置表加载
            // 这里返回默认配置
            return new LevelConfig
            {
                LevelId = levelId,
                Name = $"关卡 {levelId}",
                TargetType = 1,
                TargetCount = 50,
                MoveLimit = 30,
                TimeLimit = 0,
                BoardWidth = DEFAULT_BOARD_WIDTH,
                BoardHeight = DEFAULT_BOARD_HEIGHT,
                ElementTypes = new List<string> { "red", "blue", "green", "yellow", "purple" },
                EnableSkill = true,
                Difficulty = 1,
                StarScores = new int[] { 1000, 2000, 3000 }
            };
        }

        /// <summary>
        /// 获取可用元素类型
        /// </summary>
        private List<int> GetAvailableElementTypes()
        {
            if (_currentLevel != null && _currentLevel.ElementTypes != null)
            {
                List<int> types = new List<int>();
                for (int i = 0; i < _currentLevel.ElementTypes.Count; i++)
                {
                    types.Add(i);
                }
                return types;
            }
            
            // 默认5种颜色
            return new List<int> { 0, 1, 2, 3, 4 };
        }

        /// <summary>
        /// 坐标是否有效
        /// </summary>
        private bool IsValidCoord(int x, int y)
        {
            if (_boardData == null)
                return false;
            
            int width = GetBoardWidth();
            int height = GetBoardHeight();
            
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        /// <summary>
        /// 是否相邻
        /// </summary>
        private bool IsAdjacent(int x1, int y1, int x2, int y2)
        {
            int dx = Math.Abs(x1 - x2);
            int dy = Math.Abs(y1 - y2);
            
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }

        /// <summary>
        /// 交换棋盘数据
        /// </summary>
        private void SwapInBoard(int x1, int y1, int x2, int y2)
        {
            if (!IsValidCoord(x1, y1) || !IsValidCoord(x2, y2))
                return;
            
            int temp = _boardData[x1, y1];
            _boardData[x1, y1] = _boardData[x2, y2];
            _boardData[x2, y2] = temp;
        }

        /// <summary>
        /// 添加分数
        /// </summary>
        private void AddScore(int score)
        {
            int oldScore = _currentScore;
            _currentScore += score;
            
            OnScoreChanged?.Invoke(_currentScore, score);
            Debug.Log(string.Format("[BattleSystem] Score added: +{0}, total={1}", score, _currentScore));
        }

        /// <summary>
        /// 计算消除分数
        /// </summary>
        private int CalculateEliminateScore(int count, int combo)
        {
            // 基础分数: 每个元素10分
            int baseScore = count * 10;
            
            // 连锁加成: 2连以上有加成
            float comboMultiplier = 1.0f + (combo - 1) * 0.5f;
            
            // 大范围加成
            float massiveMultiplier = count >= 5 ? 2.0f : 1.0f;
            
            return (int)(baseScore * comboMultiplier * massiveMultiplier);
        }

        /// <summary>
        /// 计算星级
        /// </summary>
        private int CalculateStarCount()
        {
            if (_currentLevel == null || _currentLevel.StarScores == null)
                return 0;
            
            for (int i = _currentLevel.StarScores.Length - 1; i >= 0; i--)
            {
                if (_currentScore >= _currentLevel.StarScores[i])
                {
                    return i + 1;
                }
            }
            
            return 0;
        }

        /// <summary>
        /// 清理战斗数据
        /// </summary>
        private void ClearBattleData()
        {
            _currentLevel = null;
            _boardData = null;
            _eliminateList.Clear();
            _skillHistory.Clear();
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        private long GetCurrentTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}

