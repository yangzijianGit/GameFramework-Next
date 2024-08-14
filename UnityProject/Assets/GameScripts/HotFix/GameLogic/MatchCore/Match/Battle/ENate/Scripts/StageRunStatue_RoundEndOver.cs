using System;
using System.Collections;
using System.Collections.Generic;
using ENate;
using UnityEngine;
public class StageRunStatue_RoundEndOver : ENate.StageRunStaue
{
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
        return ENate.StageRunningStatus.DetectingReset;
    }
}