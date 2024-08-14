/*
 * @Description: PositionOk
 * @Author: yangzijian
 * @Date: 2020-03-12 11:32:43
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-15 15:13:26
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace ENate
{
    public enum EPositionType
    {
        EPositionType_none, // 没有找到
        EPositionType_ThreeOne, // 3 + 1 位置
        EPositionType_FourOne // 4 + 1 位置
    }

    public class PositionOk
    {
        Dictionary<EPositionType, List<PositionInfo>> m_hshmpEPositionTypePositionOk;
        ChessBoard m_pChessBoard;
        FiltratePosition m_tFiltratePosition;
        List<PositionInfo> arr = new List<PositionInfo>();

        public PositionOk(ChessBoard pChessBoard, FiltratePosition tFiltratePosition)
        {
            m_pChessBoard = pChessBoard;
            m_tFiltratePosition = tFiltratePosition;
            m_hshmpEPositionTypePositionOk = new Dictionary<EPositionType, List<PositionInfo>>();
        }
        ~PositionOk()
        {

        }
        public void clear()
        {

        }

        public class SomeOne
        {
            public Grid m_pOneGrid;
            public Grid m_pExchangeGrid; // 交换的格子可以是空的
            public SomeOne(Grid pOneGrid, Grid pExchangeGrid)
            {
                m_pOneGrid = pOneGrid;
                m_pExchangeGrid = pExchangeGrid;
            }
        }

        public class PositionInfo
        {
            public EPositionType m_eEPositionType;
            Grid m_pGrid;
            public List<Grid> m_arrGridOk = new List<Grid>();
            public List<SomeOne> m_arrOne = new List<SomeOne>();
            public PositionInfo(EPositionType eEPositionType, Grid pGrid)
            {
                m_eEPositionType = eEPositionType;
                m_pGrid = pGrid;
                m_arrGridOk.Add(pGrid);
            }
            ~PositionInfo()
            {
                m_arrOne.Clear();
            }
            public void addSomeOne(Grid pOneGrid, Grid pExchangeGrid)
            {
                m_arrOne.Add(new SomeOne(pOneGrid, pExchangeGrid));
            }
        }
        public Dictionary<EPositionType, List<PositionInfo>> getm_hshmpEPositionTypePositionOk()
        {
            return m_hshmpEPositionTypePositionOk;
        }

        public void find(Grid pGrid)
        {
            UnityEngine.Profiling.Profiler.BeginSample("find");
            findThreeOne(pGrid);
            // findFourOne(pGrid);
            UnityEngine.Profiling.Profiler.EndSample();
        }
        void findThreeOne(Grid pGrid)
        {

            UnityEngine.Profiling.Profiler.BeginSample("findThreeOne");
            Action<PositionInfo, int, int, Grid> pSomeOneFunc = (PositionInfo pPositionInfo, int nLine, int nColumn, Grid pExchangeGrid) =>
            {
                UnityEngine.Profiling.Profiler.BeginSample("pSomeOneFunc");
                Grid pOneGrid = m_tFiltratePosition.getOkGrid(nLine, nColumn);
                if (pOneGrid == null)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return;
                }
                if (m_pChessBoard.m_tDropManager.exChangeMoveCheck(nLine, nColumn, pExchangeGrid.m_tGridCoord.Line, pExchangeGrid.m_tGridCoord.Col) == false)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return;
                }
                pPositionInfo.addSomeOne(pOneGrid, pExchangeGrid);
                UnityEngine.Profiling.Profiler.EndSample();
            };
            do
            {
                // 右 
                Grid pGridRight1 = m_tFiltratePosition.getOkGrid(pGrid.m_tGridCoord.Line, pGrid.m_tGridCoord.Col + 1);
                if (pGridRight1 == null)
                {
                    break;
                }
                Grid pGridRight2 = m_tFiltratePosition.getOkGrid(pGrid.m_tGridCoord.Line, pGrid.m_tGridCoord.Col + 2);
                if (pGridRight2 == null)
                {
                    break;
                }
                PositionInfo pPositionInfo = new PositionInfo(EPositionType.EPositionType_ThreeOne, pGrid);

                // 三连完成 ， 寻找特殊移动方块
                if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridRight1) == true)
                {
                    if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridRight2) == true)
                    {
                        pSomeOneFunc(pPositionInfo, pGrid.m_tGridCoord.Line, pGrid.m_tGridCoord.Col - 1, pGrid);
                        pSomeOneFunc(pPositionInfo, pGrid.m_tGridCoord.Line + 1, pGrid.m_tGridCoord.Col, pGrid);
                        pSomeOneFunc(pPositionInfo, pGrid.m_tGridCoord.Line - 1, pGrid.m_tGridCoord.Col, pGrid);
                    }
                    if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGrid) == true)
                    {
                        pSomeOneFunc(pPositionInfo, pGridRight2.m_tGridCoord.Line + 1, pGridRight2.m_tGridCoord.Col, pGridRight2);
                        pSomeOneFunc(pPositionInfo, pGridRight2.m_tGridCoord.Line - 1, pGridRight2.m_tGridCoord.Col, pGridRight2);
                        pSomeOneFunc(pPositionInfo, pGridRight2.m_tGridCoord.Line, pGridRight2.m_tGridCoord.Col + 1, pGridRight2);
                    }
                }
                if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGrid) && ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridRight2))
                {
                    pSomeOneFunc(pPositionInfo, pGridRight1.m_tGridCoord.Line + 1, pGridRight1.m_tGridCoord.Col, pGridRight1);
                    pSomeOneFunc(pPositionInfo, pGridRight1.m_tGridCoord.Line - 1, pGridRight1.m_tGridCoord.Col, pGridRight1);
                }

                if (pPositionInfo.m_arrOne.Count > 0)
                {
                    pPositionInfo.m_arrGridOk.Add(pGridRight1);
                    pPositionInfo.m_arrGridOk.Add(pGridRight2);
                    addPosition(pPositionInfo);
                    UnityEngine.Profiling.Profiler.EndSample();
                    return;
                }
            }
            while (false);
            do
            {
                Grid pGridDown1 = m_tFiltratePosition.getOkGrid(pGrid.m_tGridCoord.Line + 1, pGrid.m_tGridCoord.Col);
                if (pGridDown1 == null)
                {
                    break;
                }
                Grid pGridDown2 = m_tFiltratePosition.getOkGrid(pGrid.m_tGridCoord.Line + 2, pGrid.m_tGridCoord.Col);
                if (pGridDown2 == null)
                {
                    break;
                }
                PositionInfo pPositionInfo = new PositionInfo(EPositionType.EPositionType_ThreeOne, pGrid);
                // 三连完成 ， 寻找特殊移动方块
                if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridDown1))
                {
                    if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridDown2))
                    {
                        pSomeOneFunc(pPositionInfo, pGrid.m_tGridCoord.Line - 1, pGrid.m_tGridCoord.Col, pGrid);
                        pSomeOneFunc(pPositionInfo, pGrid.m_tGridCoord.Line, pGrid.m_tGridCoord.Col + 1, pGrid);
                        pSomeOneFunc(pPositionInfo, pGrid.m_tGridCoord.Line, pGrid.m_tGridCoord.Col - 1, pGrid);
                    }
                    if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGrid))
                    {
                        pSomeOneFunc(pPositionInfo, pGridDown2.m_tGridCoord.Line, pGridDown2.m_tGridCoord.Col + 1, pGridDown2);
                        pSomeOneFunc(pPositionInfo, pGridDown2.m_tGridCoord.Line, pGridDown2.m_tGridCoord.Col - 1, pGridDown2);
                        pSomeOneFunc(pPositionInfo, pGridDown2.m_tGridCoord.Line + 1, pGridDown2.m_tGridCoord.Col, pGridDown2);
                    }
                }
                if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGrid) && ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridDown2))
                {
                    pSomeOneFunc(pPositionInfo, pGridDown1.m_tGridCoord.Line, pGridDown1.m_tGridCoord.Col + 1, pGridDown1);
                    pSomeOneFunc(pPositionInfo, pGridDown1.m_tGridCoord.Line, pGridDown1.m_tGridCoord.Col - 1, pGridDown1);
                }

                if (pPositionInfo.m_arrOne.Count > 0)
                {
                    pPositionInfo.m_arrGridOk.Add(pGridDown1);
                    pPositionInfo.m_arrGridOk.Add(pGridDown2);
                    addPosition(pPositionInfo);
                    UnityEngine.Profiling.Profiler.EndSample();
                    return;
                }
            }
            while (false);
            UnityEngine.Profiling.Profiler.EndSample();
        }
        void findFourOne(Grid pGrid)
        {
            UnityEngine.Profiling.Profiler.BeginSample("findFourOne");
            Action<PositionInfo, int, int, Grid> pSomeOneFunc = (PositionInfo pPositionInfo, int nLine, int nColumn, Grid pExchangeGrid) =>
            {
                UnityEngine.Profiling.Profiler.BeginSample("pSomeOneFunc");
                Grid pOneGrid = m_tFiltratePosition.getOkGrid(nLine, nColumn);
                if (pOneGrid == null)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return;
                }
                if (!m_pChessBoard.m_tDropManager.exChangeMoveCheck(pOneGrid.m_tGridCoord.Line, pOneGrid.m_tGridCoord.Col, pExchangeGrid.m_tGridCoord.Line, pExchangeGrid.m_tGridCoord.Col))
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return;
                }
                pPositionInfo.addSomeOne(pOneGrid, pExchangeGrid);
                UnityEngine.Profiling.Profiler.EndSample();
            };
            // 右 
            do
            {
                Grid pGridRight = m_tFiltratePosition.getOkGrid(pGrid.m_tGridCoord.Line, pGrid.m_tGridCoord.Col + 1);
                if (pGridRight == null)
                {
                    break;
                }
                Grid pGridDown = m_tFiltratePosition.getOkGrid(pGrid.m_tGridCoord.Line + 1, pGrid.m_tGridCoord.Col);
                if (pGridDown == null)
                {
                    break;
                }
                Grid pGridRightDown = m_tFiltratePosition.getOkGrid(pGrid.m_tGridCoord.Line + 1, pGrid.m_tGridCoord.Col + 1);
                if (pGridRightDown == null)
                {
                    break;
                }
                PositionInfo pPositionInfo = new PositionInfo(EPositionType.EPositionType_ThreeOne, pGrid);

                // 方块完成 ， 寻找特殊移动方块
                if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridRight))
                {
                    if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridDown) && ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridRightDown))
                    {
                        pSomeOneFunc(pPositionInfo, pGrid.m_tGridCoord.Line - 1, pGrid.m_tGridCoord.Col, pGrid);
                        pSomeOneFunc(pPositionInfo, pGrid.m_tGridCoord.Line, pGrid.m_tGridCoord.Col - 1, pGrid);
                    }
                    if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGrid) && ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridRightDown))
                    {
                        pSomeOneFunc(pPositionInfo, pGridDown.m_tGridCoord.Line, pGridDown.m_tGridCoord.Col - 1, pGridDown);
                        pSomeOneFunc(pPositionInfo, pGridDown.m_tGridCoord.Line + 1, pGridDown.m_tGridCoord.Col, pGridDown);
                    }
                    if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGrid) && ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridDown))
                    {
                        pSomeOneFunc(pPositionInfo, pGridRightDown.m_tGridCoord.Line + 1, pGridRightDown.m_tGridCoord.Col, pGridRightDown);
                        pSomeOneFunc(pPositionInfo, pGridRightDown.m_tGridCoord.Line, pGridRightDown.m_tGridCoord.Col + 1, pGridRightDown);
                    }
                }
                if (ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGrid) && ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridDown) && ENateMatchRule.sm_tMatchRule_Exchange.Grid_match(pGridRightDown))
                {
                    pSomeOneFunc(pPositionInfo, pGridRight.m_tGridCoord.Line - 1, pGridRight.m_tGridCoord.Col, pGridRight);
                    pSomeOneFunc(pPositionInfo, pGridRight.m_tGridCoord.Line, pGridRight.m_tGridCoord.Col + 1, pGridRight);
                }
                if (pPositionInfo.m_arrOne.Count > 0)
                {
                    pPositionInfo.m_arrGridOk.Add(pGridRight);
                    pPositionInfo.m_arrGridOk.Add(pGridDown);
                    pPositionInfo.m_arrGridOk.Add(pGridRightDown);
                    addPosition(pPositionInfo);
                }
            } while (false);
            UnityEngine.Profiling.Profiler.EndSample();
        }
        public void addPosition(PositionInfo pPositionInfo)
        {
            UnityEngine.Profiling.Profiler.BeginSample("addPosition");
            if (m_hshmpEPositionTypePositionOk.ContainsKey(pPositionInfo.m_eEPositionType) == false)
            {
                arr.Clear();
                arr.Add(pPositionInfo);
                m_hshmpEPositionTypePositionOk.Add(pPositionInfo.m_eEPositionType, arr);
            }
            m_hshmpEPositionTypePositionOk[pPositionInfo.m_eEPositionType].Add(pPositionInfo);
            UnityEngine.Profiling.Profiler.EndSample();
        }

    }
}