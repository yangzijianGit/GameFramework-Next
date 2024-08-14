using System;
using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace ENate
{
    public class StageTips
    {
        public List<Element> m_arrTipsElement = new List<Element>();
        public Element m_tSpecialElement;
        public Direction m_eMoveDirection = Direction.NULL;

        public enum ETips
        {
            normal,
            super
        }
        public ETips m_eETips;

        bool m_bIsShow = false;

        static public StageTips create(Element tElement)
        {
            StageTips tStageTips = new StageTips();
            tStageTips.m_arrTipsElement.Add(tElement);
            tStageTips.m_eETips = ETips.super;
            tStageTips.m_tSpecialElement = tElement;
            return tStageTips;
        }
        static public StageTips create(Element tElement, Direction eMoveDirection, List<Element> arrTipsElement)
        {
            StageTips tStageTips = new StageTips();
            tStageTips.m_arrTipsElement.AddRange(arrTipsElement);
            tStageTips.m_eETips = ETips.normal;
            tStageTips.m_tSpecialElement = tElement;
            tStageTips.m_arrTipsElement.Remove(tElement);
            tStageTips.m_eMoveDirection = eMoveDirection;
            return tStageTips;
        }

        public bool checkIsTips()
        {
            return m_bIsShow;
        }

        public void show()
        {
            m_bIsShow = true;
            if (m_tSpecialElement != null)
            {
                m_tSpecialElement.playAniWithBehaviorId("hint");
            }
            foreach (var tTElement in m_arrTipsElement)
            {
                if (tTElement == null)
                {
                    continue;
                }
                tTElement.playAniWithBehaviorId("hint");
            }
        }

        public void clear()
        {
            m_bIsShow = false;
            if (m_tSpecialElement != null)
            {
                m_tSpecialElement.playAniWithBehaviorId("normal");
            }
            foreach (var tTElement in m_arrTipsElement)
            {
                if (tTElement == null)
                {
                    continue;
                }
                tTElement.playAniWithBehaviorId("normal");
            }
        }
    }
}