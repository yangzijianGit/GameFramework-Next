/*
        author      :       yangzijian
        time        :       2019-12-23 14:13:28
        function    :       drop unit
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{

    public class DropUnit
    {
        public enum MoveType
        {
            normal,
            dropGenerate,
            exchange,
            reset,
            openPortal
        }

        // 掉落元素组， 一组一组掉落
        public int m_nID;
        public List<Element> m_arrElement;
        public int m_nLine;
        public int m_nCol;
        public int m_nAimLine;
        public int m_nAimCol;
        DropManager m_tDropManager;
        /**
         * @Author: yangzijian
         * @description:  for move limit 
         */
        public bool m_bIsMoveLimit;
        MoveType m_eMoveType;
        public MoveType EMoveType
        {
            get
            {
                return m_eMoveType;
            }
            set
            {
                if (value == m_eMoveType)
                {
                    return;
                }
                m_eMoveType = value;
                playStartAni();
            }
        }
        public MoveUnit m_tMoveUnit;

        string getAniName(bool bIsStart)
        {
            Direction eDirection = Util.getDirectionWithLineCol(m_nLine, m_nCol, m_nAimLine, m_nAimCol);
            switch (m_eMoveType)
            {
                case MoveType.dropGenerate:
                    {
                        switch (eDirection)
                        {
                            case Direction.Up:
                                {
                                    return "buildU";
                                }
                            case Direction.Down:
                                {
                                    return "buildD";
                                }
                            case Direction.Left:
                                {
                                    return "buildL";
                                }
                            case Direction.Right:
                                {
                                    return "buildR";
                                }
                            default:
                                {
                                    return "buildD";
                                }
                        }
                    }
                case MoveType.normal:
                    {
                        switch (eDirection)
                        {
                            case Direction.Up:
                                {
                                    if (bIsStart == true)
                                        return "dropU_s";
                                    else
                                        return "dropU_e";
                                }
                            case Direction.Down:
                                {
                                    if (bIsStart == true)
                                        return "dropD_s";
                                    else
                                        return "dropD_e";
                                }
                            case Direction.Left:
                                {
                                    if (bIsStart == true)
                                        return "dropL_s";
                                    else
                                        return "dropL_e";
                                }
                            case Direction.Right:
                                {
                                    if (bIsStart == true)
                                        return "dropR_s";
                                    else
                                        return "dropR_e";
                                }
                            default:
                                {
                                    if (bIsStart == true)
                                        return "dropD_s";
                                    else
                                        return "dropD_e";
                                }
                        }
                    }
            }
            return "";
        }
        public void playStartAni()
        {
            string strAni = getAniName(true);
            if (string.IsNullOrEmpty(strAni) == true)
            {
                return;
            }
            foreach (var tElement in m_arrElement)
            {
                tElement.playAniWithBehaviorId(strAni);
            }
        }
        public void playEndAni()
        {
            string strAni = getAniName(false);
            if (string.IsNullOrEmpty(strAni) == true)
            {
                return;
            }
            foreach (var tElement in m_arrElement)
            {
                tElement.playAniWithBehaviorId(strAni);
            }
        }

        public DropUnit(DropManager tDropManager, int nId, int nLine, int nCol, int nAimLine, int nAimCol, List<Element> arrElement, MoveType eMoveType = MoveType.normal)
        {
            m_tDropManager = tDropManager;
            m_nLine = nLine;
            m_nCol = nCol;
            m_nAimLine = nAimLine;
            m_nAimCol = nAimCol;
            m_arrElement = arrElement;
            m_nID = nId;
            EMoveType = eMoveType;
            List<RectTransform> arrRectTransform = new List<RectTransform>();

            foreach (var tElement in arrElement)
            {
                arrRectTransform.Add(tElement.gameObject.GetComponent<RectTransform>());
            }
            m_tMoveUnit = MoveUnitUtil.createMoveUnit(nId, eMoveType, arrRectTransform, nLine, nCol, nAimLine, nAimCol);
        }

        public void changeAim(int nLine, int nCol, int nAimLine, int nAimCol)
        {
            m_nLine = nLine;
            m_nCol = nCol;
            m_nAimLine = nAimLine;
            m_nAimCol = nAimCol;
            EMoveType = MoveType.normal;
            m_tMoveUnit = m_tMoveUnit.changeAim(MoveUnitUtil.getPositionWithLineCol(nAimLine, nAimCol), true);
        }
        public bool move(float fDeltalTime)
        {
            // update element position and move 
            m_tMoveUnit.move(fDeltalTime);
            return m_tMoveUnit.isOver();
        }

    }
}