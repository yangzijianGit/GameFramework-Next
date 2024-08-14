/*
 * @Description: collect element count
 * @Author: yangzijian
 * @Date: 2020-01-08 20:31:35
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-14 16:10:24
 */

using System;
using System.Collections.Generic;

namespace ENate
{
    public class ENateCollecter : ENateDispose<ENateCollecter>
    {
        Dictionary<int, Element> m_mpElement;

        Dictionary<string, Dictionary<int, Element>> m_mpHypotaxisIdElementCount;
        Dictionary<string, Dictionary<int, Element>> m_mpElementCount;
        Dictionary<int, Dictionary<string, int>> m_mpCollect;
        jc.EventManager.EventObj m_tEventObj;
        Stage m_tStage;
        public Dictionary<string, Dictionary<int, Element>> mpElementCount
        {
            get
            {
                return m_mpElementCount;
            }
        }
        public ENateCollecter(Stage tStage)
        {
            m_tStage = tStage;
            m_mpElement = new Dictionary<int, Element>(); // <elementId : element >
            m_mpHypotaxisIdElementCount = new Dictionary<string, Dictionary<int, Element>>(); // <elementStringId : <elementId : element >>
            m_mpElementCount = new Dictionary<string, Dictionary<int, Element>>(); // <elementStringId : <elementId : element >>
            m_mpCollect = new Dictionary<int, Dictionary<string, int>>(); // <chessboardIndex : <elementStringId : count>> ;-1 is all chessBoard Count
            m_tEventObj = new jc.EventManager.EventObj();
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_CREATE, event_createElement);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE, event_eliminateElement);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ElementChange, event_changeElement);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_SkillElementChange, event_changeElement);

            addDisposeCallback(clear);
        }
        public void clear()
        {
            m_tEventObj.clear();
            m_tEventObj = null;
        }
        void event_createElement(object obj)
        {
            Element tElement = obj as Element;
            addElement(tElement);

        }
        void event_eliminateElement(object obj)
        {
            Element tElement = obj as Element;
            removeElement(tElement);
        }

        void event_changeElement(object obj)
        {
            KeyValuePair<string, Element> tKeyPair = (KeyValuePair<string, Element>) obj;
            if (string.IsNullOrEmpty(tKeyPair.Key) == false)
            {
                delElementCount(tKeyPair.Key, tKeyPair.Value.ID);
                string strHypotaxis = Config.ElementConfig.getConfig_element_hypotaxisId(tKeyPair.Key);
                if (strHypotaxis != tKeyPair.Value.getHypotaxisId())
                {
                    collectElement(tKeyPair.Value.m_tGrid.m_tChessBoard.Index, strHypotaxis);
                }
            }
            addElementCount(tKeyPair.Value);
        }
        //////////////////////////////////////////
        public void getBlockElement(string strElementId, List<Element> arrElement)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getBlockElement");
            foreach (var itElement in m_mpElement)
            {
                if (itElement.Value.IsLock == true)
                {
                    continue;
                }
                if (itElement.Value.ElementId == strElementId)
                {
                    arrElement.Add(itElement.Value);
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
        public Element getBlockElement(int nId)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getBlockElement id");
            if (m_mpElement.ContainsKey(nId))
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return m_mpElement[nId];
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return null;
        }

        public int getHypotaxisIdBlockCount(string strHypotaxisId, int nChessBoardIndex)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getBlockCount");
            int nReCount = 0;
            if (m_mpHypotaxisIdElementCount.ContainsKey(strHypotaxisId) == true)
            {
                if (nChessBoardIndex == -1)
                {
                    nReCount = m_mpHypotaxisIdElementCount[strHypotaxisId].Count;
                }
                else
                {
                    foreach (var itElement in m_mpHypotaxisIdElementCount[strHypotaxisId])
                    {
                        if (itElement.Value == null)
                        {
                            continue;
                        }
                        int nElementChessBoardIndex = m_tStage.CurrentChessBoardIndex;
                        if (itElement.Value.m_tGrid != null)
                        {
                            nElementChessBoardIndex = itElement.Value.m_tGrid.m_tChessBoard.Index;
                        }
                        if (nElementChessBoardIndex == nChessBoardIndex)
                        {
                            ++nReCount;
                        }
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return nReCount;
        }
        public int getBlockCount(string strElementId, int nChessBoardIndex)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getBlockCount");
            int nReCount = 0;
            if (m_mpElementCount.ContainsKey(strElementId) == true)
            {
                if (nChessBoardIndex == -1)
                {
                    nReCount = m_mpElementCount[strElementId].Count;
                }
                else
                {
                    foreach (var itElement in m_mpElementCount[strElementId])
                    {
                        if (itElement.Value == null)
                        {
                            continue;
                        }
                        int nElementChessBoardIndex = m_tStage.CurrentChessBoardIndex;
                        if (itElement.Value.m_tGrid != null)
                        {
                            nElementChessBoardIndex = itElement.Value.m_tGrid.m_tChessBoard.Index;
                        }
                        if (nElementChessBoardIndex == nChessBoardIndex)
                        {
                            ++nReCount;
                        }
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return nReCount;
        }

        public int getBlockNormalCount(string strElementId, int nChessBoardIndex)
        {
            var arrElement = getBlockElement(strElementId, nChessBoardIndex);
            return arrElement.Count;
        }
        public List<Element> getHypotaxisIdBlockElement(string strHypotaxisId, int nChessBoardIndex)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getBlockElement");
            List<Element> arrElement = new List<Element>();
            if (m_mpHypotaxisIdElementCount.ContainsKey(strHypotaxisId) == true)
            {
                if (nChessBoardIndex == -1)
                {
                    arrElement.AddRange(m_mpHypotaxisIdElementCount[strHypotaxisId].Values);
                }
                else
                {
                    foreach (var itElement in m_mpHypotaxisIdElementCount[strHypotaxisId])
                    {
                        if (itElement.Value == null)
                        {
                            continue;
                        }
                        int nElementChessBoardIndex = m_tStage.CurrentChessBoardIndex;
                        if (itElement.Value.m_tGrid != null)
                        {
                            nElementChessBoardIndex = itElement.Value.m_tGrid.m_tChessBoard.Index;
                        }
                        if (nElementChessBoardIndex == nChessBoardIndex)
                        {
                            arrElement.Add(itElement.Value);
                        }
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return arrElement;
        }
        public List<Element> getBlockElement(string strElementId, int nChessBoardIndex)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getBlockElement");
            List<Element> arrElement = new List<Element>();
            if (m_mpElementCount.ContainsKey(strElementId) == true)
            {
                if (nChessBoardIndex == -1)
                {
                    arrElement.AddRange(m_mpElementCount[strElementId].Values);
                }
                else
                {
                    foreach (var itElement in m_mpElementCount[strElementId])
                    {
                        if (itElement.Value == null)
                        {
                            continue;
                        }
                        int nElementChessBoardIndex = m_tStage.CurrentChessBoardIndex;
                        if (itElement.Value.m_tGrid != null)
                        {
                            nElementChessBoardIndex = itElement.Value.m_tGrid.m_tChessBoard.Index;
                        }
                        if (nElementChessBoardIndex == nChessBoardIndex)
                        {
                            arrElement.Add(itElement.Value);
                        }
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return arrElement;
        }

        //////////////////////////////////////////////////////

        public int getCollectNum(int nChessBoardIndex, string strHypotaxisId)
        {
            try
            {
                return m_mpCollect[nChessBoardIndex][strHypotaxisId];
            }
            catch (System.Exception) { }
            return 0;
        }

        public int getCollectNum(string strHypotaxisId)
        {
            try
            {
                return m_mpCollect[-1][strHypotaxisId];
            }
            catch (System.Exception) { }
            return 0;
        }

        void collectElement(int nChessBoardIndex, string strHypotaxisId)
        {
            if (m_mpCollect.ContainsKey(nChessBoardIndex) == false)
            {
                var mpCollectElement = new Dictionary<string, int>();
                m_mpCollect.Add(nChessBoardIndex, mpCollectElement);
                mpCollectElement.Add(strHypotaxisId, 1);
            }
            else
            {
                if (m_mpCollect[nChessBoardIndex].ContainsKey(strHypotaxisId) == false)
                {
                    m_mpCollect[nChessBoardIndex].Add(strHypotaxisId, 1);
                }
                else
                {
                    m_mpCollect[nChessBoardIndex][strHypotaxisId]++;
                }
            }
            if (nChessBoardIndex == -1)
            {
                return;
            }
            collectElement(-1, strHypotaxisId);
        }

        /////////////////////////////////

        void addElementCount(Element tElement)
        {
            string strHypotaxisId = tElement.getHypotaxisId();
            if (m_mpHypotaxisIdElementCount.ContainsKey(strHypotaxisId) == false)
            {
                m_mpHypotaxisIdElementCount.Add(strHypotaxisId, new Dictionary<int, Element>());
            }
            m_mpHypotaxisIdElementCount[strHypotaxisId].Add(tElement.ID, tElement);
            if (m_mpElementCount.ContainsKey(tElement.ElementId) == false)
            {
                m_mpElementCount.Add(tElement.ElementId, new Dictionary<int, Element>());
            }
            m_mpElementCount[tElement.ElementId].Add(tElement.ID, tElement);
        }

        void delElementCount(string strElementId, int nElementId)
        {
            string strHypotaxisId = Config.ElementConfig.getConfig_element_hypotaxisId(strElementId);
            if (m_mpHypotaxisIdElementCount.ContainsKey(strHypotaxisId) == true)
            {
                m_mpHypotaxisIdElementCount[strHypotaxisId].Remove(nElementId);
            }
            if (m_mpElementCount.ContainsKey(strElementId) == true)
            {
                m_mpElementCount[strElementId].Remove(nElementId);
            }
        }

        void addElement(Element tElement)
        {
            m_mpElement.Add(tElement.ID, tElement);
            addElementCount(tElement);
        }

        void removeElement(Element tElement)
        {
            m_mpElement.Remove(tElement.ID);
            collectElement(tElement.m_tGrid.m_tChessBoard.Index, tElement.getHypotaxisId());
            delElementCount(tElement.ElementId, tElement.ID);
        }

    }
}