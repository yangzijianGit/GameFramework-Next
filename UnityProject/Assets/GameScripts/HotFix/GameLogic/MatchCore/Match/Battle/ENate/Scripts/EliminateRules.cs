/*
        author      :       yangzijian
        time        :       2019-12-25 14:17:06
        function    :       eliminate rules 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using ENate;
using UnityEngine;
/*
        Eliminate judgment manager.
        By a point, judge to get the shape of the same color, and judge to eliminate.

*/

namespace ENate
{
    public class ElementContainer
    {
        Dictionary<int, Element> m_mpElement = new Dictionary<int, Element>();

        public void Add(Element tElement)
        {
            if (tElement == null || m_mpElement.ContainsKey(tElement.ID) == true)
            {
                return;
            }
            m_mpElement.Add(tElement.ID, tElement);
        }

        public Dictionary<int, Element>.Enumerator GetEnumerator()
        {
            return m_mpElement.GetEnumerator();
        }
        public int Count
        {
            get
            {
                return m_mpElement.Count;
            }
        }

        public Element this [int key]
        {
            get
            {
                return m_mpElement[key];
            }
            set
            {
                m_mpElement[key] = value;
            }
        }

        public Element first()
        {
            if (m_mpElement.Count <= 0)
            {
                return null;
            }
            var elementIt = m_mpElement.GetEnumerator();
            elementIt.MoveNext();
            var current = elementIt.Current;
            return current.Value;
        }
        public Vector3 computerCenter()
        {
            if (Count <= 0)
            {
                return Vector2.zero;
            }
            Vector3 tVec3 = Vector3.zero;
            foreach (var itElement in this)
            {
                var tElement = itElement.Value;
                RectTransform tRectTransform = tElement.GetComponent<RectTransform>();
                tVec3 += tRectTransform.position;
            }
            return tVec3 / (float) Count;
        }

        public void clear()
        {
            m_mpElement.Clear();
        }

    }

    public static class EliminateRules
    {
        // the traversed grid 
        // Collect the shape according to the rectangle
        static ElementValue<int> tEliminateTransmitValue = new ElementValue<int>(0);

        public static ENateMatch Collect(ChessBoard tChessBoard, Grid tGrid)
        {
            UnityEngine.Profiling.Profiler.BeginSample("EliminateRules.cs Collect");
            ENateMatch tMatchTheElement = new ENateMatch();
            tMatchTheElement.choseDefault(tChessBoard, tGrid);
            UnityEngine.Profiling.Profiler.EndSample();
            return tMatchTheElement;
        }

        public static ENateCompose compose(ChessBoard tChessBoard, Dictionary<int, Grid> mpGrid, Grid tGeneratorGrid)
        {
            UnityEngine.Profiling.Profiler.BeginSample("EliminateRules.cs compose");
            var tRValue = ENateCompose.compose(tChessBoard, mpGrid, tGeneratorGrid);
            UnityEngine.Profiling.Profiler.EndSample();
            return tRValue;
        }

        public static void eliminateGrid(Grid tGrid, ElementDestroy tElementDestroy, ElementContainer arrElement)
        {
            if (tGrid == null || tElementDestroy == null)
            {
                return;
            }
            StageRunStatue_Fever tStageRunStatue_Fever = tGrid.m_tChessBoard.m_tStage.CurrentStageRun as StageRunStatue_Fever;
            //console.log("grid gridCoord ", tGrid.m_tGridCoord.Coord);
            foreach (var itElement in tGrid.m_sortedElement)
            {
                if (itElement.Value == null)
                {
                    continue;
                }
                ElementDestroy tStopOtherElementDestroy = itElement.Value.getElementAttribute(ElementAttribute.Attribute.stopOtherDestroyType) as ElementDestroy;
                if (tStopOtherElementDestroy == null)
                {
                    continue;
                }
                tStopOtherElementDestroy.filterOut(tElementDestroy);
            }
            foreach (var itElement in tGrid.m_sortedElement)
            {
                if (tStageRunStatue_Fever != null && tStageRunStatue_Fever.isElementInProtected(itElement.Value) == true)
                {
                    continue;
                }
                if (itElement.Value == null)
                {
                    continue;
                }
                if (itElement.Value.IsLock == false)
                {
                    //console.log("element ", itElement.Value.ElementId);
                    ElementDestroy tAElementDestroy = itElement.Value.getElementAttribute(ElementAttribute.Attribute.destroyType) as ElementDestroy;
                    if (tAElementDestroy != null && tAElementDestroy.Equals(tElementDestroy) == true)
                    {
                        arrElement.Add(itElement.Value);
                    }
                }
                ElementValue<int> tEliminateTransmitElementValue = itElement.Value.getElementAttribute(ElementAttribute.Attribute.eliminateTransmit) as ElementValue<int>;
                if (tEliminateTransmitElementValue != null && tEliminateTransmitElementValue.Equals(tEliminateTransmitValue) == true)
                {
                    break;
                }
            }
        }

        public static void eliminateGrid(List<Grid> arrGrid, ElementDestroy tElementDestroy, ElementContainer arrElement)
        {
            foreach (var tGrid in arrGrid)
            {
                tElementDestroy.addDestroyType(tGrid.checkPassDestroy());
            }
            foreach (var tGrid in arrGrid)
            {
                eliminateGrid(tGrid, tElementDestroy, arrElement);
            }
        }
    }
}