using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRunStatue_Over : ENate.StageRunStaue
{
    public void prefix(ENate.Stage tStage)
    {
        tStage.bIsLock = true;
    }
    public void run(ENate.Stage tStage)
    {

    }
    public bool isOver(ENate.Stage tStage)
    {
        return true;
    }
    public ENate.StageRunningStatus end(ENate.Stage tStage)
    {
        tStage.bIsOver = true;
        return ENate.StageRunningStatus.Over;
    }
}