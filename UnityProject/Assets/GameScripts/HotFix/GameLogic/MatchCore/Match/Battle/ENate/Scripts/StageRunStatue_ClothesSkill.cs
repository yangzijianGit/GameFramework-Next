using System;
using System.Collections;
using System.Collections.Generic;
using ENate;
using UnityEngine;

public class StageRunStatue_ClothesSkill : ENate.StageRunStaue
{
    public void prefix(ENate.Stage tStage)
    {
        tStage.bIsLock = false;
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
        return ENate.StageRunningStatus.ClothesSkill;
    }
}