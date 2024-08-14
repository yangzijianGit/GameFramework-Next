using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace ENate
{
    public class ElementCreater
    {
        Stage m_tStage;
        Counter m_tElementIdCounter;
        int m_nExcuteCount;
        Counter m_tEliminateOperatorCounter;
        public ElementCreater(Stage tStage)
        {
            m_tStage = tStage;
            m_tElementIdCounter = new Counter();
            m_tEliminateOperatorCounter = new Counter();
            m_nExcuteCount = m_tStage.m_tExcuteCounter.count();
        }

        //static Dictionary<string, ElementAttribute> m_mpElementAttribute;

        public Element create(string strElementId, int nIndex, int nLine, int nColumn, bool bIsSetPosition = false, bool bIsPlayDefaultAni = true)
        {
            GameObject obj = ENateResource.loadPrefab("ENate/Prefabs/Element/Element", null, false);
            Element tElement = obj.GetComponent<Element>();
            ChessBoard tChessBoard = m_tStage.getChessBoardWithIndex(nIndex);
            JsonData.Element_Config.Element tConfigElement = Config.ElementConfig.getConfig_element(strElementId);
            if (tConfigElement.m_bIsRandomElement == "1")
            {
                DropDeviceUnit tDropDeviceUnit = m_tStage.m_tDropStrategy.randomDropUnit(nIndex, nLine, nColumn);
                if (tDropDeviceUnit.m_arrElementId.Count > 0)
                {
                    strElementId = tDropDeviceUnit.m_arrElementId[0];
                }
            }
            tElement.init(m_tElementIdCounter.count(), strElementId);
            obj.name = tElement.ID.ToString() + "_" + strElementId;
            // float fR = ((strElementId.GetHashCode() < 0 ? -strElementId.GetHashCode() : strElementId.GetHashCode()) / 256 % 255) / 255.0f;
            // float fG = ((strElementId.GetHashCode() < 0 ? -strElementId.GetHashCode() : strElementId.GetHashCode()) / 16 % 255) / 255.0f;
            // float fB = ((strElementId.GetHashCode() < 0 ? -strElementId.GetHashCode() : strElementId.GetHashCode()) / 4 % 255) / 255.0f;
            // obj.AddComponent<Image>().color = new Color(fR, fG, fB, 1);
            ElementValue<int> tLevel = tElement.getElementAttribute(ElementAttribute.Attribute.level) as ElementValue<int>;
            obj.attachObj(tChessBoard.getLayer(tLevel.Value));
            if (bIsSetPosition == true)
            {
                Vector2 tVector2 = MoveUnitUtil.getPositionWithLineCol(nLine, nColumn);
                RectTransform tRectTransform = tElement.GetComponent<RectTransform>();
                tRectTransform.anchoredPosition = tVector2;
            }
            var tGridCoord = new GridCoord(nIndex, nLine, nColumn);
            if (bIsPlayDefaultAni == true) tElement.playAniWithBehaviorId("normal");
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_CREATE, tElement);
            excuteGeneraterSkill(tChessBoard, tElement, tGridCoord);
            return tElement;
        }

        public void excuteGeneraterSkill(ChessBoard tChessBoard, Element tElement, GridCoord tGridCoord)
        {
            var arrSkill = Config.SkillConfig.getGeneratorSkillNode(tElement.ElementId);
            if (arrSkill != null)
            {
                ConditionConfig.MapArg mpArg = new ConditionConfig.MapArg();
                mpArg.Stage = m_tStage;
                mpArg.GridCoord = tGridCoord;
                mpArg.Element = tElement;
                mpArg.ChessBoard = tChessBoard;
                m_tStage.m_tSkillManager.excuteSkill(tChessBoard, arrSkill, tGridCoord, mpArg);
            }

        }

        public Element create(string strGeneratedId, Grid tGeneratedGrid)
        {
            Element tElement = null;
            if (strGeneratedId != "" && tGeneratedGrid != null)
            {
                tElement = create(strGeneratedId, tGeneratedGrid.m_tChessBoard.Index, tGeneratedGrid.m_tGridCoord.Line, tGeneratedGrid.m_tGridCoord.Col, true);
                //console.log("strGeneratedId " + strGeneratedId + " generatedElement:", tElement.ID);
                tGeneratedGrid.addElement(tElement);
            }
            return tElement;
        }

        void destroyEliminateCreate(ChessBoard tChessBoard, GridCoord tMineGridCoord, ElementDestroy tElementDestroy, Direction eDirection, ElementContainer arrElement)
        {
            GridCoord tGridCoord = Util.getDirectionLineCol(ref tMineGridCoord, eDirection);
            if (tGridCoord.isNull() == false)
            {
                Grid tGrid = tChessBoard.getGrid(tGridCoord.Line, tGridCoord.Col);
                if (tGrid == null)
                {
                    return;
                }
                foreach (var tElement in tGrid.m_sortedElement)
                {
                    ElementDestroy tStopMineElementDestroy = tElement.Value.getElementAttribute(ElementAttribute.Attribute.stopMineDestroyType) as ElementDestroy;
                    if (tElementDestroy == null)
                    {
                        continue;
                    }
                    tStopMineElementDestroy.filterOut(tElementDestroy);
                }
                EliminateRules.eliminateGrid(tGrid, tElementDestroy, arrElement);
            }
        }
        void destroyEliminateCreate(Element tElement, bool bIsImpactNear = true)
        {
            ElementContainer arrElement = new ElementContainer();
            ElementEliminateCreate tElementEliminateCreate = tElement.getElementAttribute(ElementAttribute.Attribute.eliminateCreateDestroy) as ElementEliminateCreate;
            if (tElementEliminateCreate == null)
            {
                return;
            }
            if (tElement == null || tElement.m_tGrid == null)
            {
                return;
            }
            GridCoord tGridCoord = tElement.m_tGrid.m_tGridCoord;
            ChessBoard tChessBoard = tElement.m_tGrid.m_tChessBoard;
            Grid tGrid = tElement.m_tGrid;
            tGrid.popElement(tElement);
            destroyEliminateCreate(tChessBoard, tGridCoord, tElementEliminateCreate.m_nEliminateMine, Direction.Mine, arrElement);
            tGrid.addElement(tElement);
            if (bIsImpactNear == true)
            {
                destroyEliminateCreate(tChessBoard, tGridCoord, tElementEliminateCreate.m_nEliminateUp, Direction.Up, arrElement);
                destroyEliminateCreate(tChessBoard, tGridCoord, tElementEliminateCreate.m_nEliminateDown, Direction.Down, arrElement);
                destroyEliminateCreate(tChessBoard, tGridCoord, tElementEliminateCreate.m_nEliminateLeft, Direction.Left, arrElement);
                destroyEliminateCreate(tChessBoard, tGridCoord, tElementEliminateCreate.m_nEliminateRight, Direction.Right, arrElement);
            }
            ElementCreater.DestroyInfo tDestroyInfo = m_tStage.m_tElementCreater.createDestroyInfo(arrElement);
            tDestroyInfo.destroyElement();
        }

        void destroyEvent(Element tElement)
        {
            if (tElement == null)
            {
                return;
            }
            if (tElement.m_tGrid == null)
            {
                return;
            }
            GridCoord tGridCoord = tElement.m_tGrid.m_tGridCoord;
            string strElementId = tElement.ElementId;
            var arrSkill = Config.SkillConfig.getDestroySkillNode(strElementId);
            if (arrSkill != null)
            {
                ConditionConfig.MapArg mpArg = new ConditionConfig.MapArg();
                mpArg.Stage = m_tStage;
                mpArg.Grid = tElement.m_tGrid;
                mpArg.Element = tElement;
                ChessBoard tChessBoard = m_tStage.CurrentChessBoard;
                if (tElement.m_tGrid != null)
                {
                    mpArg.ChessBoard = tElement.m_tGrid.m_tChessBoard;
                    tChessBoard = tElement.m_tGrid.m_tChessBoard;
                }
                m_tStage.m_tSkillManager.excuteSkill(tChessBoard, arrSkill, tGridCoord, mpArg);
            }

        }

        public void removeElement(Element tElement)
        {
            if (tElement == null)
            {
                Debug.LogError("ERROR: collectAndSynthesizeElements element is null");
                return;
            }
            tElement.delLockCount();
            if (tElement.m_tGrid == null)
            {
                Debug.LogError( "ERROR: collectAndSynthesizeElements grid is null:"); // .MoreStringFormat(tElement.ID));
                return;
            }
            //console.log("removeElement:", tElement.ID, " elementPropId:", tElement.ElementId);
            ElementValue<string> tDestroyToValue = tElement.getElementAttribute(ElementAttribute.Attribute.destroyTo) as ElementValue<string>;
            if (tDestroyToValue != null && tDestroyToValue.Value != null)
            {
                tElement.gameObject.SetActive(true);
                tElement.changeElementId(tDestroyToValue.Value);
            }
            else
            {
                tElement.m_tGrid.popElement(tElement);
                tElement.gameObject.SetActive(false);
                GameObject.Destroy(tElement.gameObject, 10);
            }
        }

        void playRemoveElementAni(Element tElement)
        {
            if (tElement == null)
            {
                Debug.LogError( "ERROR: collectAndSynthesizeElements element is null");
                return;
            }
            if (tElement.m_tGrid == null)
            {
                Debug.LogError( "ERROR: collectAndSynthesizeElements grid is null:"); // .MoreStringFormat(tElement.ID));
                return;
            }
            //console.log("removeElement:", tElement.ID, " elementPropId:", tElement.ElementId);
            ElementValue<string> tDestroyToValue = tElement.getElementAttribute(ElementAttribute.Attribute.destroyTo) as ElementValue<string>;
            if (tDestroyToValue == null || tDestroyToValue.Value == null)
            {
                jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE, tElement);
            }
            else
            {
                tElement.gameObject.SetActive(true);
            }
        }

        public class DestroyInfo
        {
            public enum EEliminatePlay
            {
                normal,
                delay,
                merge
            }
            ElementCreater m_tElementCreater;
            public ElementContainer arrElement;
            public Grid tGeneratedGrid = null;
            public string strGeneratedId = "";
            public bool bIsGenerateEliminateCreate = true;
            public string strExchangeAniId = "";
            public EEliminatePlay eEEliminatePlay = EEliminatePlay.normal;
            bool bIsOver = false;
            public GridCoord tTriggerGridCoord;

            public DestroyInfo(ElementCreater tElementCreater, ElementContainer tElementContainer)
            {
                m_tElementCreater = tElementCreater;
                arrElement = tElementContainer;
            }
            public void destroyElement()
            {
                if (arrElement.Count <= 0)
                {
                    return;
                }
                List<Element> m_arrEliminateElement = new List<Element>();
                List<Element> m_arrEliminateNotInAniElement = new List<Element>();
                List<KeyValuePair<Grid, int>> m_mpEliminateMark = new List<KeyValuePair<Grid, int>>();
                List<KeyValuePair<Grid, int>> m_mpNotInAniEliminateMark = new List<KeyValuePair<Grid, int>>();
                Action<List<Element>, List<KeyValuePair<Grid, int>>> pCallBack = (List<Element> arrEliminateElement, List<KeyValuePair<Grid, int>> mpEliminateMark) =>
                {
                    foreach (var tElement in arrEliminateElement)
                    {
                        m_tElementCreater.destroyEvent(tElement);
                        m_tElementCreater.destroyEliminateCreate(tElement, bIsGenerateEliminateCreate);
                    }
                    foreach (var itMarkCount in mpEliminateMark)
                    {
                        itMarkCount.Key.m_tChessBoard.m_tDropManager.excute_releaseGrid(itMarkCount.Key.m_tGridCoord.Line, itMarkCount.Key.m_tGridCoord.Col, itMarkCount.Value);
                    }
                    foreach (var tElement in arrEliminateElement)
                    {
                        m_tElementCreater.removeElement(tElement);
                    }
                };
                Action pGeneratorFunc = () =>
                {
                    Element tCreateElement = m_tElementCreater.create(strGeneratedId, tGeneratedGrid);
                    if (tCreateElement != null)
                    {
                        tCreateElement.playAniWithBehaviorId("generate", true);
                    }
                };
                float fWaitTime = float.Parse(ElementBehavior.getAniArgValue("normaleliminateTime"));
                float fDelayTime = float.Parse(ElementBehavior.getAniArgValue("normaleliminateDelay"));
                foreach (var itElement in arrElement)
                {
                    var tElement = itElement.Value;
                    if (tElement.IsLock == true)
                    {
                        continue;
                    }
                    if (tElement.isCanChangeLevel() == false)
                    {
                        continue;
                    }
                    bool bIsHaveAni = true;
                    tElement.addLockCount();

                    if (BattleArg.Instance.isUITarget(tElement.ElementId) == false && BattleArg.Instance.isGenerPower(tElement) == false)
                    {
                        var eElementEliminatePlay = eEEliminatePlay;
                        ElementValue<int> tColorValue = tElement.getElementAttribute(ElementAttribute.Attribute.color) as ElementValue<int>;
                        if (tColorValue == null && eElementEliminatePlay == EEliminatePlay.merge)
                        {
                            eElementEliminatePlay = EEliminatePlay.normal;
                        }
                        switch (eElementEliminatePlay)
                        {
                            case EEliminatePlay.normal:
                                {
                                    if (string.IsNullOrEmpty(strExchangeAniId) == false)
                                    {
                                        bIsHaveAni = tElement.playAni(strExchangeAniId, true, null);
                                    }
                                    else
                                    {
                                        bIsHaveAni = tElement.playAniWithBehaviorId("eliminate", true, null);
                                    }
                                }
                                break;
                            case EEliminatePlay.delay:
                                {
                                    float fCurrentDelay = 0.0f;
                                    if (tElement.m_tGrid != null)
                                    {
                                        fCurrentDelay += Mathf.Abs(tElement.m_tGrid.m_tGridCoord.Line - tTriggerGridCoord.Line) * fDelayTime;
                                        fCurrentDelay += Mathf.Abs(tElement.m_tGrid.m_tGridCoord.Col - tTriggerGridCoord.Col) * fDelayTime;
                                    }
                                    if (fWaitTime < fCurrentDelay + fDelayTime)
                                    {
                                        fWaitTime = fCurrentDelay + fDelayTime;
                                    }
                                    if (string.IsNullOrEmpty(strExchangeAniId) == false)
                                    {
                                        bIsHaveAni = tElement.playAni(strExchangeAniId, true, null, fCurrentDelay);
                                    }
                                    else
                                    {
                                        bIsHaveAni = tElement.playAniWithBehaviorId("eliminate", true, null, fCurrentDelay);
                                    }
                                }
                                break;
                            case EEliminatePlay.merge:
                                {
                                    // merge 的时候所有元素都要等待 merge 动画结束 所以 bIsHaveAni 是true 
                                    Direction eDirection = Util.getDirectionWithLineCol(tElement.m_tGrid.m_tGridCoord.Line, tElement.m_tGrid.m_tGridCoord.Col, tGeneratedGrid.m_tGridCoord.Line, tGeneratedGrid.m_tGridCoord.Col);
                                    string strBehaviorId = "";
                                    switch (eDirection)
                                    {
                                        case Direction.Up:
                                            strBehaviorId = "mergeU";
                                            break;
                                        case Direction.Down:
                                            strBehaviorId = "mergeD";
                                            break;
                                        case Direction.Left:
                                            strBehaviorId = "mergeL";
                                            break;
                                        case Direction.Right:
                                            strBehaviorId = "mergeR";
                                            break;
                                        case Direction.UpLeft:
                                            strBehaviorId = "mergeUL";
                                            break;
                                        case Direction.UpRight:
                                            strBehaviorId = "mergeUR";
                                            break;
                                        case Direction.DownLeft:
                                            strBehaviorId = "mergeDL";
                                            break;
                                        case Direction.DownRight:
                                            strBehaviorId = "mergeDR";
                                            break;
                                    }
                                    tElement.playAniWithBehaviorId(strBehaviorId, true, null);
                                    tElement.playAni("ani_movetoTarget", true, null, 0, tElement.transform.position, tGeneratedGrid.transform.position);
                                }
                                break;
                        }
                    }
                    else
                    {
                        tElement.gameObject.SetActive(false);
                    }
                    m_tElementCreater.playRemoveElementAni(tElement);
                    if (bIsHaveAni == true)
                    {
                        m_arrEliminateElement.Add(tElement);
                        if (tElement.m_tGrid != null)
                        {
                            int nEliminateOperatorCount = m_tElementCreater.m_tEliminateOperatorCounter.count();
                            int nMarkCount = Util.getExcuteOperatorMarkId(m_tElementCreater.m_nExcuteCount, nEliminateOperatorCount);
                            m_mpEliminateMark.Add(new KeyValuePair<Grid, int>(tElement.m_tGrid, nMarkCount));
                            tElement.m_tGrid.m_tChessBoard.m_tDropManager.excute_markGrid(tElement.m_tGrid.m_tGridCoord.Line, tElement.m_tGrid.m_tGridCoord.Col, nMarkCount);
                        }
                    }
                    else
                    {
                        if (tElement.m_tGrid != null)
                        {
                            int nEliminateOperatorCount = m_tElementCreater.m_tEliminateOperatorCounter.count();
                            int nMarkCount = Util.getExcuteOperatorMarkId(m_tElementCreater.m_nExcuteCount, nEliminateOperatorCount);
                            m_mpNotInAniEliminateMark.Add(new KeyValuePair<Grid, int>(tElement.m_tGrid, nMarkCount));
                            tElement.m_tGrid.m_tChessBoard.m_tDropManager.excute_markGrid(tElement.m_tGrid.m_tGridCoord.Line, tElement.m_tGrid.m_tGridCoord.Col, nMarkCount);
                        }
                        m_arrEliminateNotInAniElement.Add(tElement);
                    }
                }
                pCallBack(m_arrEliminateNotInAniElement, m_mpNotInAniEliminateMark);
                jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_CreateTask, new KeyValuePair<float, Action>(fWaitTime, () =>
                {
                    pCallBack(m_arrEliminateElement, m_mpEliminateMark);
                    pGeneratorFunc();
                }));
            }
        }

        public DestroyInfo createDestroyInfo(ElementContainer tElementContainer)
        {
            return new DestroyInfo(this, tElementContainer);
        }

    }
}