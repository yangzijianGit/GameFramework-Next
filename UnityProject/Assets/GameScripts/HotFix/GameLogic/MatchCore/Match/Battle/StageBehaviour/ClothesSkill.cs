/*
 * @Description: ClothesSkill
 * @Author: yangzijian
 * @Date: 2020-04-10 11:28:04
 * @LastEditors: yangzijian
 * @LastEditTime: 2024-08-15 01:56:49
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ENate
{

    public class ClothesSkill : MonoBehaviour
    {
        int m_nComboCurrentPower = 0;
        public int ComboCurrentPower
        {
            set
            {
                m_nComboCurrentPower = value;
                changeSkillPro();
            }
            get
            {
                return m_nComboCurrentPower;
            }
        }
        int m_nComboPowerCount = 0;

        bool m_bIsGetOneSkillFull = false;

        public GameObject Skill_btn;
        public GameObject Skill_pic;
        public GameObject Skill_bar;
        public GameObject SkillNotice_p;
        public GameObject SkillNotice_num;
        public GameObject StageUIAttachNode_mask;
        public Transform StageUIAttachNode;
        public Transform StageUIAttachNode2;

        Stage m_tStage;
        // Protocol.Stage.S_StagePrePareResult m_tS_StageStartResult;
        public jc.EventManager.EventObj m_tEventObj = new jc.EventManager.EventObj();

        List<BattleBuff> m_arrBuff = new List<BattleBuff>();
        JsonData.Clothes_Config.Clothes m_tClothConfig;
        bool m_bIsNeedOperatingTargetChoose = false;
        List<string> m_arrCheckOperationCond = new List<string>();

        bool m_bIsEnterWaitChooseTargetStatus = false;

        public bool bIsEnterWaitChooseTargetStatus
        {
            get
            {
                return m_bIsEnterWaitChooseTargetStatus;
            }
            set
            {
                if (m_bIsEnterWaitChooseTargetStatus == value)
                {
                    return;
                }
                m_bIsEnterWaitChooseTargetStatus = value;
                switchOperationStatus(value);
            }
        }

        void initConfig()
        {
            //LogUtil.AddLog("battle","m_nComboCurrentPower       "); // .MoreStringFormat(m_nComboCurrentPower));
            // m_nComboPowerCount = int.Parse(JsonManager.clothes_config.root.game.trigger.energyTotal);
            //LogUtil.AddLog("battle", "m_nComboPowerCount        "); // .MoreStringFormat(m_nComboPowerCount));
            ComboCurrentPower = 0;
            m_bIsGetOneSkillFull = false;
        }

        private void Start()
        {
            // try
            // {
            //     jc.EventManager.Instance.BindBtnClick(Skill_btn, (object o) =>
            //     {
            //         jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_CLOTHESSKILL_POWERFULL_OPERATING);
            //     });
            // }
            // catch
            // {

            // }
        }

        private void OnEnable()
        {
            // m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Init, event_initStage);
            // m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_CLOTHESSKILL_POWERADD, event_addPowerAim);
            // m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_CLOTHESSKILL_POWERFULL_OPERATING, event_powerFull_Operating);
            // m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_CLOTHESSKILL_POWERFULL_ProChange, event_powerFull_proChange);
            // m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Handle_SelectGrid, event_targetSelect);
            // m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_UI_BATTLE_MASK_click, event_clickMask);
            // StageUIAttachNode.gameObject.SetActive(true);
            // StageUIAttachNode2.gameObject.SetActive(true);
            // SkillNotice_p.SetActive(false);
            // Skill_btn.SetActive(string.IsNullOrEmpty(Data.PlayerData.Instance.CurrentClothId) == false);
        }

        private void OnDisable()
        {
            m_tEventObj.clear();
        }

        int computeAddNumBuff(int nValue)
        {
            foreach (var tBuff in m_arrBuff)
            {
                if (tBuff.EBuffType == BattleBuff.BuffType.skillPower_trigger_addSpeedPercent)
                {
                    nValue += nValue * int.Parse(tBuff.m_arrParam[0]) / 100;
                }
            }
            return nValue;
        }

        void excuteBuff()
        {
            foreach (var tBuff in m_arrBuff)
            {
                if (tBuff.EBuffType == BattleBuff.BuffType.skillPower_stageBegin_addSpeedPercent)
                {
                    ComboCurrentPower += m_nComboPowerCount * int.Parse(tBuff.m_arrParam[0]) / 100;
                }
                else if (tBuff.EBuffType == BattleBuff.BuffType.step_stageBegin_addStepNum)
                {
                    m_tStage.addStep(int.Parse(tBuff.m_arrParam[0]));
                }
            }
        }
        void excuteStartSkill()
        {
            // foreach (var strSkillId in m_tS_StageStartResult.StartSkills)
            // {
            //     m_tStage.m_tSkillManager.excuteSkill(m_tStage.CurrentChessBoard, strSkillId, GridCoord.NULL, null);
            // }
        }
        void excuteOperationSkill(ChessBoard tChessBoard, GridCoord tGridCoord)
        {
            // m_bIsGetOneSkillFull = false;
            // ComboCurrentPower = 0;
            // changeSkillPro();
            // List<string> arrExcuteSkillId = new List<string>();
            // // Protocol.Stage.SkillGroup tCurrentSkillGroup = null;
            // int nWeightCount = 0;
            // foreach (var tSkillGroup in m_tS_StageStartResult.SkillGroups)
            // {
            //     nWeightCount += tSkillGroup.Weight;
            // }
            // var lRandomWeight = Stage.m_tComputeRandom.random(0, nWeightCount);
            // foreach (var tSkillGroup in m_tS_StageStartResult.SkillGroups)
            // {
            //     lRandomWeight -= tSkillGroup.Weight;
            //     if (lRandomWeight <= 0)
            //     {
            //         tCurrentSkillGroup = tSkillGroup;
            //         break;
            //     }
            // }
            // if (tCurrentSkillGroup == null)
            // {
            //     return;
            // }
            // foreach (var tSkillRandom in tCurrentSkillGroup.SkillList)
            // {
            //     var lRandomPower = Stage.m_tComputeRandom.random(0, 101);
            //     if (lRandomPower <= tSkillRandom.Probability)
            //     {
            //         arrExcuteSkillId.Add(tSkillRandom.Skill_);
            //     }
            // }
            // foreach (var strSkillId in arrExcuteSkillId)
            // {
            //     m_tStage.m_tSkillManager.excuteSkill(tChessBoard, strSkillId, tGridCoord, null);
            // }
            // jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ChangeStatuesToCheckWin, m_tStage);
        }
        void event_initStage(object obj)
        {
            // m_arrBuff.Clear();
            // m_tStage = obj as Stage;
            // initConfig();
            // m_tS_StageStartResult = BattleArg.Instance.m_tS_StageStartResult;
            // m_bIsEnterWaitChooseTargetStatus = false;
            // foreach (var strBuffId in m_tS_StageStartResult.StartBuffs)
            // {
            //     BattleBuff tBattleBuff = BattleBuff.create(strBuffId);
            //     if (tBattleBuff == null)
            //     {
            //         continue;
            //     }
            //     m_arrBuff.Add(tBattleBuff);
            // }
            // m_tClothConfig = Config.ClothesConfig.getClothesConfig(Data.PlayerData.Instance.CurrentClothId);
            // if (m_tClothConfig != null)
            // {
            //     m_bIsNeedOperatingTargetChoose = int.Parse(m_tClothConfig.targetChoose) != 0;
            //     m_arrCheckOperationCond.Clear();
            //     if (string.IsNullOrEmpty(m_tClothConfig.targetChooseCondition) == false)
            //     {
            //         m_arrCheckOperationCond.Add(m_tClothConfig.targetChooseCondition);
            //     }
            //     Skill_pic.setImage(m_tClothConfig.imgIcon);
            // }
            // excuteBuff();
            // excuteStartSkill();
        }
        float m_fNoticeShowTime;
        void changeSkillPro()
        {
            // float fPercent = (float) (ComboCurrentPower * 100) / (float) m_nComboPowerCount;
            // //LogUtil.AddLog("battle","fPercent       "); // .MoreStringFormat(fPercent));
            // if (fPercent < 0)
            // {
            //     fPercent = 0;
            // }
            // else if (fPercent >= 100)
            // {
            //     fPercent = 100;
            //     m_bIsGetOneSkillFull = true;
            //     // if (m_bIsGetOneSkillFull == true)
            //     // {
            //     //     fPercent = 1;
            //     // }
            //     // else
            //     // {
            //     //     m_nComboCurrentPower = 0;
            //     //     fPercent = 0;
            //     //     m_bIsGetOneSkillFull = true;
            //     // }
            // }
            // Image tImage = Skill_bar.GetComponent<Image>();
            // tImage.fillAmount = 1 - fPercent / 100;
            // //LogUtil.AddLog("battle", "fillAmount        "); // .MoreStringFormat(tImage.fillAmount));
            // jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_CLOTHESSKILL_POWERFULL_ProChange);
            // m_fNoticeShowTime = Time.time;
            // SkillNotice_p.SetActive(true);
            // SkillNotice_num.setTextParam(((int) fPercent).ToString());
            // StartCoroutine(ENateYield.WaitForSeconds(1, () =>
            // {
            //     SkillNotice_p.SetActive(false);
            // }));

        }

        void switchOperationStatus(bool bIsEnterWaitChoose)
        {
            if (bIsEnterWaitChoose == true)
            {
                if (m_tClothConfig == null)
                {
                    return;
                }
                m_tStage.CurrentStageRunningStatus = ENate.StageRunningStatus.ClothesSkill;
            }
            else
            {
                if (m_tStage != null && m_tStage.CurrentStageRunningStatus == ENate.StageRunningStatus.ClothesSkill)
                {
                    m_tStage.CurrentStageRunningStatus = ENate.StageRunningStatus.Calmness;
                }
            }
            // jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_UI_BATTLE_MASK, bIsEnterWaitChoose);
        }

        void event_addPowerAim(object obj = null)
        {
            ComboCurrentPower += computeAddNumBuff((int) obj);
        }

        void event_powerFull_Operating(object obj = null)
        {
            if (m_tStage.CurrentStageRunningStatus != StageRunningStatus.Calmness)
            {
                return;
            }
            if (bIsEnterWaitChooseTargetStatus == true)
            {
                return;
            }
            if (m_bIsGetOneSkillFull == true)
            {
                if (m_bIsNeedOperatingTargetChoose == true)
                {
                    bIsEnterWaitChooseTargetStatus = true;
                }
                else
                {
                    excuteOperationSkill(m_tStage.CurrentChessBoard, GridCoord.Zero);
                }
            }
        }
        void event_powerFull_proChange(object obj = null)
        {
            // BSKumaPower_pic1.SetActive(m_bIsGetOneSkillFull);
            // BSKumaPower_p.SetActive(m_bIsGetOneSkillFull);
        }

        void event_targetSelect(object obj)
        {
            if (m_tStage.CurrentStageRunningStatus != StageRunningStatus.ClothesSkill)
            {
                return;
            }
            var tGrid = obj as ENate.Grid;
            if (tGrid == null)
            {
                return;
            }
            ConditionConfig.MapArg tMapArg = new ConditionConfig.MapArg();
            tMapArg.Stage = m_tStage;
            tMapArg.ChessBoard = m_tStage.CurrentChessBoard;
            tMapArg.Grid = tGrid;
            if (ConditionConfig.checkCondition(m_arrCheckOperationCond, tMapArg) == false)
            {
                return;
            }
            if (bIsEnterWaitChooseTargetStatus == true)
            {
                excuteOperationSkill(tGrid.m_tChessBoard, tGrid.m_tGridCoord);
                bIsEnterWaitChooseTargetStatus = false;
            }
        }

        void event_clickMask(object obj)
        {
            bIsEnterWaitChooseTargetStatus = false;
        }

    }

}