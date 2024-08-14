using System;
using System.Collections;
using System.Collections.Generic;
using ENate;
using UnityEngine;
using UnityEngine.UI;

public class FeverOperator
{
    int m_nOperationId;
    public int OperationId
    {
        get
        {
            return m_nOperationId;
        }
    }
    public EOperatorType m_eEOperatorType;
    public int m_nSrcCoord;
    public int m_nAimCoord;
    Stage m_tStage;
    StageRunStatue_Fever m_tStageRunStatue_Fever;
    public enum EOperatorType
    {
        none,
        normal,
        oneSuper,
        twoSuper
    }
    /**
     * @Author: yangzijian
     * @description: Detection operation type.
     * @param {int, int}
     * @return: EOperatorType
     */
    public static EOperatorType checkOperatorType(Stage tStage, int nSrcCoord, int nAimCoord, ref List<Element> arrElement)
    {
        var tSrcGridCoord = GridCoord.CoordToPos(nSrcCoord);
        var tAimGridCoord = GridCoord.CoordToPos(nAimCoord);
        var tDetectTwoElementsExchange = tStage.CurrentChessBoard.abilityToDetectTwoElementsExchange(tSrcGridCoord.Line, tSrcGridCoord.Col, tAimGridCoord.Line, tAimGridCoord.Col);
        if (tDetectTwoElementsExchange != null)
        {
            arrElement.Add(tDetectTwoElementsExchange.m_arrElement1);
            arrElement.Add(tDetectTwoElementsExchange.m_arrElement2);
            return EOperatorType.twoSuper;
        }
        var tExchangeAndSyntheizeElements = tStage.CurrentChessBoard.exchangeAndSynthesizeElementsCheck(tSrcGridCoord.Line, tSrcGridCoord.Col, tAimGridCoord.Line, tAimGridCoord.Col);
        if (tExchangeAndSyntheizeElements != null)
        {
            if (tExchangeAndSyntheizeElements.m_arrElement != null)
            {
                if (tExchangeAndSyntheizeElements.m_arrElement.Count > 0)
                {
                    var tSrcGrid = tStage.CurrentChessBoard.getGrid(tSrcGridCoord);
                    if (tSrcGrid != null)
                    {
                        Element tElement = tSrcGrid.getElementWithElementAttribute(ElementAttribute.Attribute.type, 1);
                        if (tElement != null)
                        {
                            arrElement.Add(tElement);
                            return EOperatorType.oneSuper;
                        }
                    }
                }
            }
            if (tExchangeAndSyntheizeElements.m_tSrcENateComputeInfo != null && tExchangeAndSyntheizeElements.m_tSrcENateComputeInfo.m_arrElement != null)
            {
                foreach (var itElement in tExchangeAndSyntheizeElements.m_tSrcENateComputeInfo.m_arrElement)
                {
                    arrElement.Add(itElement.Value);
                }
            }
        }
        return EOperatorType.normal;
    }
    public static EOperatorType checkOperatorType(Stage tStage, int nCoord, ref List<Element> arrElement)
    {
        var tGridCoord = GridCoord.CoordToPos(nCoord);
        ElementContainer tElementContainer = tStage.CurrentChessBoard.doubleClickGridCheck(tGridCoord.Line, tGridCoord.Col);
        if (tElementContainer != null)
        {
            if (tElementContainer.Count > 0)
            {
                foreach (var itElement in tElementContainer)
                {
                    arrElement.Add(itElement.Value);
                }
                return EOperatorType.oneSuper;
            }
        }
        return EOperatorType.none;
    }
    public void cancel()
    {
        //LogUtil.AddLog("battle", "cancel    "); // .MoreStringFormat(m_arrShowObj.Count, " ", m_nSrcCoord, " ", m_nAimCoord, " type:", m_eEOperatorType));
        foreach (var obj in m_arrShowObj)
        {
            obj.SetActive(false);
        }
    }

    List<GameObject> m_arrShowObj = new List<GameObject>();
    public void show(int nSrcCoord, int nAimCoord)
    {
        m_arrShowObj.Clear();
        List<Element> arrShowElement = new List<Element>();
        m_eEOperatorType = FeverOperator.checkOperatorType(m_tStage, nSrcCoord, nAimCoord, ref arrShowElement);
        //LogUtil.AddLog("battle", "show nSrcCoord:   "); // .MoreStringFormat(nSrcCoord, " nAimCoord:", nAimCoord, " OperationId:", OperationId, " m_eEOperatorType:", m_eEOperatorType));
        if (m_eEOperatorType == FeverOperator.EOperatorType.none)
        {
            return;
        }
        foreach (var tElement in arrShowElement)
        {
            ENate.Grid tGrid = null;
            if (tElement != null)
            {
                tGrid = tElement.m_tGrid;
            }
            if (tGrid == null)
            {
                return;
            }
            var tFeverEffect = tGrid.m_tGridUI.transform.Find("FevereEffect");
            if (tFeverEffect == null)
            {
                return;
            }
            m_arrShowObj.Add(tFeverEffect.gameObject);
            tFeverEffect.gameObject.SetActive(true);
            m_tStageRunStatue_Fever.mardCoordOperation(tGrid.m_tGridCoord.Coord, OperationId);
        }
    }
    public void show(int nCoord)
    {
        List<Element> arrShowElement = new List<Element>();
        m_eEOperatorType = FeverOperator.checkOperatorType(m_tStage, nCoord, ref arrShowElement);
        if (m_eEOperatorType == FeverOperator.EOperatorType.none)
        {
            return;
        }
        foreach (var tElement in arrShowElement)
        {
            ENate.Grid tGrid = null;
            if (tElement != null)
            {
                tGrid = tElement.m_tGrid;
            }
            if (tGrid == null)
            {
                return;
            }
            var tFeverEffect = tGrid.m_tGridUI.transform.Find("FevereEffect");
            if (tFeverEffect == null)
            {
                return;
            }
            m_arrShowObj.Add(tFeverEffect.gameObject);
            tFeverEffect.gameObject.SetActive(true);
            m_tStageRunStatue_Fever.mardCoordOperation(tGrid.m_tGridCoord.Coord, OperationId);
        }
    }
    public void showNumber(int nNumber)
    {
        return;
        ENate.Grid tGrid = m_tStage.CurrentChessBoard.getGrid(GridCoord.CoordToPos(m_nSrcCoord));
        if (tGrid == null)
        {
            return;
        }
        string strName = "number";
        var tNewTransform = tGrid.m_tGridUI.transform.Find(strName);
        GameObject tNew = null;
        if (tNewTransform == null)
        {
            tNew = new GameObject();
            tNew.name = "number";
            tNew.AddComponent<RectTransform>().sizeDelta = new Vector2(80, 80);;
            tNew.attachObj(tGrid.m_tGridUI);
            Text tText = tNew.AddComponent<Text>();
            tText.font = Font.CreateDynamicFontFromOSFont("Arial", 28);
        }
        else
        {
            tNew = tNewTransform.gameObject;
        }

        tNew.setText(nNumber.ToString());
    }

    public FeverOperator(int nOperationId, StageRunStatue_Fever tStageRunStatue_Fever, Stage tStage, int nSrcCoord, int nAimCoord)
    {
        m_nOperationId = nOperationId;
        m_tStageRunStatue_Fever = tStageRunStatue_Fever;
        m_tStage = tStage;
        m_nSrcCoord = nSrcCoord;
        m_nAimCoord = nAimCoord;
        show(nSrcCoord, nAimCoord);
    }

    public FeverOperator(int nOperationId, StageRunStatue_Fever tStageRunStatue_Fever, Stage tStage, int nCoord)
    {
        m_nOperationId = nOperationId;
        m_tStageRunStatue_Fever = tStageRunStatue_Fever;
        m_tStage = tStage;
        m_nSrcCoord = nCoord;
        m_nAimCoord = -1;
        show(nCoord, -1);
    }
}

public class StageRunStatue_Fever : ENate.StageRunStaue
{
    bool m_bIsOver = false;

    public StageRunStatue_Fever()
    {
        m_nCostStep = int.Parse(JsonManager.fever_config.root.game.effect.costStep);
        m_fFeverTimeOfDuration = float.Parse(JsonManager.fever_config.root.game.effect.continueTime);
        m_fPlayDelay = float.Parse(JsonManager.fever_config.root.game.effect.playDelay);
        m_fInFeverTime = float.Parse(JsonManager.fever_config.root.game.inFeverTime);
    }
    public void prefix(ENate.Stage tStage)
    {
        m_bIsOver = false;
        m_tStage = tStage;
        EEFeverStatus = EFeverStatus.ani;
        m_smpFeverOperator.Clear();
        m_mpFeverOperatorMarkCoord.Clear();
        m_mpCoordMarkOperationId.Clear();
        m_mpOperationIdMarkCoord.Clear();
        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_FEVER_BEGINAni, m_fInFeverTime);
        m_fFeverBeginTime = Time.time;
        m_fFeverEndTime = m_fFeverBeginTime + m_fInFeverTime + m_fFeverTimeOfDuration;
    }
    static Stage.EWait[] arreWait =
        new Stage.EWait[2]
        {
            Stage.EWait.drop, Stage.EWait.aniTask
        };
    public void run(ENate.Stage tStage)
    {
        switch (EEFeverStatus)
        {
            case EFeverStatus.ani:
                {
                    if (Time.time - m_fFeverBeginTime >= m_fInFeverTime)
                    {
                        EEFeverStatus = EFeverStatus.exchange;
                        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_FEVER_BEGIN, m_fFeverTimeOfDuration);
                    }
                }
                break;
            case EFeverStatus.exchange:
                {
                    bool bIsDropAniDone = tStage.waitDropAniDone();
                    if (bIsDropAniDone == true && Time.time > m_fFeverEndTime)
                    {
                        EEFeverStatus = EFeverStatus.waitAniDone;
                    }
                }
                break;
            case EFeverStatus.waitAniDone:
                {
                    bool bIsDropAniDone = tStage.waitAniDone(arreWait);
                    if (bIsDropAniDone == true)
                    {
                        EEFeverStatus = EFeverStatus.carnical;
                        protectedOperationElement(tStage);
                        tStage.StartCoroutine(excuteOnePoint(tStage, () =>
                        {
                            m_bIsOver = true;
                        }));
                    }
                }
                break;
        }
    }
    public bool isOver(ENate.Stage tStage)
    {
        return m_bIsOver;
    }
    public ENate.StageRunningStatus end(ENate.Stage tStage)
    {
        for (int i = 0; i < m_nCostStep; i++)
        {
            tStage.useStep();
        }
        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_FEVER_END);
        return ENate.StageRunningStatus.DetectingRoundEnd;
    }

    Stage m_tStage;
    /**
     * @Author: yangzijian
     * @description: operation sequence.  from small to large.
     */
    SortedDictionary<int, FeverOperator> m_smpFeverOperator = new SortedDictionary<int, FeverOperator>();
    Dictionary<int, int> m_mpFeverOperatorMarkCoord = new Dictionary<int, int>();
    Dictionary<int, List<int>> m_mpCoordMarkOperationId = new Dictionary<int, List<int>>();
    Dictionary<int, List<int>> m_mpOperationIdMarkCoord = new Dictionary<int, List<int>>();
    Counter m_tOperatorCounter = new Counter();
    /**
     * @Author: yangzijian
     * @description: fever begin time mark.
     */
    float m_fFeverBeginTime;
    /**
     * @Author: yangzijian
     * @description: fever end time mark.
     */
    float m_fFeverEndTime;
    /**
     * @Author: yangzijian
     * @description: Opertational play delay.
     */
    float m_fPlayDelay;
    /**
     * @Author: yangzijian
     * @description: cost step.
     */
    int m_nCostStep;
    /**
     * @Author: yangzijian
     * @description: Duration after triggering fever.
     */
    float m_fFeverTimeOfDuration;
    /**
     * @Author: yangzijian
     * @description: fever protected element.
     */

    float m_fInFeverTime;
    public Dictionary<int, List<Element>> m_mpProtectedElement = new Dictionary<int, List<Element>>();
    public List<Element> m_arrProtectedElement = new List<Element>();
    public List<int> m_arrOperation = new List<int>();
    public enum EFeverStatus
    {
        ani,
        exchange,
        waitAniDone,
        carnical
    }

    EFeverStatus m_eEFeverStatus;

    EFeverStatus EEFeverStatus
    {
        get
        {
            return m_eEFeverStatus;
        }
        set
        {
            m_eEFeverStatus = value;
            switch (m_eEFeverStatus)
            {
                case EFeverStatus.ani:
                    {
                        m_tStage.bIsLock = true;
                    }
                    break;
                case EFeverStatus.exchange:
                    {
                        m_tStage.bIsLock = false;
                    }
                    break;
                case EFeverStatus.waitAniDone:
                    {
                        m_tStage.bIsLock = true;
                    }
                    break;
                case EFeverStatus.carnical:
                    {
                        m_tStage.bIsLock = true;
                    }
                    break;
            }
        }
    }

    public void mardCoordOperation(int nCoord, int nOperationId)
    {
        if (m_mpCoordMarkOperationId.ContainsKey(nCoord) == false)
        {
            m_mpCoordMarkOperationId[nCoord] = new List<int>();
        }
        if (m_mpCoordMarkOperationId[nCoord].Contains(nOperationId) == true)
        {
            return;
        }
        if (m_mpOperationIdMarkCoord.ContainsKey(nOperationId) == false)
        {
            m_mpOperationIdMarkCoord[nOperationId] = new List<int>();
        }
        //LogUtil.AddLog("battle", "mardCoordOperation:"); // .MoreStringFormat(nCoord, " ", nOperationId));
        m_mpCoordMarkOperationId[nCoord].Add(nOperationId);
        m_mpOperationIdMarkCoord[nOperationId].Add(nCoord);
    }

    public List<int> getCoordOperationId(int nCoord)
    {
        if (m_mpCoordMarkOperationId.ContainsKey(nCoord) == true)
        {
            return m_mpCoordMarkOperationId[nCoord];
        }
        return null;
    }

    public List<int> getOperationIdCoord(int nOperationId)
    {
        if (m_mpOperationIdMarkCoord.ContainsKey(nOperationId) == true)
        {
            return m_mpOperationIdMarkCoord[nOperationId];
        }
        return null;
    }

    public void cancelOperation(int nCoord)
    {
        //LogUtil.AddLog("battle", "cancelOperation:"); // .MoreStringFormat(nCoord));
        do
        {
            if (m_mpFeverOperatorMarkCoord.ContainsKey(nCoord) == false)
            {
                break;
            }
            int nId = m_mpFeverOperatorMarkCoord[nCoord];
            if (m_smpFeverOperator.ContainsKey(nId) == false)
            {
                break;
            }
            FeverOperator tFeverOperator = m_smpFeverOperator[nId];
            tFeverOperator.cancel();
            if (tFeverOperator.m_eEOperatorType == FeverOperator.EOperatorType.twoSuper)
            {
                int nAimOperationId = getOperationId(tFeverOperator.m_nAimCoord);
                var tAimStageRunStatue_Fever = getOperation(nAimOperationId);
                if (tAimStageRunStatue_Fever != null)
                {
                    tAimStageRunStatue_Fever.show(tFeverOperator.m_nAimCoord);
                }
            }
            //LogUtil.AddLog("battle", "remvoe nId:"); // .MoreStringFormat(nId, " nCoord:", nCoord));
            m_smpFeverOperator.Remove(nId);
            m_mpFeverOperatorMarkCoord.Remove(nCoord);

            List<int> arrCoord = getOperationIdCoord(nId);
            if (arrCoord != null)
            {
                foreach (var nMarkCoord in arrCoord)
                {
                    if (m_mpFeverOperatorMarkCoord.ContainsKey(nMarkCoord) == false)
                    {
                        continue;
                    }
                    int nMarkOperationId = m_mpFeverOperatorMarkCoord[nMarkCoord];
                    if (m_smpFeverOperator.ContainsKey(nMarkOperationId) == false)
                    {
                        continue;
                    }
                    var tNormalFeverOperator = getOperation(nMarkOperationId);
                    if (tNormalFeverOperator == null)
                    {
                        continue;
                    }
                    tNormalFeverOperator.cancel();
                }
                foreach (var nMarkCoord in arrCoord)
                {
                    if (m_mpFeverOperatorMarkCoord.ContainsKey(nMarkCoord) == false)
                    {
                        continue;
                    }
                    int nMarkOperationId = m_mpFeverOperatorMarkCoord[nMarkCoord];
                    if (m_smpFeverOperator.ContainsKey(nMarkOperationId) == false)
                    {
                        continue;
                    }
                    var tNormalFeverOperator = getOperation(nMarkOperationId);
                    if (tNormalFeverOperator == null)
                    {
                        continue;
                    }
                    tNormalFeverOperator.show(tNormalFeverOperator.m_nSrcCoord, tNormalFeverOperator.m_nAimCoord);
                }
            }

        } while (false);
        List<int> arrOperationId = getCoordOperationId(nCoord);
        if (arrOperationId != null)
        {
            foreach (var nOperationId in arrOperationId)
            {
                var tNormalFeverOperator = getOperation(nOperationId);
                if (tNormalFeverOperator == null)
                {
                    continue;
                }
                tNormalFeverOperator.cancel();
            }
            foreach (var nOperationId in arrOperationId)
            {
                var tNormalFeverOperator = getOperation(nOperationId);
                if (tNormalFeverOperator == null)
                {
                    continue;
                }
                tNormalFeverOperator.show(tNormalFeverOperator.m_nSrcCoord, tNormalFeverOperator.m_nAimCoord);
            }
        }
    }

    public FeverOperator getOperation(int nId)
    {
        if (m_smpFeverOperator.ContainsKey(nId) == true)
        {
            return m_smpFeverOperator[nId];
        }
        return null;
    }

    public int getOperationId(int nCoord)
    {
        if (m_mpFeverOperatorMarkCoord.ContainsKey(nCoord) == true)
        {
            return m_mpFeverOperatorMarkCoord[nCoord];
        }
        return -1;
    }
    public void addOperation(Stage tStage, int nCoord)
    {
        cancelOperation(nCoord);
        int nId = m_tOperatorCounter.count();
        FeverOperator tFeverOperator = new FeverOperator(nId, this, tStage, nCoord);
        if (tFeverOperator.m_eEOperatorType == FeverOperator.EOperatorType.none)
        {
            return;
        }
        tFeverOperator.showNumber(nId);
        m_smpFeverOperator.Add(nId, tFeverOperator);
        m_mpFeverOperatorMarkCoord.Add(nCoord, nId);
    }

    public void addOperation(Stage tStage, int nSrcCoord, int nAimCoord)
    {
        cancelOperation(nSrcCoord);
        cancelOperation(nAimCoord);
        int nSrcId = m_tOperatorCounter.count();
        int nAimId = m_tOperatorCounter.count();
        FeverOperator tSrcFeverOperator = new FeverOperator(nSrcId, this, tStage, nSrcCoord, nAimCoord);
        FeverOperator tAimFeverOperator = new FeverOperator(nAimId, this, tStage, nAimCoord, nSrcCoord);
        if (tSrcFeverOperator.m_eEOperatorType == FeverOperator.EOperatorType.none && tAimFeverOperator.m_eEOperatorType == FeverOperator.EOperatorType.none)
        {
            return;
        }
        tSrcFeverOperator.showNumber(nSrcId);
        tAimFeverOperator.showNumber(nAimId);
        m_smpFeverOperator.Add(nSrcId, tSrcFeverOperator);
        m_smpFeverOperator.Add(nAimId, tAimFeverOperator);
        m_mpFeverOperatorMarkCoord.Add(nSrcCoord, nSrcId);
        m_mpFeverOperatorMarkCoord.Add(nAimCoord, nAimId);
    }

    public bool isElementInProtected(Element tElement)
    {
        return m_arrProtectedElement.Contains(tElement);
    }

    void addProtectedElement(int nOperatorId, Element tElement)
    {
        if (m_mpProtectedElement.ContainsKey(nOperatorId) == false)
        {
            m_mpProtectedElement[nOperatorId] = new List<Element>();
        }
        m_mpProtectedElement[nOperatorId].Add(tElement);
        m_arrProtectedElement.Add(tElement);
    }

    void cancelProtectedElement(int nOperatorId)
    {
        if (m_mpProtectedElement.ContainsKey(nOperatorId) == false)
        {
            return;
        }
        foreach (var itProtectedElement in m_mpProtectedElement[nOperatorId])
        {
            m_arrProtectedElement.Remove(itProtectedElement);
        }
        m_mpProtectedElement.Remove(nOperatorId);
    }

    void protectedOperationElement(Stage tStage)
    {
        m_mpProtectedElement.Clear();
        m_arrProtectedElement.Clear();
        m_arrOperation.Clear();
        foreach (var tFeverOperator in m_smpFeverOperator)
        {
            switch (tFeverOperator.Value.m_eEOperatorType)
            {
                case FeverOperator.EOperatorType.normal:
                    {
                        var tGridCoord = GridCoord.CoordToPos(tFeverOperator.Value.m_nSrcCoord);
                        var tElement = tStage.CurrentChessBoard.getGrid(tGridCoord).getElementWithElementAttribute(ElementAttribute.Attribute.involvedCompose, 1);
                        if (tElement == null)
                        {
                            continue;
                        }
                        if (isElementInProtected(tElement) == true)
                        {
                            continue;
                        }
                        var tENateComputeInfo = tStage.CurrentChessBoard.collectAndSynthesizeElementsInfo(tGridCoord.Line, tGridCoord.Col);
                        if (tENateComputeInfo == null || tENateComputeInfo.m_arrElement == null || tENateComputeInfo.m_arrElement.Count <= 0)
                        {
                            continue;
                        }
                        foreach (var itCollectElement in tENateComputeInfo.m_arrElement)
                        {
                            addProtectedElement(tFeverOperator.Key, itCollectElement.Value);
                        }
                    }
                    break;
                case FeverOperator.EOperatorType.oneSuper:
                    {
                        var tGridCoord = GridCoord.CoordToPos(tFeverOperator.Value.m_nSrcCoord);
                        var tElement = tStage.CurrentChessBoard.getGrid(tGridCoord).getElementWithElementAttribute(ElementAttribute.Attribute.type, 1);
                        if (isElementInProtected(tElement) == true)
                        {
                            continue;
                        }
                        addProtectedElement(tFeverOperator.Key, tElement);
                    }
                    break;
                case FeverOperator.EOperatorType.twoSuper:
                    {
                        var tSrcGridCoord = GridCoord.CoordToPos(tFeverOperator.Value.m_nSrcCoord);
                        var tAimGridCoord = GridCoord.CoordToPos(tFeverOperator.Value.m_nSrcCoord);
                        var tSrcElement = tStage.CurrentChessBoard.getGrid(tSrcGridCoord).getElementWithElementAttribute(ElementAttribute.Attribute.type, 1);
                        if (isElementInProtected(tSrcElement) == true)
                        {
                            continue;
                        }
                        var tAimElement = tStage.CurrentChessBoard.getGrid(tAimGridCoord).getElementWithElementAttribute(ElementAttribute.Attribute.type, 1);
                        if (isElementInProtected(tAimElement) == true)
                        {
                            continue;
                        }
                        addProtectedElement(tFeverOperator.Key, tSrcElement);
                        addProtectedElement(tFeverOperator.Key, tAimElement);
                    }
                    break;
            }
            m_arrOperation.Add(tFeverOperator.Key);
        }
    }
    IEnumerator excuteOnePoint(Stage tStage, Action pCallback)
    {
        foreach (var tFeverOperator in m_smpFeverOperator)
        {
            tFeverOperator.Value.cancel();
        }
        foreach (var nOperatorId in m_arrOperation)
        {
            cancelProtectedElement(nOperatorId);
            var tFeverOperator = m_smpFeverOperator[nOperatorId];
            switch (tFeverOperator.m_eEOperatorType)
            {
                case FeverOperator.EOperatorType.normal:
                    {
                        var tGridCoord = GridCoord.CoordToPos(tFeverOperator.m_nSrcCoord);
                        var tElement = tStage.CurrentChessBoard.getGrid(tGridCoord).getElementWithElementAttribute(ElementAttribute.Attribute.involvedCompose, 1);
                        if (tElement == null)
                        {
                            continue;
                        }
                        var tENateComputeInfo = tStage.CurrentChessBoard.collectAndSynthesizeElementsInfo(tGridCoord.Line, tGridCoord.Col);
                        if (tENateComputeInfo == null || tENateComputeInfo.m_arrElement == null || tENateComputeInfo.m_arrElement.Count <= 0)
                        {
                            continue;
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
                    }
                    break;
                case FeverOperator.EOperatorType.oneSuper:
                    {
                        var tGridCoord = GridCoord.CoordToPos(tFeverOperator.m_nSrcCoord);
                        var tElement = tStage.CurrentChessBoard.getGrid(tGridCoord).getElementWithElementAttribute(ElementAttribute.Attribute.type, 1);
                        var tElementContainer = tStage.CurrentChessBoard.doubleClickGridCheck(tGridCoord.Line, tGridCoord.Col);
                        if (tElementContainer == null)
                        {
                            continue;
                        }
                        ElementCreater.DestroyInfo tDestroyInfo = m_tStage.m_tElementCreater.createDestroyInfo(tElementContainer);
                        tDestroyInfo.destroyElement();
                    }
                    break;
                case FeverOperator.EOperatorType.twoSuper:
                    {
                        var tSrcGridCoord = GridCoord.CoordToPos(tFeverOperator.m_nSrcCoord);
                        var tAimGridCoord = GridCoord.CoordToPos(tFeverOperator.m_nAimCoord);
                        var tSrcElement = tStage.CurrentChessBoard.getGrid(tSrcGridCoord).getElementWithElementAttribute(ElementAttribute.Attribute.type, 1);
                        var tAimElement = tStage.CurrentChessBoard.getGrid(tAimGridCoord).getElementWithElementAttribute(ElementAttribute.Attribute.type, 1);
                        var tDetectTwoElementsExchange = tStage.CurrentChessBoard.abilityToDetectTwoElementsExchange(tSrcGridCoord.Line, tSrcGridCoord.Col, tAimGridCoord.Line, tAimGridCoord.Col);
                        if (tDetectTwoElementsExchange == null)
                        {
                            continue;
                        }
                        tDetectTwoElementsExchange.excute(tStage);
                    }
                    break;
            }
            yield return new WaitForSeconds(m_fPlayDelay);
        }
        pCallback();
    }

}