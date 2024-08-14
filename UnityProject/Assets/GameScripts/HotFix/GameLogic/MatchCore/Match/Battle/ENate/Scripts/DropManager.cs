/*
 * @Description: Drop manager
 * @Author: yangzijian
 * @Date: 2019-12-23 14:21:17
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-08-07 11:56:38
 */
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    public class DropDeviceUnit
    {
        public List<string> m_arrElementId = new List<string>();
        public int m_nPower;
        public DropDeviceUnit(List<string> arrElementId, int nPower)
        {
            m_arrElementId = arrElementId;
            m_nPower = nPower;
        }

    }

    public class DropStrategy
    {
        ENateRandom m_tRandom;
        public List<DropDeviceUnit> m_arrDropDeviceUnit = new List<DropDeviceUnit>();
        Stage m_tStage;

        public DropStrategy(Stage tStage, ENateRandom tRandom)
        {
            m_tRandom = tRandom;
            m_tStage = tStage;
        }

        public void addDeviceUnit(DropDeviceUnit tDropDeviceUnit)
        {
            m_arrDropDeviceUnit.Add(tDropDeviceUnit);
        }
        public DropDeviceUnit randomDropUnit(int nChessBoardIndex, int nLine, int nColumn)
        {
            long lPower = 0;
            List<DropDeviceUnit> arrDropDeviceUnit = new List<DropDeviceUnit>();
            foreach (var tDropDeviceUnit in m_arrDropDeviceUnit)
            {
                bool bIsOk = true;
                foreach (var strElementId in tDropDeviceUnit.m_arrElementId)
                {
                    if (checkRandomElement_CanGenerate(m_tStage, nChessBoardIndex, nLine, nColumn, strElementId) == false)
                    {
                        bIsOk = false;
                        break;
                    }
                    if (checkRandomElement_whetherTooLittle(m_tStage, nChessBoardIndex, strElementId) == true)
                    {
                        return tDropDeviceUnit; // if the element is too little , return this DropDeviceUnit.
                    }

                }
                if (bIsOk == true)
                {
                    arrDropDeviceUnit.Add(tDropDeviceUnit);
                    lPower += tDropDeviceUnit.m_nPower;
                }
            }
            long lRandomPower = m_tRandom.random(0, lPower);
            foreach (var tDropDeviceUnit in arrDropDeviceUnit)
            {
                lRandomPower -= tDropDeviceUnit.m_nPower;
                if (lRandomPower <= 0)
                {
                    return tDropDeviceUnit;
                }
            }
            return null;
        }
        /*
            以下规则按照优先级排序：
            1.当“最小存在数”大于“最大同时存在数”时，该掉落规则不生效
            2.当该元素在本关卡中累积（原始存在+已经掉落）的数量大于等于“总数量”时，该元素不可能再掉落，“总数量”不配置或配置为0时，默认为不限总数量
            3.当该元素在当前棋盘上同时存在的数量大于等于“最大同时存在数”时，不再掉落该元素
            4.当该元素在当前棋盘上同时存在的数量小于“最小存在数”时，元素按照100%掉落
            5.当该元素在当前棋盘上同时存在的数量大于等于“最小存在数”并且小于“最大存在数”时，该全局掉落规则不做特殊影响
            6.在上回合消除过特殊规则配置的元素， 这回合全局掉落规则不做特殊影响。
            http://jira.inner.egls.cn/browse/XBX-185
        */

        public static bool checkRandomElement_CanGenerate(Stage tStage, int nChessBoardIndex, int nLine, int nColumn, string strElementId)
        {
            bool bIsOk = false;
            ChessBoard pChessBoard = tStage.getChessBoardWithIndex(nChessBoardIndex);
            do
            {
                if (pChessBoard == null)
                {
                    break;
                }
                Grid tGrid = pChessBoard.getGrid(nLine, nColumn);
                if (tGrid == null)
                {
                    break;
                }
                JsonData.Element_Config.Element tConfigElement = Config.ElementConfig.getConfig_element(strElementId);
                if (tConfigElement == null)
                {
                    break;
                }
                if (checkRandomElement_whetherTooMuch(tStage, nChessBoardIndex, strElementId) == true)
                {
                    break;
                }
                int eColor = -1;
                try
                {
                    eColor = int.Parse(tConfigElement.color);
                }
                catch (System.Exception) { }
                if (eColor == -1)
                {
                    break;
                }
                ENateMatch tENateMatch = new ENateMatch();
                tENateMatch.init(pChessBoard, tGrid, eColor);
                ENateMatchRule.sm_DropRandom_eMatchColor.Value = eColor;
                tENateMatch.traversedAroundGrid(pChessBoard, tGrid, ENateMatchRule.sm_tMatchRule_DropRandom);
                tENateMatch.m_mpCollectedGrid.Add(tGrid.m_tGridCoord.Coord, tGrid);
                ENateCompose tENateCompose = EliminateRules.compose(pChessBoard, tENateMatch.m_mpCollectedGrid, tENateMatch.m_tGrid);
                if (tENateCompose.m_arrComposeGrid.Count <= 0)
                {
                    bIsOk = true;
                }
            }
            while (false);
            return bIsOk;
        }
        public static bool checkRandomElement_whetherTooMuch(Stage tStage, int nChessBoardIndex, string strElementId)
        {
            bool bIsTooMuch = false;
            do
            {
                JsonData.Element_Config.Element tConfigElement = Config.ElementConfig.getConfig_element(strElementId);
                if (tConfigElement == null)
                {
                    break;
                }
                string strHypotaxisId = "";
                if (string.IsNullOrEmpty(tConfigElement.hypotaxis) == true)
                {
                    strHypotaxisId = strElementId;
                }
                else
                {
                    strHypotaxisId = tConfigElement.hypotaxis;
                }
                foreach (var tDropType in BattleArg.Instance.m_tStageArg.m_tMission.dropType)
                {
                    if (tDropType.hypotaxis == strHypotaxisId)
                    {
                        int nCount = tStage.m_tENateCollecter.getHypotaxisIdBlockCount(strHypotaxisId, nChessBoardIndex);
                        if (nCount >= int.Parse(tDropType.number.max))
                        {
                            bIsTooMuch = true;
                            break;
                        }
                        int nCollectNum = tStage.m_tENateCollecter.getCollectNum(strHypotaxisId);
                        if ((nCollectNum + nCount) >= int.Parse(tDropType.number.total))
                        {
                            bIsTooMuch = true;
                            break;
                        }
                    }
                }
            } while (false);

            return bIsTooMuch;
        }
        public static bool checkRandomElement_whetherTooLittle(Stage tStage, int nChessBoardIndex, string strElementId)
        {
            bool bIsTooLittle = false;
            do
            {
                JsonData.Element_Config.Element tConfigElement = Config.ElementConfig.getConfig_element(strElementId);
                if (tConfigElement == null)
                {
                    break;
                }
                string strHypotaxisId = "";
                if (string.IsNullOrEmpty(tConfigElement.hypotaxis) == true)
                {
                    strHypotaxisId = strElementId;
                }
                else
                {
                    strHypotaxisId = tConfigElement.hypotaxis;
                }
                foreach (var tDropType in BattleArg.Instance.m_tStageArg.m_tMission.dropType)
                {
                    if (tDropType.hypotaxis == strHypotaxisId)
                    {
                        int nCount = tStage.m_tENateCollecter.getHypotaxisIdBlockCount(strHypotaxisId, nChessBoardIndex);
                        if (nCount < int.Parse(tDropType.number.min))
                        {
                            bIsTooLittle = true;
                            break;
                        }
                    }
                }
            } while (false);
            return bIsTooLittle;
        }

    }
    /**
     * @Author: yangzijian
     * @description: drop device 
     */
    public class DropDevice : DropStrategy
    {
        DropManager m_tDropManager;
        public int m_nLine;
        public int m_nCol;
        public Grid m_tGrid;
        public ChessBoard m_tChessBoard;

        public DropDevice(Stage tStage, DropManager tDropManager, ENateRandom tRandom, int nLine, int nCol) : base(tStage, tRandom)
        {
            m_tDropManager = tDropManager;
            m_nLine = nLine;
            m_nCol = nCol;
            m_tChessBoard = tDropManager.tChessBoard;
            m_tGrid = m_tChessBoard.getGrid(nLine, nCol);
        }

        public bool ICanGenerateDrops()
        {
            do
            {
                Grid tGrid = m_tDropManager.tChessBoard.getGrid(m_nLine, m_nCol);
                if (tGrid == null || tGrid.detectElementStateNormal() == false)
                {
                    break;
                }
                if (m_tDropManager.isMarked(m_nLine, m_nCol))
                {
                    break;
                }
                // other grid
                if (ENateMatchRule.sm_tMatchRule_DropDevice.Grid_match(tGrid) == false)
                {
                    break;
                }
                return true;
            } while (false);
            return false;
        }

        /**
         * @Author: yangzijian
         * @description: Detects drops and generates drops.
         */
        public void check()
        {
            if (m_tDropManager.isCanPlay() == false || ICanGenerateDrops() == false)
            {
                return;
            }
            DropDeviceUnit tDropDeviceUnit = randomDropUnit(m_tDropManager.tChessBoard.Index, m_nLine, m_nCol);
            if (tDropDeviceUnit == null)
            {
                return;
            }
            List<Element> arrElement = new List<Element>();
            foreach (var strElementId in tDropDeviceUnit.m_arrElementId)
            {
                Element tElement = m_tDropManager.tChessBoard.m_tStage.m_tElementCreater.create(strElementId, m_tDropManager.tChessBoard.Index, m_nLine, m_nCol, false, false);
                arrElement.Add(tElement);
            }
            var tGenGridCoord = Util.getDirectionLineCol(m_tDropManager.tChessBoard.Index, m_nLine, m_nCol, Util.getDirectionOpposite(m_tGrid.DropDirection));
            m_tDropManager.createDropUnit(tGenGridCoord.Line, tGenGridCoord.Col, m_nLine, m_nCol, arrElement, DropUnit.MoveType.dropGenerate);
        }
    }
    /**
     * @Author: yangzijian
     * @description: drop direction.
     */
    public class DropManager
    {

        public DropManager(ChessBoard tChessBoard)
        {
            m_tChessBoard = tChessBoard;
            m_mpGridMarkers = new Dictionary<int, int>();
            m_mpGridMarkersSkill = new Dictionary<int, List<int>>();
            sm_tCounter = new Counter();
            m_arrDropUnit = new SortedDictionary<int, DropUnit>();
            m_mpDropDevice = new Dictionary<int, DropDevice>();
            m_arrStopDropControler = new List<int>();
        }
        public List<int> m_arrStopDropControler;
        public void stop(int nMarkOperatorId)
        {
            m_arrStopDropControler.Add(nMarkOperatorId);
        }

        public void open(int nMarkOperatorId)
        {
            m_arrStopDropControler.Remove(nMarkOperatorId);
        }

        public bool isCanPlay()
        {
            return m_arrStopDropControler.Count <= 0;
        }
        // open portal   
        public class OpenPortal
        {
            public Grid m_tGridIn;
            public Grid m_tGridOut;
        }
        // the element can cross open portal once in a round.
        public Dictionary<int, OpenPortal> m_mpOpenPortal;
        public Dictionary<int, OpenPortal> m_mpOutOpenPortalMark;

        public Dictionary<int, List<int>> m_mpRoundElementEnterOpenPortalMark;

        public void initOpenPortal()
        {
            m_mpOpenPortal = new Dictionary<int, OpenPortal>();
            m_mpOutOpenPortalMark = new Dictionary<int, OpenPortal>();
            m_mpRoundElementEnterOpenPortalMark = new Dictionary<int, List<int>>();

            List<KeyValuePair<Element, string>> arrInOpenPortalElement = new List<KeyValuePair<Element, string>>();
            List<Element> arrOutOpenPortalElement = new List<Element>();
            for (int nLine = m_tChessBoard.m_nHeight; nLine >= 0; nLine--)
            {
                for (int nCol = 0; nCol <= m_tChessBoard.m_nWidth; nCol++)
                {
                    var tGrid = m_tChessBoard.getGrid(nLine, nCol);
                    if (tGrid == null)
                    {
                        continue;
                    }
                    var tElement = tGrid.getElementWithElementAttribute(ElementAttribute.Attribute.type, 10);
                    if (tElement == null)
                    {
                        continue;
                    }
                    var tExitId = tElement.getElementAttribute(ElementAttribute.Attribute.exitid) as ElementValue<string>;

                    if (string.IsNullOrEmpty(tExitId.Value) == true)
                    {
                        arrOutOpenPortalElement.Add(tElement);
                    }
                    else
                    {
                        arrInOpenPortalElement.Add(new KeyValuePair<Element, string>(tElement, tExitId.Value));
                    }
                }
            }
            foreach (var itInElement in arrInOpenPortalElement)
            {
                foreach (var itOutElement in arrOutOpenPortalElement)
                {
                    if (itOutElement.ElementId == itInElement.Value)
                    {
                        m_mpOpenPortal.Add(itInElement.Key.m_tGrid.m_tGridCoord.Coord, new OpenPortal() { m_tGridIn = itInElement.Key.m_tGrid, m_tGridOut = itOutElement.m_tGrid });
                        m_mpOutOpenPortalMark.Add(itOutElement.m_tGrid.m_tGridCoord.Coord, new OpenPortal() { m_tGridIn = itInElement.Key.m_tGrid, m_tGridOut = itOutElement.m_tGrid });
                    }
                }
            }
        }

        public void clearOpenPortalMark()
        {
            m_mpRoundElementEnterOpenPortalMark.Clear();
        }

        void markElementEnterPortal(int nElementId, int nOpenPortalInCoord)
        {
            if (m_mpRoundElementEnterOpenPortalMark.ContainsKey(nElementId) == false)
            {
                m_mpRoundElementEnterOpenPortalMark.Add(nElementId, new List<int>());
            }
            m_mpRoundElementEnterOpenPortalMark[nElementId].Add(nOpenPortalInCoord);
        }

        bool isElementThisRoundEnter(int nElementId, int nOpenPortalInCoord)
        {
            if (m_mpRoundElementEnterOpenPortalMark.ContainsKey(nElementId) == false)
            {
                return false;
            }
            return m_mpRoundElementEnterOpenPortalMark[nElementId].Contains(nOpenPortalInCoord);
        }

        public OpenPortal getOpenPortalWithInCoord(int nInCoord)
        {
            if (m_mpOpenPortal.ContainsKey(nInCoord) == true)
            {
                return m_mpOpenPortal[nInCoord];
            }
            return null;
        }

        public OpenPortal getOpenPortalWithOutCoord(int nInCoord)
        {
            if (m_mpOutOpenPortalMark.ContainsKey(nInCoord) == true)
            {
                return m_mpOutOpenPortalMark[nInCoord];
            }
            return null;
        }

        private Direction m_tDropDirection = Direction.Down;
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
        private ChessBoard m_tChessBoard;
        public ChessBoard tChessBoard
        {
            get { return m_tChessBoard; }
        }
        public static Counter sm_tCounter;

        /**
         * @Author: yangzijian
         * @description: marks the dropped target point.
         */
        private Dictionary<int, int> m_mpGridMarkers;
        private Dictionary<int, List<int>> m_mpGridMarkersSkill;
        private SortedDictionary<int, DropUnit> m_arrDropUnit;
        private List<int> arrOverDropUnitId = new List<int>();
        private List<int> arrDelDropUnitId = new List<int>();
        private List<int> arrDropObliqueLineDropId = new List<int>();

        public Dictionary<int, DropDevice> m_mpDropDevice;

        bool bIsHaveUpDropDevice(int nLine, int nCol)
        {
            bool bIsOk = false;
            while (bIsOk == false)
            {
                Grid tGrid = m_tChessBoard.getGrid(nLine, nCol);
                if (tGrid == null)
                {
                    break;
                }
                if (getDropDevice(tGrid.m_tGridCoord.Coord) != null)
                {
                    bIsOk = true;
                    break;
                }
                if (getOpenPortalWithOutCoord(tGrid.m_tGridCoord.Coord) != null)
                {
                    bIsOk = true;
                    break;
                }
                GridCoord tGridCoord = Util.getDirectionLineCol(m_tChessBoard.Index, nLine, nCol, tGrid.DropDirection);
                nLine = tGridCoord.Line;
                nCol = tGridCoord.Col;
            }
            return true;
        }

        // 是否掉落方向无法掉落元素
        bool bIsDirectionFixWhile(GridCoord tGridCoord)
        {
            bool bIsOk = true;
            List<int> arrMarkOpenPortal = new List<int>();
            while (bIsOk == true)
            {
                Grid tGrid = m_tChessBoard.getGrid(tGridCoord);
                if (tGrid == null)
                {
                    bIsOk = false;
                    break;
                }
                if (getDropDevice(tGrid.m_tGridCoord.Coord) != null)
                {
                    break;
                }
                if (m_tChessBoard.ConnectChessBoard != null)
                {
                    bool bIsConnectCheck = true;
                    switch (m_tChessBoard.eConnectDirection)
                    {
                        case Direction.Up:
                            {
                                if (tGridCoord.Line == m_tChessBoard.m_nHeight - 1)
                                {
                                    bIsConnectCheck = false;
                                    break;
                                }
                            }
                            break;
                        case Direction.Down:
                            {
                                if (tGridCoord.Line == 0)
                                {
                                    bIsConnectCheck = false;
                                    break;
                                }
                            }
                            break;
                        case Direction.Left:
                            {
                                if (tGridCoord.Col == m_tChessBoard.m_nHeight - 1)
                                {
                                    bIsConnectCheck = false;
                                    break;
                                }
                            }
                            break;
                        case Direction.Right:
                            {
                                if (tGridCoord.Col == 0)
                                {
                                    bIsConnectCheck = false;
                                    break;
                                }
                            }
                            break;
                    }
                    if (bIsConnectCheck == false)
                    {
                        break;
                    }
                }
                var tOpenPortal = getOpenPortalWithOutCoord(tGrid.m_tGridCoord.Coord);
                if (tOpenPortal != null)
                {
                    if (arrMarkOpenPortal.Contains(tGrid.m_tGridCoord.Coord) == true)
                    {
                        bIsOk = false;
                        break;
                    }
                    tGridCoord = tOpenPortal.m_tGridIn.m_tGridCoord;
                    arrMarkOpenPortal.Add(tGrid.m_tGridCoord.Coord);
                }
                GridCoord tUpGridCoord = getDirectionLineCol(tGridCoord.ChessBoardIndex, tGridCoord.Line, tGridCoord.Col, Util.getDirectionOpposite(tGrid.DropDirection));
                if (tUpGridCoord.isNull() == false)
                {
                    var tUpGrid = m_tChessBoard.getGrid(tUpGridCoord);
                    if (tUpGrid == null)
                    {
                        bIsOk = false;
                        break;
                    }
                    if (checkTwoGridFixType(tUpGridCoord.Line, tUpGridCoord.Col, tGridCoord.Line, tGridCoord.Col) == false)
                    {
                        bIsOk = false;
                        break;
                    }
                    else if (ifIHaveMoveElemnt(tUpGridCoord.Line, tUpGridCoord.Col) == true)
                    {
                        break;
                    }
                    else if (ifIHaveCanNotMoveElemnt(tUpGridCoord.Line, tUpGridCoord.Col) == true)
                    {
                        bIsOk = false;
                        break;
                    }
                }
                tGridCoord = tUpGridCoord;
            }
            return bIsOk;
        }

        public bool isCalmness()
        {
            return m_arrDropUnit.Count <= 0;
        }

        public DropUnit getDropUnit(int nDropId)
        {
            if (m_arrDropUnit.ContainsKey(nDropId) == true)
            {
                return m_arrDropUnit[nDropId];
            }
            return null;
        }
        public void addDropDevice(int nLine, int nCol, DropDevice tDropDevice)
        {
            int nCoord = GridCoord.posToCoord(m_tChessBoard.Index, nLine, nCol);
            m_mpDropDevice.Add(nCoord, tDropDevice);
        }

        public DropDevice getDropDevice(int nLine, int nCol)
        {
            int nCoord = GridCoord.posToCoord(m_tChessBoard.Index, nLine, nCol);
            return getDropDevice(nCoord);
        }
        public DropDevice getDropDevice(int nCoord)
        {
            if (m_mpDropDevice.ContainsKey(nCoord) == true)
            {
                return m_mpDropDevice[nCoord];
            }
            return null;
        }

        public void markGrid(int nLine, int nCol, int nDropUnitId)
        {
            int nGridCoord = GridCoord.posToCoord(m_tChessBoard.Index, nLine, nCol);
            m_mpGridMarkers[nGridCoord] = nDropUnitId;
        }
        public void releaseGrid(int nLine, int nCol, int nDropUnitId)
        {
            int nGridCoord = GridCoord.posToCoord(m_tChessBoard.Index, nLine, nCol);
            if (m_mpGridMarkers[nGridCoord] != -1 && m_mpGridMarkers[nGridCoord] != nDropUnitId)
            {
                // console.log("release is not ok");
                return;
            }
            m_mpGridMarkers[nGridCoord] = -1;
        }

        public int getMarkedDropUnitId(int nLine, int nCol)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getMarkedDropUnitId");
            int nGridCoord = GridCoord.posToCoord(m_tChessBoard.Index, nLine, nCol);
            int nDropUnitId = -1;
            try
            {
                nDropUnitId = m_mpGridMarkers[nGridCoord];
            }
            catch (Exception ex) { }
            UnityEngine.Profiling.Profiler.EndSample();
            return nDropUnitId;
        }

        public bool isMarked(int nLine, int nCol)
        {
            if (getMarkedDropUnitId(nLine, nCol) != -1)
            {
                return true;
            }
            var arrMarkId = excute_getMarkedDropUnitId(nLine, nCol);
            if (arrMarkId != null && arrMarkId.Count > 0)
            {
                return true;
            }
            return false;
        }

        public void excute_markGrid(int nLine, int nCol, int nSkillOperatorId)
        {
            int nGridCoord = GridCoord.posToCoord(m_tChessBoard.Index, nLine, nCol);
            if (m_mpGridMarkersSkill.ContainsKey(nGridCoord) == false)
            {
                m_mpGridMarkersSkill[nGridCoord] = new List<int>();
            }
            m_mpGridMarkersSkill[nGridCoord].Add(nSkillOperatorId);
        }
        public void excute_releaseGrid(int nLine, int nCol, int nSkillOperatorId)
        {
            int nGridCoord = GridCoord.posToCoord(m_tChessBoard.Index, nLine, nCol);
            if (m_mpGridMarkersSkill.ContainsKey(nGridCoord) == false)
            {
                return;
            }
            if (m_mpGridMarkersSkill[nGridCoord].Contains(nSkillOperatorId) == false)
            {
                // console.log("release is not ok");
                return;
            }
            m_mpGridMarkersSkill[nGridCoord].Remove(nSkillOperatorId);
        }

        public List<int> excute_getMarkedDropUnitId(int nLine, int nCol)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getMarkedDropUnitId");
            int nGridCoord = GridCoord.posToCoord(m_tChessBoard.Index, nLine, nCol);
            if (m_mpGridMarkersSkill.ContainsKey(nGridCoord) == false)
            {
                return null;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return m_mpGridMarkersSkill[nGridCoord];
        }

        // 由其他的地方调用  fixUpdate 
        public void addDropUnit(DropUnit tDropUnit)
        {
            m_arrDropUnit.Add(tDropUnit.m_nID, tDropUnit);
        }
        /**
         * @Author: yangzijian
         * @description: Detect if i can move.
         * @param {int} nLine board of the line.
         * @param {int} nCol board of the column.
         * @return: whether
         */
        public bool ifICanMove(int nLine, int nCol)
        {
            UnityEngine.Profiling.Profiler.BeginSample("ifICanMove");
            bool bIsOk = false;
            do
            {
                if (isCanPlay() == false)
                {
                    break;
                }
                Grid tGrid = m_tChessBoard.getGrid(nLine, nCol);
                if (isMarked(nLine, nCol) == true)
                {
                    break;
                }
                if (tGrid == null || tGrid.detectElementStateNormal() == false)
                {
                    break;
                }
                if (ENateMatchRule.sm_tIfICanMoveMath.Grid_match(tGrid) == false)
                {
                    break;
                }
                bIsOk = true;
            } while (false);
            UnityEngine.Profiling.Profiler.EndSample();
            return bIsOk;
        }
        /**
         * @Author: yangzijian
         * @description: Detect if you can move to the target.
         * @param {int} nLine board of the line.
         * @param {int} nCol board of the column.
         * @return: whether
         */

        static bool checkTwoGridFixTypeStraight(Grid tSrcGrid, Grid tAimGrid, Direction eCheckFixTypeDirection)
        {
            if (tSrcGrid != null)
            {
                var eSrcFixType = Util.getDirectionFixType(eCheckFixTypeDirection, true);
                ENateMatchRule.sm_tCanIMoveThereMath_selfGridMatch.Value = eSrcFixType;
                if (ENateMatchRule.sm_tCanIMoveThereMath_selfGrid_FixType.Grid_match(tSrcGrid) == true)
                {
                    return false;
                }
            }
            if (tAimGrid != null)
            {
                var eAimFixType = Util.getDirectionFixType(eCheckFixTypeDirection, false);
                ENateMatchRule.sm_tCanIMoveThereMath_otherGridMatch.Value = eAimFixType;
                if (ENateMatchRule.sm_tCanIMoveThereMath_otherGrid_FixType.Grid_match(tAimGrid) == true)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool checkTwoGridFixType(Grid tSrcGrid, Grid tAimGrid, Direction eDirection)
        {
            if (ENateMatchRule.sm_tCanIMoveThereMath_selfGrid.Grid_match(tSrcGrid) == false)
            {
                return false;
            }
            if (ENateMatchRule.sm_tCanIMoveThereMath_otherGrid.Grid_match(tAimGrid) == false)
            {
                return false;
            }
            /*
             UpLeft = 1,
    Up = 2,
    UpRight = 3,
    Right = 4,
    DownRight = 5,
    Down = 6,
    DownLeft = 7,
    Left = 8,
*/
            switch (eDirection)
            {
                case Direction.Up:
                    {
                        return checkTwoGridFixTypeStraight(tSrcGrid, tAimGrid, Direction.Up);
                    }
                case Direction.Down:
                    {
                        return checkTwoGridFixTypeStraight(tSrcGrid, tAimGrid, Direction.Down);
                    }
                case Direction.Left:
                    {
                        return checkTwoGridFixTypeStraight(tSrcGrid, tAimGrid, Direction.Left);
                    }
                case Direction.Right:
                    {
                        return checkTwoGridFixTypeStraight(tSrcGrid, tAimGrid, Direction.Right);
                    }
                case Direction.UpLeft:
                    {
                        {
                            var tSubAimGrid = tSrcGrid.m_tChessBoard.getGrid(Util.getDirectionLineCol(ref tSrcGrid.m_tGridCoord, Direction.Up));
                            if (checkTwoGridFixTypeStraight(tSrcGrid, tSubAimGrid, Direction.Up) == true &&
                                checkTwoGridFixTypeStraight(tSubAimGrid, tAimGrid, Direction.Left) == true)
                            {
                                return true;
                            }
                        }
                        {
                            var tSubAimGrid = tSrcGrid.m_tChessBoard.getGrid(Util.getDirectionLineCol(ref tSrcGrid.m_tGridCoord, Direction.Left));
                            if (checkTwoGridFixTypeStraight(tSrcGrid, tSubAimGrid, Direction.Left) == true &&
                                checkTwoGridFixTypeStraight(tSubAimGrid, tAimGrid, Direction.Up) == true)
                            {
                                return true;
                            }
                        }
                    }
                    break;

                case Direction.UpRight:
                    {
                        {
                            var tSubAimGrid = tSrcGrid.m_tChessBoard.getGrid(Util.getDirectionLineCol(ref tSrcGrid.m_tGridCoord, Direction.Up));
                            if (checkTwoGridFixTypeStraight(tSrcGrid, tSubAimGrid, Direction.Up) == true &&
                                checkTwoGridFixTypeStraight(tSubAimGrid, tAimGrid, Direction.Right) == true)
                            {
                                return true;
                            }
                        }
                        {
                            var tSubAimGrid = tSrcGrid.m_tChessBoard.getGrid(Util.getDirectionLineCol(ref tSrcGrid.m_tGridCoord, Direction.Right));
                            if (checkTwoGridFixTypeStraight(tSrcGrid, tSubAimGrid, Direction.Right) == true &&
                                checkTwoGridFixTypeStraight(tSubAimGrid, tAimGrid, Direction.Up) == true)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case Direction.DownLeft:
                    {
                        {
                            var tSubAimGrid = tSrcGrid.m_tChessBoard.getGrid(Util.getDirectionLineCol(ref tSrcGrid.m_tGridCoord, Direction.Down));
                            if (checkTwoGridFixTypeStraight(tSrcGrid, tSubAimGrid, Direction.Down) == true &&
                                checkTwoGridFixTypeStraight(tSubAimGrid, tAimGrid, Direction.Left) == true)
                            {
                                return true;
                            }
                        }
                        {
                            var tSubAimGrid = tSrcGrid.m_tChessBoard.getGrid(Util.getDirectionLineCol(ref tSrcGrid.m_tGridCoord, Direction.Left));
                            if (checkTwoGridFixTypeStraight(tSrcGrid, tSubAimGrid, Direction.Left) == true &&
                                checkTwoGridFixTypeStraight(tSubAimGrid, tAimGrid, Direction.Down) == true)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case Direction.DownRight:
                    {
                        {
                            var tSubAimGrid = tSrcGrid.m_tChessBoard.getGrid(Util.getDirectionLineCol(ref tSrcGrid.m_tGridCoord, Direction.Down));
                            if (checkTwoGridFixTypeStraight(tSrcGrid, tSubAimGrid, Direction.Down) == true &&
                                checkTwoGridFixTypeStraight(tSubAimGrid, tAimGrid, Direction.Right) == true)
                            {
                                return true;
                            }
                        }
                        {
                            var tSubAimGrid = tSrcGrid.m_tChessBoard.getGrid(Util.getDirectionLineCol(ref tSrcGrid.m_tGridCoord, Direction.Right));
                            if (checkTwoGridFixTypeStraight(tSrcGrid, tSubAimGrid, Direction.Right) == true &&
                                checkTwoGridFixTypeStraight(tSubAimGrid, tAimGrid, Direction.Down) == true)
                            {
                                return true;
                            }
                        }
                    }
                    break;

            }
            return false;
        }
        public bool checkTwoGridFixType(int nSrcLine, int nSrcCol, int nDestLine, int nDestCol, Direction tDirection)
        {
            Grid tSrcGrid = m_tChessBoard.getGrid(nSrcLine, nSrcCol);
            Grid tDestGrid = m_tChessBoard.getGrid(nDestLine, nDestCol);
            return checkTwoGridFixType(tSrcGrid, tDestGrid, tDirection);
        }
        public bool checkTwoGridFixType(int nSrcLine, int nSrcCol, int nDestLine, int nDestCol)
        {
            Direction tDirection = Util.getDirectionWithLineCol(nSrcLine, nSrcCol, nDestLine, nDestCol);
            return checkTwoGridFixType(nSrcLine, nSrcCol, nDestLine, nDestCol, tDirection);
        }
        public bool canIMoveThere(int nSrcLine, int nSrcCol, int nDestLine, int nDestCol)
        {
            Direction tDirection = Util.getDirectionWithLineCol(nSrcLine, nSrcCol, nDestLine, nDestCol);
            return canIMoveThere(nSrcLine, nSrcCol, nDestLine, nDestCol, tDirection);
        }

        public bool canIMoveThere(int nSrcLine, int nSrcCol, int nDestLine, int nDestCol, Direction tDirection)
        {
            Grid tSrcGrid = m_tChessBoard.getGrid(nSrcLine, nSrcCol);
            Grid tDestGrid = m_tChessBoard.getGrid(nDestLine, nDestCol);
            return canIMoveThere(tSrcGrid, tDestGrid, tDirection);
        }
        public bool canIMoveThere(Grid tSrcGrid, Grid tDestGrid, Direction tDirection)
        {
            UnityEngine.Profiling.Profiler.BeginSample("canIMoveThere");
            bool bIsOk = false;
            do
            {
                if (tSrcGrid == null || tDestGrid == null)
                {
                    break;
                }
                if (isMarked(tDestGrid.m_tGridCoord.Line, tDestGrid.m_tGridCoord.Col) == true)
                {
                    break;
                }
                if (tSrcGrid.detectElementStateNormal() == false || tDestGrid.detectElementStateNormal() == false)
                {
                    break;
                }
                if (checkTwoGridFixType(tSrcGrid, tDestGrid, tDirection) == false)
                {
                    break;
                }
                bIsOk = true;
            } while (false);
            UnityEngine.Profiling.Profiler.EndSample();
            return bIsOk;
        }

        public bool ifICanExchange(int nLine, int nCol)
        {
            bool bIsOk = false;
            do
            {
                Grid tGrid = m_tChessBoard.getGrid(nLine, nCol);
                if (tGrid == null || tGrid.detectElementStateNormal() == false)
                {
                    break;
                }

                if (ENateMatchRule.sm_tIfICanExchangeMath.Grid_match(tGrid) == false)
                {
                    break;
                }
                bIsOk = true;
            } while (false);
            return bIsOk;
        }
        /**
         * @Author: yangzijian
         * @description: Detect if i can move.
         * @param {int} nLine board of the line.
         * @param {int} nCol board of the column.
         * @return: whether
         */
        public bool ifIHaveMoveElemnt(int nLine, int nCol)
        {
            UnityEngine.Profiling.Profiler.BeginSample("ifIHaveMoveElemnt");
            bool bIsOk = false;
            do
            {
                Grid tGrid = m_tChessBoard.getGrid(nLine, nCol);
                if (tGrid == null)
                {
                    break;
                }

                if (ENateMatchRule.sm_tIfIHaveMoveElemntMath.Grid_match(tGrid) == false)
                {
                    break;
                }
                bIsOk = true;
            } while (false);
            UnityEngine.Profiling.Profiler.EndSample();
            return bIsOk;
        }
        public bool ifIHaveCanNotMoveElemnt(int nLine, int nCol)
        {
            UnityEngine.Profiling.Profiler.BeginSample("ifIHaveCanNotMoveElemnt");
            bool bIsOk = false;
            do
            {
                Grid tGrid = m_tChessBoard.getGrid(nLine, nCol);
                if (tGrid == null)
                {
                    break;
                }
                if (ENateMatchRule.sm_tifIHaveCanNotMoveElemntMath.Grid_match(tGrid) == false)
                {
                    break;
                }
                bIsOk = true;
            } while (false);
            UnityEngine.Profiling.Profiler.EndSample();
            return bIsOk;
        }

        public int createDropUnit(int nSrcLine, int nSrcCol, int nDestLine, int nDestCol, List<Element> arrElement, DropUnit.MoveType eMoveType)
        {
            UnityEngine.Profiling.Profiler.BeginSample("createDropUnit");
            int nDropUnitId = sm_tCounter.count();
            string strShow = "";
            foreach (var tElement in arrElement)
            {
                strShow += tElement.ID;
            }
            //console.log("createDropUnit " + nSrcLine + " " + nSrcCol + " " + nDestLine + " " + nDestCol + " " + strShow);
            DropUnit tDropUnit = new DropUnit(this, nDropUnitId, nSrcLine, nSrcCol, nDestLine, nDestCol, arrElement, eMoveType);
            addDropUnit(tDropUnit);
            markGrid(nDestLine, nDestCol, nDropUnitId);
            UnityEngine.Profiling.Profiler.EndSample();
            return nDropUnitId;
        }

        void changeDropUnitAim(DropUnit tDropUnit, int nSrcLine, int nSrcCol, int nDestLine, int nDestCol)
        {
            Grid tGrid = m_tChessBoard.getGrid(tDropUnit.m_nAimLine, tDropUnit.m_nAimCol);
            foreach (var tElement in tDropUnit.m_arrElement)
            {
                tGrid.popElement(tElement);
            }
            releaseGrid(nSrcLine, nSrcCol, tDropUnit.m_nID);
            markGrid(nDestLine, nDestCol, tDropUnit.m_nID);
            tDropUnit.changeAim(nSrcLine, nSrcCol, nDestLine, nDestCol);
        }

        static public GridCoord getDirectionLineCol(int nChessBoardIndex, int nLine, int nCol, Direction eDirection)
        {
            GridCoord tGridCoord = ENate.Util.getDirectionLineCol(nChessBoardIndex, nLine, nCol, eDirection);
            if (tGridCoord.isNull() == true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return GridCoord.NULL;
            }
            return tGridCoord;
        }

        public GridCoord dropStraightLineCheck(int nLine, int nCol)
        {
            UnityEngine.Profiling.Profiler.BeginSample("dropStraightLineCheck");

            Grid tGrid = m_tChessBoard.getGrid(nLine, nCol);
            if (tGrid == null)
            {
                return GridCoord.NULL;
            }
            GridCoord tGridCoord = getDirectionLineCol(m_tChessBoard.Index, nLine, nCol, tGrid.DropDirection);
            if (tGridCoord.isNull() == true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return GridCoord.NULL;
            }
            if (canIMoveThere(nLine, nCol, tGridCoord.Line, tGridCoord.Col, tGrid.DropDirection) == false)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return GridCoord.NULL;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return tGridCoord;
        }

        List<Element> getMoveElement(int nLine, int nCol, bool bIsPop)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getMoveElement");
            List<Element> arrElement = new List<Element>();
            Grid tGrid = m_tChessBoard.getGrid(nLine, nCol);
            Element tElement = tGrid.getElementWithElementAttribute(ElementAttribute.Attribute.moveType, sm_tElementValueDefaultMoveType, bIsPop);
            if (tElement != null)
            {
                arrElement.Add(tElement);
            }
            tGrid.getElementWithElementAttributeArray(ElementAttribute.Attribute.followBasic, sm_tElementValueDefaultFollowBasic, ref arrElement, bIsPop);
            UnityEngine.Profiling.Profiler.EndSample();
            return arrElement;
        }

        void moveElement(int nSrcLine, int nSrcCol, int nAimLine, int nAimColumn, DropUnit.MoveType eMoveType)
        {
            // create dropunit
            UnityEngine.Profiling.Profiler.BeginSample("moveElement");
            var arrElement = getMoveElement(nSrcLine, nSrcCol, true);
            createDropUnit(nSrcLine, nSrcCol, nAimLine, nAimColumn, arrElement, eMoveType);
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public static ElementValue<int> sm_tElementValueDefaultMoveType = new ElementValue<int>(1);
        public static ElementValue<int> sm_tElementValueDefaultFollowBasic = new ElementValue<int>(1);
        public bool dropOpenPortal(int nLine, int nCol)
        {
            UnityEngine.Profiling.Profiler.BeginSample("dropOpenPortal");
            var tOpenPortal = getOpenPortalWithInCoord(GridCoord.posToCoord(m_tChessBoard.Index, nLine, nCol));
            if (tOpenPortal == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }
            if (canIMoveThere(tOpenPortal.m_tGridIn.m_tGridCoord.Line, tOpenPortal.m_tGridIn.m_tGridCoord.Col, tOpenPortal.m_tGridOut.m_tGridCoord.Line, tOpenPortal.m_tGridOut.m_tGridCoord.Col, tOpenPortal.m_tGridOut.DropDirection) == false)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }
            var arrElement = getMoveElement(nLine, nCol, false);
            var tGrid = m_tChessBoard.getGrid(nLine, nCol);
            foreach (var itElement in arrElement)
            {
                if (isElementThisRoundEnter(itElement.ID, tGrid.m_tGridCoord.Coord) == true)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return false;
                }
            }
            foreach (var itElement in arrElement)
            {
                markElementEnterPortal(itElement.ID, tGrid.m_tGridCoord.Coord);
            }
            moveElement(tOpenPortal.m_tGridIn.m_tGridCoord.Line, tOpenPortal.m_tGridIn.m_tGridCoord.Col, tOpenPortal.m_tGridOut.m_tGridCoord.Line, tOpenPortal.m_tGridOut.m_tGridCoord.Col, DropUnit.MoveType.openPortal);
            UnityEngine.Profiling.Profiler.EndSample();
            return true;
        }
        public bool dropStraightLine(int nLine, int nCol)
        {
            UnityEngine.Profiling.Profiler.BeginSample("dropStraightLine");

            GridCoord tGridCoord = dropStraightLineCheck(nLine, nCol);
            if (tGridCoord.isNull() == true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }
            moveElement(nLine, nCol, tGridCoord.Line, tGridCoord.Col, DropUnit.MoveType.normal);
            UnityEngine.Profiling.Profiler.EndSample();
            return true;
        }
        public GridCoord dropObliqueLineCheck(int nLine, int nCol)
        {
            UnityEngine.Profiling.Profiler.BeginSample("dropObliqueLineCheck");
            Grid tGrid = m_tChessBoard.getGrid(nLine, nCol);
            if (tGrid == null)
            {
                return GridCoord.NULL;
            }
            do
            {
                Direction eBranchDirection = Util.getDirectionBranch(tGrid.DropDirection, true);
                GridCoord tGridCoord = getDirectionLineCol(m_tChessBoard.Index, nLine, nCol, eBranchDirection);
                if (tGridCoord.isNull() == true)
                {
                    break;
                }
                if (canIMoveThere(nLine, nCol, tGridCoord.Line, tGridCoord.Col, eBranchDirection) == false)
                {
                    break;
                }

                // 满足的条件应该是： 
                if (bIsDirectionFixWhile(tGridCoord) == true)
                {
                    break;
                }
                // GridCoord tUpGridCoord = getDirectionLineCol(tGridCoord.ChessBoardIndex, tGridCoord.Line, tGridCoord.Col, Util.getDirectionOpposite(tGrid.DropDirection));
                // if (tUpGridCoord.isNull() == false)
                // {
                //     GridCoord tCurrentUpGridCoord = tUpGridCoord;
                //     GridCoord tCurrentGridCoord = tGridCoord;
                //     while (ifIHaveMoveElemnt(tCurrentUpGridCoord.Line, tCurrentUpGridCoord.Col) == false)
                //     {
                //         tCurrentGridCoord = tCurrentUpGridCoord;
                //         tCurrentUpGridCoord = getDirectionLineCol(tCurrentUpGridCoord.ChessBoardIndex, tCurrentUpGridCoord.Line, tCurrentUpGridCoord.Col, Util.getDirectionOpposite(DropDirection));
                //         if (m_tChessBoard.getGrid(tCurrentUpGridCoord) == null)
                //         {
                //             break;
                //         }
                //     }
                //     if (m_tChessBoard.getGrid(tCurrentUpGridCoord) == null)
                //     {
                //         break;
                //     }
                //     if (canIMoveThere(tCurrentUpGridCoord.Line, tCurrentUpGridCoord.Col, tCurrentGridCoord.Line, tCurrentGridCoord.Col) == true)
                //     {
                //         break;
                //     }
                // }
                UnityEngine.Profiling.Profiler.EndSample();
                return tGridCoord;
            }
            while (false);
            do
            {
                Direction eBranchDirection = Util.getDirectionBranch(tGrid.DropDirection, false);
                GridCoord tGridCoord = getDirectionLineCol(m_tChessBoard.Index, nLine, nCol, eBranchDirection);
                if (tGridCoord.isNull() == true)
                {
                    break;
                }
                if (canIMoveThere(nLine, nCol, tGridCoord.Line, tGridCoord.Col, eBranchDirection) == false)
                {
                    break;
                }
                // 满足的条件应该是： 
                // 当前上面有掉落器， 并且对应的 被挡住了， 才能斜着掉落， 如果没有掉落器则可以斜着掉落
                if (bIsDirectionFixWhile(tGridCoord) == true)
                {
                    break;
                }
                // GridCoord tUpGridCoord = getDirectionLineCol(tGridCoord.ChessBoardIndex, tGridCoord.Line, tGridCoord.Col, Util.getDirectionOpposite(tGrid.DropDirection));
                // if (tUpGridCoord.isNull() == false)
                // {
                //     GridCoord tCurrentUpGridCoord = tUpGridCoord;
                //     GridCoord tCurrentGridCoord = tGridCoord;
                //     while (ifIHaveMoveElemnt(tCurrentUpGridCoord.Line, tCurrentUpGridCoord.Col) == false)
                //     {
                //         tCurrentGridCoord = tCurrentUpGridCoord;
                //         tCurrentUpGridCoord = getDirectionLineCol(tCurrentUpGridCoord.ChessBoardIndex, tCurrentUpGridCoord.Line, tCurrentUpGridCoord.Col, Util.getDirectionOpposite(DropDirection));
                //         if (m_tChessBoard.getGrid(tCurrentUpGridCoord) == null)
                //         {
                //             break;
                //         }
                //     }
                //     if (m_tChessBoard.getGrid(tCurrentUpGridCoord) == null)
                //     {
                //         break;
                //     }
                //     if (canIMoveThere(tCurrentUpGridCoord.Line, tCurrentUpGridCoord.Col, tCurrentGridCoord.Line, tCurrentGridCoord.Col) == true)
                //     {
                //         break;
                //     }
                // }
                UnityEngine.Profiling.Profiler.EndSample();
                return tGridCoord;
            } while (false);
            UnityEngine.Profiling.Profiler.EndSample();
            return GridCoord.NULL;
        }

        public bool dropObliqueLine(int nLine, int nCol)
        {
            UnityEngine.Profiling.Profiler.BeginSample("dropObliqueLine");
            GridCoord tGridCoord = dropObliqueLineCheck(nLine, nCol);
            if (tGridCoord.isNull() == true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }
            moveElement(nLine, nCol, tGridCoord.Line, tGridCoord.Col, DropUnit.MoveType.normal);
            UnityEngine.Profiling.Profiler.EndSample();
            return true;
        }

        public bool exChangeMoveCheck(int nSrcLine, int nSrcColumn, int nAimLine, int nAimColumn)
        {
            UnityEngine.Profiling.Profiler.BeginSample("exChangeMoveCheck");
            // console.log(nSrcLine, nSrcColumn, nAimLine, nAimColumn);
            Grid tSrcGrid = m_tChessBoard.getGrid(nSrcLine, nSrcColumn);
            Grid tAimGrid = m_tChessBoard.getGrid(nAimLine, nAimColumn);
            if (tSrcGrid == null || tAimGrid == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }

            bool bIsCanMove1 = ifICanExchange(nSrcLine, nSrcColumn);
            bool bIsCanMove2 = ifICanExchange(nAimLine, nAimColumn);
            if (bIsCanMove1 != true || bIsCanMove2 != true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }
            Action<Grid, List<Element>> pAddElement = (Grid tAddGrid, List<Element> arrElement) =>
            {
                foreach (var tElement in arrElement)
                {
                    tAddGrid.addElement(tElement);
                }
            };
            var arrElementMove1 = tSrcGrid.getExchangeMoveElement();
            var arrElementMove2 = tAimGrid.getExchangeMoveElement();
            bool bIsCanMoveThere1 = canIMoveThere(nSrcLine, nSrcColumn, nAimLine, nAimColumn);
            bool bIsCanMoveThere2 = canIMoveThere(nAimLine, nAimColumn, nSrcLine, nSrcColumn);
            tSrcGrid.addElementArray(arrElementMove1);
            tAimGrid.addElementArray(arrElementMove2);
            if (bIsCanMoveThere1 != true || bIsCanMoveThere2 != true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return false;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return true;
        }
        public void exChangeMove(int nSrcLine, int nSrcColumn, int nAimLine, int nAimColumn, List<int> arrDropUnitId)
        {
            UnityEngine.Profiling.Profiler.BeginSample("exChangeMove");
            Grid tSrcGrid = m_tChessBoard.getGrid(nSrcLine, nSrcColumn);
            Grid tAimGrid = m_tChessBoard.getGrid(nAimLine, nAimColumn);
            var arrElementMove1 = tSrcGrid.getExchangeMoveElement();
            var arrElementMove2 = tAimGrid.getExchangeMoveElement();
            //console.log("exChangeMove : " + nSrcLine + ", " + nSrcColumn + " aim " + nAimLine + ", " + nAimColumn);
            var eMoveDirection = Util.getDirectionWithLineCol(nSrcLine, nSrcColumn, nAimLine, nAimColumn);
            string strFrontAniId = Util.getExchangeAniId(true, eMoveDirection);
            string strBackAniId = Util.getExchangeAniId(false, Util.getDirectionOpposite(eMoveDirection));
            foreach (var tElement in arrElementMove1)
            {
                tElement.transform.SetAsLastSibling();
                //console.log(tElement.ElementId, tElement.ID);
                tElement.playAniWithBehaviorId(strFrontAniId);
            }
            //console.log("aim");
            foreach (var tElement in arrElementMove2)
            {
                tElement.transform.SetAsFirstSibling();
                tElement.playAniWithBehaviorId(strBackAniId);
                //console.log(tElement.ElementId, tElement.ID);
            }
            int nDropUnitId1 = createDropUnit(nSrcLine, nSrcColumn, nAimLine, nAimColumn, arrElementMove1, DropUnit.MoveType.exchange);
            int nDropUnitId2 = createDropUnit(nAimLine, nAimColumn, nSrcLine, nSrcColumn, arrElementMove2, DropUnit.MoveType.exchange);
            if (arrDropUnitId != null)
            {
                arrDropUnitId.Add(nDropUnitId1);
                arrDropUnitId.Add(nDropUnitId2);
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void updateDropDevice()
        {
            foreach (var tDropIt in m_mpDropDevice)
            {
                tDropIt.Value.check();
            }
        }

        public void updateMove()
        {
            arrOverDropUnitId.Clear();
            arrDelDropUnitId.Clear();
            arrDropObliqueLineDropId.Clear();
            foreach (var itDropUnit in m_arrDropUnit)
            {
                var tDropUnit = itDropUnit.Value;
                bool bIsMoveOver = tDropUnit.move(Time.deltaTime);
                if (bIsMoveOver)
                {
                    arrOverDropUnitId.Add(itDropUnit.Key);
                }
            }
            foreach (var nDropUnitId in arrOverDropUnitId)
            {
                DropUnit tDropUnit = m_arrDropUnit[nDropUnitId];
                GridCoord tGridCoord = dropStraightLineCheck(tDropUnit.m_nAimLine, tDropUnit.m_nAimCol);
                if (tGridCoord.isNull() == true || isCanPlay() == false)
                {
                    arrDropObliqueLineDropId.Add(nDropUnitId);
                    continue;
                }
                changeDropUnitAim(tDropUnit, tDropUnit.m_nAimLine, tDropUnit.m_nAimCol, tGridCoord.Line, tGridCoord.Col);
            }
            foreach (var nDropUnitId in arrDropObliqueLineDropId)
            {
                DropUnit tDropUnit = m_arrDropUnit[nDropUnitId];
                GridCoord tGridCoord = dropObliqueLineCheck(tDropUnit.m_nAimLine, tDropUnit.m_nAimCol);
                if (tGridCoord.isNull() == true || isCanPlay() == false)
                {
                    arrDelDropUnitId.Add(nDropUnitId);
                    continue;
                }
                changeDropUnitAim(tDropUnit, tDropUnit.m_nAimLine, tDropUnit.m_nAimCol, tGridCoord.Line, tGridCoord.Col);
            }
            foreach (var nDropUnitId in arrDelDropUnitId)
            {
                DropUnit tDropUnit = m_arrDropUnit[nDropUnitId];
                Grid tGrid = m_tChessBoard.getGrid(tDropUnit.m_nAimLine, tDropUnit.m_nAimCol);
                releaseGrid(tGrid.m_tGridCoord.Line, tGrid.m_tGridCoord.Col, nDropUnitId);
                foreach (var tElement in tDropUnit.m_arrElement)
                {
                    tGrid.addElement(tElement);
                }
                if (tDropUnit.EMoveType != DropUnit.MoveType.exchange)
                {
                    m_tChessBoard.addCheckPoint(tDropUnit.m_nAimLine, tDropUnit.m_nAimCol);
                }
                tDropUnit.playEndAni();
                m_arrDropUnit.Remove(nDropUnitId);
            }
            if (arrDelDropUnitId.Count > 0)
            {
                jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_DROPDOWN);
            }
        }

        public bool doesTheDropEnd(int nDropUnitId)
        {
            return m_arrDropUnit.ContainsKey(nDropUnitId) == false;
        }
    }
}