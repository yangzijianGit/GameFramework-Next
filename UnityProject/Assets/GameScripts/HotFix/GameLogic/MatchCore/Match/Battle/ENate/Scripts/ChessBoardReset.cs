/*
 * @Description: for reset chessboard
 * @Author: yangzijian
 * @Date: 2020-02-24 10:54:45
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-19 21:38:55
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    public class ChessBoardReset
    {

        public class BlockAniMove
        {
            BlockAniMove(int nMoveSrcIndex)
            {
                m_nMoveIndexSrc = nMoveSrcIndex;
                m_nMoveIndexAim = nMoveSrcIndex;
            }
            public int m_nMoveIndexSrc;
            public int m_nMoveIndexAim;
        }

        public enum ECheckResetType
        {
            someOneOk = 0, // 某个元素可以交换合成
            super = 1, // 有某个超能元素
            noneStep = 2, // 没有可以交换合成的元素了， 重置一下
            randomSuper_BlockNum = 3, // 没有可以合成的元素了， 随机一个普通的位超能元素
            randomSuper_Position = 4, // 位置不行
            resetAniWait = 5, // 等待重置动画
            over // 不行了
        }
        ChessBoard m_pChessBoard;
        private static int m_nCoposeLevel;
        private Dictionary<int, bool> m_arrComposeCheck = new Dictionary<int, bool>();
        private FiltratePosition m_tFiltratePosition;

        private static List<string> m_arrpEliminateSuperAttriubte = new List<string>();
        // private static List<string> m_arrpExchangeSuperAttribute;
        List<int> m_arrDropUnitId = new List<int>();
        public ChessBoardReset(ChessBoard pChessBoard)
        {
            m_pChessBoard = pChessBoard;
            m_tFiltratePosition = new FiltratePosition(pChessBoard);
        }

        public void setSize(int nWidth, int nHeight)
        {
            m_tFiltratePosition.setSize(nWidth, nHeight);
        }
        public void clear() { }
        public static void initConfigAttribute()
        {
            try
            {
                foreach (var itElement in JsonManager.element_config.root.game.element)
                {
                    if (int.Parse(itElement.type) == 1)
                    {
                        m_arrpEliminateSuperAttriubte.Add(itElement.id);
                    }
                }
            }
            catch { }
        }
        public static void initStatic()
        {
            // 如果有屏蔽基层元素组合， 就跳过
            // 如果不让点击的元素， 不能进入重置判断中
            initConfigAttribute();
        }

        /*
         * 判断是否有m_bIsInitiativeEliminate 为true的超能元素， 如果有则直接提示这个
         * 如果m_bIsInitiativeEliminate 为false的超能元素， 则看周围是否可移动， 可移动并消除判断ComposeRulesConfig里面的rules是否可以消除， 如果可以， 则返回true。
         */

        public ECheckResetType reset()
        {
            UnityEngine.Profiling.Profiler.BeginSample("reset");
            ECheckResetType eCheckType = checkResetBlock();
            bool bIsOk = true;
            switch (eCheckType)
            {
                case ECheckResetType.noneStep:
                    {
                        resetBlock();
                        eCheckType = ECheckResetType.resetAniWait;
                    }
                    break;
                case ECheckResetType.randomSuper_BlockNum:
                case ECheckResetType.randomSuper_Position:
                    {
                        bIsOk = randomSuperElement();
                        eCheckType = ECheckResetType.resetAniWait;
                    }
                    break;
                case ECheckResetType.over:
                    {
                        bIsOk = false;
                    }
                    break;
                default:
                    break;
            }
            //LogPrintf("doCheck function using time : %d", 2, GameTime::getTime() - nBeginTime);
            // if (!bIsOk)
            // {
            //     m_pChessBoard . m_pStage . setWinFailed();
            // }
            // m_pChessBoard . m_pStage . setm_bCanNotMoveAnyMore(!bIsOk);
            UnityEngine.Profiling.Profiler.EndSample();
            return eCheckType;
        }
        public void exChangeGridMoveElement(int nSrcLine, int nSrcColumn, int nAimLine, int nAimColumn, Action<List<Element>, int, int> pMarkElementPath = null)
        {
            UnityEngine.Profiling.Profiler.BeginSample("exChangeGridMoveElement");
            Grid tSrcGrid = m_pChessBoard.getGrid(nSrcLine, nSrcColumn);
            Grid tAimGrid = m_pChessBoard.getGrid(nAimLine, nAimColumn);
            var arrElementMove1 = tSrcGrid.getExchangeMoveElement();
            var arrElementMove2 = tAimGrid.getExchangeMoveElement();
            if (pMarkElementPath != null)
            {
                pMarkElementPath(arrElementMove1, nAimLine, nAimColumn);
                pMarkElementPath(arrElementMove2, nSrcLine, nSrcColumn);
            }
            tSrcGrid.addElementArray(arrElementMove2);
            tAimGrid.addElementArray(arrElementMove1);
            UnityEngine.Profiling.Profiler.EndSample();
        }
        public void resetBlock()
        {
            Dictionary<int, List<Element>> m_mpMarkGridElement = new Dictionary<int, List<Element>>();
            void markGridElement()
            {
                var arrGrid = m_pChessBoard.getAllGrid();
                foreach (var tGrid in arrGrid)
                {
                    List<Element> arrElement = new List<Element>();
                    m_mpMarkGridElement.Add(tGrid.m_tGridCoord.Coord, arrElement);
                    foreach (var itElement in tGrid.m_sortedElement)
                    {
                        arrElement.Add(itElement.Value);
                    }
                }
            }

            void resetElementGrid()
            {
                foreach (var it in m_mpMarkGridElement)
                {
                    Grid tGrid = m_pChessBoard.getGrid(GridCoord.CoordToPos(it.Key));
                    tGrid.m_sortedElement.Clear();
                }
                foreach (var it in m_mpMarkGridElement)
                {
                    Grid tGrid = m_pChessBoard.getGrid(GridCoord.CoordToPos(it.Key));
                    foreach (var itElement in it.Value)
                    {
                        tGrid.addElement(itElement);
                    }
                }
            }
            // level 0 elementid : 
            Dictionary<Element, int> m_mpPath = new Dictionary<Element, int>();
            void addPath(List<Element> arrElement, int nAimLine, int nAimColumn)
            {
                // 获取基本元素id， 然后映射路径和对应交换的元素
                foreach (var tElement in arrElement)
                {
                    m_mpPath[tElement] = GridCoord.posToCoord(m_pChessBoard.Index, nAimLine, nAimColumn);
                }
            }

            void exChangeBlock(int nLine1, int nColumn1, int nLine2, int nColumn2)
            {
                exChangeGridMoveElement(nLine1, nColumn1, nLine2, nColumn2, addPath);
            }

            void moveElement()
            {
                resetElementGrid();
                foreach (var itPath in m_mpPath)
                {
                    try
                    {
                        GridCoord tSrcGridCoord = itPath.Key.m_tGrid.m_tGridCoord;
                        GridCoord tAimGridCoord = GridCoord.CoordToPos(itPath.Value);
                        if (tSrcGridCoord.Equals(tAimGridCoord) == true)
                        {
                            continue;
                        }
                        itPath.Key.m_tGrid.popElement(itPath.Key);
                        List<Element> arrElement = new List<Element>();
                        arrElement.Add(itPath.Key);
                        m_arrDropUnitId.Add(m_pChessBoard.m_tDropManager.createDropUnit(tSrcGridCoord.Line, tSrcGridCoord.Col, tAimGridCoord.Line, tAimGridCoord.Col, arrElement, DropUnit.MoveType.reset));
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            UnityEngine.Profiling.Profiler.BeginSample("resetBlock");

            // 1 找到合适的位置，先把可交换合成的元素放上
            PositionOk tPositionOk = m_tFiltratePosition.getm_tPositionOk();
            var arrGridIndex = m_tFiltratePosition.getm_arrPosition();
            markGridElement();
            var hshmpPositionType = tPositionOk.getm_hshmpEPositionTypePositionOk();
            string strMaxCountElementId;
            do
            {
                List<PositionOk.PositionInfo> arrPositionInfo = null;
                EPositionType eType = EPositionType.EPositionType_ThreeOne;
                if (hshmpPositionType.ContainsKey(eType) != false)
                {
                    arrPositionInfo = hshmpPositionType[eType];
                }
                else if (hshmpPositionType.ContainsKey(EPositionType.EPositionType_FourOne) != false)
                {
                    eType = EPositionType.EPositionType_FourOne;
                    arrPositionInfo = hshmpPositionType[eType];
                }
                if (arrPositionInfo == null)
                {
                    break;
                }
                List<Grid> arrExchangeGrid = new List<Grid>();
                // 随机出来一个位置， 用来摆放格子

                int nPositionRandomIndex = (int) Stage.m_tComputeRandom.random(0, arrPositionInfo.Count);
                PositionOk.PositionInfo pPositionInfo = arrPositionInfo[nPositionRandomIndex];
                int nPositionRandomExchangeIndex = (int) Stage.m_tComputeRandom.random(0, pPositionInfo.m_arrOne.Count);
                PositionOk.SomeOne pSomeOne = pPositionInfo.m_arrOne[nPositionRandomExchangeIndex];
                arrExchangeGrid.Add(pSomeOne.m_pOneGrid);
                foreach (var pGrid in pPositionInfo.m_arrGridOk)
                {
                    if (pGrid == pSomeOne.m_pOneGrid || pGrid == pSomeOne.m_pExchangeGrid)
                    {
                        continue;
                    }
                    arrExchangeGrid.Add(pGrid);
                }
                // 改为不随机出来一个 ， 直接用最大数量的那个
                var sortMpCountBlockCount = m_tFiltratePosition.getm_sortedmpBlockCountId();
                var it = sortMpCountBlockCount.Values.GetEnumerator();
                it.MoveNext();
                strMaxCountElementId = it.Current[0];

                var hshmpBlockIndexMap = m_tFiltratePosition.m_hshmpBlockElement;
                var hshBlockElement = hshmpBlockIndexMap[strMaxCountElementId];
                int nExchangeForIndex = 0;
                while (nExchangeForIndex < arrExchangeGrid.Count)
                {
                    var itBlock = hshBlockElement.GetEnumerator();
                    itBlock.MoveNext();
                    Element pBlock = itBlock.Current.Value;
                    Grid pGrid = arrExchangeGrid[nExchangeForIndex++];
                    int nRemoveCoord1 = GridCoord.posToCoord(m_pChessBoard.Index, pBlock.m_tGrid.m_tGridCoord.Line, pBlock.m_tGrid.m_tGridCoord.Col);
                    arrGridIndex.Remove(nRemoveCoord1);
                    int nRemoveCoord2 = GridCoord.posToCoord(m_pChessBoard.Index, pGrid.m_tGridCoord.Line, pGrid.m_tGridCoord.Col);
                    arrGridIndex.Remove(nRemoveCoord2);
                    exChangeBlock(pBlock.m_tGrid.m_tGridCoord.Line, pBlock.m_tGrid.m_tGridCoord.Col, pGrid.m_tGridCoord.Line, pGrid.m_tGridCoord.Col);
                    hshBlockElement.Remove(pBlock.ID);
                }
            }
            while (false);
            // 2 随机摆放其他的方块
            List<int> arrRandomGridIndex = arrGridIndex.cloneSelf();
            while (arrRandomGridIndex.Count > 1)
            {
                List<Element> arrBlock1 = new List<Element>();
                List<Element> arrBlock2 = new List<Element>();
                int nPos1 = (int) Stage.m_tComputeRandom.random(0, arrRandomGridIndex.Count);
                GridCoord v2i1 = GridCoord.CoordToPos(arrRandomGridIndex[nPos1]);
                arrRandomGridIndex.RemoveAt(nPos1);
                int nPos2 = (int) Stage.m_tComputeRandom.random(0, arrRandomGridIndex.Count);
                GridCoord v2i2 = GridCoord.CoordToPos(arrRandomGridIndex[nPos2]);
                arrRandomGridIndex.RemoveAt(nPos2);
                exChangeBlock(v2i1.Line, v2i1.Col, v2i2.Line, v2i2.Col);
            }
            // 排序 arrGridIndex 从已经摆放好的格子上
            List<int> arrNotOkGrid = arrGridIndex.cloneSelf();
            while (arrNotOkGrid.Count > 0)
            {
                int nIndexNum = arrNotOkGrid[0];
                arrNotOkGrid.RemoveAt(0);
                GridCoord v2i = GridCoord.CoordToPos(nIndexNum);
                if (m_pChessBoard.collectAndSynthesizeElementsCheck(v2i.Line, v2i.Col) == false)
                {
                    continue;
                }
                // 如果是可以合成的位置， 
                bool bIsRandomOk = false;
                for (int i = 0; i < arrGridIndex.Count && !bIsRandomOk; i++)
                {
                    int nForIndexNum = arrGridIndex[i];
                    if (nIndexNum == nForIndexNum)
                    {
                        continue;
                    }
                    var v2ifor = GridCoord.CoordToPos(nForIndexNum);
                    exChangeBlock(v2i.Line, v2i.Col, v2ifor.Line, v2ifor.Col);
                    if (!m_pChessBoard.collectAndSynthesizeElementsCheck(v2i.Line, v2i.Col) &&
                        !m_pChessBoard.collectAndSynthesizeElementsCheck(v2ifor.Line, v2ifor.Col))
                    {
                        // 如果交换完毕之后会能组成合成， 则再找下一个 ， 如果没有则ok
                        // ok 了 , 删掉这个
                        arrNotOkGrid.Remove(nForIndexNum);
                        bIsRandomOk = true;
                        break;
                    }
                    else
                    {
                        exChangeBlock(v2i.Line, v2i.Col, v2ifor.Line, v2ifor.Col);
                    }
                }
                if (!bIsRandomOk)
                {
                    arrNotOkGrid.Add(nIndexNum);
                }
            }
            moveElement();

            UnityEngine.Profiling.Profiler.EndSample();
        }
        public ECheckResetType checkResetBlock()
        {
            UnityEngine.Profiling.Profiler.BeginSample("checkResetBlock");
            jc.EventManager.Instance.NoticeEvent((int)jc.STAGEEVENTTYPE.ET_TIPS_Remove);
            // 1 是否有超能元素， 可以双击触发的
            {
                UnityEngine.Profiling.Profiler.BeginSample("check is Super element exist. ");
                List<Element> arrpBlockElement = new List<Element>();
                foreach (var strSuperElementId in m_arrpEliminateSuperAttriubte)
                {
                    m_pChessBoard.m_tStage.m_tENateCollecter.getBlockElement(strSuperElementId, arrpBlockElement);
                }
                if (arrpBlockElement.Count > 0)
                {
                    // 检测是超能元素， 可双击
                    foreach (var pBlock in arrpBlockElement)
                    {
                        if (pBlock == null)
                        {
                            continue;
                        }
                        Grid pGrid = pBlock.m_tGrid;
                        if (pGrid == null)
                        {
                            continue;
                        }
                        if (pGrid.m_tChessBoard != m_pChessBoard)
                        {
                            continue;
                        }
                        // 如果有屏蔽基层元素组合， 就跳过
                        if (ENateMatchRule.sm_tMatchRule_ForbiddenOperator.Grid_match(pGrid) == false)
                        {
                            continue;
                        }
                        var mpArg = new Dictionary<string, object>();
                        mpArg.Add("special", pBlock);
                        mpArg.Add("type", "super");
                        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_TIPS_Add, mpArg);
                        UnityEngine.Profiling.Profiler.EndSample();
                        UnityEngine.Profiling.Profiler.EndSample();
                        return ECheckResetType.super;
                    }
                }
                UnityEngine.Profiling.Profiler.EndSample();
            }
            // 2 先检测基础元素的数量
            {
                m_tFiltratePosition.findComposePosition();
                Dictionary<string, Dictionary<int, Element>> hshmpBlockNum = m_tFiltratePosition.m_hshmpBlockElement;
                if (hshmpBlockNum.Count <= 0)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return ECheckResetType.over; // 说明已经没有元素可以重置
                }
                // 3 看可移动的基础元素是否可以足够数量来， 或者是否元素能变为超能元素 
                int nMaxBlockNum = m_tFiltratePosition.getm_nMaxNum();
                if (nMaxBlockNum < 3)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return ECheckResetType.randomSuper_BlockNum;
                }

                PositionOk tPositionOk = m_tFiltratePosition.getm_tPositionOk();
                var arrPositionOk = tPositionOk.getm_hshmpEPositionTypePositionOk();
                if (arrPositionOk.Count <= 0)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return ECheckResetType.randomSuper_Position; // 说明已经没有位置了
                }
                Func<Grid, Grid, bool> pCheckFunc = (Grid pSomeOne, Grid pCheckGrid) =>
                {

                    UnityEngine.Profiling.Profiler.BeginSample("pCheckFunc");
                    bool bIsOk = false;
                    do
                    {
                        if (pSomeOne == null || pCheckGrid == null)
                        {
                            break;
                        }
                        // 如果有屏蔽基层元素组合， 就跳过
                        if (ENateMatchRule.sm_tMatchRule_ForbiddenOperator.Grid_match(pSomeOne) == false ||
                            ENateMatchRule.sm_tMatchRule_ForbiddenOperator.Grid_match(pCheckGrid) == false)
                        {
                            break;
                        }
                        Element pBlockElement = pSomeOne.getElementWithElementAttribute(ElementAttribute.Attribute.moveType, 1, true);
                        Element pCheckBlockElement = pCheckGrid.getElementWithElementAttribute(ElementAttribute.Attribute.moveType, 1, true);
                        if (pBlockElement == null || pCheckBlockElement == null)
                        {
                            break;
                        }
                        //LogPrintf("pCheckGrid : %d, %d, pSomeOne : %d, %d", 3, pCheckGrid->getm_nLine(), pCheckGrid->getm_nColumn(), pSomeOne->getm_nLine(), pSomeOne->getm_nColumn());
                        pCheckGrid.addElement(pBlockElement);
                        pSomeOne.addElement(pCheckBlockElement);
                        do
                        {
                            /*
                            if (m_pChessBoard.collectAndSynthesizeElementsCheck(pSomeOne.m_tGridCoord.Line, pSomeOne.m_tGridCoord.Col) == true ||
                                m_pChessBoard.collectAndSynthesizeElementsCheck(pCheckGrid.m_tGridCoord.Line, pCheckGrid.m_tGridCoord.Col) == true)
                            {
                                bIsOk = true;
                                break;
                            }
                            */
                            var tComposeSomeOne = m_pChessBoard.collectAndSynthesizeElementsCheckCompose(pSomeOne.m_tGridCoord.Line, pSomeOne.m_tGridCoord.Col);
                            var tComposeCheckGrid = m_pChessBoard.collectAndSynthesizeElementsCheckCompose(pCheckGrid.m_tGridCoord.Line, pCheckGrid.m_tGridCoord.Col);
                            Action<ENateCompose, Element, Direction> pAdTips = (ENateCompose tENateCompose, Element tCheckBlock, Direction eMoveDirection) =>
                            {
                                var mpArg = new Dictionary<string, object>();
                                mpArg.Add("special", tCheckBlock);
                                mpArg.Add("type", "normal");
                                mpArg.Add("dir", eMoveDirection);
                                var arrTipsElement = new List<Element>();
                                var tCheckElementValue = new ElementValue<string>(tCheckBlock.ElementId);
                                foreach (var tTipsGrid in tENateCompose.m_arrComposeGrid)
                                {
                                    var tTipsElement = tTipsGrid.getElementWithElementAttribute(ElementAttribute.Attribute.id, tCheckElementValue);
                                    if (tTipsElement != null)
                                    {
                                        arrTipsElement.Add(tTipsElement);
                                    }
                                }
                                mpArg.Add("tipsElement", arrTipsElement);
                                jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_TIPS_Add, mpArg);
                            };
                            if (tComposeSomeOne != null && tComposeSomeOne.m_arrComposeGrid.Count > 0)
                            {
                                pAdTips(tComposeSomeOne, pCheckBlockElement, Util.getDirectionWithLineCol(pSomeOne.m_tGridCoord.Line, pSomeOne.m_tGridCoord.Col, pCheckGrid.m_tGridCoord.Line, pCheckGrid.m_tGridCoord.Col));
                                bIsOk = true;
                                break;
                            }
                            if (tComposeCheckGrid != null && tComposeCheckGrid.m_arrComposeGrid.Count > 0)
                            {
                                pAdTips(tComposeCheckGrid, pBlockElement, Util.getDirectionWithLineCol(pCheckGrid.m_tGridCoord.Line, pCheckGrid.m_tGridCoord.Col, pSomeOne.m_tGridCoord.Line, pSomeOne.m_tGridCoord.Col));
                                bIsOk = true;
                                break;
                            }
                        }
                        while (false);
                        pBlockElement = pSomeOne.getElementWithElementAttribute(ElementAttribute.Attribute.moveType, 1, true);
                        pCheckBlockElement = pCheckGrid.getElementWithElementAttribute(ElementAttribute.Attribute.moveType, 1, true);
                        //LogPrintf("pCheckGrid : %d, %d, pSomeOne : %d, %d", 3, pCheckGrid->getm_nLine(), pCheckGrid->getm_nColumn(), pSomeOne->getm_nLine(), pSomeOne->getm_nColumn());
                        pCheckGrid.addElement(pBlockElement);
                        pSomeOne.addElement(pCheckBlockElement);
                    }
                    while (false);
                    UnityEngine.Profiling.Profiler.EndSample();
                    return bIsOk;
                };
                foreach (var pPositionType in arrPositionOk)
                {
                    foreach (var pPositionInfo in pPositionType.Value)
                    {
                        foreach (var pSomeOne in pPositionInfo.m_arrOne)
                        {
                            if (pCheckFunc(pSomeOne.m_pOneGrid, pSomeOne.m_pExchangeGrid))
                            {
                                UnityEngine.Profiling.Profiler.EndSample();
                                return ECheckResetType.someOneOk;
                            }
                        }
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return ECheckResetType.noneStep;
        }
        private void changeElement()
        {
            var sortMpCountBlockCount = m_tFiltratePosition.getm_sortedmpBlockCountId();
            if (sortMpCountBlockCount.Count > 0)
            {
                var itMax = sortMpCountBlockCount.GetEnumerator();
                itMax.MoveNext();
                string strMaxCountElementId = itMax.Current.Value[0];
                var itMaxKey = sortMpCountBlockCount.GetEnumerator();
                itMaxKey.MoveNext();
                int nNum = itMaxKey.Current.Key;
                var hshmpPositionType = m_tFiltratePosition.getm_tPositionOk().getm_hshmpEPositionTypePositionOk();
                if (hshmpPositionType.ContainsKey(EPositionType.EPositionType_ThreeOne) == true)
                {
                    nNum = 3 - nNum;
                }
                else if (hshmpPositionType.ContainsKey(EPositionType.EPositionType_FourOne) == true)
                {
                    nNum = 4 - nNum;
                }
                var hshBlockMap = m_tFiltratePosition.m_hshmpBlockElement;

                foreach (var it in hshBlockMap)
                {
                    if (nNum <= 0)
                    {
                        break;
                    }
                    if (it.Key == strMaxCountElementId)
                    {
                        continue;
                    }
                    foreach (var itBlock in it.Value)
                    {
                        itBlock.Value.skillChangeElementId(strMaxCountElementId);
                        if (--nNum <= 0)
                        {
                            break;
                        }
                    }
                }
            }
        }

        void begin()
        {
            ECheckResetType eCheckType = checkResetBlock();
            switch (eCheckType)
            {
                case ECheckResetType.randomSuper_BlockNum:
                    {
                        changeElement();
                    }
                    break;
                default:
                    break;
            }
            reset();
        }

        public IEnumerator waitResetEnd(Action pCallback)
        {
            bool bIsOk = false;
            while (bIsOk == false)
            {
                bIsOk = true;
                foreach (int nDropUnitId in m_arrDropUnitId)
                {
                    if (m_pChessBoard.m_tDropManager.doesTheDropEnd(nDropUnitId) == false)
                    {
                        bIsOk = false;
                    }
                }
                yield return null;
            }
            m_arrDropUnitId.Clear();
            pCallback();
        }

        private bool randomSuperElement()
        {
            UnityEngine.Profiling.Profiler.BeginSample("randomSuperElement");
            Dictionary<string, Dictionary<int, Element>> hshmpBlockCount = m_tFiltratePosition.m_hshmpBlockElement;
            List<Element> arrpBlockElement = new List<Element>();
            foreach (var it in hshmpBlockCount)
            {
                foreach (var pBlockIt in it.Value)
                {
                    Grid pGrid = m_pChessBoard.getGrid(pBlockIt.Value.m_tGrid.m_tGridCoord.Line, pBlockIt.Value.m_tGrid.m_tGridCoord.Col);
                    if (ENateMatchRule.sm_tMatchRule_ForbiddenOperator.Grid_match(pGrid) == false)
                    {
                        continue;
                    }
                    arrpBlockElement.Add(pBlockIt.Value);
                }
            }
            if (arrpBlockElement.Count > 0)
            {
                int nRandomSub = (int) Stage.m_tComputeRandom.random(0, arrpBlockElement.Count);
                Element pChangeBlock = arrpBlockElement[nRandomSub];
                int nEliminateSub = (int) Stage.m_tComputeRandom.random(0, m_arrpEliminateSuperAttriubte.Count);
                string strSuperElementId = m_arrpEliminateSuperAttriubte[nEliminateSub];
                pChangeBlock.skillChangeElementId(strSuperElementId);
                UnityEngine.Profiling.Profiler.EndSample();
                return true;
            }
            else
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }
        }

    }
}