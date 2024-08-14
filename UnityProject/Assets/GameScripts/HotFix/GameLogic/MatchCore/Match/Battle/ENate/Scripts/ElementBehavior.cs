/*
 * @Description: element behaviour 
 * @Author: yangzijian
 * @Date: 2020-01-02 10:15:59
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-22 10:51:04
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    public static class ElementBehavior
    {
        public static JsonData.ElementBehavior_Config.Element getConfig_element(string strElementId)
        {
            foreach (var tElement in JsonManager.elementbehavior_config.root.game.element)
            {
                if (tElement.id == strElementId)
                {
                    return tElement;
                }
            }
            return null;
        }

        public static JsonData.ElementBehavior_Config.Show getElementShow(string strElementId, string strBehaviorId)
        {
            JsonData.ElementBehavior_Config.Element tConfigElement = getConfig_element(strElementId);
            if (tConfigElement == null)
            {
                return null;
            }
            foreach (var tBehavior in tConfigElement.behavior)
            {
                if (tBehavior.id == strBehaviorId)
                {
                    return tBehavior.show;
                }
            }
            return null;
        }

        public static string getElementShowAni(string strElementId, string strBehaviorId)
        {
            var tShowConfig = getElementShow(strElementId, strBehaviorId);
            if (tShowConfig == null || tShowConfig.ani == null || tShowConfig.ani.Count <= 0)
            {
                return "";
            }
            var lRandomIndex = Stage.m_tENateRandom.random(0, tShowConfig.ani.Count);
            string strAniName = tShowConfig.ani[(int) lRandomIndex];
            return strAniName;
        }

        public static string showTime(this Element tElement, string strBehaviorId)
        {
            if (tElement == null)
            {
                return "";
            }
            return getElementShowAni(tElement.ElementId, strBehaviorId);
        }

        public static string getAniArgValue(string strKey)
        {
            foreach (var tAniTag in JsonManager.elementbehavior_config.root.game.aniArg.tag)
            {
                if (tAniTag.id == strKey)
                {
                    return tAniTag.value;
                }
            }
            return "";
        }
    }
}