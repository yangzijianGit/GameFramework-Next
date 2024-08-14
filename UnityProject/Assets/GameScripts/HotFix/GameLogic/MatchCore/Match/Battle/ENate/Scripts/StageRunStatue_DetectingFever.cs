using System;
using System.Collections;
using System.Collections.Generic;
using ENate;
using UnityEngine;
public class StageRunStatue_DetectingFever : ENate.StageRunStaue
{
    public StageRunStatue_DetectingFever()
    {
        m_nTriggerCount = int.Parse(BattleArg.Instance.m_tStageArg.m_tMission.feverCount);
        // m_nComboCount = short.Parse(JsonManager.fever_config.root.game.trigger.combo);
    }
    /**
     * @Author: yangzijian
     * @description: how many times does it take you to get fever.
     */
    short m_nComboCount = 0;
    float m_nTriggerCount;
    float m_nCheckFeverCount = 100;

    public void prefix(ENate.Stage tStage)
    {
        tStage.bIsLock = true;
    }
    bool m_bIsOver = false;
    static Stage.EWait[] arreWait =
        new Stage.EWait[2]
        {
            Stage.EWait.drop, Stage.EWait.aniTask
        };
    public void run(ENate.Stage tStage)
    {
        m_bIsOver = false;
        if (tStage.waitAniDone(arreWait) == false)
        {
            return;
        }
        m_bIsOver = true;
    }
    public bool isOver(ENate.Stage tStage)
    {
        return m_bIsOver;
    }
    public ENate.StageRunningStatus end(ENate.Stage tStage)
    {
        if (checkTrigger(Data.PlayerData.Instance.lFeverPowerNum) == true)
        {
            trigger();
            return ENate.StageRunningStatus.Fever;
        }
        else
        {
            return StageRunningStatus.DetectingRoundEnd;
        }
    }

    public bool checkTrigger(long lFeverPower)
    {
        return lFeverPower >= Data.PlayerData.Instance.lCheckFeverCount && m_nTriggerCount > 0;
    }

    public void trigger()
    {
        // play effect and in trigger 
        m_nTriggerCount--;
        Data.PlayerData.Instance.lFeverPowerNum = 0;
    }
}