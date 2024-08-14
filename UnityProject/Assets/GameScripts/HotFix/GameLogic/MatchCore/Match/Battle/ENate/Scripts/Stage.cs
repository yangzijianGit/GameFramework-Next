/*
 *      author      :       yangzijian
 *      time        :       2019-12-16 14:57:20
 *      function    :       stage
 */

using System;
using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace ENate
{

    public partial class Stage : MonoBehaviour
    {
        public List<ChessBoard> m_arrChessBoard;
        private int m_nCurrentChessBoardIndex;
        public int CurrentChessBoardIndex
        {
            get
            {
                return m_nCurrentChessBoardIndex;
            }
        }
        public GameObject m_tChessBaordAttachNode;
        public ChessBoard CurrentChessBoard
        {
            get
            {
                return m_arrChessBoard[m_nCurrentChessBoardIndex];
            }
        }
        public ChessBoard getChessBoardWithIndex(int nIndex)
        {
            try
            {
                return m_arrChessBoard[nIndex];
            }
            catch (System.Exception)
            {

            }
            return null;
        }

        public void changeChessBoard(int nChessBoardIndex)
        {
            m_nCurrentChessBoardIndex = nChessBoardIndex;
        }

        public ElementCreater m_tElementCreater;
        static public ENateRandom m_tENateRandom = new ENateRandom();
        public StageData m_tStageData;
        static public ENateRandom m_tComputeRandom = new ENateRandom();
        public GameObject m_tStageUI; // Stage.prefab
        public remove m_tRemove;

        public DropStrategy m_tDropStrategy;

        public WinRules m_tWinRules;

        public ENateCollecter m_tENateCollecter;

        public SkillManager m_tSkillManager;
        public ENateAnimation.ENateAniManager m_tAniManager;

        public Group m_tGroup;

        private bool m_bIsWin = false;
        public bool bIsWin
        {
            get { return m_bIsWin; }
            set
            {
                m_bIsWin = value;

            }
        }

        private bool m_bIsOver = false;
        public bool bIsOver
        {
            get { return m_bIsOver; }
            set
            {
                m_bIsOver = value;

            }
        }

        private bool m_bIsLock = false;
        public bool bIsLock
        {
            get { return m_bIsLock; }
            set
            {
                m_bIsLock = value;

            }
        }

        public StageTips m_tStageTips = null;

        jc.EventManager.EventObj m_tEventObj;
        ENateTaskManagement m_tENateTaskManagement;

        public int createTask(Func<bool> pTaskFunction, Action<ENateTask> pCallBack = null)
        {
            return m_tENateTaskManagement.createTask(pTaskFunction, pCallBack);
        }

        public bool isTaskOver(int nTaskId)
        {
            return m_tENateTaskManagement.isTaskRunning(nTaskId) == false && Target.isTargetAniOver() == true;
        }

        public int m_nCombo = 0;

        void event_changeStatuesToCheckWin(object o)
        {
            CurrentStageRunningStatus = StageRunningStatus.DetectingWin;
        }
        void event_stepFresh(object o)
        {
            roundRefresh();
        }

        public bool isTarget(string strHypotaxisId)
        {
            return m_tWinRules.isTarget(strHypotaxisId);
        }
        void Awake()
        {
            m_tElementCreater = new ElementCreater(this);
            m_tStageData = new StageData();
            m_tDropStrategy = new DropStrategy(this, m_tComputeRandom);
            var tDropNode = DropDeviceRead.getDropNode("default");
            foreach (var tDropStrategy in tDropNode.m_pDropStrategy)
            {
                m_tDropStrategy.addDeviceUnit(new DropDeviceUnit(tDropStrategy.m_strElementId, int.Parse(tDropStrategy.m_nPower)));
            }
            m_tWinRules = new WinRules(this);
            m_tENateCollecter = new ENateCollecter(this);
            m_tSkillManager = this.gameObject.AddComponent<SkillManager>();
            m_tAniManager = this.gameObject.AddComponent<ENateAnimation.ENateAniManager>();
            m_tGroup = new Group(this);
            m_tEventObj = new jc.EventManager.EventObj();
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_STEPFRESH, event_stepFresh);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ChangeStatuesToCheckWin, event_changeStatuesToCheckWin);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_CreateTask, event_createTask);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_TIPS_Add, event_newTips);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_TIPS_Hide, event_hideTips);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_TIPS_Remove, event_removeTips);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_TIPS_Show, event_showTips);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_FEVER_ADDPOWER, event_addFeverPower);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_ENTER_STAGE_START, event_EnterChessBoard);

            m_tENateTaskManagement = gameObject.AddComponent<ENateTaskManagement>();
        }

        public void OnDestroy()
        {
            m_tEventObj.clear();
            m_tENateCollecter.Dispose();
        }

        /**
         * @Author: yangzijian
         * @description: drop manager and call in every fixUpdate
         */

        public static void Create(string strStageId, Transform tAttachNode)
        {
            load(strStageId, tAttachNode);
        }

        protected void read(string strStageId)
        {
            StageConfig tStageConfig = StageRead.readStageConfig(strStageId);
            this.readStageConfig(tStageConfig);
            m_arrStageRunningStatusExcutor = new StageRunStaue[(int) StageRunningStatus.num];
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.DetectingWin] = new StageRunStatue_DetectingWin();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.DetectingReset] = new StageRunStatue_DetectingReset();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.ResetAniOverCheckWin] = new StageRunStatue_ResetAniOverCheckWin();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.DetectingRoundEnd] = new StageRunStatue_DetectingRoundEnd();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.RoundEnd] = new StageRunStatue_RoundEnd();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.RoundEndOver] = new StageRunStatue_RoundEndOver();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.Calmness] = new StageRunStatue_Calmness();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.Win] = new StageRunStatue_Win();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.MoveToNextChessBoard] = new StageRunStatue_MoveToNextChessBoard();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.Carnical] = new StageRunStatue_Carnical();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.Failed] = new StageRunStatue_Failed();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.Over] = new StageRunStatue_Over();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.DetectingFever] = new StageRunStatue_DetectingFever();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.Fever] = new StageRunStatue_Fever();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.ClothesSkill] = new StageRunStatue_ClothesSkill();
            m_arrStageRunningStatusExcutor[(int) StageRunningStatus.UseHammer] = new StageRunStatue_UseHammer();

            CurrentStageRunningStatus = StageRunningStatus.DetectingReset;
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_Init, this);
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_DROP_DROPOVERCHECK_Prefix);

            Data.PlayerData.Instance.lCheckFeverCount = long.Parse(JsonManager.fever_config.root.game.trigger.energyTotal);
        }

        protected static void load(string strStageId, Transform tAttachNode)
        {
            // load stage prefab 
            // fix chessboard and layer
            // 从配置中读取属性， 并填充
            Action<GameObject> callback = (o) =>
            {
                GameObject tInStageUI = o;
                tInStageUI.name = "Stage";
                Stage tStage = tInStageUI.GetComponent<Stage>();
                tStage.attch(tAttachNode);
                tStage.read(ENate.BattleArg.Instance.m_tStageArg.m_tMission.stgId);
            };
            jc.ResourceManager.Instance.LoadPrefab("ENate/Prefabs/Stage", callback, true);
        }

        public void attch(Transform tAttachNode)
        {
            var transform = gameObject.GetComponent<RectTransform>();
            transform.SetParent(tAttachNode);
            transform.localScale = Vector3.one;
            transform.anchorMin = new Vector2(0.0f, 0.0f);
            transform.anchorMax = new Vector2(1.0f, 1.0f);
            transform.offsetMin = new Vector2(0.0f, 0.0f);
            transform.offsetMax = new Vector2(0.0f, 0.0f);
        }

        /**
         * @Author: yangzijian
         * @description: 
         * @param {type} 
         * @return: whether or not it was reset
         */

        /**
         * @Author: yangzijian
         * @description: update stage statue 
            DetectingWin, // 检测胜利
            DetectingReset, // 检测重置
            Prepare, // 棋盘准备状态
            Drop, // 掉落状态
            Calmness, // 平静
            Win, // 胜利
            Carnical, // 结束之后播放动画状态
            Failed, // 失败
            Over // 结束
         */

        public bool waitDropAniDone()
        {
            UnityEngine.Profiling.Profiler.BeginSample("waitDropAniDone");
            CurrentChessBoard.updateDropManager();
            CurrentChessBoard.checkConnectChessBoard();
            bool bIsOk = true;
            do
            {
                // wait drop unit over 
                if (CurrentChessBoard.isCalmness() == false)
                {
                    bIsOk = false;
                    break;
                }
                if (CurrentChessBoard.detectingPoint() == true)
                {
                    jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_DROP_DROPOVERCHECK_Prefix);
                    jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_DROP_DROPOVERCHECK);

                    bIsOk = false;
                    break;
                }

            } while (false);
            UnityEngine.Profiling.Profiler.EndSample();
            return bIsOk;
        }

        public bool waitTaskAniDone()
        {
            return m_tENateTaskManagement.isAnyAni() == false && Target.isTargetAniOver() == true && m_tAniManager.isAnyAni() == false && m_tSkillManager.isAnyPlaying() == false;
        }
        public enum EWait
        {
            drop,
            aniTask
        }

        public bool waitAniDone(EWait[] arreWait)
        {
            bool bIsDone = true;
            foreach (var eWait in arreWait)
            {
                switch (eWait)
                {
                    case EWait.drop:
                        {
                            if (waitDropAniDone() == false)
                            {
                                bIsDone = false;
                            }
                        }
                        break;
                    case EWait.aniTask:
                        {
                            if (waitTaskAniDone() == false)
                            {
                                bIsDone = false;
                            }
                        }
                        break;
                }
                if (bIsDone == false)
                {
                    break;
                }
            }
            return bIsDone;
        }

        int m_nRoundCount = 0;
        Counter m_tRoundCounter = new Counter();
        public Counter m_tExcuteCounter = new Counter();

        public int RoundCount
        {
            get
            {
                return m_nRoundCount;
            }
        }
        void roundRefresh()
        {
            m_nRoundCount = m_tRoundCounter.count();
        }

        public bool isCurrentRound(int nRoundCount)
        {
            return nRoundCount == m_nRoundCount;
        }

        StageRunStaue[] m_arrStageRunningStatusExcutor;
        StageRunningStatus m_eCurrentStageRunningStatus;

        public StageRunningStatus CurrentStageRunningStatus
        {
            set
            {
                if (m_eCurrentStageRunningStatus == value)
                {
                    return;
                }
                m_eCurrentStageRunningStatus = value;
                CurrentStageRun.prefix(this);
            }
            get
            {
                return m_eCurrentStageRunningStatus;
            }
        }
        public StageRunStaue CurrentStageRun
        {
            get
            {
                return m_arrStageRunningStatusExcutor[(int) m_eCurrentStageRunningStatus];
            }
        }

        public void Update()
        {
            try
            {
                if (bIsOver == true)
                {
                    return;
                }
                if (CurrentChessBoard == null)
                {
                    return;
                }
                if (CurrentStageRun != null)
                {
                    CurrentStageRun.run(this);
                    if (CurrentStageRun.isOver(this) == true)
                    {
                        CurrentStageRunningStatus = CurrentStageRun.end(this);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        public void useStep()
        {
            m_tStageData.m_nStep--;
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_STEPFRESH, this);
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ChangeStatuesToCheckWin, this);
        }

        public void addStep(int nAddStep)
        {
            bIsOver = false;
            m_tStageData.m_nStep += nAddStep;
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ADDSTEP, this);
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_STEPFRESH, this);
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ChangeStatuesToCheckWin, this);
        }

        public void delStep(int nDelStep)
        {
            m_tStageData.m_nStep -= nDelStep;
            if (m_tStageData.m_nStep < 0)
            {
                m_tStageData.m_nStep = 0;
            }
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_STEPFRESH, this);
        }

        void event_createTask(object o)
        {
            KeyValuePair<float, Action> tDelayTask = (KeyValuePair<float, Action>) o;
            waitCallback(tDelayTask.Key, tDelayTask.Value);
        }
        public void waitCallback(float fDuration, Action pCallback)
        {
            float fBeginTime = Time.time;
            createTask(() =>
            {
                return Time.time - fBeginTime >= fDuration;
            }, (ENateTask t) =>
            {
                if (pCallback != null) pCallback();
            });
        }

        void event_newTips(object o)
        {
            event_removeTips(null);
            Dictionary<string, object> mpArg = o as Dictionary<string, object>;
            if (mpArg == null)
            {
                return;
            }
            if (mpArg["type"] as string == "super")
            {
                m_tStageTips = StageTips.create(mpArg["special"] as Element);
            }
            else if (mpArg["type"] as string == "normal")
            {
                var arrTipsElement = mpArg["tipsElement"] as List<Element>;
                var tSpecialElement = mpArg["special"] as Element;
                var eDir = (Direction) mpArg["dir"];
                m_tStageTips = StageTips.create(tSpecialElement, eDir, arrTipsElement);
            }
        }

        void event_hideTips(object o)
        {
            if (m_tStageTips != null)
                m_tStageTips.clear();
        }
        void event_removeTips(object o)
        {
            if (m_tStageTips != null)
                m_tStageTips.clear();
            m_tStageTips = null;
        }
        void event_showTips(object o)
        {
            if (m_tStageTips != null)
                m_tStageTips.show();
        }

        void event_addFeverPower(object o)
        {
            Data.PlayerData.Instance.lFeverPowerNum += (int) o;
        }

        public void excuteStartItem()
        {
            // foreach (var strItemId in ENate.BattleArg.Instance.m_tS_StageStartResult.SuccessItem)
            // {
            //     var tItemConfig = Config.ItemConfig.getItemConfig(strItemId);
            //     if (tItemConfig == null)
            //     {
            //         Debug.LogError( "ERROR:item is not exist "); // .MoreStringFormat(strItemId));
            //         continue;
            //     }
            //     m_tSkillManager.excuteSkill(CurrentChessBoard, tItemConfig.startSkill, GridCoord.NULL, null);
            // }
        }

        void event_EnterChessBoard(object o)
        {
            excuteStartItem();
        }
    }
}