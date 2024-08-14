/*
 * @Description: 
 * @Author: yangzijian
 * @Date: 2020-04-16 18:04:07
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-08-04 11:34:17
 */

using System;
using System.Collections;
using System.Collections.Generic;

static public class ConditionConfig
{

    ////////////////////////////////////////

    public class Reputation
    {
        static Dictionary<string, int> m_mpReputation = new Dictionary<string, int>();
        static public string getReputationId(string strReputation, ConditionConfig.MapArg mpArg)
        {
            if (strReputation == null)
            {
                return "";
            }

            string str = strReputation.Clone().ToString();
            try
            {
                if (mpArg.Element != null)
                {
                    str = str.Replace("{#ID}", mpArg.Element.ID.ToString());
                }
                if (mpArg.ChessBoard != null)
                {
                    str = str.Replace("{#ChI}", mpArg.ChessBoard.Index.ToString());
                }
                if (mpArg.Grid != null)
                {
                    str = str.Replace("{#GL}", mpArg.Grid.m_tGridCoord.Line.ToString());
                    str = str.Replace("{#GC}", mpArg.Grid.m_tGridCoord.Col.ToString());
                }
            }
            catch { }
            return str;
        }
        static Dictionary<string, KeyValuePair<int, int>> sm_mpReputationValue = new Dictionary<string, KeyValuePair<int, int>>();
        public static void initConfig()
        {
            foreach (var tGroup in JsonManager.reputation_config.root.game.group)
            {
                foreach (var tReputation in tGroup.reputation)
                {
                    sm_mpReputationValue.Add(tReputation.id, new KeyValuePair<int, int>(int.Parse(tReputation.min), int.Parse(tReputation.max)));
                }
            }
        }

        public static KeyValuePair<int, int> ? getReputationMinMax(string strReputationId)
        {
            if (sm_mpReputationValue.ContainsKey(strReputationId) == true)
            {
                return sm_mpReputationValue[strReputationId];
            }
            return null;
        }

        static public void clear()
        {
            m_mpReputation.Clear();
        }

        static public int get(string strKey)
        {
            try
            {
                return m_mpReputation[strKey];
            }
            catch
            {
                return 0;
            }
        }
        static public void set(string strKey, int nValue)
        {
            m_mpReputation[strKey] = nValue;
            //LogUtil.AddLog("config", "reputation:"); // .MoreStringFormat(strKey, " value:", nValue));
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_REPUTATION_CHANGE, new KeyValuePair<string, int>(strKey, nValue));
        }

        static public void set(string strSrcKey, string strSrcValue, ConditionConfig.MapArg mpArg)
        {
            string strId = ConditionConfig.Reputation.getReputationId(strSrcKey, mpArg);
            int nReputationValue = ConditionConfig.Reputation.get(strId);
            if (string.IsNullOrEmpty(strId) == true)
            {
                return;
            }
            var tMinMax = ConditionConfig.Reputation.getReputationMinMax(strSrcKey);
            int nComputeValue = -1;
            try
            {
                string strExpression = strSrcValue.Replace("$0", nReputationValue.ToString());
                var value = ExpressionEnate.Calculate.Compute(strExpression);
                string strValue = value.ToString();
                nComputeValue = int.Parse(strValue);
                if (tMinMax.HasValue == true)
                {
                    nComputeValue = tMinMax.Value.Key > nComputeValue ? tMinMax.Value.Key : nComputeValue;
                    nComputeValue = tMinMax.Value.Value < nComputeValue ? tMinMax.Value.Key : nComputeValue;
                }
                ConditionConfig.Reputation.set(strId, nComputeValue);
            }
            catch { }
        }

    }

    public class MapArg
    {
        Dictionary<string, object> m_mpArg = new Dictionary<string, object>();

        public bool ContainsKey(string strKey)
        {
            return m_mpArg.ContainsKey(strKey);
        }

        public object this [string key]
        {
            get
            {
                return m_mpArg[key];
            }
            set
            {
                m_mpArg[key] = value;
            }
        }

        //////////////////////////////////////

        public ENate.Element Element
        {
            set
            {
                m_mpArg["Element"] = value;

            }
            get
            {
                try
                {
                    return m_mpArg["Element"] as ENate.Element;
                }
                catch
                {
                    return null;
                }
            }
        }
        public ENate.Grid Grid
        {
            set
            {
                m_mpArg["Grid"] = value;

            }
            get
            {
                try
                {
                    return m_mpArg["Grid"] as ENate.Grid;
                }
                catch
                {
                    return null;
                }
            }
        }
        public ENate.ChessBoard ChessBoard
        {
            set
            {
                m_mpArg["ChessBoard"] = value;

            }
            get
            {
                try
                {
                    return m_mpArg["ChessBoard"] as ENate.ChessBoard;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ENate.Stage Stage
        {
            set
            {
                m_mpArg["Stage"] = value;

            }
            get
            {
                try
                {
                    return m_mpArg["Stage"] as ENate.Stage;
                }
                catch
                {
                    return null;
                }
            }
        }
        public ENate.GridCoord GridCoord
        {
            set
            {
                m_mpArg["GridCoord"] = value;
            }
            get
            {
                try
                {
                    return (ENate.GridCoord) m_mpArg["GridCoord"];
                }
                catch
                {
                    return ENate.GridCoord.NULL;
                }
            }
        }
        public Direction GridCoordDropDirection
        {
            set
            {
                m_mpArg["GridCoordDropDirection"] = value;
            }
            get
            {
                try
                {
                    return (Direction) m_mpArg["GridCoordDropDirection"];
                }
                catch
                {
                    return Direction.NULL;
                }
            }
        }

        public string Param
        {
            set
            {
                m_mpArg["Param"] = value;
            }
            get
            {
                try
                {
                    return m_mpArg["Param"] as string;
                }
                catch
                {
                    return "";
                }
            }
        }

    }
    ////////////////////////////////////////

    readonly static public MapArg m_tConditionArg = new MapArg();
    static Dictionary<string, JsonData.Client_Config.Condition> sm_mpConditionConfig;
    static readonly char smc_cParaConditionStart = '{';
    static readonly char smc_cParaConditionEnd = '}';
    static readonly char smc_cParaBeginTag = '#';
    static readonly char smc_cParaParaTag = ';';

    static readonly char smc_cParaFuncStart = '[';
    static readonly char smc_cParaFuncEnd = ']';

    static readonly char smc_cParaFuncSplitChar = '%';

    static public void initConfig()
    {
        sm_mpConditionConfig = new Dictionary<string, JsonData.Client_Config.Condition>();
        foreach (var tCondition in JsonManager.client_config.root.game.condition)
        {
            sm_mpConditionConfig.Add(tCondition.clientId, tCondition);
        }
    }

    static public JsonData.Client_Config.Condition getCondition(string strConditionID)
    {
        if (sm_mpConditionConfig.ContainsKey(strConditionID) == true)
        {
            var tSrcCondtidion = sm_mpConditionConfig[strConditionID];
            JsonData.Client_Config.Condition tCondition = new JsonData.Client_Config.Condition();
            tCondition.description = tSrcCondtidion.description;
            tCondition.clientId = tSrcCondtidion.clientId;
            tCondition.type = tSrcCondtidion.type;
            tCondition.para = tSrcCondtidion.para.cloneSelf();
            return tCondition;
        }
        return null;
    }

    static public List<string> splitPara(string strPara, char cSplit, char cSubBegin, char cSubEnd)
    {
        // String[]  arrArg = new String[];
        List<string> arrArg = new List<string>();
        int nSubIndex = 0;
        int nInCount = 0;
        for (int nIndex = 0; nIndex < strPara.Length; ++nIndex)
        {
            if (strPara[nIndex] == cSplit)
            {
                if (nInCount != 0)
                {
                    continue;
                }
                arrArg.Add(strPara.Substring(nSubIndex, nIndex - nSubIndex));
                nSubIndex = nIndex + 1;
            }
            else if (strPara[nIndex] == cSubBegin)
            {
                ++nInCount;
            }
            else if (strPara[nIndex] == cSubEnd)
            {
                --nInCount;
            }
        }
        if (nSubIndex < strPara.Length)
        {
            arrArg.Add(strPara.Substring(nSubIndex, strPara.Length - nSubIndex));
        }
        return arrArg;
    }

    static public JsonData.Client_Config.Condition parseCondition(JsonData.Client_Config.Condition tParent, string strPara)
    {
        if (string.IsNullOrEmpty(strPara) == true)
        {
            return null;
        }
        var tCondition = tParent.getConditionParsePara(strPara);
        if (tCondition != null)
        {
            return tCondition;
        }
        int nLeftIndex = strPara.IndexOf(smc_cParaConditionStart) + 1;
        int nRightIndex = strPara.LastIndexOf(smc_cParaConditionEnd);
        if (nLeftIndex >= nRightIndex)
        {
            return null;
        }
        string strSubCondition = strPara.Substring(nLeftIndex, nRightIndex - nLeftIndex);
        int nSubBeginIndex = strSubCondition.IndexOf(smc_cParaBeginTag);
        string strConditionID = "";
        List<string> strParaList = null;
        if (nSubBeginIndex == -1)
        {
            strConditionID = strSubCondition;
        }
        else
        {
            strConditionID = strSubCondition.Substring(0, nSubBeginIndex);
            nSubBeginIndex++;
            string strSubParaString = strSubCondition.Substring(nSubBeginIndex, strSubCondition.Length - nSubBeginIndex);
            strParaList = splitPara(strSubParaString, smc_cParaParaTag, smc_cParaConditionStart, smc_cParaConditionEnd);
        }
        tCondition = new JsonData.Client_Config.Condition();
        tCondition.para = new List<JsonData.Client_Config.Para>();
        tCondition.type = strConditionID;
        if (strParaList != null)
        {
            foreach (var strSubPara in strParaList)
            {
                tCondition.para.Add(new JsonData.Client_Config.Para() { text = strSubPara });
            }
        }
        tParent.markCondtion(strPara, tCondition);
        return tCondition;
    }

    static bool runParaCondition(JsonData.Client_Config.Condition tParentCondition, string strPara, MapArg mpArg)
    {
        var tCondition = parseCondition(tParentCondition, strPara);
        return checkCondition(tCondition, mpArg);
    }

    static string translatePara(string strPara, MapArg mpArg)
    {
        if (string.IsNullOrEmpty(strPara))
        {
            return strPara;
        }
        int nLeftIndex = strPara.IndexOf(smc_cParaFuncStart) + 1;
        int nRightIndex = strPara.LastIndexOf(smc_cParaFuncEnd);
        if (nLeftIndex >= nRightIndex)
        {
            return strPara;
        }
        string strBeginTag = strPara.Substring(0, nLeftIndex - 1);
        string strEndTag = strPara.Substring(nRightIndex + 1, strPara.Length - nRightIndex - 1);
        string strRe = strBeginTag;
        string strSub = strPara.Substring(nLeftIndex, nRightIndex - nLeftIndex);
        int nSubBeginIndex = strSub.IndexOf(smc_cParaFuncSplitChar);
        string strFuncId = "";
        string[] arrSubPara = new string[1];
        if (nSubBeginIndex == -1)
        {
            strFuncId = strSub;
        }
        else
        {
            strFuncId = strSub.Substring(0, nSubBeginIndex);
            nSubBeginIndex++;
            string strSubParaString = strSub.Substring(nSubBeginIndex, strSub.Length - nSubBeginIndex);
            string strSubPara = translatePara(strSubParaString, mpArg);
            arrSubPara = strSubPara.Split(smc_cParaFuncSplitChar);
        }
        switch (strFuncId)
        {
            case "getTriggerGridCoordElementId":
                {
                    int nLevel = int.Parse(arrSubPara[0]);
                    ENate.Grid tTriggerGrid = mpArg.ChessBoard.getGrid(mpArg.GridCoord);
                    ENate.Element tElement = tTriggerGrid.getElementWithElementAttribute(ENate.ElementAttribute.Attribute.level, nLevel);
                    if (tElement != null)
                    {
                        strRe += tElement.ElementId;
                    }
                }
                break;
            case "getTriggerGridCoordColor":
                {
                    int nLevel = int.Parse(arrSubPara[0]);
                    ENate.Grid tTriggerGrid = mpArg.ChessBoard.getGrid(mpArg.GridCoord);
                    ENate.Element tElement = tTriggerGrid.getElementWithElementAttribute(ENate.ElementAttribute.Attribute.level, nLevel);
                    if (tElement != null)
                    {
                        var tColorValue = tElement.getElementAttribute(ENate.ElementAttribute.Attribute.color) as ENate.ElementValue<int>;
                        if (tColorValue != null && tColorValue.Value != -1)
                        {
                            strRe += tColorValue.Value.ToString();
                        }
                    }
                }
                break;
            case "randomColor":
                {
                    List<int> arrRandomColor = new List<int>();
                    List<int> arrForbiddenColor = new List<int>();
                    foreach (var strSubParaColor in arrSubPara)
                    {
                        if (string.IsNullOrEmpty(strSubParaColor) == true)
                        {
                            continue;
                        }
                        arrForbiddenColor.Add(int.Parse(strSubParaColor));
                    }
                    foreach (var itElementCounter in mpArg.Stage.m_tENateCollecter.mpElementCount)
                    {
                        if (itElementCounter.Value.Count <= 0)
                        {
                            continue;
                        }
                        var tConfig = Config.ElementConfig.getConfig_element(itElementCounter.Key);
                        if (tConfig == null || string.IsNullOrEmpty(tConfig.color) == true)
                        {
                            continue;
                        }
                        int eColor = int.Parse(tConfig.color);
                        if (arrForbiddenColor.Contains(eColor) == true)
                        {
                            continue;
                        }
                        arrRandomColor.Add(eColor);
                    }
                    int nIndex = (int) ENate.Stage.m_tComputeRandom.random(0, arrRandomColor.Count);
                    strRe += arrRandomColor[nIndex].ToString();
                }
                break;
        }
        strRe += strEndTag;
        return strRe;
    }
    static string translatePara(JsonData.Client_Config.Para tPara, MapArg mpArg)
    {
        if (tPara == null)
        {
            return "";
        }
        return translatePara(tPara.text, mpArg);
    }

    static bool checkOr(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        foreach (var tPara in tCondition.para)
        {
            if (runParaCondition(tCondition, tPara.text, mpArg) == true)
            {
                return true;
            }
        }
        return tCondition.para.Count <= 0;
    }
    static bool checkAnd(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        foreach (var tPara in tCondition.para)
        {
            if (runParaCondition(tCondition, tPara.text, mpArg) == false)
            {
                return false;
            }
        }
        return true;
    }
    static bool checkNor(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        foreach (var tPara in tCondition.para)
        {
            if (runParaCondition(tCondition, tPara.text, mpArg) == false)
            {
                return true;
            }
            break;
        }
        return false;
    }

    static bool checkFitType(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        ENate.Grid tGrid = mpArg.Grid;
        if (tGrid == null)
        {
            return false;
        }
        foreach (var tPara in tCondition.para)
        {
            int nFitType = int.Parse(tPara.text);
            if (tGrid.getNormalElementWithElementAttribute(ENate.ElementAttribute.Attribute.type, nFitType) == null)
            {
                return false;
            }
        }
        return true;
    }

    static bool checkUnFitType(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        ENate.Grid tGrid = mpArg.Grid;
        if (tGrid == null)
        {
            return false;
        }
        foreach (var tPara in tCondition.para)
        {
            int nUnFitType = int.Parse(tPara.text);
            if (tGrid.getNormalElementWithElementAttribute(ENate.ElementAttribute.Attribute.type, nUnFitType) != null)
            {
                return false;
            }
        }
        return true;
    }
    static bool checkReputation(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        string strReputationId = tCondition.para[0].text;
        string strId = Reputation.getReputationId(strReputationId, mpArg);
        int nValue = Reputation.get(strId);
        int nMin = int.Parse(tCondition.para[1].text);
        int nMax = int.Parse(tCondition.para[2].text);
        //LogUtil.AddLog("config","checkReputation:"); // .MoreStringFormat(strId, ",nValue:", nValue, ",nMin:", nMin, ",nMax:", nMax));
        return nValue >= nMin && nValue <= nMax;
    }

    static bool checkColor(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        int eColor = -1;
        if (tCondition.para.Count > 1)
        {
            eColor = int.Parse(tCondition.para[1].text);
        }
        else
        {
            eColor = int.Parse(translatePara(tCondition.para[0], mpArg));
            tCondition.para.Add(new JsonData.Client_Config.Para() { text = eColor.ToString() });
        }
        ENate.Grid tGrid = mpArg.Grid;
        return tGrid.getElementWithElementAttribute(ENate.ElementAttribute.Attribute.color, eColor) != null;
    }

    static bool checkGenColor(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        string strElementId = mpArg.Param;
        var tConfig = Config.ElementConfig.getConfig_element(strElementId);
        var strSelGridColor = translatePara(tCondition.para[0], mpArg);
        return tConfig.color == strSelGridColor;
    }
    static bool checkRoundGrid(JsonData.Client_Config.Condition tCondition, MapArg mpArg, Direction eDirection)
    {
        if (tCondition.para.Count <= 0)
        {
            return true;
        }
        ENate.ChessBoard tChessBoard = mpArg.ChessBoard;
        if (tChessBoard == null)
        {
            return false;
        }
        ENate.Grid tCheckGrid = null;
        ENate.GridCoord tCheckSrc = ENate.GridCoord.NULL;
        if (mpArg.Grid != null)
        {
            tCheckSrc = mpArg.Grid.m_tGridCoord;
        }
        else if (mpArg.GridCoord.isNull() == false)
        {
            tCheckSrc = mpArg.GridCoord;
        }
        if (tCheckSrc.isNull() == true)
        {
            return false;
        }
        ENate.GridCoord tGridCoord = ENate.Util.getDirectionLineCol(tChessBoard.Index, tCheckSrc.Line, tCheckSrc.Col, eDirection);
        tCheckGrid = mpArg.ChessBoard.getGrid(tGridCoord);
        if (tCheckGrid == null)
        {
            return false;
        }
        MapArg tMapArg = new MapArg();
        tMapArg.Grid = tCheckGrid;
        tMapArg.ChessBoard = tChessBoard;
        tMapArg.Stage = mpArg.Stage;
        return runParaCondition(tCondition, tCondition.para[0].text, tMapArg);
    }

    static bool check_GrC_DD(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        ENate.Grid tGrid = mpArg.Grid;
        do
        {
            if (tGrid == null)
            {
                break;
            }
            ENate.Grid tCheckGrid = mpArg.ChessBoard.getGrid(ENate.Util.getDirectionLineCol(ref tGrid.m_tGridCoord, tGrid.DropDirection));
            if (tCheckGrid != null)
            {
                MapArg tMapArg = new MapArg();
                tMapArg.Grid = tCheckGrid;
                tMapArg.ChessBoard = mpArg.ChessBoard;
                tMapArg.Stage = mpArg.Stage;
                return runParaCondition(tCondition, tCondition.para[0].text, tMapArg);
            }
            return false;
        } while (false);
        return false;
    }
    static bool check_GrC_OD(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        ENate.Grid tGrid = mpArg.Grid;
        do
        {
            if (tGrid == null)
            {
                break;
            }
            Func<Direction, bool> pFunc_checkDirectionGrid = (Direction eDirection) =>
            {
                ENate.Grid tCheckGrid = mpArg.ChessBoard.getGrid(ENate.Util.getDirectionLineCol(ref tGrid.m_tGridCoord, eDirection));
                if (tCheckGrid != null && tCheckGrid.DropDirection == ENate.Util.getDirectionOpposite(eDirection))
                {
                    MapArg tMapArg = new MapArg();
                    tMapArg.Grid = tCheckGrid;
                    tMapArg.ChessBoard = mpArg.ChessBoard;
                    tMapArg.Stage = mpArg.Stage;
                    tMapArg.Element = mpArg.Element;
                    tMapArg.GridCoord = mpArg.GridCoord;
                    return runParaCondition(tCondition, tCondition.para[0].text, tMapArg);
                }
                return false;
            };
            if (pFunc_checkDirectionGrid(Direction.Up) == true ||
                pFunc_checkDirectionGrid(Direction.Down) == true ||
                pFunc_checkDirectionGrid(Direction.Left) == true ||
                pFunc_checkDirectionGrid(Direction.Right) == true
            )
            {
                return true;
            }
        } while (false);
        return false;
    }
    static bool checkIsTarget(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        var tStage = mpArg.Stage;
        foreach (var tElement in mpArg.Grid.m_sortedElement)
        {
            string strHypotaxisId = tElement.Value.getHypotaxisId();
            if (ENate.BattleArg.Instance.m_arrTargetNum.ContainsKey(strHypotaxisId) == true && ENate.BattleArg.Instance.m_arrTargetNum[strHypotaxisId] > 0)
            {
                return true;
            }

        }
        return false;
    }
    static bool check_Keji_DropDir(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        var tGrid = mpArg.Grid;
        do
        {
            if (tGrid == null)
            {
                break;
            }
            var eDirection = (Direction) int.Parse(tCondition.para[0].text);
            return eDirection == tGrid.DropDirection;
        } while (false);
        return false;
    }

    static bool check_DropDirDistance(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        var tGrid = mpArg.Grid;
        int nDis = int.Parse(tCondition.para[0].text);
        do
        {
            if (tGrid == null)
            {
                break;
            }
            switch (mpArg.GridCoordDropDirection)
            {
                case Direction.Up:
                    {
                        return tGrid.m_tGridCoord.Line > mpArg.GridCoord.Line + nDis;
                    }
                case Direction.Down:
                    {
                        return tGrid.m_tGridCoord.Line < mpArg.GridCoord.Line - nDis;
                    }
                case Direction.Left:
                    {
                        return tGrid.m_tGridCoord.Col > mpArg.GridCoord.Col + nDis;
                    }
                case Direction.Right:
                    {
                        return tGrid.m_tGridCoord.Col < mpArg.GridCoord.Col - nDis;
                    }
            }
        } while (false);
        return false;
    }
    static bool check_fitElementId(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        string strElementId = translatePara(tCondition.para[0], mpArg);
        var tElemnet = mpArg.Grid.getElementWithElementAttribute(ENate.ElementAttribute.Attribute.id, new ENate.ElementValue<string>(strElementId));
        return tElemnet != null;
    }
    static bool check_curGrid(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        return mpArg.ChessBoard.getGrid(mpArg.GridCoord) == mpArg.Grid;
    }

    static bool check_targetNumPercent(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        int nAllTargetNum = 0;
        if (ENate.BattleArg.Instance.m_arrTargetSrcNum != null)
        {
            foreach (var iter in ENate.BattleArg.Instance.m_arrTargetSrcNum)
            {
                nAllTargetNum += iter.Value;
            }
        }

        int nCurrentTargetNum = 0;
        if (ENate.BattleArg.Instance.m_arrTargetNum != null)
        {
            foreach (var iter in ENate.BattleArg.Instance.m_arrTargetNum)
            {
                nCurrentTargetNum += iter.Value;
            }
        }
        int nPercent = int.Parse(tCondition.para[0].text);
        return (nCurrentTargetNum * 100 / nAllTargetNum) <= nPercent;
    }
    static bool check_getClothes(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        bool bIsHave = int.Parse(tCondition.para[0].text) == 1;
        bool bIsEq = string.IsNullOrEmpty(Data.PlayerData.Instance.CurrentClothId) == false;
        return (bIsHave && bIsEq) || (!bIsHave && !bIsEq);
    }
    static public bool checkCondition(JsonData.Client_Config.Condition tCondition, MapArg mpArg)
    {
        if (tCondition == null)
        {
            return true;
        }
        try
        {
            switch (tCondition.type)
            {
                case "or":
                    {
                        return checkOr(tCondition, mpArg);
                    }
                case "and":
                    {
                        return checkAnd(tCondition, mpArg);
                    }
                case "nor":
                    {
                        return checkNor(tCondition, mpArg);
                    }
                case "fitType":
                    {
                        return checkFitType(tCondition, mpArg);
                    }
                case "unFitType":
                    {
                        return checkUnFitType(tCondition, mpArg);
                    }
                case "reputation":
                    {
                        return checkReputation(tCondition, mpArg);
                    }
                case "color":
                    {
                        return checkColor(tCondition, mpArg);
                    }
                case "genColor":
                    {
                        return checkGenColor(tCondition, mpArg);
                    }
                case "GrC_U":
                    {
                        return checkRoundGrid(tCondition, mpArg, Direction.Up);
                    }
                case "GrC_D":
                    {
                        return checkRoundGrid(tCondition, mpArg, Direction.Down);
                    }
                case "GrC_L":
                    {
                        return checkRoundGrid(tCondition, mpArg, Direction.Left);
                    }
                case "GrC_R":
                    {
                        return checkRoundGrid(tCondition, mpArg, Direction.Right);
                    }
                case "GrC_DD":
                    {
                        return check_GrC_DD(tCondition, mpArg);
                    }
                case "GrC_OD":
                    {
                        return check_GrC_OD(tCondition, mpArg);
                    }
                case "iGT":
                    {
                        return checkIsTarget(tCondition, mpArg);
                    }
                case "DropDir":
                    {
                        return check_Keji_DropDir(tCondition, mpArg);
                    }
                case "DropDirDistance":
                    {
                        return check_DropDirDistance(tCondition, mpArg);
                    }
                case "fitElementId":
                    {
                        return check_fitElementId(tCondition, mpArg);
                    }
                case "curGrid":
                    {
                        return check_curGrid(tCondition, mpArg);
                    }
                case "targetNumPercent":
                    {
                        return check_targetNumPercent(tCondition, mpArg);
                    }
                case "getClothes":
                    {
                        return check_getClothes(tCondition, mpArg);
                    }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError( ex.ToString());
        }
        return false;
    }

    static Dictionary<JsonData.Client_Config.Condition, Dictionary<string, JsonData.Client_Config.Condition>> mpCondition = new Dictionary<JsonData.Client_Config.Condition, Dictionary<string, JsonData.Client_Config.Condition>>();
    static void markCondtion(this JsonData.Client_Config.Condition tParent, string strPara, JsonData.Client_Config.Condition tSubSeed)
    {
        if (mpCondition.ContainsKey(tParent) == false)
        {
            mpCondition[tParent] = new Dictionary<string, JsonData.Client_Config.Condition>();
        }
        if (mpCondition[tParent].ContainsKey(strPara) == true)
        {
            return;
        }
        mpCondition[tParent].Add(strPara, tSubSeed);
    }

    static JsonData.Client_Config.Condition getConditionParsePara(this JsonData.Client_Config.Condition tParent, string strPara)
    {
        if (mpCondition.ContainsKey(tParent) == false)
        {
            return null;
        }
        if (string.IsNullOrEmpty(strPara) || mpCondition[tParent].ContainsKey(strPara) == false)
        {
            return null;
        }
        return mpCondition[tParent][strPara];
    }
    static void clearParaCondition(this JsonData.Client_Config.Condition tParent)
    {
        if (mpCondition.ContainsKey(tParent) == false)
        {
            return;
        }
        mpCondition[tParent].Clear();
    }

    static public bool checkCondition(string strConditionID, MapArg mpArg)
    {
        UnityEngine.Profiling.Profiler.BeginSample("checkCondition");
        if (string.IsNullOrEmpty(strConditionID) == true)
        {
            UnityEngine.Profiling.Profiler.EndSample();
            return true;
        }
        if (mpArg == null) mpArg = m_tConditionArg;
        var tCondition = getCondition(strConditionID);
        do
        {

            return checkCondition(tCondition, mpArg);
        } while (false);
        clearParaCondition(tCondition);
        UnityEngine.Profiling.Profiler.EndSample();
        return false;
    }

    static public bool checkCondition(List<string> arrConditionID, MapArg mpArg, bool bIsClearCache = true)
    {
        UnityEngine.Profiling.Profiler.BeginSample("arrConditionID");
        if (mpArg == null) mpArg = m_tConditionArg;
        bool bIsOk = true;
        foreach (var strConditionID in arrConditionID)
        {
            var tCondition = getCondition(strConditionID);
            if (tCondition == null)
            {
                continue;
            }
            if (checkCondition(tCondition, mpArg) == false)
            {
                bIsOk = false;
                break;
            }
        }
        if (bIsClearCache == true)
        {
            clearConditionCache(arrConditionID);
        }

        UnityEngine.Profiling.Profiler.EndSample();
        return bIsOk;
    }
    static public void clearConditionCache(string strConditionID)
    {
        var tCondition = getCondition(strConditionID);
        if (tCondition == null)
        {
            return;
        }
        clearParaCondition(tCondition);
    }

    static public void clearConditionCache(List<string> arrConditionID)
    {
        foreach (var strConditionID in arrConditionID)
        {
            clearConditionCache(strConditionID);
        }
    }

}