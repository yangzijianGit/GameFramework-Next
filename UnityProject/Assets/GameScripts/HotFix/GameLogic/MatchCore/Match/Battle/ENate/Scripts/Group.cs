/*
 * @Description: Group
 * @Author: yangzijian
 * @Date: 2020-04-28 11:54:07
 * @LastEditors: yangzijian
 * @LastEditTime: 2024-08-15 01:43:27
 */

using System;
using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace ENate
{
    public class GroupInfo
    {
        public string m_strGroupId;
        public string m_strElementId;
        public ElementDestroy m_tDestroyType = new ElementDestroy();
        public List<GridCoord> m_arrGridLineCol = new List<GridCoord>();

        public GroupInfo(string strGroupId)
        {
            m_strGroupId = strGroupId;
        }

        public void setElementDestroy(string strElementId, int eDestroyType)
        {
            m_strElementId = strElementId;
            m_tDestroyType.addDestroyType(eDestroyType);
        }

        public void addLineCol(GridCoord tGridCoord)
        {
            m_arrGridLineCol.Add(tGridCoord);
        }

    }
    public class Group : ENateDispose<Group>
    {
        Stage m_tStage;

        jc.EventManager.EventObj m_tEventObj;
        public Group(Stage tStage)
        {
            m_tStage = tStage;
            addDisposeCallback(clear);
            bindEvent();
        }

        void bindEvent()
        {
            clear();
            m_tEventObj = new jc.EventManager.EventObj();
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_DROP_DROPOVERCHECK, event_check);
        }

        public void clear()
        {
            if (m_tEventObj != null)
            {
                m_tEventObj.clear();
                m_tEventObj = null;
            }
        }
        Dictionary<int, Dictionary<string, GroupInfo>> m_arrGroup = new Dictionary<int, Dictionary<string, GroupInfo>>(); // chessBoardIndex : GroupInfo

        public void addGroupLineCol(string strGroupId, string strElementId, string strDestroyType, GridCoord tGridCoord)
        {
            if (m_arrGroup.ContainsKey(tGridCoord.ChessBoardIndex) == false)
            {
                m_arrGroup.Add(tGridCoord.ChessBoardIndex, new Dictionary<string, GroupInfo>());
            }
            var mpGroupInfo = m_arrGroup[tGridCoord.ChessBoardIndex];
            if (mpGroupInfo.ContainsKey(strGroupId) == false)
            {
                mpGroupInfo.Add(strGroupId, new GroupInfo(strGroupId));
                int eDestroyType = int.Parse(strDestroyType);
                mpGroupInfo[strGroupId].setElementDestroy(strElementId, eDestroyType);
            }
            mpGroupInfo[strGroupId].addLineCol(tGridCoord);
        }

        Dictionary<string, GroupInfo> getGroupMap(int nChessBoardIndex)
        {
            if (m_arrGroup.ContainsKey(nChessBoardIndex) == true)
            {
                return m_arrGroup[nChessBoardIndex];
            }
            return null;
        }

        public void check(int nChessBoardIndex)
        {
            var mpGroup = getGroupMap(nChessBoardIndex);
            var tChessBoard = m_tStage.getChessBoardWithIndex(nChessBoardIndex);
            if (mpGroup == null || tChessBoard == null)
            {
                return;
            }
            var arrElement = new ElementContainer();
            foreach (var itGroup in mpGroup)
            {
                var tGroup = itGroup.Value;
                bool bIsOk = true;
                var tElementCheckValue = new ElementValue<string>(tGroup.m_strElementId);
                foreach (var tGridCoord in tGroup.m_arrGridLineCol)
                {
                    var tGrid = tChessBoard.getGrid(tGridCoord);
                    if (tGrid == null)
                    {
                        bIsOk = false;
                        break;
                    }
                    if (tGrid.detectElementStateNormal() == false)
                    {
                        bIsOk = false;
                        break;
                    }
                    if (tGrid.getElementWithElementAttribute(ElementAttribute.Attribute.id, tElementCheckValue) == null)
                    {
                        bIsOk = false;
                        break;
                    }
                }
                if (bIsOk == false)
                {
                    continue;
                }
                foreach (var tGridCoord in tGroup.m_arrGridLineCol)
                {
                    var tGrid = tChessBoard.getGrid(tGridCoord);
                    EliminateRules.eliminateGrid(tGrid, tGroup.m_tDestroyType, arrElement);
                }
            }
            ElementCreater.DestroyInfo tDestroyInfo = m_tStage.m_tElementCreater.createDestroyInfo(arrElement);
            tDestroyInfo.destroyElement();
        }
        public void checkCurrentChessBoard()
        {
            check(m_tStage.CurrentChessBoardIndex);
        }

        void event_check(object o)
        {
            checkCurrentChessBoard();
        }

    }
}