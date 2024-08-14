/*
        author      :       yangzijian
        time        :       2019-12-26 16:21:16
        function    :       Collect eligible elements.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{

    public class Match
    {
        Dictionary<ElementAttribute.Attribute, ElementAttribute> m_mpAttributeConformity = new Dictionary<ElementAttribute.Attribute, ElementAttribute>();
        Dictionary<ElementAttribute.Attribute, ElementAttribute> m_mpAttributeInconformity = new Dictionary<ElementAttribute.Attribute, ElementAttribute>();

        public Match() { }
        public void setConformityValue(ElementAttribute.Attribute eAttribute, ElementAttribute tElementAttribute)
        {
            m_mpAttributeConformity[eAttribute] = tElementAttribute;
        }
        public void setInconformmityValue(ElementAttribute.Attribute eAttribute, ElementAttribute tElementAttribute)
        {
            m_mpAttributeInconformity[eAttribute] = tElementAttribute;
        }

        public bool match(Element tElement)
        {
            foreach (var tInconformity in m_mpAttributeInconformity)
            {
                if (tElement.compareElementAttribute(tInconformity.Key, tInconformity.Value) == true)
                {
                    return false;
                }
            }
            foreach (var tconformity in m_mpAttributeConformity)
            {
                if (tElement.compareElementAttribute(tconformity.Key, tconformity.Value) == false)
                {
                    return false;
                }
            }
            return m_mpAttributeConformity.Count > 0;
        }
    }

    public class ENateMatchRule
    {
        List<Match> m_arrConfirmToValue = new List<Match>();
        List<Match> m_arrInconformityValue = new List<Match>();

        public void addConform(Match tMatch)
        {
            m_arrConfirmToValue.Add(tMatch);
        }

        public void addUnConform(Match tMatch)
        {
            m_arrInconformityValue.Add(tMatch);
        }

        public bool Grid_match(Grid tGrid)
        {
            return tGrid.match(m_arrConfirmToValue, m_arrInconformityValue);
        }

        ///////////////////////////////////////////////////////
        public static ENateMatchRule sm_tMatch_ComposeRule;
        public static ElementValue<int> sm_tMatch_ComposeColorValue;

        public static ENateMatchRule sm_tMatch_ForbiddenMove;
        public static ENateMatchRule sm_tMatchRule_ForbiddenOperator;
        public static ENateMatchRule sm_tMatchRule_DropRandom;
        public static ElementValue<int> sm_DropRandom_eMatchColor;
        public static ENateMatchRule sm_tMatchRule_DropDevice;

        //drop detect 
        public static ENateMatchRule sm_tIfICanMoveMath;
        public static ENateMatchRule sm_tIfICanExchangeMath;
        public static ElementValue<int> sm_tCanIMoveThereMath_selfGridMatch;
        public static ENateMatchRule sm_tCanIMoveThereMath_selfGrid_FixType;
        public static ENateMatchRule sm_tCanIMoveThereMath_selfGrid;

        public static ElementValue<int> sm_tCanIMoveThereMath_otherGridMatch;
        public static ENateMatchRule sm_tCanIMoveThereMath_otherGrid_FixType;
        public static ENateMatchRule sm_tCanIMoveThereMath_otherGrid;
        public static ENateMatchRule sm_tIfIHaveMoveElemntMath;
        public static ENateMatchRule sm_tifIHaveCanNotMoveElemntMath;

        public static ENateMatchRule sm_tExchangeMatchRule;
        public static ENateMatchRule sm_tMatchRule_ComposeGrid;
        public static ENateMatchRule sm_tMatchRule_Exchange;
        public static void initStatic()
        {
            {
                sm_tMatch_ComposeRule = new ENateMatchRule();
                sm_tMatch_ComposeColorValue = new ElementValue<int>(-1);
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.color, sm_tMatch_ComposeColorValue);
                    sm_tMatch_ComposeRule.addConform(tMatch);
                }
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.forbiddenCompose, new ElementValue<int>(1));
                    sm_tMatch_ComposeRule.addUnConform(tMatch);
                }
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.isLock, new ElementValue<bool>(true));
                    sm_tMatch_ComposeRule.addUnConform(tMatch);
                }
            }
            //////////////////////////////////////////////////////////////////
            {
                sm_tMatch_ForbiddenMove = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.forbiddenMove, new ElementValue<int>(1));
                    sm_tMatch_ForbiddenMove.addConform(tMatch);
                }
            }
            {
                sm_tMatchRule_ForbiddenOperator = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.canMove, new ElementValue<int>(1));
                    sm_tMatchRule_ForbiddenOperator.addConform(tMatch);
                }
            }
            {
                sm_tMatchRule_DropRandom = new ENateMatchRule();
                sm_DropRandom_eMatchColor = new ElementValue<int>(0);
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.color, sm_DropRandom_eMatchColor);
                    sm_tMatchRule_DropRandom.addConform(tMatch);
                }
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.forbiddenCompose, new ElementValue<int>(1));
                    sm_tMatchRule_DropRandom.addUnConform(tMatch);
                }
            }
            {
                sm_tMatchRule_DropDevice = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.m_bIsCellOccupy, new ElementValue<int>(1));
                    sm_tMatchRule_DropDevice.addUnConform(tMatch);
                }
            }
            {
                sm_tIfICanMoveMath = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.moveType, new ElementValue<int>(1));
                    tMatch.setConformityValue(ElementAttribute.Attribute.m_bIsCellOccupy, new ElementValue<int>(1));
                    sm_tIfICanMoveMath.addConform(tMatch);
                }
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.fixType, new ElementValue<int>(6));
                    sm_tIfICanMoveMath.addUnConform(tMatch);
                }
            }
            {
                // self grid 
                sm_tCanIMoveThereMath_selfGrid = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.fixType, new ElementValue<int>(6));
                    sm_tCanIMoveThereMath_selfGrid.addUnConform(tMatch);
                }
                sm_tCanIMoveThereMath_selfGrid_FixType = new ENateMatchRule();
                {
                    sm_tCanIMoveThereMath_selfGridMatch = new ElementValue<int>(0);
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.fixType, sm_tCanIMoveThereMath_selfGridMatch);
                    sm_tCanIMoveThereMath_selfGrid_FixType.addConform(tMatch);
                }
            }
            {
                sm_tCanIMoveThereMath_otherGrid = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.m_bIsCellOccupy, new ElementValue<int>(1));
                    sm_tCanIMoveThereMath_otherGrid.addUnConform(tMatch);
                }
                sm_tCanIMoveThereMath_otherGrid_FixType = new ENateMatchRule();
                {
                    sm_tCanIMoveThereMath_otherGridMatch = new ElementValue<int>(0);
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.fixType, sm_tCanIMoveThereMath_otherGridMatch);
                    sm_tCanIMoveThereMath_otherGrid_FixType.addConform(tMatch);
                }
            }
            {
                sm_tIfICanExchangeMath = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.moveType, new ElementValue<int>(1));
                    tMatch.setConformityValue(ElementAttribute.Attribute.canMove, new ElementValue<int>(1));
                    tMatch.setConformityValue(ElementAttribute.Attribute.m_bIsCellOccupy, new ElementValue<int>(1));
                    sm_tIfICanExchangeMath.addConform(tMatch);
                }
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.fixType, new ElementValue<int>(6));
                    sm_tIfICanExchangeMath.addUnConform(tMatch);
                }
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.forbiddenMove, new ElementValue<int>(1));
                    sm_tIfICanExchangeMath.addUnConform(tMatch);
                }
            }
            {
                sm_tIfIHaveMoveElemntMath = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.moveType, new ElementValue<int>(1));
                    tMatch.setConformityValue(ElementAttribute.Attribute.m_bIsCellOccupy, new ElementValue<int>(1));
                    sm_tIfIHaveMoveElemntMath.addConform(tMatch);
                }
            }
            {
                sm_tifIHaveCanNotMoveElemntMath = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.moveType, new ElementValue<int>(0));
                    tMatch.setConformityValue(ElementAttribute.Attribute.m_bIsCellOccupy, new ElementValue<int>(1));
                    sm_tifIHaveCanNotMoveElemntMath.addConform(tMatch);
                }
            }
            {
                // 如果有屏蔽基层元素组合， 就跳过
                // 如果不让点击的元素， 不能进入重置判断中
                sm_tExchangeMatchRule = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.type, new ElementValue<int>(0));
                    tMatch.setConformityValue(ElementAttribute.Attribute.moveType, new ElementValue<int>(1));
                    tMatch.setConformityValue(ElementAttribute.Attribute.m_bIsCellOccupy, new ElementValue<int>(1));
                    sm_tExchangeMatchRule.addConform(tMatch);
                }
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.fixType, new ElementValue<int>(6));
                    sm_tExchangeMatchRule.addUnConform(tMatch);
                }
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.forbiddenMove, new ElementValue<int>(1));
                    sm_tExchangeMatchRule.addUnConform(tMatch);
                }
            }
            {
                sm_tMatchRule_ComposeGrid = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.involvedCompose, new ElementValue<int>(1));
                    sm_tMatchRule_ComposeGrid.addConform(tMatch);
                }
            }
            {
                sm_tMatchRule_Exchange = new ENateMatchRule();
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.involvedCompose, new ElementValue<int>(1));
                    sm_tMatchRule_Exchange.addConform(tMatch);
                }
                {
                    var tMatch = new Match();
                    tMatch.setConformityValue(ElementAttribute.Attribute.forbiddenCompose, new ElementValue<int>(1));
                    sm_tMatchRule_Exchange.addUnConform(tMatch);
                }
            }
        }

    }
    public class ENateMatch
    {
        public ChessBoard m_tChessBoard;
        public Grid m_tGrid;
        public int m_eColor;
        public GridCoord m_tTriggerGridCoord;
        Dictionary<int, bool> m_hsmpTraversedGridCoord = new Dictionary<int, bool>();
        public Dictionary<int, Grid> m_mpCollectedGrid = new Dictionary<int, Grid>();

        public void init(ChessBoard tChessBoard, Grid tGrid, int eColor)
        {
            m_tChessBoard = tChessBoard;
            m_tGrid = tGrid;
            m_eColor = eColor;
            m_hsmpTraversedGridCoord.Clear();
            m_mpCollectedGrid.Clear();
        }
        public void traversedAroundGrid(ChessBoard tChessBoard, Grid tGrid, ENateMatchRule tMatchRule)
        {
            Grid tUpGrid = tGrid.getGridByDirection(Direction.Up);
            traversedGrid(tChessBoard, tUpGrid, tMatchRule);
            Grid tDownGrid = tGrid.getGridByDirection(Direction.Down);
            traversedGrid(tChessBoard, tDownGrid, tMatchRule);
            Grid tLeftGrid = tGrid.getGridByDirection(Direction.Left);
            traversedGrid(tChessBoard, tLeftGrid, tMatchRule);
            Grid tRightGrid = tGrid.getGridByDirection(Direction.Right);
            traversedGrid(tChessBoard, tRightGrid, tMatchRule);
        }

        public static void traversedAllGrid(ChessBoard tChessBoard, List<Match> arrConfirmToValue, List<Match> arrInconformityValue, ref List<Grid> arrGrid)
        {
            for (int nLine = 0; nLine < tChessBoard.m_nWidth; ++nLine)
            {
                for (int nColumn = 0; nColumn < tChessBoard.m_nHeight; ++nColumn)
                {
                    Grid tGrid = tChessBoard.getGrid(nLine, nColumn);
                    if (tGrid == null)
                    {
                        continue;
                    }
                    if (tGrid.match(arrConfirmToValue, arrInconformityValue) == false)
                    {
                        continue;
                    }
                    arrGrid.Add(tGrid);
                }
            }
        }
        public static void traversedAllGrid(ChessBoard tChessBoard, GridCoord tGridCoord, List<string> arrConditionId, ref List<Grid> arrGrid)
        {
            List<JsonData.Client_Config.Condition> arrCondition = new List<JsonData.Client_Config.Condition>();
            foreach (var strConditionId in arrConditionId)
            {
                arrCondition.Add(ConditionConfig.getCondition(strConditionId));
            }
            traversedAllGrid(tChessBoard, tGridCoord, arrCondition, ref arrGrid);
            ConditionConfig.clearConditionCache(arrConditionId);
        }
        public static void traversedAllGrid(ChessBoard tChessBoard, GridCoord tGridCoord, List<JsonData.Client_Config.Condition> arrCondition, ref List<Grid> arrGrid)
        {
            ConditionConfig.MapArg tMapArg = new ConditionConfig.MapArg();
            tMapArg.Stage = tChessBoard.m_tStage;
            tMapArg.ChessBoard = tChessBoard;
            tMapArg.GridCoord = tGridCoord;
            Grid tSrcGrid = tChessBoard.getGrid(tGridCoord);
            if (tSrcGrid != null)
            {
                tMapArg.GridCoordDropDirection = tSrcGrid.DropDirection;
            }
            for (int nLine = 0; nLine < tChessBoard.m_nWidth; ++nLine)
            {
                for (int nColumn = 0; nColumn < tChessBoard.m_nHeight; ++nColumn)
                {
                    Grid tGrid = tChessBoard.getGrid(nLine, nColumn);
                    if (tGrid == null)
                    {
                        continue;
                    }
                    tMapArg.Grid = tGrid;
                    bool bIsOk = true;
                    foreach (var tCondition in arrCondition)
                    {
                        if (ConditionConfig.checkCondition(tCondition, tMapArg) == false)
                        {
                            bIsOk = false;
                            break;
                        }
                    }
                    if (bIsOk == false)
                    {
                        continue;
                    }
                    arrGrid.Add(tGrid);
                }
            }
        }
        public void traversedGrid(ChessBoard tChessBoard, Grid tGrid, ENateMatchRule tMatchRule)
        {
            if (tGrid == null || tChessBoard == null)
            {
                return;
            }
            if (m_hsmpTraversedGridCoord.ContainsKey(tGrid.m_tGridCoord.Coord) == true)
            {
                return;
            }
            m_hsmpTraversedGridCoord.Add(tGrid.m_tGridCoord.Coord, true);
            if (tMatchRule.Grid_match(tGrid) == false)
            {
                return;
            }
            m_mpCollectedGrid.Add(tGrid.m_tGridCoord.Coord, tGrid);
            traversedAroundGrid(tChessBoard, tGrid, tMatchRule);
        }

        public void choseArroundWithColor(ChessBoard tChessBoard, Grid tGrid, int eColor)
        {
            init(tChessBoard, tGrid, eColor);
            ENateMatchRule.sm_tMatch_ComposeColorValue.Value = eColor;
            traversedGrid(tChessBoard, tGrid, ENateMatchRule.sm_tMatch_ComposeRule);
        }
        public void choseDefault(ChessBoard tChessBoard, Grid tGrid)
        {
            UnityEngine.Profiling.Profiler.BeginSample("ENateMatch.cs choseDefault");
            Element tDefaultElement = tGrid.getElementWithElementAttribute<int>(ElementAttribute.Attribute.involvedCompose, 1);
            if (tDefaultElement == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            ElementValue<int> tColorValue = tDefaultElement.getElementAttribute(ElementAttribute.Attribute.color) as ElementValue<int>;
            int eColor = tColorValue != null ? tColorValue.Value : -1;
            if (eColor == -1)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            m_tTriggerGridCoord = tGrid.m_tGridCoord;
            choseArroundWithColor(tChessBoard, tGrid, eColor);
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }

    public class ENateCompose
    {
        ChessBoard m_tChessBoard;
        public string m_strGeneratedId = "";
        public List<Grid> m_arrComposeGrid = new List<Grid>();
        int m_nPriority = -1;
        List<Grid> arrOkGrid = new List<Grid>();
        public Grid m_tGeneratedGrid;
        public bool m_bIsNeedReCheckPoint = false;

        ENateCompose(ChessBoard tChessBoard)
        {
            m_tChessBoard = tChessBoard;
        }

        bool checkAndChose(ChessBoard tChessBoard, Dictionary<int, Grid> mpGrid, int nLine, int nColumn, int nLineNum, int nColumnNum)
        {
            UnityEngine.Profiling.Profiler.BeginSample("ENateMatch.cs checkAndChose");

            Func<int, int, Grid> pGetGridFunc = (int nGetLine, int nGetColumn) =>
            {
                Grid tGrid = null;
                try
                {
                    tGrid = mpGrid[GridCoord.posToCoord(tChessBoard.Index, nGetLine, nGetColumn)];
                }
                catch { }
                return tGrid;
            };
            nLineNum = nLineNum < 1 ? 1 : nLineNum;
            nColumnNum = nColumnNum < 1 ? 1 : nColumnNum;

            bool bIsDetectOk = true;
            arrOkGrid.Clear();
            for (int nAddLine = 0; nAddLine < nColumnNum; ++nAddLine)
            {
                int nDetectLine = nLine + nAddLine;
                for (int nAddColumn = 0; nAddColumn < nLineNum; ++nAddColumn)
                {
                    int nDetectColumn = nColumn + nAddColumn;
                    Grid tGrid = pGetGridFunc(nDetectLine, nDetectColumn);
                    if (tGrid == null)
                    {
                        bIsDetectOk = false;
                        break;
                    }
                    arrOkGrid.Add(tGrid);
                }
            }
            if (bIsDetectOk)
            {
                m_arrComposeGrid.AddRange(arrOkGrid);
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return bIsDetectOk;
        }

        void composeDetectedGrid(ChessBoard tChessBoard, Dictionary<int, Grid> mpGrid, List<Vector2Int> arrLineColumns, int nPriority, string strGenerateId)
        {
            UnityEngine.Profiling.Profiler.BeginSample("ENateMatch.cs composeDetectedGrid");
            if (mpGrid == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            List<bool> arrOk = new List<bool>();
            for (int i = 0; i < arrLineColumns.Count; i++)
            {
                arrOk.Add(false);
            }
            foreach (var itGrid in mpGrid)
            {
                Grid tGrid = itGrid.Value;
                for (int i = 0; i < arrLineColumns.Count; i++)
                {
                    Vector2Int vLineColumns = arrLineColumns[i];
                    if (checkAndChose(tChessBoard, mpGrid, tGrid.m_tGridCoord.Line, tGrid.m_tGridCoord.Col, vLineColumns.x, vLineColumns.y) == true)
                    {
                        arrOk[i] = true;
                    }
                }
            }
            bool bIsOk = true;
            foreach (var bCheck in arrOk)
            {
                if (bCheck == false)
                {
                    bIsOk = false;
                    break;
                }
            }
            if (bIsOk == true)
            {
                if (m_nPriority < nPriority)
                {
                    m_strGeneratedId = strGenerateId;
                    m_nPriority = nPriority;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public static ENateCompose compose(ChessBoard tChessBoard, Dictionary<int, Grid> mpGrid, Grid tGeneratorGrid)
        {
            UnityEngine.Profiling.Profiler.BeginSample("ENateMatch.cs compose");
            ENateCompose tENateCompose = new ENateCompose(tChessBoard);
            foreach (var tComposeRules in JsonManager.composerules_config.root.game.ComposeTemplate.Compose)
            {
                string strGenerateId = "";
                try
                {
                    if (tComposeRules.create != null && tComposeRules.create.m_strCreateElementId != null)
                    {
                        strGenerateId = tComposeRules.create.m_strCreateElementId;
                    }
                }
                catch (System.Exception) { }
                List<Vector2Int> arrLineColumns = new List<Vector2Int>();
                foreach (var tAccord in tComposeRules.accord)
                {
                    arrLineColumns.Add(new Vector2Int(int.Parse(tAccord.m_nLine), int.Parse(tAccord.m_nColumns)));
                }
                tENateCompose.composeDetectedGrid(tChessBoard, mpGrid, arrLineColumns, int.Parse(tComposeRules.m_nPriority), strGenerateId);
            }
            Func<Grid, int, bool> pCheckGeneratorGrid = (Grid tCheckGrid, int nCheckLevel) =>
            {
                if (tCheckGrid == null)
                {
                    return false;
                }
                foreach (var tCheckElement in tCheckGrid.m_sortedElement)
                {
                    if (tCheckElement.Value.IsLock == true)
                    {
                        continue;
                    }
                    var tDirValue = tCheckElement.Value.getElementAttribute(ElementAttribute.Attribute.eliminateTransmit) as ElementValue<int>;
                    var tLevelValue = tCheckElement.Value.getElementAttribute(ElementAttribute.Attribute.level) as ElementValue<int>;
                    if (nCheckLevel == tLevelValue.Value)
                    {
                        var tDestroyToValue = tCheckElement.Value.getElementAttribute(ElementAttribute.Attribute.destroyTo) as ElementValue<string>;
                        if (tDestroyToValue != null && string.IsNullOrEmpty(tDestroyToValue.Value) == false)
                        {
                            return false;
                        }
                    }
                    else if (nCheckLevel < tLevelValue.Value)
                    {
                        if (tDirValue != null && tDirValue.Value == 0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            };
            if (string.IsNullOrEmpty(tENateCompose.m_strGeneratedId) == false)
            {
                do
                {
                    var tGenLevelValue = Config.ElementConfig.getElementLevel(tENateCompose.m_strGeneratedId);
                    if (tGenLevelValue == null)
                    {
                        break;
                    }

                    if (pCheckGeneratorGrid(tGeneratorGrid, tGenLevelValue.Value) == true)
                    {
                        tENateCompose.m_tGeneratedGrid = tGeneratorGrid;
                        break;
                    }
                    foreach (var tCheckGeneratedGrid in tENateCompose.m_arrComposeGrid)
                    {
                        if (pCheckGeneratorGrid(tCheckGeneratedGrid, tGenLevelValue.Value) == true)
                        {
                            tENateCompose.m_tGeneratedGrid = tCheckGeneratedGrid;
                            break;
                        }
                    }
                } while (false);
            }
            else
            {
                tENateCompose.m_tGeneratedGrid = null;
            }
            ////compute check point is null element 
            if (pCheckGeneratorGrid(tGeneratorGrid, 0) == false)
            {
                tENateCompose.m_bIsNeedReCheckPoint = true;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return tENateCompose;
        }

    }

}