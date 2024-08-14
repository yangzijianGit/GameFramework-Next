using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRunStatue_MoveToNextChessBoard : ENate.StageRunStaue
{
    int m_nMoveAniId = -1;
    public void prefix(ENate.Stage tStage)
    {
        tStage.bIsLock = true;
        var tCurrent = tStage.CurrentChessBoard;
        var tNextChessBoard = tStage.getChessBoardWithIndex(tStage.CurrentChessBoardIndex + 1);
        var vDifPos = tNextChessBoard.GetComponent<RectTransform>().anchoredPosition3D - tCurrent.GetComponent<RectTransform>().anchoredPosition3D;

        float fSpeed = 15.0f;
        var tMoveRectTransform = tStage.m_tChessBaordAttachNode.GetComponent<RectTransform>();
        var vBeginPosition = tMoveRectTransform.anchoredPosition3D;
        var vEndPosition = vBeginPosition - vDifPos;
        var fAddX = vEndPosition.x > vBeginPosition.x ? fSpeed : -fSpeed;
        var fAddY = vEndPosition.y > vBeginPosition.y ? fSpeed : -fSpeed;

        m_nMoveAniId = tStage.createTask(() =>
        {
            bool bIsXOver = false;
            bool bIsYOver = false;
            var vCurrentPosition = tMoveRectTransform.anchoredPosition3D;
            vCurrentPosition.x += fAddX;
            vCurrentPosition.y += fAddY;
            if (fAddX > 0)
            {
                if (vCurrentPosition.x > vEndPosition.x)
                {
                    bIsXOver = true;
                    vCurrentPosition.x = vEndPosition.x;
                }
            }
            else
            {
                if (vCurrentPosition.x < vEndPosition.x)
                {
                    bIsXOver = true;
                    vCurrentPosition.x = vEndPosition.x;
                }
            }
            if (fAddY > 0)
            {
                if (vCurrentPosition.y > vEndPosition.y)
                {
                    bIsYOver = true;
                    vCurrentPosition.y = vEndPosition.y;
                }
            }
            else
            {
                if (vCurrentPosition.y < vEndPosition.y)
                {
                    bIsYOver = true;
                    vCurrentPosition.y = vEndPosition.y;
                }
            }
            tMoveRectTransform.anchoredPosition3D = vCurrentPosition;
            return bIsXOver && bIsYOver;
        }, delegate { m_bIsOver = true; });
    }
    bool m_bIsOver = false;
    public void run(ENate.Stage tStage)
    {
        m_bIsOver = false;
        if (m_nMoveAniId == -1)
        {
            return;
        }
        if (tStage.isTaskOver(m_nMoveAniId) == true)
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
        tStage.changeChessBoard(tStage.CurrentChessBoardIndex + 1);
        return ENate.StageRunningStatus.DetectingReset;
    }
}