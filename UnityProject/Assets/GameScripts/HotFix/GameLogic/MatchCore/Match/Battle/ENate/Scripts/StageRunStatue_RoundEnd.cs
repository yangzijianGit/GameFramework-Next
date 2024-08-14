using System;
using System.Collections;
using System.Collections.Generic;
using ENate;
using UnityEngine;
public class StageRunStatue_RoundEnd : ENate.StageRunStaue
{
    int m_nRoundEndPriority;

    public int RoundEndPriority
    {
        get
        {
            return m_nRoundEndPriority;
        }
    }

    public void initRoundEndSkill()
    {
        m_nRoundEndPriority = Config.SkillConfig.getFirstRondEndPriority();
    }
    public int nextRoundEndSkill()
    {
        m_nRoundEndPriority = Config.SkillConfig.getNextRoundEndPriority(m_nRoundEndPriority);
        return m_nRoundEndPriority;
    }

    public void prefix(ENate.Stage tStage)
    {
        tStage.bIsLock = true;
        initRoundEndSkill();
    }
    static Stage.EWait[] arreWait =
        new Stage.EWait[2]
        {
            Stage.EWait.drop, Stage.EWait.aniTask
        };
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
        List<Element> arrElement = new List<Element>();
        bool bIsStageRunning = false;
        while (arrElement.Count <= 0 && RoundEndPriority != -1)
        {
            var arrElementId = Config.SkillConfig.getRoundEndElementId(RoundEndPriority);
            nextRoundEndSkill();
            do
            {
                if (arrElementId == null)
                {
                    break;
                }
                foreach (var strElementId in arrElementId)
                {
                    if (bIsStageRunning == false && strElementId == "stage")
                    {
                        bIsStageRunning = true;
                    }
                    arrElement.AddRange(tStage.m_tENateCollecter.getBlockElement(strElementId, tStage.CurrentChessBoard.Index));
                }
            } while (false);
        }
        if (arrElement.Count <= 0 && bIsStageRunning == false)
        {
            return StageRunningStatus.RoundEndOver;
        }
        else
        {
            ConditionConfig.MapArg mpArg = new ConditionConfig.MapArg();
            mpArg.ChessBoard = tStage.CurrentChessBoard;
            mpArg.Stage = tStage;

            foreach (var tElement in arrElement)
            {
                var arrSkill = Config.SkillConfig.getRoundEndSkillNode(tElement.ElementId);
                if (arrSkill != null)
                {
                    GridCoord tGridCoord = GridCoord.NULL;
                    if (tElement.m_tGrid != null)
                    {
                        tGridCoord = tElement.m_tGrid.m_tGridCoord;
                    }
                    mpArg.GridCoord = tGridCoord;
                    mpArg.Element = tElement;
                    mpArg.Grid = tElement.m_tGrid;
                    if (tElement.m_tGrid != null)
                    {
                        mpArg.ChessBoard = tElement.m_tGrid.m_tChessBoard;
                    }
                    tStage.m_tSkillManager.excuteSkill(tStage.CurrentChessBoard, arrSkill, tGridCoord, mpArg);
                }
            }
            if (bIsStageRunning == true)
            {
                var arrSkill = Config.SkillConfig.getRoundEndSkillNode("stage");
                tStage.m_tSkillManager.excuteSkill(tStage.CurrentChessBoard, arrSkill, GridCoord.NULL, null);
                bIsStageRunning = false;
            }
        }
        return StageRunningStatus.RoundEnd;
    }
}