using System;
using System.Collections;
using System.Collections.Generic;
using ENate;
using UnityEngine;

public class StageRunStatue_Calmness : ENate.StageRunStaue
{
    float m_fBeginTime;
    float m_fTipsCheckTime;

    public StageRunStatue_Calmness()
    {
        m_fTipsCheckTime = float.Parse(ElementBehavior.getAniArgValue("tipsCheckTime"));
    }
    public void prefix(ENate.Stage tStage)
    {
        tStage.bIsLock = false;
        m_fBeginTime = Time.time;
    }
    public void run(ENate.Stage tStage)
    {
        tStage.waitDropAniDone();
        if (tStage.m_tStageTips != null && tStage.m_tStageTips.checkIsTips() == false)
        {
            if (Time.time - m_fBeginTime > m_fTipsCheckTime)
            {
                tStage.m_tStageTips.show();
            }
        }

    }
    public bool isOver(ENate.Stage tStage)
    {
        return false;
    }
    public ENate.StageRunningStatus end(ENate.Stage tStage)
    {
        return ENate.StageRunningStatus.Calmness;
    }
}