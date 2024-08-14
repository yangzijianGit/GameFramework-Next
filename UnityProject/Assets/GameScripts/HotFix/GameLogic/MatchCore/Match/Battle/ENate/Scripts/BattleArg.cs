/*
        author      :       yangzijian
        time        :       2019-12-17 14:09:28
        function    :       battle arg
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{

    public class StageArg
    {
        public StageArg(JsonData.Stage_mission_Client.Mission tMission)
        {
            m_tMission = tMission;
        }
        public JsonData.Stage_mission_Client.Mission m_tMission;
        public bool m_bIsWin;
        public bool m_bIsOutOfSteps;
    }
    public class BattleArg : GameBase.Singleton<BattleArg>
    {
        public Dictionary<string, int> m_arrTargetNum = new Dictionary<string, int>(); // 当前关卡目标数量 
        public Dictionary<string, int> m_arrTargetSrcNum = new Dictionary<string, int>(); // 当前关卡目标数量 
        public StageArg m_tStageArg;
        // public Protocol.Stage.S_StagePrePareResult m_tS_StageStartResult;
        // public Protocol.Stage.S_StageEndResult m_tS_StageEndResult;
        // public void createArg(StageArg tStageArg, Protocol.Stage.S_StagePrePareResult tS_StageStartResult)
        // {
        //     m_tStageArg = tStageArg;
        //     m_tS_StageStartResult = tS_StageStartResult;
        //     m_pIsUITarget = null;
        //     m_arrTargetNum.Clear();
        //     m_arrTargetSrcNum.Clear();
        // }
        // public void setBattleNetResult(Protocol.Stage.S_StageEndResult tS_StageEndResult)
        // {
        //     m_tS_StageEndResult = tS_StageEndResult;
        // }

        Func<string, bool> m_pIsUITarget = null;

        Func<Element, bool> m_pTriggerPower = null;

        public void setTargetFunc(Func<string, bool> pIsUITarget)
        {
            m_pIsUITarget = pIsUITarget;
        }
        public void setTriggerPowerFunc(Func<Element, bool> pTriggerPower)
        {
            m_pTriggerPower = pTriggerPower;
        }

        public bool isUITarget(string strElementId)
        {
            if (m_pIsUITarget == null) return false;
            return m_pIsUITarget(strElementId);
        }

        public bool isGenerPower(Element tElement)
        {
            if (m_pTriggerPower == null) return false;
            return m_pTriggerPower(tElement);
        }

        /////////////////////////////////////
        ///参数
        /////////////////////////////////////

        public bool m_bIsBuyStep = false;
        public int m_nAddStep = 0;
        public void buyStep(int nAddStep)
        {
            m_nAddStep = nAddStep;
            m_bIsBuyStep = true;
        }

        public void clearBuyStepCache()
        {
            m_nAddStep = 0;
            m_bIsBuyStep = false;
        }
    }

}