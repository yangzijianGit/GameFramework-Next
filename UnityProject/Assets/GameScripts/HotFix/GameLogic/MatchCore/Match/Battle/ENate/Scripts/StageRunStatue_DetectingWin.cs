using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    public class StageRunStatue_DetectingWin : ENate.StageRunStaue
    {
        public enum WinType
        {
            win,
            CurrentChessBoardWin,
            failed,
            none
        }
        public WinType checkWin(Stage tStage)
        {
            UnityEngine.Profiling.Profiler.BeginSample("checkWin");
            if (tStage.m_tWinRules.check(-1) == true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return WinType.win;
            }
            if (tStage.m_tWinRules.check(tStage.CurrentChessBoardIndex) == true)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return WinType.CurrentChessBoardWin;
            }
            if (tStage.m_tStageData.m_nStep <= 0)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return WinType.failed;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return WinType.none;
        }

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
            WinType eWinType = checkWin(tStage);
            switch (eWinType)
            {
                case WinType.win:
                    {
                        return StageRunningStatus.Win;
                    }
                case WinType.CurrentChessBoardWin:
                    {
                        var tNextChessBoard = tStage.getChessBoardWithIndex(tStage.CurrentChessBoardIndex + 1);
                        if (tNextChessBoard == null)
                        {
                            return StageRunningStatus.Win;
                        }
                        else
                        {
                            return StageRunningStatus.MoveToNextChessBoard;

                        }
                    }
                case WinType.failed:
                    {
                        return StageRunningStatus.Failed;
                    }
                case WinType.none:
                    {
                        return StageRunningStatus.DetectingFever;
                    }
                default:
                    {
                        return StageRunningStatus.DetectingFever;
                    }
            }
        }
    }
}