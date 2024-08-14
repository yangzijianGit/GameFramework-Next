/*
 * @Description: grid 
 * @Author: yangzijian
 * @Date: 2019-12-16 14:58:22
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-08-06 16:18:07
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ENate
{
    // 三维坐标转一维坐标

    public class ElementSorted : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y.CompareTo(x);
        }
    }

    public class Grid : MonoBehaviour, IPointerUpHandler, IDragHandler, IPointerDownHandler
    {
        public GridCoord m_tGridCoord;
        public ChessBoard m_tChessBoard;
        public GameObject m_tGridUI;
        public GameObject gPattern;

        // 当前在此格子上的元素
        public SortedDictionary<int, Element> m_sortedElement;
        // 是否为临时掉落器
        public bool m_bIsTemporaryDrop;

        float m_fLastClickTime;
        private Direction m_tDropDirection = Direction.Down;
        /// <summary>
        /// 格子方向
        /// </summary>
        public Direction DropDirection
        {
            set
            {
                m_tDropDirection = value;
            }
            get
            {
                return m_tDropDirection;
            }
        }

        void Awake()
        {
            m_sortedElement = new SortedDictionary<int, Element>(new ElementSorted());
        }

        public void setArg(ChessBoard tChessBoard, int nLine, int nCol)
        {
            m_tGridCoord = new GridCoord(tChessBoard.Index, nLine, nCol);
            m_tChessBoard = tChessBoard;
        }

        public void markColor()
        {
            return;
            var tImage = transform.Find("Image").gameObject.GetComponent<Image>();
            tImage.color = new Color(1, 0, 0, 1f);
            Timer.Schedule(this, 2, () =>
            {
                tImage.color = m_tColor;
            });
        }

        /**
         * @Author: yangzijian
         * @LastEditTime: Do not edit
         * @description: create the grid and load the prefab.
         * @param {ChessBoard} chessboard
         * @param {int} line
         * @param {int} column
         * @return: 
         */
        public Color m_tColor;
        public static Grid create(ChessBoard tChessBoard, int nLine, int nCol)
        {
            GameObject tGridUI = ENateResource.loadPrefab("ENate/Prefabs/Grid", null, false);
            Grid tGrid = tGridUI.GetComponent<Grid>();
            tGrid.setArg(tChessBoard, nLine, nCol);
            tGridUI.transform.SetParent(tChessBoard.m_tGridPanel.transform);
            tGridUI.SetActive(true);
            tGridUI.name = "grid[" + nLine + "]Col[" + nCol + "]";
            RectTransform tRectTransform = tGridUI.GetComponent<RectTransform>();
            tRectTransform.localScale = Vector3.one;
            tRectTransform.anchoredPosition = MoveUnitUtil.getPositionWithLineCol(nLine, nCol);
            if ((nLine + nCol) % 2 == 1)
            {
                tGrid.gPattern.SetActive(true);
            }
            else
            {
                tGrid.gPattern.SetActive(false);
            }
            tGrid.m_tGridUI = tGridUI;
            return tGrid;
        }

        public void addElement(Element tElement)
        {
            UnityEngine.Profiling.Profiler.BeginSample("addElement");
            if (tElement == null)
            {
                return;
            }
            ElementValue<int> tLevel = tElement.getElementAttribute(ElementAttribute.Attribute.level) as ElementValue<int>;
            if (tLevel == null)
            {
                Debug.LogError( "ERROR:element level attribute is null ");
            }
            try
            {
                tElement.changeGrid(this);
                m_sortedElement.Add(tLevel.Value, tElement);
            }
            catch (System.Exception)
            {
                Debug.LogError( "Error: the grid is have common level"); // .MoreStringFormat(m_sortedElement[tLevel.Value].ID));
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void popElement(int nLevel)
        {
            UnityEngine.Profiling.Profiler.BeginSample("popElement");
            try
            {
                m_sortedElement[nLevel].changeGrid(null);
                m_sortedElement.Remove(nLevel);
            }
            catch (System.Exception) { }
            UnityEngine.Profiling.Profiler.EndSample();
        }
        public void popElement(Element tElement)
        {
            if (tElement == null)
            {
                return;
            }
            ElementValue<int> tLevel = tElement.getElementAttribute(ElementAttribute.Attribute.level) as ElementValue<int>;
            if (tLevel == null)
            {
                Debug.LogError( "ERROR:element level attribute is null ");
            }
            popElement(tLevel.Value);
        }

        public Element getElementWithElementAttribute<T>(ElementAttribute.Attribute eAttribute, T compareValue, bool bIsPop = false)
        where T : struct
        {
            UnityEngine.Profiling.Profiler.BeginSample("getElementWithElementAttribute<T>");
            Element tReValue = null;
            foreach (var itElement in m_sortedElement)
            {
                ElementValue<T> tElementAttribute = itElement.Value.getElementAttribute(eAttribute) as ElementValue<T>;
                if (tElementAttribute != null && tElementAttribute.Value.Equals(compareValue))
                {
                    tReValue = itElement.Value;
                    if (bIsPop) popElement(itElement.Key);
                    break;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return tReValue;
        }

        public Element getNormalElementWithElementAttribute<T>(ElementAttribute.Attribute eAttribute, T compareValue, bool bIsPop = false)
        where T : struct
        {
            Element tElement = getElementWithElementAttribute(eAttribute, compareValue, bIsPop);
            if (tElement != null && tElement.IsLock == false)
            {
                return tElement;
            }
            return null;
        }
        public Element getElementWithElementAttribute(ElementAttribute.Attribute eAttribute, ElementAttribute tCompareElementAttribute, bool bIsPop = false)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getElementWithElementAttribute");
            Element tReValue = null;
            foreach (var itElement in m_sortedElement)
            {
                bool bIsOk = itElement.Value.compareElementAttribute(eAttribute, tCompareElementAttribute);
                if (bIsOk == true)
                {
                    tReValue = itElement.Value;
                    if (bIsPop) popElement(itElement.Key);
                    break;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return tReValue;
        }

        public Element getNormalElementWithElementAttribute(ElementAttribute.Attribute eAttribute, ElementAttribute tCompareElementAttribute, bool bIsPop = false)
        {
            Element tElement = getElementWithElementAttribute(eAttribute, tCompareElementAttribute, bIsPop);
            if (tElement != null && tElement.IsLock == false)
            {
                return tElement;
            }
            return null;
        }

        public void getElementWithElementAttributeArray(ElementAttribute.Attribute eAttribute, ElementAttribute tCompareElementAttribute, ref List<Element> arrElement, bool bIsPop = false)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getElementWithElementAttributeArray");
            List<int> arrDelKey = new List<int>();
            foreach (var itElement in m_sortedElement)
            {
                ElementAttribute tElementAttribute = itElement.Value.getElementAttribute(eAttribute);
                bool bIsOk = false;
                if (tElementAttribute == null)
                {
                    if (tCompareElementAttribute == null)
                    {
                        bIsOk = true;
                    }
                }
                else if (tElementAttribute.Equals(tCompareElementAttribute))
                {
                    bIsOk = true;
                }
                if (bIsOk)
                {
                    arrElement.Add(itElement.Value);
                    if (bIsPop) arrDelKey.Add(itElement.Key);
                }
            }
            foreach (var tDelKey in arrDelKey)
            {
                popElement(tDelKey);
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public Grid getGridByDirection(Direction eDirection)
        {
            GridCoord tDirectionGridCoord = ENate.Util.getDirectionLineCol(ref m_tGridCoord, ref eDirection);
            if (tDirectionGridCoord.isNull() == true)
            {
                return null;
            }
            return m_tChessBoard.getGrid(tDirectionGridCoord);
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        // 比较属性值是否相等
        ///////////////////////////////////////////////////////////////////////////////////////

        public bool match(List<Match> arrConfirmToValue, List<Match> arrInconformityValue)
        {
            UnityEngine.Profiling.Profiler.BeginSample("match");

            foreach (var itElement in m_sortedElement)
            {
                var tElement = itElement.Value;
                if (tElement == null)
                {
                    continue;
                }
                foreach (Match tInconformmityMatch in arrInconformityValue)
                {
                    if (tInconformmityMatch.match(tElement) == true)
                    {
                        return false;
                    }
                }
            }

            foreach (var itElement in m_sortedElement)
            {
                var tElement = itElement.Value;
                if (tElement == null)
                {
                    continue;
                }
                foreach (Match tConfirmToMatch in arrConfirmToValue)
                {
                    if (tConfirmToMatch.match(tElement) == true)
                    {
                        return true;
                    }
                }
            }
            return arrConfirmToValue.Count <= 0;
        }

        public bool detectElementStateNormal()
        {
            UnityEngine.Profiling.Profiler.BeginSample("detectElementState");
            foreach (var itElement in m_sortedElement)
            {
                if (itElement.Value.IsLock == true)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return false;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        public void OnPointerUp(PointerEventData eventData)
        {
            showElementClickTip();

            /**
             * @Author: yangzijian
             * @description: instead of dragging, check to see if it's a double-click.
             */
            float fDoubletime = float.Parse(JsonManager.eliminate_config.root.game.doubleTime);
            float currentTime = UnityEngine.Time.time;
            if (currentTime - m_fLastClickTime < fDoubletime)
            {
                m_fLastClickTime = 0;
                m_tChessBoard.doubleClickGrid(m_tGridCoord.Line, m_tGridCoord.Col);
            }
            else
            {
                m_fLastClickTime = currentTime;
            }
        }
        bool bIsInDrag = false;
        public void OnDrag(PointerEventData eventData)
        {
            if (bIsInDrag == true)
            {
                return;
            }
            do
            {
                /**
                 * @Author: yangzijian
                 * @description: check if it's dragging.
                 */
                float nBetX = eventData.position.x - eventData.pressPosition.x;
                float nBetY = -(eventData.position.y - eventData.pressPosition.y);
                float fDistanceThresholder = float.Parse(JsonManager.eliminate_config.root.game.dragDistance);

                int nOffectX = 1;
                int nOffectY = 1;
                float nBetXAbs = Math.Abs(nBetX);
                float nBetYAbs = Math.Abs(nBetY);

                if (nBetXAbs > fDistanceThresholder || nBetYAbs > fDistanceThresholder)
                {
                    if (nBetX < 0)
                    {
                        nBetX = -nBetX;
                        nOffectX = -1;
                    }
                    if (nBetY < 0)
                    {
                        nBetY = -nBetY;
                        nOffectY = -1;
                    }

                    if (nBetX > nBetY)
                    {
                        nOffectY = 0;
                    }
                    else
                    {
                        nOffectX = 0;
                    }
                }
                else
                {
                    /**
                     * @Author: yangzijian
                     * @description: Here is not drag, and exit the loop.
                     */
                    break;
                }
                int nAimLine = m_tGridCoord.Line + nOffectY;
                int nAimCol = m_tGridCoord.Col + nOffectX;
                m_fLastClickTime = 0;
                bIsInDrag = true;
                StartCoroutine(m_tChessBoard.exchangeGrid(m_tGridCoord.Line, m_tGridCoord.Col, nAimLine, nAimCol, () =>
                {
                    bIsInDrag = false;
                }));
                return;
            } while (false);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_Handle_SelectGrid, this);
        }

        public List<Element> getExchangeMoveElement(bool bIsPop = true)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getExchangeMoveElement");
            List<Element> arrElement = new List<Element>();
            Element tElement = getElementWithElementAttribute(ElementAttribute.Attribute.moveType, new ElementValue<int>(1), bIsPop);
            if (tElement != null)
            {
                arrElement.Add(tElement);
            }
            getElementWithElementAttributeArray(ElementAttribute.Attribute.followBasic, new ElementValue<int>(1), ref arrElement, bIsPop);
            UnityEngine.Profiling.Profiler.EndSample();
            return arrElement;
        }

        public void addElementArray(List<Element> arrElement)
        {
            foreach (var tElement in arrElement)
            {
                addElement(tElement);
            }
        }

        void showElementClickTip()
        {
            foreach (var itElement in m_sortedElement)
            {
                if (m_tChessBoard.m_tStage.m_tStageTips.m_arrTipsElement.Contains(itElement.Value) == true)
                {
                    return;
                }
            }
            foreach (var itElement in m_sortedElement)
            {
                if (itElement.Value.IsLock == true)
                {
                    continue;
                }

                itElement.Value.playAniWithBehaviorId("normal");
                itElement.Value.playAniWithBehaviorId("click");
            }
        }
    }
}