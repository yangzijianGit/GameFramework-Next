/*
 * @Description: Sports, about the performance of sports.
 * @Author: yangzijian
 * @Date: 2020-01-02 16:40:16
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-27 15:18:09
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ENate
{
    public static class MoveUnitUtil
    {

        // 运动初想 
        /*
                the motion has an acceleration.
                the initial velocity is zero.
                calculate the current position based on time.
                maximum speed.
                Move to the target point and determine the move limit to continue looking for the next move point.
        */

        public static float m_fMaximumSpeed = 450.0f;
        public static float m_fAcceleratedSpeed = 8.0f;

        public static float m_fExchangeMaximumSpeed = 450.0f;
        public static float m_fExchangeAcceleratedSpeed = 8.0f;

        public static void initConfig()
        {
            m_fMaximumSpeed = float.Parse(JsonManager.eliminate_config.root.game.m_fMaximumSpeed);
            m_fAcceleratedSpeed = float.Parse(JsonManager.eliminate_config.root.game.m_fAcceleratedSpeed);
            //m_fExchangeMaximumSpeed = float.Parse(JsonManager.parameter.root.game.m_fExchangeMaximumSpeed);
            //m_fExchangeAcceleratedSpeed = float.Parse(JsonManager.parameter.root.game.m_fExchangeAcceleratedSpeed);
        }
        public static Vector2 getPositionWithLineCol(int nLine, int nCol)
        {
            Vector2 tVector = new Vector2(nCol * 78 + 38, nLine * -78 - 38);
            return tVector;
        }

        public static MoveUnit createMoveUnit(int nId, DropUnit.MoveType eMoveType, List<RectTransform> arrRectTransform, int nSrcLine, int nSrcCol, int nDestLine, int nDestCol)
        {
            switch (eMoveType)
            {
                case DropUnit.MoveType.normal:
                    {
                        Move_Normal tMove_Normal = new Move_Normal(nId, arrRectTransform);
                        tMove_Normal.init(getPositionWithLineCol(nSrcLine, nSrcCol), getPositionWithLineCol(nDestLine, nDestCol));
                        return tMove_Normal;
                    }
                case DropUnit.MoveType.dropGenerate:
                    {
                        MOve_DropGenerate tMOve_DropGenerate = new MOve_DropGenerate(nId, arrRectTransform);
                        tMOve_DropGenerate.init(getPositionWithLineCol(nSrcLine, nSrcCol), getPositionWithLineCol(nDestLine, nDestCol));
                        return tMOve_DropGenerate;
                    }
                case DropUnit.MoveType.exchange:
                    {
                        Move_Normal tMove_Normal = new Move_Normal(nId, arrRectTransform);
                        tMove_Normal.init(getPositionWithLineCol(nSrcLine, nSrcCol), getPositionWithLineCol(nDestLine, nDestCol));
                        return tMove_Normal;
                    }
                case DropUnit.MoveType.reset:
                    {
                        Move_Normal tMove_Normal = new Move_Normal(nId, arrRectTransform);
                        tMove_Normal.init(getPositionWithLineCol(nSrcLine, nSrcCol), getPositionWithLineCol(nDestLine, nDestCol));
                        return tMove_Normal;
                    }
                case DropUnit.MoveType.openPortal:
                    {
                        Move_Portal tMove_Portal = new Move_Portal(nId, arrRectTransform);
                        tMove_Portal.init(getPositionWithLineCol(nSrcLine, nSrcCol), getPositionWithLineCol(nDestLine, nDestCol));
                        return tMove_Portal;
                    }
            }
            return null;
        }

        public static float getCurrentSpeed(float fCurrentSpeed, float fDeltalTime, float fMaximumSpeed, float fAcceleratedSpeed)
        {
            fCurrentSpeed += fDeltalTime * fAcceleratedSpeed;
            return fCurrentSpeed > fMaximumSpeed ? fMaximumSpeed : fCurrentSpeed;
        }

        public static Vector2 getCurrentPos(Vector2 vDirect, ref float fCurrentSpeed, float fDeltalTime, float fMaximumSpeed, float fAcceleratedSpeed)
        {
            fCurrentSpeed = getCurrentSpeed(fCurrentSpeed, fDeltalTime, fMaximumSpeed, fAcceleratedSpeed);
            return vDirect * fCurrentSpeed * fDeltalTime;
        }
    }
    public interface MoveUnit
    {
        void move(float fDeltalTime);
        bool isOver();

        void init(Vector2 vBegin, Vector2 vEnd); 

        MoveUnit changeAim(Vector2 vEnd, bool bIsFixPath = true);

    }

    public class Move_Normal : MoveUnit
    {
        protected List<RectTransform> m_arrRectTransform;
        protected float m_fSpeed;
        protected Vector2 m_vDirect;
        protected Vector2 m_vEnd;
        protected int m_nId;
        protected Vector2 m_tCurrentPos;

        public Move_Normal(int nId, List<RectTransform> arrRectTransform)
        {
            m_arrRectTransform = arrRectTransform;
            m_fSpeed = 0.0f;
            m_nId = nId;
            m_tCurrentPos = Vector2.zero;
        }

        protected void setPosition(Vector2 vPosition, bool bIsSet = true)
        {
            //consoconsole.log("setPosition:", vPosition.x, vPosition.y);
            foreach (var tRectTransform in m_arrRectTransform)
            {
                tRectTransform.anchoredPosition = vPosition;
            }
            if (bIsSet)
            {
                m_tCurrentPos = vPosition;
            }
        }

        protected Vector2 getPosition()
        {
            return m_tCurrentPos;
        }

        public void move(float fDeltalTime)
        {
            Vector2 tCurrentPos = MoveUnitUtil.getCurrentPos(m_vDirect, ref m_fSpeed, fDeltalTime, MoveUnitUtil.m_fMaximumSpeed, MoveUnitUtil.m_fAcceleratedSpeed) + getPosition();
            setPosition(tCurrentPos);
            if (isOver())
            {
                setPosition(m_vEnd, false);
            }
        }

        public void init(Vector2 vBegin, Vector2 vEnd)
        {
            setPosition(vBegin);
            m_vDirect = (vEnd - vBegin).normalized;
            m_vEnd = vEnd;
        }

        public bool isOver()
        {
            return Vector2.Dot(m_vDirect, getPosition() - m_vEnd) >= 0;
        }

        public MoveUnit changeAim(Vector2 vEnd, bool bIsFixPath = true)
        {
            if (bIsFixPath)
            {
                Vector2 tCurrent = getPosition();
                float fOffect = (tCurrent - m_vEnd).magnitude;
                m_vEnd += fOffect * (vEnd - m_vEnd).normalized;
            }
            init(m_vEnd, vEnd);
            return this;
        }

    }

    public class Move_Portal : MoveUnit
    {
        protected List<RectTransform> m_arrRectTransform;
        protected List<RectTransform> m_arrCopyRectTransform;
        protected float m_fSpeed;
        protected Vector2 m_vDirect;
        protected Vector2 m_vBegin;
        protected Vector2 m_vEnd;
        protected int m_nId;
        protected Vector2 m_tCurrentPos;

        protected List<Transform> m_arrParent;
        RectTransform tInRectMaskTransform;
        RectTransform tOutRectMaskTransform;
        public Move_Portal(int nId, List<RectTransform> arrRectTransform)
        {
            m_arrRectTransform = arrRectTransform;
            m_arrCopyRectTransform = new List<RectTransform>();
            m_arrParent = new List<Transform>();

            GameObject tInRectMask = new GameObject();
            tInRectMaskTransform = tInRectMask.AddComponent<RectTransform>();
            tInRectMask.AddComponent<RectMask2D>();
            tInRectMask.attachObj(m_arrRectTransform[0].parent.gameObject);
            tInRectMaskTransform.sizeDelta = m_arrRectTransform[0].sizeDelta;
            tInRectMaskTransform.pivot = m_arrRectTransform[0].pivot;
            tInRectMaskTransform.anchorMin = m_arrRectTransform[0].anchorMin;
            tInRectMaskTransform.anchorMax = m_arrRectTransform[0].anchorMax;
            GameObject tOutRectMask = new GameObject();
            tOutRectMaskTransform = tOutRectMask.AddComponent<RectTransform>();
            tOutRectMask.AddComponent<RectMask2D>();
            tOutRectMask.attachObj(m_arrRectTransform[0].parent.gameObject);
            tOutRectMaskTransform.sizeDelta = m_arrRectTransform[0].sizeDelta;
            tOutRectMaskTransform.pivot = m_arrRectTransform[0].pivot;
            tOutRectMaskTransform.anchorMin = m_arrRectTransform[0].anchorMin;
            tOutRectMaskTransform.anchorMax = m_arrRectTransform[0].anchorMax;

            foreach (var tRectTransform in m_arrRectTransform)
            {
                m_arrParent.Add(tRectTransform.parent);
                var tCopy = GameObject.Instantiate(tRectTransform.gameObject);
                tCopy.attachObj(tInRectMaskTransform.gameObject);
                tRectTransform.gameObject.attachObj(tOutRectMaskTransform.gameObject);
                var tCopyRectTransform = tCopy.GetComponent<RectTransform>();
                m_arrCopyRectTransform.Add(tCopyRectTransform);
            }

            m_fSpeed = 0.0f;
            m_nId = nId;
            m_tCurrentPos = Vector2.zero;
        }

        protected void setPosition(Vector2 vPosition, bool bIsSet = true)
        {
            var localEndPosition = vPosition - m_vEnd;
            var localPosition = localEndPosition + tOutRectMaskTransform.sizeDelta * m_vDirect;

            //consoconsole.log("setPosition:", vPosition.x, vPosition.y);
            foreach (var tRectTransform in m_arrRectTransform)
            {
                tRectTransform.anchoredPosition = localEndPosition;
            }
            foreach (var tRectTransform in m_arrCopyRectTransform)
            {
                tRectTransform.anchoredPosition = localPosition;
            }
            if (bIsSet)
            {
                m_tCurrentPos = vPosition;
            }
        }

        protected Vector2 getPosition()
        {
            return m_tCurrentPos;
        }

        public void move(float fDeltalTime)
        {
            Vector2 tCurrentPos = MoveUnitUtil.getCurrentPos(m_vDirect, ref m_fSpeed, fDeltalTime, MoveUnitUtil.m_fMaximumSpeed, MoveUnitUtil.m_fAcceleratedSpeed) + getPosition();
            setPosition(tCurrentPos);
            if (isOver())
            {
                for (int nIndex = 0; nIndex < m_arrRectTransform.Count; nIndex++)
                {
                    m_arrRectTransform[nIndex].gameObject.attachObj(m_arrParent[nIndex].gameObject);
                    m_arrRectTransform[nIndex].anchoredPosition = m_vEnd;
                }
                GameObject.Destroy(tInRectMaskTransform.gameObject);
                GameObject.Destroy(tOutRectMaskTransform.gameObject);
                // setPosition(m_vEnd, false);
            }
        }

        public void init(Vector2 vBegin, Vector2 vEnd)
        {
            m_vEnd = vEnd;
            m_vBegin = vBegin;
            m_vDirect = (new Vector2(0, -1)).normalized;
            tInRectMaskTransform.anchoredPosition = vBegin;
            tOutRectMaskTransform.anchoredPosition = vEnd;
            setPosition(m_vEnd - tOutRectMaskTransform.sizeDelta * m_vDirect);

        }

        public bool isOver()
        {
            return Vector2.Dot(m_vDirect, getPosition() - m_vEnd) >= 0;
        }

        public MoveUnit changeAim(Vector2 vEnd, bool bIsFixPath = true)
        {
            if (bIsFixPath)
            {
                Vector2 tCurrent = getPosition();
                float fOffect = (tCurrent - m_vEnd).magnitude;
                m_vEnd += fOffect * (vEnd - m_vEnd).normalized;
            }
            var tMoveNormal = new Move_Normal(m_nId, m_arrRectTransform);
            tMoveNormal.init(m_vEnd, vEnd);
            return tMoveNormal;
        }

    }

    public class Move_Exchange : Move_Normal
    {
        public Move_Exchange(int nId, List<RectTransform> arrRectTransform) : base(nId, arrRectTransform)
        {

        }
        public new void move(float fDeltalTime)
        {
            Vector2 tCurrentPos = MoveUnitUtil.getCurrentPos(m_vDirect, ref m_fSpeed, fDeltalTime, MoveUnitUtil.m_fExchangeMaximumSpeed, MoveUnitUtil.m_fExchangeAcceleratedSpeed) + getPosition();
            setPosition(tCurrentPos);
            if (isOver())
            {
                setPosition(m_vEnd, false);
            }
        }
    }
    public class MOve_DropGenerate : MoveUnit
    {

        List<RectTransform> m_arrRectTransform;
        float m_fSpeed;
        Vector2 m_vDirect;
        Vector2 m_vEnd;
        int m_nId;
        Vector2 m_tCurrentPos;
        public MOve_DropGenerate(int nId, List<RectTransform> arrRectTransform)
        {
            m_arrRectTransform = arrRectTransform;
            m_fSpeed = 0.0f;
            m_nId = nId;
            m_tCurrentPos = Vector2.zero;
        }

        void setPosition(Vector2 vPosition, bool bIsSet = true)
        {
            foreach (var tRectTransform in m_arrRectTransform)
            {
                tRectTransform.anchoredPosition = vPosition;
            }
            if (bIsSet)
            {
                m_tCurrentPos = vPosition;
            }
        }

        Vector2 getPosition()
        {
            return m_tCurrentPos;
        }

        public void move(float fDeltalTime)
        {
            Vector2 tCurrentPos = MoveUnitUtil.getCurrentPos(m_vDirect, ref m_fSpeed, fDeltalTime, MoveUnitUtil.m_fMaximumSpeed, MoveUnitUtil.m_fAcceleratedSpeed) + getPosition();
            setPosition(tCurrentPos);
            if (isOver())
            {
                setPosition(m_vEnd, false);
            }
        }

        public void init(Vector2 vBegin, Vector2 vEnd)
        {
            setPosition(vBegin);
            m_vDirect = (vEnd - vBegin).normalized;
            m_vEnd = vEnd;
        }

        public bool isOver()
        {
            return Vector2.Dot(m_vDirect, getPosition() - m_vEnd) >= 0;
        }
        public MoveUnit changeAim(Vector2 vEnd, bool bIsFixPath = true)
        {
            if (bIsFixPath)
            {
                Vector2 tCurrent = getPosition();
                float fOffect = (tCurrent - m_vEnd).magnitude;
                m_vEnd += fOffect * (vEnd - m_vEnd).normalized;
            }
            init(m_vEnd, vEnd);
            return this;
        }

    }

}