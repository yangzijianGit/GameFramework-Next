using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    public class StageRunStatue_DetectingReset : ENate.StageRunStaue
    {
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
            ENate.ChessBoardReset.ECheckResetType eCheckResetType = tStage.CurrentChessBoard.m_pChessBoardReset.reset();
            switch (eCheckResetType)
            {
                case ENate.ChessBoardReset.ECheckResetType.someOneOk:
                case ENate.ChessBoardReset.ECheckResetType.super:
                    {
                        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE_CALMNESS);
                        return ENate.StageRunningStatus.ResetAniOverCheckWin;
                    }
                case ENate.ChessBoardReset.ECheckResetType.over:
                    {
                        return ENate.StageRunningStatus.Failed;
                    }
                case ENate.ChessBoardReset.ECheckResetType.resetAniWait:
                    {
                        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_Reset);
                        return ENate.StageRunningStatus.ResetAniOverCheckWin;
                    }
                default:
                    {
                        return ENate.StageRunningStatus.Failed;
                    }
            }
        }
    }
}