/*
 * @Description: SkillGroup
 * @Author: yangzijian
 * @Date: 2020-07-07 15:42:11
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-08-07 17:32:49
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{

    public class SkillGroup
    {
        SkillPlayer m_tSkillPlayer;
        Stage m_tStage;
        Dictionary<string, string> m_mpCache = new Dictionary<string, string>();
        int m_nSkillOperatorId;
        int m_nSkillGroupId;
        JsonData.Skill_Config.Group m_tSkillGroup;
        ChessBoard m_tChessBoard;
        GridCoord m_tRootGridCoord;
        float m_fExcuteBeginTime;
        int m_nSkillCount = 0;
        int m_nSkillExistCount = 0;
        Action<int> m_pOverCallBack;
        List<Grid> m_arrMarkGrid = new List<Grid>();

        int getSkillMarkId()
        {
            return Util.getExcuteOperatorMarkId(m_nSkillOperatorId, m_nSkillGroupId);
        }

        void markGrid(Grid tGrid)
        {
            if (tGrid == null)
            {
                return;
            }
            m_arrMarkGrid.Add(tGrid);
            m_tChessBoard.m_tDropManager.excute_markGrid(tGrid.m_tGridCoord.Line, tGrid.m_tGridCoord.Col, getSkillMarkId());
        }

        void releaseAllGrid()
        {
            foreach (var tGrid in m_arrMarkGrid)
            {
                m_tChessBoard.m_tDropManager.excute_releaseGrid(tGrid.m_tGridCoord.Line, tGrid.m_tGridCoord.Col, getSkillMarkId());
            }
            m_arrMarkGrid.Clear();
        }
        public SkillGroup(int nSkillOperatorId, SkillPlayer tSkillPlayer, int nSkillGroupId, JsonData.Skill_Config.Group tSkillGroup, ChessBoard tChessBoard, GridCoord tRootGridCoord)
        {
            m_tStage = tChessBoard.m_tStage;
            m_nSkillOperatorId = nSkillOperatorId;
            m_tSkillPlayer = tSkillPlayer;
            m_nSkillGroupId = nSkillGroupId;
            m_tSkillGroup = tSkillGroup;
            m_tChessBoard = tChessBoard;
            m_tRootGridCoord = tRootGridCoord;
        }
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return m_tStage.StartCoroutine(routine);
        }

        void addOperatorCache(string strKey, string strValue)
        {
            m_mpCache[strKey] = strValue;
        }

        string getOperatorCache(int nOperatorId, string strKey)
        {
            if (m_mpCache.ContainsKey(strKey) == true)
            {
                return m_mpCache[strKey];
            }
            return "";
        }

        void excuteSkill_area(ChessBoard tChessBoard, JsonData.Skill_Config.Skill tSkill, GridCoord tGridCoord, ElementDestroy tElementDestroy)
        {
            ElementDestroy tPassElementDestroy = new ElementDestroy();
            ElementContainer arrElement = new ElementContainer();
            ElementContainer arrPassElement = new ElementContainer();
            // if center point is have carpet element. 
            ENate.Grid pPointGrid = tChessBoard.getGrid(m_tRootGridCoord.Line, m_tRootGridCoord.Col);
            if (pPointGrid != null)
            {
                tPassElementDestroy = pPointGrid.checkPassDestroy();
            }

            foreach (var strArea in tSkill.area)
            {
                ENate.Grid tGrid = null;
                try
                {
                    String[] arrSplit = strArea.Split(',');
                    int nLine = int.Parse(arrSplit[0]);
                    int nColumn = int.Parse(arrSplit[1]);
                    tGrid = tChessBoard.getGrid(tGridCoord.Line + nLine, tGridCoord.Col + nColumn);
                }
                catch (System.Exception) { }
                if (tGrid == null)
                {
                    continue;
                }
                tGrid.markColor();
                markGrid(tGrid);
                EliminateRules.eliminateGrid(tGrid, tElementDestroy, arrElement);
                EliminateRules.eliminateGrid(tGrid, tPassElementDestroy, arrPassElement);
            }
            ElementCreater.DestroyInfo tDestroyInfo = m_tStage.m_tElementCreater.createDestroyInfo(arrPassElement);
            tDestroyInfo.strExchangeAniId = tSkill.animationId;
            tDestroyInfo.destroyElement();
            ElementCreater.DestroyInfo tDestroyInfo2 = m_tStage.m_tElementCreater.createDestroyInfo(arrElement);
            tDestroyInfo2.strExchangeAniId = tSkill.animationId;
            tDestroyInfo2.destroyElement();
        }
        IEnumerator excuteSkill_Direction_Dir(ChessBoard tChessBoard, GridCoord tGridCoord, GridCoord tCenterGridCoord, ElementDestroy tSrcElementDestroy, JsonData.Skill_Config.Direction tConfigDirection)
        {
            ENateAnimation.ENateAni tENateAni = null;
            ElementContainer arrElement = new ElementContainer();
            ElementContainer arrPassElement = new ElementContainer();
            List<ENate.Grid> arrGrid = new List<ENate.Grid>();
            ENate.Grid pPointGrid = tChessBoard.getGrid(tCenterGridCoord.Line, tCenterGridCoord.Col);
            ENate.Grid tLastDestroyGrid = pPointGrid;
            ElementDestroy tMinePassElementDestroy = new ElementDestroy();
            ENateAniArg tEnateAniArg = new ENateAniArg();
            if (pPointGrid != null)
            {
                tMinePassElementDestroy = pPointGrid.checkPassDestroy();
                tEnateAniArg.tRootPos = pPointGrid.transform.position;
            }

            if (string.IsNullOrEmpty(tConfigDirection.offect) == false)
            {
                var tOffect = tConfigDirection.offect.parseVector2Int();
                if (tOffect != null)
                {
                    tGridCoord.coord(tGridCoord.ChessBoardIndex, tGridCoord.Line + tOffect.Value.x, tGridCoord.Col + tOffect.Value.y);
                }
            }
            ENate.Grid pTargetPointGrid = tChessBoard.getGrid(tGridCoord);
            if (pTargetPointGrid != null)
            {
                tEnateAniArg.tTriggerPos = pTargetPointGrid.transform.position;
            }
            if (string.IsNullOrEmpty(tConfigDirection.flyAni) == false)
            {
                tENateAni = m_tStage.m_tAniManager.play(tConfigDirection.flyAni, tEnateAniArg);
            }
            Direction eDirection = Direction.NULL;
            float fDelay = 0;
            ElementDestroy tDirectionConnectMineElementDestroy = null;
            ElementDestroy tDirectionConnectOppositeElementDestroy = null;
            try
            {
                eDirection = (Direction) int.Parse(tConfigDirection.dir);
                Util.getDirectionGrid(tChessBoard, tGridCoord, eDirection, ref arrGrid, false);
                tDirectionConnectMineElementDestroy = Util.getDirectionConnectDestroyType(eDirection);
                tDirectionConnectOppositeElementDestroy = Util.getDirectionConnectDestroyType(Util.getDirectionOpposite(eDirection));
                fDelay = float.Parse(tConfigDirection.delay);
            }
            catch (System.Exception) { }
            ElementDestroy tPassElementDestroy = tMinePassElementDestroy.clone();

            foreach (var tElminateGrid in arrGrid)
            {
                if (fDelay > 0)
                {
                    yield return ENateYield.WaitForSeconds(fDelay);
                }
                markGrid(tElminateGrid);
                tElminateGrid.markColor();
                bool bIsBreak = false;
                // if center point is have carpet element. 
                arrElement.clear();
                arrPassElement.clear();
                ElementDestroy tElementDestroy = new ElementDestroy();
                tElementDestroy.addDestroyType(tSrcElementDestroy);
                EliminateRules.eliminateGrid(tLastDestroyGrid, tDirectionConnectMineElementDestroy, arrElement);
                EliminateRules.eliminateGrid(tElminateGrid, tDirectionConnectOppositeElementDestroy, arrElement);
                EliminateRules.eliminateGrid(tElminateGrid, tElementDestroy, arrElement);
                EliminateRules.eliminateGrid(tElminateGrid, tPassElementDestroy, arrPassElement);
                tLastDestroyGrid = tElminateGrid;
                foreach (var tDestroyElement in arrElement)
                {
                    ElementValue<int> tValue = tDestroyElement.Value.getElementAttribute(ElementAttribute.Attribute.dirEliminateTransmit) as ElementValue<int>;
                    if (tValue != null && tValue.Value == 1)
                    {
                        bIsBreak = true;
                        break;
                    }
                }
                ElementCreater.DestroyInfo tDestroyInfo = m_tStage.m_tElementCreater.createDestroyInfo(arrPassElement);
                // tDestroyInfo.strExchangeAniId = tSkill.animationId;
                tDestroyInfo.bIsGenerateEliminateCreate = false;
                tDestroyInfo.destroyElement();
                ElementCreater.DestroyInfo tDestroyInfo2 = m_tStage.m_tElementCreater.createDestroyInfo(arrElement);
                // tDestroyInfo.strExchangeAniId = tSkill.animationId;
                tDestroyInfo2.bIsGenerateEliminateCreate = false;
                tDestroyInfo2.destroyElement();

                /**
                 * @Author: yangzijian
                 * @description: for normal eliminate. this can destroy element not stop.
                 */
                if (bIsBreak) break;
                if (tPassElementDestroy.isNull() == true)
                {
                    tPassElementDestroy = tElminateGrid.checkPassDestroy();
                }
            }
            // if(tENateAni != null) tENateAni.stop();
        }
        IEnumerator excuteSkill_Direction(ChessBoard tChessBoard, JsonData.Skill_Config.Skill tSkill, GridCoord tGridCoord, GridCoord tCenterGridCoord, ElementDestroy tSrcElementDestroy)
        {
            if (tSkill.direction == null || tSkill.direction.Count <= 0)
            {
                yield break;
            }
            // mine grid
            ElementContainer arrElement = new ElementContainer();
            ENate.Grid pPointGrid = tChessBoard.getGrid(tCenterGridCoord.Line, tCenterGridCoord.Col);
            markGrid(pPointGrid);
            EliminateRules.eliminateGrid(pPointGrid, tSrcElementDestroy, arrElement);
            ElementCreater.DestroyInfo tDestroyInfo = m_tStage.m_tElementCreater.createDestroyInfo(arrElement);
            tDestroyInfo.bIsGenerateEliminateCreate = false;
            tDestroyInfo.destroyElement();

            /// direction grid 
            ENateCoroutine tENateCoroutine = new ENateCoroutine();
            foreach (var tConfigDirection in tSkill.direction)
            {
                tENateCoroutine.Add(excuteSkill_Direction_Dir(tChessBoard, tGridCoord, tCenterGridCoord, tSrcElementDestroy, tConfigDirection));
            }
            yield return tENateCoroutine.play();
        }
        void excuteSkill_CostStep(ChessBoard tChessBoard, JsonData.Skill_Config.Skill tSkill)
        {
            UnityEngine.Profiling.Profiler.BeginSample("excuteSkill_CostStep");
            if (string.IsNullOrEmpty(tSkill.costStep) == true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            m_tStage.delStep(int.Parse(tSkill.costStep));
            UnityEngine.Profiling.Profiler.EndSample();
        }
        IEnumerator excuteSkill_Ray(ChessBoard tChessBoard, JsonData.Skill_Config.Skill tSkill, List<Grid> arrGrid, GridCoord tCenterGridCoord, Action<GridCoord> pChangeElementSKill)
        {
            if (tSkill.ray == null || tSkill.ray.rayAni == null || tSkill.ray.rayAni.rayAniId.Count <= 0)
            {
                foreach (var tGrid in arrGrid)
                {
                    pChangeElementSKill(tGrid.m_tGridCoord);
                }
                yield break;
            }
            float fRayDelay = 0;
            float fStandDelay = 0;
            if (string.IsNullOrEmpty(tSkill.ray.RayDelay) == false)
            {
                fRayDelay = float.Parse(tSkill.ray.RayDelay);
            }
            if (string.IsNullOrEmpty(tSkill.ray.StandDelay) == false)
            {
                fStandDelay = float.Parse(tSkill.ray.StandDelay);
            }
            var tCenterGrid = tChessBoard.getGrid(tCenterGridCoord);
            List<GameObject> arrNew = new List<GameObject>();
            foreach (var tGrid in arrGrid)
            {
                GameObject tObj = new GameObject();
                tObj.attachObj(m_tStage.gameObject);
                arrNew.Add(tObj);
                string strAniId = tSkill.ray.rayAni.rayAniId[(int) Stage.m_tENateRandom.random(0, tSkill.ray.rayAni.rayAniId.Count)];
                Util.playENateAni(strAniId, tObj,
                    tCenterGrid != null ? (Vector3?) tCenterGrid.transform.position : null,
                    tGrid != null ? (Vector3?) tGrid.transform.position : null, () =>
                    {
                        pChangeElementSKill(tGrid.m_tGridCoord);
                    });
                if (fRayDelay > 0) yield return ENateYield.WaitForSeconds(fRayDelay);
            }
            if (fStandDelay > 0) yield return ENateYield.WaitForSeconds(fStandDelay);
            foreach (var tNewObj in arrNew)
            {
                GameObject.Destroy(tNewObj);
            }
        }
        void excuteSkill_SubTractTargetNum(JsonData.Skill_Config.Skill tSkill, Element tElement)
        {
            if (tSkill.subTractTarget == null || string.IsNullOrEmpty(tSkill.subTractTarget.hypotaxisId))
            {
                return;
            }
            int nExcuteCount = int.Parse(tSkill.subTractTarget.num);
            for (int i = 0; i < nExcuteCount; i++)
            {
                jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE, tElement);
            }
        }
        IEnumerator excuteSkill_subSkill(int nSkillOperatorId, ChessBoard tChessBoard, JsonData.Skill_Config.Skill tSkill, GridCoord tTriggerGridCoord, GridCoord tRootGridCoord, ConditionConfig.MapArg mpArg)
        {
            var tNewMapArg = new ConditionConfig.MapArg();
            tNewMapArg.ChessBoard = tChessBoard;
            tNewMapArg.Stage = tChessBoard.m_tStage;
            tNewMapArg.Grid = tChessBoard.getGrid(tTriggerGridCoord);
            if (mpArg != null)
            {
                tNewMapArg.Element = mpArg.Element;
                tNewMapArg.GridCoord = mpArg.GridCoord;
            }
            ENateCoroutine tENateCoroutine = new ENateCoroutine();
            foreach (var strSubSkill in tSkill.subSkill)
            {
                if (strSubSkill == "")
                {
                    continue;
                }
                var tSkillInfo = Config.SkillConfig.getSkillInfo(strSubSkill);
                if (tSkillInfo == null)
                {
                    continue;
                }
                var tSkillPlayer = new SkillPlayer(nSkillOperatorId, tSkillInfo, tChessBoard, tRootGridCoord, m_tSkillPlayer.m_tCounter);
                tENateCoroutine.Add(tSkillPlayer.play(tTriggerGridCoord, mpArg, null));
            }
            yield return tENateCoroutine.play();
        }
        void excuteSkill_ChangeElement(int nSkillOperatorId, ChessBoard tChessBoard, JsonData.Skill_Config.Skill tSkill, GridCoord tTriggerGridCoord)
        {
            UnityEngine.Profiling.Profiler.BeginSample("excuteSkill_ChangeElement");
            if (string.IsNullOrEmpty(tSkill.changeElement.id) == true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            string strChangeElementId = "";
            string strCacheChangeElementId = getOperatorCache(nSkillOperatorId, "changeElement");
            if (tSkill.changeElement.type == "allTheSame" && string.IsNullOrEmpty(strCacheChangeElementId) == false)
            {
                strChangeElementId = strCacheChangeElementId;
            }
            else if (tSkill.changeElement.type == "selGridElement")
            {
                var tSelectGrid = tChessBoard.getGrid(m_tRootGridCoord);
                if (tSelectGrid != null)
                {
                    var tSelElement = tSelectGrid.getElementWithElementAttribute(ElementAttribute.Attribute.level, int.Parse(tSkill.changeElement.id));
                    if (tSelElement != null)
                    {
                        strChangeElementId = tSelElement.ElementId;
                    }
                }
            }
            else
            {
                string[] arrChangeElementId = tSkill.changeElement.id.Split(';');
                if (arrChangeElementId.Length <= 0)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return;
                }
                List<string> arrRChangeElementId = new List<string>();
                ConditionConfig.MapArg tMapArg = new ConditionConfig.MapArg();
                tMapArg.Stage = tChessBoard.m_tStage;
                tMapArg.ChessBoard = tChessBoard;
                tMapArg.GridCoord = m_tRootGridCoord;
                foreach (var strChangeElement in arrChangeElementId)
                {
                    tMapArg.Param = strChangeElement;
                    if (ConditionConfig.checkCondition(tSkill.changeElement.condition, tMapArg) == true)
                    {
                        arrRChangeElementId.Add(strChangeElement);
                    }
                }
                strChangeElementId = arrRChangeElementId[(int) Stage.m_tComputeRandom.random(0, arrRChangeElementId.Count)];
            }
            addOperatorCache("changeElement", strChangeElementId);
            int? nLevel = Config.ElementConfig.getElementLevel(strChangeElementId);
            if (nLevel == null)
            {
                addOperatorCache("changeElement", "");
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            //LogUtil.AddLog("battle", "excuteSkill_ChangeElement "); // .MoreStringFormat(tTriggerGridCoord.Line, tTriggerGridCoord.Col, strChangeElementId));
            ENate.Grid tGrid = tChessBoard.getGrid(tTriggerGridCoord);
            if (tGrid == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            Element tElement = tGrid.getElementWithElementAttribute(ElementAttribute.Attribute.level, new ElementValue<int>(nLevel.Value));
            if (tElement == null)
            {
                tElement = m_tStage.m_tElementCreater.create(strChangeElementId, tGrid);
            }
            else
            {
                string strSrcHypotaxisId = tElement.getHypotaxisId();
                tElement.skillChangeElementId(strChangeElementId);
                string strAimHypotaxisId = tElement.getHypotaxisId();
                if (strSrcHypotaxisId != strAimHypotaxisId)
                {
                    m_tStage.m_tElementCreater.excuteGeneraterSkill(tElement.m_tGrid.m_tChessBoard, tElement, tElement.m_tGrid.m_tGridCoord);
                }
            }

            UnityEngine.Profiling.Profiler.EndSample();
        }

        void excuteSkill_Move(ChessBoard tChessBoard, JsonData.Skill_Config.Skill tSkill, GridCoord tSrcGridCoord, GridCoord tAimGridCoord)
        {
            UnityEngine.Profiling.Profiler.BeginSample("excuteSkill_Move");
            if (tSkill.move == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }
            var tSrcGrid = tChessBoard.getGrid(tSrcGridCoord);
            var tAimGrid = tChessBoard.getGrid(tAimGridCoord);
            if (tSrcGrid == null || tAimGrid == null)
            {
                return;
            }
            int nMoveLevel = 0;
            try
            {
                nMoveLevel = int.Parse(tSkill.move.level);
            }
            catch
            {
                return;
            }
            if (tSkill.move.moveType == "exchange")
            {
                List<Element> arrElementMove1 = new List<Element>();
                List<Element> arrElementMove2 = new List<Element>();
                var tSrcMoveElement = tSrcGrid.getElementWithElementAttribute(ElementAttribute.Attribute.level, nMoveLevel, true);
                if (tSrcMoveElement != null)
                {
                    arrElementMove1.Add(tSrcMoveElement);
                }
                var tAimMoveElement = tAimGrid.getElementWithElementAttribute(ElementAttribute.Attribute.level, nMoveLevel, true);
                if (tAimMoveElement != null)
                {
                    arrElementMove2.Add(tAimMoveElement);
                }
                if (arrElementMove1.Count > 0 && arrElementMove2.Count > 0)
                {
                    tChessBoard.m_tDropManager.createDropUnit(tSrcGrid.m_tGridCoord.Line, tSrcGrid.m_tGridCoord.Col, tAimGrid.m_tGridCoord.Line, tAimGrid.m_tGridCoord.Col, arrElementMove1, DropUnit.MoveType.normal);
                    tChessBoard.m_tDropManager.createDropUnit(tAimGrid.m_tGridCoord.Line, tAimGrid.m_tGridCoord.Col, tSrcGrid.m_tGridCoord.Line, tSrcGrid.m_tGridCoord.Col, arrElementMove2, DropUnit.MoveType.normal);
                }
                else
                {
                    if (tSrcMoveElement != null)
                    {
                        tSrcGrid.addElement(tSrcMoveElement);
                    }
                    if (tAimMoveElement != null)
                    {
                        tAimGrid.addElement(tAimMoveElement);
                    }
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        void excuteSkill_Reputation(JsonData.Skill_Config.Skill tSkill, ConditionConfig.MapArg mpArg)
        {
            if (mpArg == null)
            {
                mpArg = ConditionConfig.m_tConditionArg;
            }
            foreach (var tReputation in tSkill.reputation)
            {
                ConditionConfig.Reputation.set(tReputation.id, tReputation.value, mpArg);
            }
        }

        void excuteSkill_ExReputation(JsonData.Skill_Config.Skill tSkill, ConditionConfig.MapArg mpArg)
        {
            if (mpArg == null)
            {
                mpArg = ConditionConfig.m_tConditionArg;
            }
            foreach (var tReputation in tSkill.Ex_Reputation)
            {
                ConditionConfig.Reputation.set(tReputation.id, tReputation.value, mpArg);
            }
        }
        void excuteSkill_TriggerGridReputation(JsonData.Skill_Config.Skill tSkill, ChessBoard tChessBoard, GridCoord tGridCoord)
        {
            ConditionConfig.MapArg tMapArg = new ConditionConfig.MapArg();
            tMapArg.ChessBoard = tChessBoard;
            tMapArg.Grid = tChessBoard.getGrid(tGridCoord);
            foreach (var tReputation in tSkill.TargetGridReputation)
            {
                ConditionConfig.Reputation.set(tReputation.id, tReputation.value, tMapArg);
            }
        }
        IEnumerator excuteSkill_playSkillAnimation(JsonData.Skill_Config.Skill tSkill, ChessBoard tChessBoard, GridCoord tRootGridCoord, GridCoord tTriggerGridCoord)
        {
            if (string.IsNullOrEmpty(tSkill.SkillAnimationId) == true)
            {
                yield break;
            }
            Grid tRootGrid = tChessBoard.getGrid(tRootGridCoord);
            Grid tTriggerGrid = tChessBoard.getGrid(tTriggerGridCoord);
            Util.playENateAni(tSkill.SkillAnimationId, null, tRootGrid != null ? (Vector3?) tRootGrid.transform.position : null, tTriggerGrid != null ? (Vector3?) tTriggerGrid.transform.position : null);
        }

        IEnumerator excuteSkill(int nSkillOperatorId, ChessBoard tChessBoard, JsonData.Skill_Config.Skill tSkill, GridCoord tTriggerGridCoord, GridCoord tRootGridCoord, ConditionConfig.MapArg mpArg, Action pOverCallback)
        {
            do
            {
                if (ConditionConfig.checkCondition(tSkill.prefixCondition, mpArg) == false)
                {
                    break;
                }
                if (string.IsNullOrEmpty(tSkill.time) == false && float.Parse(tSkill.time) > 0)
                {
                    yield return ENateYield.WaitForSeconds(float.Parse(tSkill.time));
                }
                if (tSkill.waitDrop != null && tSkill.waitDrop.isWait == "true")
                {
                    float fEndWaitTime = 0;
                    if (string.IsNullOrEmpty(tSkill.waitDrop.endTime) == false)
                    {
                        fEndWaitTime = float.Parse(tSkill.waitDrop.endTime);
                    }
                    jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_DROP_STOP, getSkillMarkId());
                    pOverCallback += () =>
                    {
                        m_tStage.waitCallback(fEndWaitTime, () =>
                        {
                            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_DROP_OPEN, getSkillMarkId());
                        });
                    };
                    bool bIsDropDone = m_tStage.waitDropAniDone();
                    while (bIsDropDone == false)
                    {
                        yield return null;
                        bIsDropDone = m_tStage.waitDropAniDone();
                    }
                }
                List<ENate.Grid> arrGrid = new List<ENate.Grid>();
                excuteSkill_Reputation(tSkill, mpArg);
                excuteSkill_CostStep(tChessBoard, tSkill);
                if (mpArg != null && mpArg.Element != null)
                {
                    excuteSkill_SubTractTargetNum(tSkill, mpArg.Element);
                }
                switch (tSkill.target.type)
                {
                    case "point":
                        {
                            ConditionConfig.MapArg tMapArg = new ConditionConfig.MapArg();
                            tMapArg.Stage = tChessBoard.m_tStage;
                            tMapArg.ChessBoard = tChessBoard;
                            List<JsonData.Client_Config.Condition> arrCheckCondition = new List<JsonData.Client_Config.Condition>();
                            foreach (var strConditionId in tSkill.target.condition)
                            {
                                arrCheckCondition.Add(ConditionConfig.getCondition(strConditionId));
                            }
                            foreach (var strCheckPoint in tSkill.target.point)
                            {
                                try
                                {
                                    String[] arrstrPoint = strCheckPoint.Split(',');
                                    int nLine = int.Parse(arrstrPoint[0]);
                                    int nColumn = int.Parse(arrstrPoint[1]);
                                    ENate.Grid tGrid = tChessBoard.getGrid(tTriggerGridCoord.Line + nLine, tTriggerGridCoord.Col + nColumn);
                                    if (tGrid == null)
                                    {
                                        continue;
                                    }
                                    tMapArg.Grid = tGrid;
                                    if (mpArg != null)
                                    {
                                        tMapArg.Element = mpArg.Element;
                                    }
                                    bool bIsOk = true;
                                    foreach (var tCondition in arrCheckCondition)
                                    {
                                        if (ConditionConfig.checkCondition(tCondition, tMapArg) == false)
                                        {
                                            bIsOk = false;
                                            break;
                                        }
                                    }
                                    if (bIsOk == false)
                                    {
                                        continue;
                                    }
                                    arrGrid.Add(tGrid);
                                }
                                catch (System.Exception)
                                {
                                    Debug.LogError("excuteSkill point Split error ");
                                }
                            }
                            ConditionConfig.clearConditionCache(tSkill.target.condition);
                        }
                        break;
                    case "traverse":
                        {
                            ENateMatch.traversedAllGrid(tChessBoard, tTriggerGridCoord, tSkill.target.condition, ref arrGrid);
                        }
                        break;
                    default:
                        {
                            Debug.LogError("ERROR : skill is not have target type");
                            break;
                        }
                }
                if (arrGrid.Count <= 0)
                {
                    break;
                }
                excuteSkill_ExReputation(tSkill, mpArg);
                List<ENate.Grid> arrExcuteGrid = new List<ENate.Grid>();
                int nCount = 1;
                try
                {
                    nCount = int.Parse(tSkill.count);
                }
                catch (System.Exception) { }
                if (nCount >= arrGrid.Count)
                {
                    arrExcuteGrid = arrGrid;
                }
                else
                {
                    for (int i = 0; i < nCount; ++i)
                    {
                        if (arrGrid.Count <= 0)
                        {
                            break;
                        }
                        int nSub = (int) Stage.m_tComputeRandom.random(0, arrGrid.Count);
                        arrExcuteGrid.Add(arrGrid[nSub]);
                        arrGrid.RemoveAt(nSub);
                    }
                }
                ElementDestroy tElementDestroy = new ElementDestroy();
                foreach (var strDestoryedType in tSkill.destroyType)
                {
                    try
                    {
                        tElementDestroy.addDestroyType(int.Parse(strDestoryedType));
                    }
                    catch (System.Exception) { }
                }
                yield return excuteSkill_Ray(tChessBoard, tSkill, arrExcuteGrid, tRootGridCoord, (GridCoord ttTriggerGridCoord) =>
                {
                    excuteSkill_ChangeElement(nSkillOperatorId, tChessBoard, tSkill, ttTriggerGridCoord);
                });

                ENateCoroutine tENateCoroutine = new ENateCoroutine();
                // 挑选出对应的格子
                foreach (var tGrid in arrExcuteGrid)
                {
                    excuteSkill_TriggerGridReputation(tSkill, tChessBoard, tGrid.m_tGridCoord);
                    tENateCoroutine.Add(excuteSkill_playSkillAnimation(tSkill, tChessBoard, tRootGridCoord, tGrid.m_tGridCoord));
                    excuteSkill_Move(tChessBoard, tSkill, tTriggerGridCoord, tGrid.m_tGridCoord);
                    excuteSkill_area(tChessBoard, tSkill, tGrid.m_tGridCoord, tElementDestroy);
                    tENateCoroutine.Add(excuteSkill_Direction(tChessBoard, tSkill, tGrid.m_tGridCoord, tRootGridCoord, tElementDestroy));
                    tENateCoroutine.Add(excuteSkill_subSkill(nSkillOperatorId, tChessBoard, tSkill, tGrid.m_tGridCoord, tRootGridCoord, mpArg));
                }
                yield return tENateCoroutine.play();
            } while (false);
            if (pOverCallback != null)
                pOverCallback();
        }

        void event_skillOverCallBack(int nSkillIndex)
        {
            //LogUtil.AddLog("skillGroup", "nSkillIndex :" ); // .MoreStringFormat( nSkillIndex , "  Over"));
            if (--m_nSkillExistCount <= 0)
            {
                if (m_pOverCallBack != null)
                    m_pOverCallBack(m_nSkillGroupId);
                releaseAllGrid();
            }
        }
        public IEnumerator play(GridCoord tTriggerGridCoord, ConditionConfig.MapArg mpArg, Action<int> pOverCallBack)
        {
            ENateCoroutine tENateCoroutine = new ENateCoroutine();
            m_fExcuteBeginTime = Time.time;
            int nIndex = 0;
            m_nSkillCount = m_tSkillGroup.skill.Count;
            m_nSkillExistCount = m_nSkillCount;
            m_pOverCallBack = pOverCallBack;
            foreach (var tSkill in m_tSkillGroup.skill)
            {
                tENateCoroutine.Add(excuteSkill(m_nSkillOperatorId, m_tChessBoard, tSkill, tTriggerGridCoord, m_tRootGridCoord, mpArg, () =>
                {
                    event_skillOverCallBack(nIndex++);
                }));
            }
            return tENateCoroutine.play();
        }
    }
}