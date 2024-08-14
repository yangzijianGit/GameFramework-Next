/*
 * @Description: stage read 
 * @Author: yangzijian
 * @Date: 2019-12-31 12:05:42
 * @LastEditors: yangzijian
 * @LastEditTime: 2024-08-15 02:21:40
 */
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{

    public static class StageRead
    {
        static StageConfig m_tCacheStageConfig; // 缓存 stageConfig 
        static string m_strStageId;
        public static void readStageConfig(this Stage tStage, StageConfig tStageConfig)
        {
            tStage.m_tStageData.m_nStep = tStageConfig.m_nStep;
            tStage.m_arrChessBoard = new List<ChessBoard>();
            foreach (BoardParam tBoardParam in tStageConfig.m_listBoard)
            {
                ChessBoard tChessBoard = ChessBoard.create(tStage, tBoardParam.nId, tBoardParam.m_bIsConnectLastChessBoard, tBoardParam.m_eConnectDirection);
                tChessBoard.setSize(tBoardParam.nCol, tBoardParam.nRow);
                tStage.m_arrChessBoard.Add(tChessBoard);
                List<KeyValuePair<Grid, string>> arrNormalElement = new List<KeyValuePair<Grid, string>>();
                List<KeyValuePair<Grid, string>> arrBrandomElement = new List<KeyValuePair<Grid, string>>();
                foreach (var tCellGrid in tBoardParam.m_listCellGrid)
                {
                    int nLine = tCellGrid.nPosY - 1;
                    int nColumn = tCellGrid.nPosX - 1;
                    Grid tGrid = tChessBoard.createGrid(nLine, nColumn);
                    //jc.LogManager.Error("intDirection = "+ tCellGrid.intDirection);
                    tGrid.DropDirection = (Direction) tCellGrid.intDirection;
                    foreach (var tCellElement in tCellGrid.m_listElementId)
                    {
                        if (tCellElement == "cBrandom")
                        {
                            arrBrandomElement.Add(new KeyValuePair<Grid, string>(tGrid, tCellElement));
                        }
                        else
                        {
                            arrNormalElement.Add(new KeyValuePair<Grid, string>(tGrid, tCellElement));
                        }
                    }
                    if (tCellGrid.strDropId != string.Empty)
                    {
                        tChessBoard.createDropDevice(tCellGrid.strDropId, nLine, nColumn);
                    }
                }
                foreach (var tCreateNoraml in arrNormalElement)
                {
                    tStage.m_tElementCreater.create(tCreateNoraml.Value, tCreateNoraml.Key);
                }
                foreach (var tCreateBrandom in arrBrandomElement)
                {
                    tStage.m_tElementCreater.create(tCreateBrandom.Value, tCreateBrandom.Key);
                }
                tChessBoard.m_tDropManager.initOpenPortal();
            }
            bool bIsMust = false;
            if (tStageConfig.m_listBoard.Count == 1)
            {
                bIsMust = true;
            }
            foreach (var tConfigWinRules in tStageConfig.m_listElement)
            {
                int nChessBoardIndex = tConfigWinRules.nBoard - 1;
                if (bIsMust)
                {
                    nChessBoardIndex = -1;
                }
                string strHypotaxisId = Config.ElementConfig.getConfig_element_hypotaxisId(tConfigWinRules.strName);
                if (tConfigWinRules.nType == 1)
                {
                    tStage.m_tWinRules.addRule(nChessBoardIndex, new EliminateRule(strHypotaxisId, tConfigWinRules.nCount));
                }
                else if (tConfigWinRules.nType == 3)
                {
                    tStage.m_tWinRules.addRule(nChessBoardIndex, new NoOnetRule(strHypotaxisId, 0));
                }
            }

            //read group info 
            Group tGroup = tStage.m_tGroup;
            // foreach (var tConfigGroup in tStageConfig.m_arrGroupParam)
            // {
            //     foreach (var tLineCol in tConfigGroup.m_arrGridLineCol)
            //     {
            //         tGroup.addGroupLineCol(tConfigGroup.m_strGroupId, tConfigGroup.m_strElementId, tConfigGroup.m_strDestroyType,
            //             new GridCoord(tLineCol.m_nChessBoardIndex - 1, tLineCol.m_nCol - 1, tLineCol.m_nLine - 1));
            //     }
            // }

            tStage.changeChessBoard(0);
        }

        public static StageConfig readStageConfig(string strStageId)
        {
            if (m_strStageId == strStageId)
            {
                return m_tCacheStageConfig;
            }
            StageConfig tStageConfig = jc.ResourceManager.Instance.LoadStage(strStageId);
            m_tCacheStageConfig = tStageConfig;
            m_strStageId = strStageId;
            return tStageConfig;

        }
    }

    public static class DropDeviceRead
    {
        public static JsonData.DropConfig.DropNode getDropNode(string strDropDeviceId)
        {
            foreach (var tDropNode in JsonManager.dropconfig.root.game.DropNode)
            {
                if (tDropNode.id == strDropDeviceId)
                {
                    return tDropNode;
                }
            }
            return null;
        }

        public static void createDropDevice(this ChessBoard tChessBoard, string strDropDeviceId, int nLine, int nCol)
        {
            var tDropNode = getDropNode(strDropDeviceId);
            if (tDropNode == null)
            {
                return;
            }
            DropDevice tDropDevice = new DropDevice(tChessBoard.m_tStage, tChessBoard.m_tDropManager, Stage.m_tComputeRandom, nLine, nCol);
            foreach (var tDropStrategy in tDropNode.m_pDropStrategy)
            {
                tDropDevice.addDeviceUnit(new DropDeviceUnit(tDropStrategy.m_strElementId, int.Parse(tDropStrategy.m_nPower)));
            }
            tChessBoard.createDropDevice(nLine, nCol, tDropDevice, tDropNode);
        }
    }

}