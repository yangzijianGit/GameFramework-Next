/*
 * @Description: FiltratePosition
 * @Author: yangzijian
 * @Date: 2020-03-12 11:26:15
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-15 15:11:11
 */

using System.Collections;
using System.Collections.Generic;

namespace ENate
{
    public class FiltratePosition
    {
        public class BlockCountCompare : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return y.CompareTo(x);
            }

        }
        public ChessBoard m_pChessBoard;
        public List<int> m_arrPosition; // 要交换的位置集合  GridIndex
        public SortedDictionary<int, List<string>> m_sortedmpBlockCountId;
        public int m_nMaxNum = -1;
        public Dictionary<string, Dictionary<int, Element>> m_hshmpBlockElement; // 元素blockId
        public PositionOk m_tPositionOk;

        Grid[][] m_hshmpGrid;
        int m_nHeight;
        int m_nWidth;

        void addOkGrid(Grid pGrid)
        {
            m_hshmpGrid[pGrid.m_tGridCoord.Line][pGrid.m_tGridCoord.Col] = pGrid;
        }

        public Grid getOkGrid(int nLine, int nCol)
        {
            try
            {
                return m_hshmpGrid[nLine][nCol];
            }
            catch { }
            return null;
        }
        public Grid getOkGrid(int nGridCoord)
        {
            GridCoord tGridCoord = GridCoord.CoordToPos(nGridCoord);
            return getOkGrid(tGridCoord.Line, tGridCoord.Col);
        }
        public void setSize(int nWidth, int nHeight)
        {
            m_nWidth = nWidth;
            m_nHeight = nHeight;
            m_hshmpGrid = new Grid[nHeight][];
            for (int i = 0; i < nHeight; i++)
            {
                m_hshmpGrid[i] = new Grid[nWidth];
            }
        }

        public FiltratePosition(ChessBoard pChessBoard)
        {
            m_pChessBoard = pChessBoard;
            m_arrPosition = new List<int>();
            m_tPositionOk = new PositionOk(pChessBoard, this);
            m_sortedmpBlockCountId = new SortedDictionary<int, List<string>>(new BlockCountCompare());
            m_hshmpBlockElement = new Dictionary<string, Dictionary<int, Element>>(); // 元素blockId
        }

        // 找到对应的重置位置
        public void findComposePosition()
        {
            UnityEngine.Profiling.Profiler.BeginSample("findComposePosition");
            clear();
            for (int nL = 0; nL < m_pChessBoard.m_nWidth; ++nL)
            {
                for (int nC = 0; nC < m_pChessBoard.m_nHeight; ++nC)
                {
                    Grid pGrid = m_pChessBoard.getGrid(nL, nC);
                    if (pGrid == null)
                    {
                        continue;
                    }
                    if (ENateMatchRule.sm_tExchangeMatchRule.Grid_match(pGrid) == false)
                    {
                        continue;
                    }
                    addOkGrid(pGrid);
                    m_arrPosition.Add(pGrid.m_tGridCoord.Coord);
                    Element pBlockElement = pGrid.getElementWithElementAttribute(ElementAttribute.Attribute.involvedCompose, 1);
                    if (pBlockElement == null)
                    {
                        continue;
                    }
                    countBlockElement(pBlockElement);
                }
            }
            if (m_arrPosition.Count < 3)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            findOkPosition();
            findComposeElementNumOk();
            UnityEngine.Profiling.Profiler.EndSample();
        }
        public void findComposeElementNumOk()
        {
            UnityEngine.Profiling.Profiler.BeginSample("findComposeElementNumOk");
            foreach (var it in m_hshmpBlockElement)
            {
                if (m_sortedmpBlockCountId.ContainsKey(it.Value.Count) == false)
                {
                    m_sortedmpBlockCountId.Add(it.Value.Count, new List<string>());
                }
                m_sortedmpBlockCountId[it.Value.Count].Add(it.Key);
                m_nMaxNum = m_nMaxNum < it.Value.Count ? it.Value.Count : m_nMaxNum;
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
        public void clear()
        {
            UnityEngine.Profiling.Profiler.BeginSample("filtratePosition.clear");
            for (int nLine = 0; nLine < m_nHeight; nLine++)
            {
                for (int nCol = 0; nCol < m_nWidth; nCol++)
                {
                    m_hshmpGrid[nLine][nCol] = null;
                }
            }
            m_arrPosition.Clear();
            m_hshmpBlockElement.Clear();
            m_sortedmpBlockCountId.Clear();
            m_tPositionOk.clear();
            m_nMaxNum = -1;
            UnityEngine.Profiling.Profiler.EndSample();
        }

        // 找到 位置即可 
        public void findOkPosition() // 找到对应符合对应 3+1 4+1这种位置
        {
            UnityEngine.Profiling.Profiler.BeginSample("findOkPosition");
            for (int nLine = 0; nLine < m_nHeight; nLine++)
            {
                for (int nCol = 0; nCol < m_nWidth; nCol++)
                {
                    Grid pGrid = getOkGrid(nLine, nCol);
                    if (pGrid == null)
                    {
                        continue;
                    }
                    m_tPositionOk.find(pGrid);
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
        void countBlockElement(Element pBlock)
        {
            if (m_hshmpBlockElement.ContainsKey(pBlock.ElementId) == true)
            {
                m_hshmpBlockElement[pBlock.ElementId].Add(pBlock.ID, pBlock);
            }
            else
            {
                Dictionary<int, Element> hshmp = new Dictionary<int, Element>();
                hshmp.Add(pBlock.ID, pBlock);
                m_hshmpBlockElement.Add(pBlock.ElementId, hshmp);
            }
        }
        public PositionOk getm_tPositionOk()
        {
            return m_tPositionOk;
        }
        public SortedDictionary<int, List<string>> getm_sortedmpBlockCountId()
        {
            return m_sortedmpBlockCountId;
        }
        public List<int> getm_arrPosition()
        {
            return m_arrPosition;
        }
        public int getm_nMaxNum()
        {
            return m_nMaxNum;
        }
    }

}