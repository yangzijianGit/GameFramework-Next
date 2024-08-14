using System;
using System.Collections;
using System.Collections.Generic;
using ENate;
using UnityEngine;
public class StageRunStatue_DetectingRoundEnd : ENate.StageRunStaue
{
    int nCurrentExcuteRoundEnd = -1;
    void refreshCurrentExcuteRoundEnd(int nRoundCount)
    {
        nCurrentExcuteRoundEnd = nRoundCount;
    }
    public bool isCurrentRoundExcuteEndEvent(int nRoundCount)
    {
        return nCurrentExcuteRoundEnd == nRoundCount;
    }

    static Stage.EWait[] arreWait =
        new Stage.EWait[2]
        {
            Stage.EWait.drop, Stage.EWait.aniTask
        };

    public void prefix(ENate.Stage tStage)
    {
        tStage.bIsLock = true;
    }
    bool m_bIsOver = false;
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
        if (isCurrentRoundExcuteEndEvent(tStage.RoundCount) == true)
        {
            return StageRunningStatus.RoundEndOver;
        }
        refreshCurrentExcuteRoundEnd(tStage.RoundCount);
        jc.EventManager.Instance.NoticeEvent((int) (jc.STAGEEVENTTYPE.ET_STAGE_CALMNESS_PREPARE));
        return StageRunningStatus.RoundEnd;
    }
}