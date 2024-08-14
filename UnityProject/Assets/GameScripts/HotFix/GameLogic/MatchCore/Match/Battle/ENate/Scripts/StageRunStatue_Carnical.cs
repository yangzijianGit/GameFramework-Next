using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRunStatue_Carnical : ENate.StageRunStaue
{
    public void prefix(ENate.Stage tStage)
    {

    }
    public void run(ENate.Stage tStage)
    {

    }
    public bool isOver(ENate.Stage tStage)
    {
        return false;
    }
    public ENate.StageRunningStatus end(ENate.Stage tStage)
    {
        return ENate.StageRunningStatus.Over;
    }
}