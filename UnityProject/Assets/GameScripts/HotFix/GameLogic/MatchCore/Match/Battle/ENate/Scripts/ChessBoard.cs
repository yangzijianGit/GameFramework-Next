using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    // 棋盘id
    public class ChessBoard : MonoBehaviour
    {
        private int m_nIndex;
        public int Index
        {
            get
            {
                return m_nIndex;
            }
        }

        /**
         * @Author: yangzijian
         * @description: drop dirction.
         */
        private Direction m_tDropDirection;
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

        public Stage m_tStage;
        public Transform m_tGridPanel;
        public Transform m_tLayerPanel;
        public Transform m_tDropPanel;
        public DropManager m_tDropManager;
        // 棋盘格子
        private Grid[][] m_mpGrid;

        public List<Grid> getAllGrid()
        {
            List<Grid> arrGrid = new List<Grid>();
            foreach (var mpGridSub in m_mpGrid)
            {
                foreach (var tGrid in mpGridSub)
                {
                    if (tGrid == null)
                    {
                        continue;
                    }
                    arrGrid.Add(tGrid);
                }
            }
            return arrGrid;
        }

        [NonSerialized]
        public int m_nWidth;
        [NonSerialized]
        public int m_nHeight;

        int m_nGridCount = 0;
        public int GridCount
        {
            get
            {
                return m_nGridCount;
            }
        }

        jc.EventManager.EventObj m_tEventObj;
        private void OnDestroy()
        {
            m_tEventObj.clear();
            m_tEventObj = null;
        }
        ChessBoard m_tConnectChessBoard;

        public ChessBoard ConnectChessBoard
        {
            get { return m_tConnectChessBoard; }
            set { m_tConnectChessBoard = value; }
        }

        Direction m_eConnectDirection = Direction.Down;
        public Direction eConnectDirection
        {
            get
            {
                return m_eConnectDirection;
            }
        }
        List<GridCoord> arrDetectingPoint = new List<GridCoord>();

        public ChessBoardReset m_pChessBoardReset;

        public void addCheckPoint(int nLine, int nColumn)
        {
            arrDetectingPoint.Add(new GridCoord(Index, nLine, nColumn));
        }

        void Awake()
        {
            m_tDropDirection = Direction.Down;
            m_pChessBoardReset = new ChessBoardReset(this);
            m_tEventObj = new jc.EventManager.EventObj();
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_STEPFRESH, event_ClearRound);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ElementChange, event_elementChange);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_SkillElementChange, event_elementChange);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_DROP_OPEN, event_openDrop);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_DROP_STOP, event_stopDrop);
        }

        public void setArg(Stage tStage, int nIndex)
        {
            m_nIndex = nIndex;
            m_tStage = tStage;
            m_tDropManager = new DropManager(this);
        }

        public void setSize(int nWidth, int nHeight)
        {
            m_nWidth = nWidth;
            m_nHeight = nHeight;
            m_mpGrid = new Grid[m_nHeight][];
            for (int i = 0; i < m_nHeight; i++)
            {
                m_mpGrid[i] = new Grid[m_nWidth];
            }
            m_pChessBoardReset.setSize(nWidth, nHeight);
        }

        public static Vector3 getChessBoardIndexPosition(ChessBoard tLastChessBoard, int nIndex, float nWidth, float nHeight, bool bIsConnectLastChessBoard, Direction eConnectDirection)
        {
            if (tLastChessBoard == null)
            {
                return Vector3.zero;
            }
            var tRectTransform = tLastChessBoard.GetComponent<RectTransform>();
            var tPosition = tRectTransform.anchoredPosition3D;
            if (bIsConnectLastChessBoard == true)
            {
                switch (eConnectDirection)
                {
                    case Direction.Down:
                        {
                            tPosition -= new Vector3(0, nHeight, 0);
                        }
                        break;
                    case Direction.Up:
                        {
                            tPosition += new Vector3(0, nHeight, 0);
                        }
                        break;
                    case Direction.Left:
                        {
                            tPosition -= new Vector3(nWidth, 0, 0);
                        }
                        break;
                    case Direction.Right:
                        {
                            tPosition += new Vector3(nWidth, 0, 0);
                        }
                        break;
                }
            }
            else
            {
                tPosition += new Vector3(nWidth, 0, 0);
            }
            return tPosition;
        }

        public static ChessBoard create(Stage tStage, int nIndex, bool bIsConnectLastChessBoard, Direction eConnectDirection)
        {
            // ChessBoard tChessBoard = new ChessBoard (tStage, nIndex);
            GameObject tChessBoardUI = ENateResource.loadPrefab("ENate/Prefabs/chessBoard", null, false);
            ChessBoard tChessBoard = tChessBoardUI.GetComponent<ChessBoard>();
            tChessBoard.setArg(tStage, nIndex);
            tChessBoardUI.attachObj(tStage.m_tChessBaordAttachNode);
            tChessBoardUI.SetActive(true);
            tChessBoardUI.name = "chessBoard_" + nIndex;
            var tLastChessBoard = tStage.getChessBoardWithIndex(nIndex - 1);
            if (bIsConnectLastChessBoard == true)
            {
                tChessBoard.ConnectChessBoard = tLastChessBoard;
                tChessBoard.m_eConnectDirection = eConnectDirection;
            }
            var tAttachRectTransform = tStage.m_tChessBaordAttachNode.GetComponent<RectTransform>();
            var tRectTransform = tChessBoardUI.GetComponent<RectTransform>();
            tRectTransform.sizeDelta = new Vector2(tRectTransform.sizeDelta.x, tAttachRectTransform.rect.height);
            tRectTransform.anchoredPosition3D = getChessBoardIndexPosition(tLastChessBoard, nIndex, tRectTransform.rect.width, tRectTransform.rect.height, bIsConnectLastChessBoard, eConnectDirection);
            return tChessBoard;
        }

        public Grid createGrid(int nLine, int nCol)
        {
            Grid tGrid = Grid.create(this, nLine, nCol);
            m_mpGrid[nLine][nCol] = tGrid;
            ++m_nGridCount;
            return tGrid;
        }

        // public Grid getGrid(int nGridCoord)
        // {
        //     Grid tGrid = null;
        //     try
        //     {
        //         tGrid = m_mpGrid[nGridCoord];
        //     }
        //     catch (Exception ex) { }
        //     return tGrid;
        // }
        public Grid getGrid(int nLine, int nCol)
        {
            Grid tGrid = null;
            try
            {
                tGrid = m_mpGrid[nLine][nCol];
            }
            catch (Exception ex)
            {
                // console.log("ERROR: grid is not have this line col ", nLine, nCol);
            }
            return tGrid;
        }

        public Grid getGrid(GridCoord tGridCoord)
        {
            return getGrid(tGridCoord.Line, tGridCoord.Col);
        }

        /**
         * @Author: yangzijian
         * @description: temporarily write dead, fall driection is downward.
         */

        void checkDropUnitStraight()
        {
            UnityEngine.Profiling.Profiler.BeginSample("checkDropUnitStraight");
            for (int nLine = m_nHeight; nLine >= 0; nLine--)
            {
                for (int nCol = 0; nCol <= m_nWidth; nCol++)
                {
                    if (m_tDropManager.ifICanMove(nLine, nCol) == false)
                    {
                        continue;
                    }
                    if (m_tDropManager.dropOpenPortal(nLine, nCol) == true)
                    {
                        continue;
                    }
                    if (m_tDropManager.dropStraightLine(nLine, nCol) == true)
                    {
                        continue;
                    }
                    if (m_tDropManager.dropObliqueLine(nLine, nCol) == true)
                    {
                        continue;
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public GameObject getLayer(int nLevel)
        {
            GameObject obj = null;
            Transform layerPanel = m_tLayerPanel;
            Transform tLayer = layerPanel.Find(nLevel.ToString());
            if (tLayer == null)
            {
                GameObject tLayerObj = ENateResource.loadPrefab("ENate/Prefabs/layer", null, false);
                tLayerObj.name = nLevel.ToString();
                int nIndex = layerPanel.childCount;
                for (int i = 0; i < layerPanel.childCount; i++)
                {
                    Transform tSortLayer = layerPanel.GetChild(i);
                    int nSortIndex = int.Parse(tSortLayer.name);
                    if (nLevel < nSortIndex)
                    {
                        nIndex = i;
                        break;
                    }
                }
                tLayerObj.attachObj(layerPanel.gameObject);
                tLayerObj.transform.SetSiblingIndex(nIndex);
                obj = tLayerObj;
            }
            else
            {
                obj = tLayer.gameObject;
            }
            return obj;
        }

        public void checkConnectChessBoard()
        {
            UnityEngine.Profiling.Profiler.BeginSample("checkConnectChessBoard");
            do
            {
                if (ConnectChessBoard == null)
                {
                    break;
                }
                ConnectChessBoard.updateDropManager();
                Action<Grid, Grid, Direction> pUpdateDrop = (Grid tSrcGrid, Grid tAimGrid, Direction eMoveDirection) =>
                {
                    if (tSrcGrid == null || tAimGrid == null)
                    {
                        return;
                    }
                    if (ConnectChessBoard.m_tDropManager.ifICanMove(tSrcGrid.m_tGridCoord.Line, tSrcGrid.m_tGridCoord.Col) == false)
                    {
                        return;
                    }
                    if (m_tDropManager.canIMoveThere(tSrcGrid, tAimGrid, eMoveDirection) == false)
                    {
                        return;
                    }
                    List<Element> arrElement = new List<Element>();
                    Element tElement = tSrcGrid.getElementWithElementAttribute(ElementAttribute.Attribute.moveType, DropManager.sm_tElementValueDefaultMoveType, true);
                    if (tElement != null)
                    {
                        arrElement.Add(tElement);
                    }
                    tSrcGrid.getElementWithElementAttributeArray(ElementAttribute.Attribute.followBasic, DropManager.sm_tElementValueDefaultMoveType, ref arrElement, true);
                    foreach (var itElement in arrElement)
                    {
                        var tLevel = itElement.getElementAttribute(ElementAttribute.Attribute.level) as ElementValue<int>;
                        var tLayer = getLayer(tLevel.Value);
                        itElement.gameObject.attachObj(tLayer);
                    }
                    GridCoord tGridCoord = DropManager.getDirectionLineCol(Index, tAimGrid.m_tGridCoord.Line, tAimGrid.m_tGridCoord.Col, Util.getDirectionOpposite(eMoveDirection));
                    m_tDropManager.createDropUnit(tGridCoord.Line, tGridCoord.Col, tAimGrid.m_tGridCoord.Line, tAimGrid.m_tGridCoord.Col, arrElement, DropUnit.MoveType.normal);
                };
                switch (m_eConnectDirection)
                {
                    case Direction.Down:
                        {
                            for (int nCol = 0; nCol < m_nWidth; nCol++)
                            {
                                Grid tSrcGrid = ConnectChessBoard.getGrid(ConnectChessBoard.m_nHeight - 1, nCol);
                                if (tSrcGrid == null || tSrcGrid.DropDirection != m_eConnectDirection)
                                {
                                    continue;
                                }
                                Grid tAimGrid = getGrid(0, nCol);
                                pUpdateDrop(tSrcGrid, tAimGrid, tSrcGrid.DropDirection);
                            }
                        }
                        break;
                    case Direction.Up:
                        {
                            for (int nCol = 0; nCol < m_nWidth; nCol++)
                            {
                                Grid tSrcGrid = ConnectChessBoard.getGrid(0, nCol);
                                if (tSrcGrid == null || tSrcGrid.DropDirection != m_eConnectDirection)
                                {
                                    continue;
                                }
                                Grid tAimGrid = getGrid(m_nHeight - 1, nCol);
                                pUpdateDrop(tSrcGrid, tAimGrid, tSrcGrid.DropDirection);
                            }
                        }
                        break;
                    case Direction.Left:
                        {
                            for (int nLine = 0; nLine < m_nHeight; nLine++)
                            {
                                Grid tSrcGrid = ConnectChessBoard.getGrid(nLine, 0);
                                if (tSrcGrid == null || tSrcGrid.DropDirection != m_eConnectDirection)
                                {
                                    continue;
                                }
                                Grid tAimGrid = getGrid(nLine, m_nWidth - 1);
                                pUpdateDrop(tSrcGrid, tAimGrid, tSrcGrid.DropDirection);
                            }
                        }
                        break;
                    case Direction.Right:
                        {
                            for (int nLine = 0; nLine < m_nHeight; nLine++)
                            {
                                Grid tSrcGrid = ConnectChessBoard.getGrid(nLine, ConnectChessBoard.m_nWidth - 1);
                                if (tSrcGrid == null || tSrcGrid.DropDirection != m_eConnectDirection)
                                {
                                    continue;
                                }
                                Grid tAimGrid = getGrid(nLine, 0);
                                pUpdateDrop(tSrcGrid, tAimGrid, tSrcGrid.DropDirection);
                            }
                        }
                        break;
                }
            }
            while (false);
            UnityEngine.Profiling.Profiler.EndSample();
        }
        void checkDropDevice()
        {
            UnityEngine.Profiling.Profiler.BeginSample("checkDropDevice");
            m_tDropManager.updateDropDevice();
            UnityEngine.Profiling.Profiler.EndSample();
        }

        void upateMove()
        {
            UnityEngine.Profiling.Profiler.BeginSample("upateMove");
            m_tDropManager.updateMove();
            UnityEngine.Profiling.Profiler.EndSample();
        }
        public void updateDropManager()
        {
            UnityEngine.Profiling.Profiler.BeginSample("updateDropManager");
            upateMove();
            checkDropUnitStraight();
            checkDropDevice();
            // checkDropUnitOblique();
            // checkDropDevice();
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public class ExchangeAndSyntheizeElements
        {
            public ElementContainer m_arrElement;
            public ENateComputeInfo m_tSrcENateComputeInfo;
            public ENateComputeInfo m_tAimENateComputeInfo;
            public ChessBoard m_tChessBoard;
            public Grid m_tSrcSelectGrid;
            public Grid m_tAimSelectGrid;

            ExchangeAndSyntheizeElements(ElementContainer arrElement,
                ENateComputeInfo tSrcENateComputeInfo,
                ENateComputeInfo tAimENateComputeInfo,
                ChessBoard tChessBoard,
                Grid tSrcSelectGrid,
                Grid tAimSelectGrid)
            {
                m_arrElement = arrElement;
                m_tSrcENateComputeInfo = tSrcENateComputeInfo;
                m_tAimENateComputeInfo = tAimENateComputeInfo;
                m_tChessBoard = tChessBoard;
                m_tSrcSelectGrid = tSrcSelectGrid;
                m_tAimSelectGrid = tAimSelectGrid;
            }

            public static ExchangeAndSyntheizeElements create(ElementContainer arrElement,
                ENateComputeInfo tSrcENateComputeInfo,
                ENateComputeInfo tAimENateComputeInfo,
                ChessBoard tChessBoard,
                Grid tSrcSelectGrid,
                Grid tAimSelectGrid)
            {
                if (arrElement != null && arrElement.Count > 0 || tSrcENateComputeInfo != null && tSrcENateComputeInfo.m_arrElement.Count > 0 || tAimENateComputeInfo != null && tAimENateComputeInfo.m_arrElement.Count > 0)
                {
                    return new ExchangeAndSyntheizeElements(arrElement, tSrcENateComputeInfo, tAimENateComputeInfo, tChessBoard, tSrcSelectGrid, tAimSelectGrid);
                }
                return null;
            }

            public bool bIsSucceed()
            {
                return m_arrElement.Count > 0 || m_tSrcENateComputeInfo.m_arrElement.Count > 0 || m_tAimENateComputeInfo.m_arrElement.Count > 0;
            }

            public void excute(Stage tStage)
            {
                bool bIsSucceed1 = false;
                bool bIsSucceed2 = false;
                if (m_tAimENateComputeInfo != null)
                {
                    bIsSucceed1 = m_tAimENateComputeInfo.m_arrElement.Count > 0;
                    ElementCreater.DestroyInfo tDestroyInfo = tStage.m_tElementCreater.createDestroyInfo(m_tAimENateComputeInfo.m_arrElement);
                    tDestroyInfo.tGeneratedGrid = m_tAimENateComputeInfo.m_tGeneratedGrid;
                    tDestroyInfo.strGeneratedId = m_tAimENateComputeInfo.m_strGeneratedId;
                    if (string.IsNullOrEmpty(m_tAimENateComputeInfo.m_strGeneratedId) == false)
                    {
                        tDestroyInfo.eEEliminatePlay = ElementCreater.DestroyInfo.EEliminatePlay.merge;
                    }
                    else
                    {
                        tDestroyInfo.eEEliminatePlay = ElementCreater.DestroyInfo.EEliminatePlay.delay;
                        tDestroyInfo.tTriggerGridCoord = m_tAimENateComputeInfo.m_tTriggerGridCoord;
                    }
                    tDestroyInfo.destroyElement();
                    jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE_ROUND, m_tAimENateComputeInfo.m_arrElement);
                }
                if (m_tSrcENateComputeInfo != null)
                {
                    bIsSucceed2 = m_tSrcENateComputeInfo.m_arrElement.Count > 0;
                    ElementCreater.DestroyInfo tDestroyInfo = tStage.m_tElementCreater.createDestroyInfo(m_tSrcENateComputeInfo.m_arrElement);
                    tDestroyInfo.tGeneratedGrid = m_tSrcENateComputeInfo.m_tGeneratedGrid;
                    tDestroyInfo.strGeneratedId = m_tSrcENateComputeInfo.m_strGeneratedId;
                    if (string.IsNullOrEmpty(m_tSrcENateComputeInfo.m_strGeneratedId) == false)
                    {
                        tDestroyInfo.eEEliminatePlay = ElementCreater.DestroyInfo.EEliminatePlay.merge;
                    }
                    else
                    {
                        tDestroyInfo.eEEliminatePlay = ElementCreater.DestroyInfo.EEliminatePlay.delay;
                        tDestroyInfo.tTriggerGridCoord = m_tSrcENateComputeInfo.m_tTriggerGridCoord;
                    }
                    tDestroyInfo.destroyElement();
                    jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE_ROUND, m_tSrcENateComputeInfo.m_arrElement);
                }

                if (m_arrElement.Count > 0)
                {
                    ElementCreater.DestroyInfo tDestroyInfo = tStage.m_tElementCreater.createDestroyInfo(m_arrElement);
                    tDestroyInfo.destroyElement();
                    jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE_ROUND, m_arrElement);
                }
                m_tChessBoard.addCheckPoint(m_tSrcSelectGrid.m_tGridCoord.Line, m_tSrcSelectGrid.m_tGridCoord.Col);
                m_tChessBoard.addCheckPoint(m_tAimSelectGrid.m_tGridCoord.Line, m_tAimSelectGrid.m_tGridCoord.Col);
            }
        }

        public ExchangeAndSyntheizeElements exchangeAndSynthesizeElementsCheck(int nLine, int nColumn, int nAimLine, int nAimColumn)
        {
            UnityEngine.Profiling.Profiler.BeginSample("exchangeAndSynthesizeElements");
            ElementDestroy tElementDestroy = new ElementDestroy();
            ElementContainer arrElement = new ElementContainer();
            tElementDestroy.addDestroyType(37);
            Grid tGrid = getGrid(nLine, nColumn);
            Grid tAimGrid = getGrid(nAimLine, nAimColumn);
            if (tGrid != null && tGrid.getElementWithElementAttribute(ElementAttribute.Attribute.type, 1) != null ||
                tAimGrid != null && tAimGrid.getElementWithElementAttribute(ElementAttribute.Attribute.type, 1) != null)
            {
                ElementDestroy tPassElementDestroy = new ElementDestroy();
                var tPassDestroy = tGrid.checkPassDestroy();
                var tAimPassDestroy = tGrid.checkPassDestroy();
                tPassElementDestroy.addDestroyType(tPassDestroy);
                tPassElementDestroy.addDestroyType(tAimPassDestroy);
                EliminateRules.eliminateGrid(tGrid, tPassElementDestroy, arrElement);
                EliminateRules.eliminateGrid(tAimGrid, tPassElementDestroy, arrElement);
            }
            var tENateComputeInfoAim = collectAndSynthesizeElementsInfo(nAimLine, nAimColumn);
            var tENateComputeInfoSrc = collectAndSynthesizeElementsInfo(nLine, nColumn);
            EliminateRules.eliminateGrid(tGrid, tElementDestroy, arrElement);
            EliminateRules.eliminateGrid(tAimGrid, tElementDestroy, arrElement);

            UnityEngine.Profiling.Profiler.EndSample();
            return ExchangeAndSyntheizeElements.create(arrElement, tENateComputeInfoSrc, tENateComputeInfoAim, this, tGrid, tAimGrid);
        }

        public class ENateComputeInfo
        {
            public ElementContainer m_arrElement;
            public Grid m_tGeneratedGrid;
            public string m_strGeneratedId;
            public bool m_bIsNeedReCheckPoint = false;
            public GridCoord m_tTriggerGridCoord;
        }

        public ENateComputeInfo collectAndSynthesizeElementsInfo(int nLine, int nColumn)
        {
            UnityEngine.Profiling.Profiler.BeginSample("collectAndSynthesizeElementsInfo");
            Grid tGrid = getGrid(nLine, nColumn);
            if (tGrid == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return null;
            }
            ENateMatch tENateMatch = EliminateRules.Collect(this, tGrid);
            ENateCompose tENateCompose = EliminateRules.compose(this, tENateMatch.m_mpCollectedGrid, tENateMatch.m_tGrid);
            if (tENateCompose.m_arrComposeGrid.Contains(tGrid) != true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return null;
            }
            ElementDestroy tElementDestroy = new ElementDestroy();
            tElementDestroy.addDestroyType(1);
            ElementContainer arrElement = new ElementContainer();
            EliminateRules.eliminateGrid(tENateCompose.m_arrComposeGrid, tElementDestroy, arrElement);
            UnityEngine.Profiling.Profiler.EndSample();
            return new ENateComputeInfo() { m_arrElement = arrElement, m_tGeneratedGrid = tENateCompose.m_tGeneratedGrid, m_strGeneratedId = tENateCompose.m_strGeneratedId, m_bIsNeedReCheckPoint = tENateCompose.m_bIsNeedReCheckPoint, m_tTriggerGridCoord = tENateMatch.m_tTriggerGridCoord };
        }

        bool collectAndSynthesizeElements(int nLine, int nColumn)
        {
            UnityEngine.Profiling.Profiler.BeginSample("collectAndSynthesizeElements");
            var tENateComputeInfo = collectAndSynthesizeElementsInfo(nLine, nColumn);
            if (tENateComputeInfo == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }
            ElementCreater.DestroyInfo tDestroyInfo = m_tStage.m_tElementCreater.createDestroyInfo(tENateComputeInfo.m_arrElement);
            tDestroyInfo.tGeneratedGrid = tENateComputeInfo.m_tGeneratedGrid;
            tDestroyInfo.strGeneratedId = tENateComputeInfo.m_strGeneratedId;
            if (string.IsNullOrEmpty(tENateComputeInfo.m_strGeneratedId) == false)
            {
                tDestroyInfo.eEEliminatePlay = ElementCreater.DestroyInfo.EEliminatePlay.merge;
            }
            else
            {
                tDestroyInfo.eEEliminatePlay = ElementCreater.DestroyInfo.EEliminatePlay.delay;
                tDestroyInfo.tTriggerGridCoord = tENateComputeInfo.m_tTriggerGridCoord;
            }
            tDestroyInfo.destroyElement();
            if (tENateComputeInfo.m_arrElement.Count > 0)
            {
                jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE_ROUND, tENateComputeInfo.m_arrElement);
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return tENateComputeInfo.m_bIsNeedReCheckPoint;
        }
        public ENateCompose collectAndSynthesizeElementsCheckCompose(int nLine, int nColumn)
        {
            UnityEngine.Profiling.Profiler.BeginSample("collectAndSynthesizeElementsCheckCompose");
            Grid tGrid = getGrid(nLine, nColumn);
            if (tGrid == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return null;
            }
            ENateMatch tENateMatch = EliminateRules.Collect(this, tGrid);
            ENateCompose tENateCompose = EliminateRules.compose(this, tENateMatch.m_mpCollectedGrid, tENateMatch.m_tGrid);
            if (tENateCompose.m_arrComposeGrid.Contains(tGrid) != true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return null;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return tENateCompose;
        }
        public bool collectAndSynthesizeElementsCheck(int nLine, int nColumn)
        {
            UnityEngine.Profiling.Profiler.BeginSample("collectAndSynthesizeElementsCheck");
            ENateCompose tENateCompose = collectAndSynthesizeElementsCheckCompose(nLine, nColumn);
            UnityEngine.Profiling.Profiler.EndSample();
            return tENateCompose != null && tENateCompose.m_arrComposeGrid.Count > 0;
        }

        bool getExchangeSkillId(string strElementId1, string strElementId2, List<string> arrSkillId)
        {
            foreach (var tExChangeConfig in JsonManager.composerules_config.root.game.ComposeTemplate.ExchangeSkillRules)
            {
                if ((tExChangeConfig.m_strExchangeMainId == strElementId1 && tExChangeConfig.m_strExchangeSecondId == strElementId2) ||
                    (tExChangeConfig.m_strExchangeMainId == strElementId2 && tExChangeConfig.m_strExchangeSecondId == strElementId1))
                {
                    arrSkillId.AddRange(tExChangeConfig.skill);
                    return true;
                }
            }
            return false;
        }

        bool getHypotaxisExchangeSkillId(string strElementId1, string strElementId2, List<string> arrSkillId)
        {
            foreach (var tExChangeConfig in JsonManager.composerules_config.root.game.ComposeTemplate.ExchangeHypotaxisSkillRules)
            {
                if ((tExChangeConfig.m_strExchangeMainId == strElementId1 && tExChangeConfig.m_strExchangeSecondId == strElementId2) ||
                    (tExChangeConfig.m_strExchangeMainId == strElementId2 && tExChangeConfig.m_strExchangeSecondId == strElementId1))
                {
                    arrSkillId.AddRange(tExChangeConfig.skill);
                    return true;
                }
            }
            return false;
        }

        bool InspectionCanBeOperated(int nLine, int nColumn)
        {
            UnityEngine.Profiling.Profiler.BeginSample("InspectionCanBeOperated");
            Grid tGrid = getGrid(nLine, nColumn);
            if (tGrid == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }
            if (ENateMatchRule.sm_tMatch_ForbiddenMove.Grid_match(tGrid) == true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return true;
        }

        public class DetectTwoElementsExchange
        {
            public Element m_arrElement1;
            public Element m_arrElement2;
            public List<string> m_arrSkillId;

            public DetectTwoElementsExchange(Element arrElement1,
                Element arrElement2,
                List<string> arrSkillId)
            {
                m_arrElement1 = arrElement1;
                m_arrElement2 = arrElement2;
                m_arrSkillId = arrSkillId;
            }

            public void excute(Stage tStage)
            {
                UnityEngine.Profiling.Profiler.BeginSample("excute");
                Grid tSrcGrid = m_arrElement1.m_tGrid;
                Grid tAimGrid = m_arrElement2.m_tGrid;

                Action pEliminateTransformGrid = () =>
                {
                    if (tSrcGrid.getElementWithElementAttribute(ElementAttribute.Attribute.type, 1) != null ||
                        tAimGrid.getElementWithElementAttribute(ElementAttribute.Attribute.type, 1) != null)
                    {
                        ElementContainer arrPassElement = new ElementContainer();
                        ElementDestroy tPassElementDestroy = new ElementDestroy();
                        var tPassDestroy = tSrcGrid.checkPassDestroy();
                        var tAimPassDestroy = tSrcGrid.checkPassDestroy();
                        tPassElementDestroy.addDestroyType(tPassDestroy);
                        tPassElementDestroy.addDestroyType(tAimPassDestroy);
                        EliminateRules.eliminateGrid(tSrcGrid, tPassElementDestroy, arrPassElement);
                        EliminateRules.eliminateGrid(tAimGrid, tPassElementDestroy, arrPassElement);
                        ElementCreater.DestroyInfo tDestroyInfo = tStage.m_tElementCreater.createDestroyInfo(arrPassElement);
                        tDestroyInfo.destroyElement();
                    }
                };
                pEliminateTransformGrid();
                List<Element> arrElement = new List<Element>();
                arrElement.Add(m_arrElement2);
                jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE_ROUND, arrElement);
                tStage.m_tElementCreater.removeElement(m_arrElement1);
                tStage.m_tElementCreater.removeElement(m_arrElement2);
                tStage.m_tSkillManager.excuteSkill(tStage.CurrentChessBoard, m_arrSkillId, tAimGrid.m_tGridCoord, null);
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        public DetectTwoElementsExchange abilityToDetectTwoElementsExchange(int nSrcLine, int nSrcColumn, int nAimLine, int nAimColumn)
        {
            UnityEngine.Profiling.Profiler.BeginSample("abilityToDetectTwoElementsExchange");
            Grid tSrcGrid = getGrid(nSrcLine, nSrcColumn);
            Grid tAimGrid = getGrid(nAimLine, nAimColumn);
            List<string> arrSkillId = new List<string>();
            if (tSrcGrid == null || tAimGrid == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return null;
            }
            List<Element> arrElement1 = tSrcGrid.getExchangeMoveElement(false);
            List<Element> arrElement2 = tAimGrid.getExchangeMoveElement(false);
            foreach (var tElement1 in arrElement1)
            {
                foreach (var tElement2 in arrElement2)
                {
                    if (getHypotaxisExchangeSkillId(tElement1.getHypotaxisId(), tElement2.getHypotaxisId(), arrSkillId))
                    {
                        UnityEngine.Profiling.Profiler.EndSample();
                        return new DetectTwoElementsExchange(tElement1, tElement2, arrSkillId);
                    }
                    if (getExchangeSkillId(tElement1.ElementId, tElement2.ElementId, arrSkillId))
                    {
                        UnityEngine.Profiling.Profiler.EndSample();
                        return new DetectTwoElementsExchange(tElement1, tElement2, arrSkillId);
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return null;
        }
        public enum ExchangeCheckOver
        {
            none,
            useStep,
            exchange
        }
        ExchangeCheckOver exchangeGridNormalCheck(int nSrcLine, int nSrcColumn, int nAimLine, int nAimColumn)
        {
            DetectTwoElementsExchange tDetectTwoElementsExchange = abilityToDetectTwoElementsExchange(nSrcLine, nSrcColumn, nAimLine, nAimColumn);
            if (tDetectTwoElementsExchange != null)
            {
                tDetectTwoElementsExchange.excute(m_tStage);
                return ExchangeCheckOver.useStep;
            }
            ExchangeAndSyntheizeElements tExchangeAndSyntheizeElements = exchangeAndSynthesizeElementsCheck(nSrcLine, nSrcColumn, nAimLine, nAimColumn);
            if (tExchangeAndSyntheizeElements != null)
            {
                tExchangeAndSyntheizeElements.excute(m_tStage);
                return ExchangeCheckOver.useStep;
            }
            return ExchangeCheckOver.exchange;
        }

        ExchangeCheckOver exchangeGridFeverCheck(int nSrcLine, int nSrcColumn, int nAimLine, int nAimColumn)
        {
            StageRunStatue_Fever tStageRunStatue_Fever = m_tStage.CurrentStageRun as StageRunStatue_Fever;
            if (tStageRunStatue_Fever == null)
            {
                return ExchangeCheckOver.exchange;
            }
            tStageRunStatue_Fever.addOperation(m_tStage, GridCoord.posToCoord(Index, nSrcLine, nSrcColumn), GridCoord.posToCoord(Index, nAimLine, nAimColumn));
            return ExchangeCheckOver.none;
        }

        IEnumerator exchangeGridWait(int nSrcLine, int nSrcColumn, int nAimLine, int nAimColumn, Func<int, int, int, int, ExchangeCheckOver> pExchangeCallBack)
        {
            yield return null;
            // 先交换元素 
            // 检测
            // 失败再交换回来
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_Handle, this);
            if (InspectionCanBeOperated(nSrcLine, nSrcColumn) == false || InspectionCanBeOperated(nAimLine, nAimColumn) == false)
            {
                yield break;
            }
            bool bIsSucceed = m_tDropManager.exChangeMoveCheck(nSrcLine, nSrcColumn, nAimLine, nAimColumn);
            if (bIsSucceed == false)
            {
                yield break;
            }
            List<int> arrDropUnitId = new List<int>();
            m_tDropManager.exChangeMove(nSrcLine, nSrcColumn, nAimLine, nAimColumn, arrDropUnitId);
            bool bIsOk = false;
            while (bIsOk == false)
            {
                bIsOk = true;
                foreach (int nDropUnitId in arrDropUnitId)
                {
                    if (m_tDropManager.doesTheDropEnd(nDropUnitId) == false)
                    {
                        bIsOk = false;
                    }
                }
                yield return null;
            }
            ExchangeCheckOver eExchangeCheckOver = pExchangeCallBack(nSrcLine, nSrcColumn, nAimLine, nAimColumn);
            if (eExchangeCheckOver == ExchangeCheckOver.useStep)
            {
                checkInBasketType(new GridCoord(Index, nSrcLine, nSrcColumn));
                checkInBasketType(new GridCoord(Index, nAimLine, nAimColumn));
                m_tStage.useStep();
            }
            else if (eExchangeCheckOver == ExchangeCheckOver.exchange)
            {
                m_tDropManager.exChangeMove(nSrcLine, nSrcColumn, nAimLine, nAimColumn, null);
            }
        }

        public IEnumerator exchangeGrid(int nSrcLine, int nSrcColumn, int nAimLine, int nAimColumn, Action pCallBack = null)
        {
            if (m_tStage.bIsLock == true)
            {
                if (pCallBack != null) pCallBack();
                yield break;
            }
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_TIPS_Hide);
            if (m_tStage.CurrentStageRunningStatus == StageRunningStatus.Fever)
            {
                yield return StartCoroutine(exchangeGridWait(nSrcLine, nSrcColumn, nAimLine, nAimColumn, exchangeGridFeverCheck));
            }
            else
            {
                yield return StartCoroutine(exchangeGridWait(nSrcLine, nSrcColumn, nAimLine, nAimColumn, exchangeGridNormalCheck));
            }
            if (pCallBack != null) pCallBack();
        }
        public ElementContainer doubleClickGridCheck(int nLine, int nColumn)
        {
            if (InspectionCanBeOperated(nLine, nColumn) == false)
            {
                return null;
            }
            Grid tGrid = getGrid(nLine, nColumn);
            ElementDestroy tElementDestroy = new ElementDestroy();
            tElementDestroy.addDestroyType(36);
            ElementContainer arrElement = new ElementContainer();
            List<Grid> arrGrid = new List<Grid>();
            arrGrid.Add(tGrid);
            EliminateRules.eliminateGrid(arrGrid, tElementDestroy, arrElement);
            return arrElement;
        }
        public void doubleClickGridNormal(int nLine, int nColumn)
        {
            var tElementContainer = doubleClickGridCheck(nLine, nColumn);
            if (tElementContainer == null)
            {
                return;
            }
            ElementCreater.DestroyInfo tDestroyInfo = m_tStage.m_tElementCreater.createDestroyInfo(tElementContainer);
            tDestroyInfo.destroyElement();
            if (tElementContainer.Count > 0)
            {
                m_tStage.useStep();
            }
        }
        public void doubleClickGridFever(int nLine, int nColumn)
        {
            StageRunStatue_Fever tStageRunStatue_Fever = m_tStage.CurrentStageRun as StageRunStatue_Fever;
            if (tStageRunStatue_Fever == null)
            {
                return;
            }
            if (InspectionCanBeOperated(nLine, nColumn) == false)
            {
                return;
            }
            tStageRunStatue_Fever.addOperation(m_tStage, GridCoord.posToCoord(Index, nLine, nColumn));
        }

        public void doubleClickGrid(int nLine, int nColumn)
        {
            if (m_tStage.bIsLock == true)
            {
                return;
            }
            if (m_tStage.CurrentStageRunningStatus == StageRunningStatus.Fever)
            {
                doubleClickGridFever(nLine, nColumn);
            }
            else
            {
                doubleClickGridNormal(nLine, nColumn);
            }
        }

        public bool isCalmness()
        {
            return m_tDropManager.isCalmness();
        }

        void checkInBasketType(GridCoord tGridCoord)
        {
            Grid tGrid = getGrid(tGridCoord);
            if (tGrid == null)
            {
                return;
            }
            ElementContainer arrDestroyElement = new ElementContainer();
            foreach (var itElement in tGrid.m_sortedElement)
            {
                ElementValue<string> tElementValue = itElement.Value.getElementAttribute(ElementAttribute.Attribute.inBasketType) as ElementValue<string>;
                if (tElementValue == null)
                {
                    continue;
                }
                var tInBasketElement = tGrid.getElementWithElementAttribute(ElementAttribute.Attribute.id, tElementValue);
                if (tInBasketElement != null)
                {
                    arrDestroyElement.Add(itElement.Value);
                }
            }
            ElementCreater.DestroyInfo tDestroyInfo = m_tStage.m_tElementCreater.createDestroyInfo(arrDestroyElement);
            tDestroyInfo.destroyElement();

        }

        /**
         * @Author: yangzijian
         * @description: 
         * @param {type} 
         * @return: bool is detecting done
         */
        public bool detectingPoint()
        {
            UnityEngine.Profiling.Profiler.BeginSample("detectingPoint");
            bool bIsDetectingGrid = arrDetectingPoint.Count > 0;
            // 检测合成点的时候, 这时候要把所有的 generator 状态的方块置为normal
            List<GridCoord> arrResetDetectingPoint = new List<GridCoord>();
            List<GridCoord> arrRDetectingPoint = new List<GridCoord>();
            arrRDetectingPoint.AddRange(arrDetectingPoint);
            arrDetectingPoint.Clear();
            foreach (var tGridCoord in arrRDetectingPoint)
            {
                if (collectAndSynthesizeElements(tGridCoord.Line, tGridCoord.Col) == true)
                {
                    arrResetDetectingPoint.Add(tGridCoord);
                }
                checkInBasketType(tGridCoord);
            }
            arrDetectingPoint.AddRange(arrResetDetectingPoint);
            UnityEngine.Profiling.Profiler.EndSample();
            return bIsDetectingGrid;
        }

        public void createDropDevice(int nLine, int nCol, DropDevice tDropDevice, JsonData.DropConfig.DropNode tDropNode)
        {
            UnityEngine.Profiling.Profiler.BeginSample("createDropDevice");
            m_tDropManager.addDropDevice(nLine, nCol, tDropDevice);
            try
            {
                if (int.Parse(tDropNode.m_bIsShowSkin) == 0)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return;
                }
            }
            catch
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            var tDropGameObject = jc.ResourceManager.Instance.LoadPrefab(JsonManager.dropconfig.root.game.prefab, null, false);
            if (tDropGameObject == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            tDropGameObject.attachObj(m_tDropPanel.gameObject);
            tDropGameObject.setImage(tDropNode.skin);
            Vector2 tVector2 = MoveUnitUtil.getPositionWithLineCol(nLine, nCol);
            RectTransform tRectTransform = tDropGameObject.GetComponent<RectTransform>();
            tRectTransform.anchoredPosition = tVector2;
            UnityEngine.Profiling.Profiler.EndSample();
        }

        void event_ClearRound(object o)
        {
            m_tDropManager.clearOpenPortalMark();
        }

        void event_elementChange(object o)
        {
            KeyValuePair<string, Element> tKeyPair = (KeyValuePair<string, Element>) o;
            if (tKeyPair.Value.m_tGrid != null)
            {
                addCheckPoint(tKeyPair.Value.m_tGrid.m_tGridCoord.Line, tKeyPair.Value.m_tGrid.m_tGridCoord.Col);
            }
        }

        void event_stopDrop(object o)
        {
            var nOperatorId = (int) o;
            m_tDropManager.stop(nOperatorId);
        }
        void event_openDrop(object o)
        {
            var nOperatorId = (int) o;
            m_tDropManager.open(nOperatorId);
        }
    }

}