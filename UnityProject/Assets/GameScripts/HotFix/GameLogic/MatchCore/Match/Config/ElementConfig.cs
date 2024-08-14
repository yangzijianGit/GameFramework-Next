using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ENate;
using UnityEngine;

namespace Config
{

    public static class ElementConfig
    {

        public static JsonData.Element_Config.Element getConfig_element(string strElementId)
        {
            foreach (var tElement in JsonManager.element_config.root.game.element)
            {
                if (tElement.id == strElementId)
                {
                    return tElement;
                }
            }
            return null;
        }

        public static string getConfig_element_hypotaxisId(string strElementId)
        {
            var tConfig = getConfig_element(strElementId);
            string strHypotaxis = strElementId;
            if (tConfig != null && string.IsNullOrEmpty(tConfig.hypotaxis) == false)
            {
                strHypotaxis = tConfig.hypotaxis;
            }
            return strHypotaxis;
        }
        public static ElementValue<int> tryParseElementIntValue(Dictionary<ElementAttribute.Attribute, ElementAttribute> arrElementAttribute, ElementAttribute.Attribute eAttribute, string strValue)
        {
            try
            {
                arrElementAttribute.Add(eAttribute, new ElementValue<int>(int.Parse(strValue)));
            }
            catch (System.Exception) { }
            return null;
        }
        public static ElementValue<string> tryParseElementStringValue(Dictionary<ElementAttribute.Attribute, ElementAttribute> arrElementAttribute, ElementAttribute.Attribute eAttribute, string strValue)
        {
            try
            {
                arrElementAttribute.Add(eAttribute, new ElementValue<string>(strValue));
            }
            catch (System.Exception) { }
            return null;
        }

        public static int? getElementLevel(string strElementId)
        {
            JsonData.Element_Config.Element tConfigElement = getConfig_element(strElementId);
            try
            {
                return int.Parse(tConfigElement.level);
            }
            catch (System.Exception) { }
            return null;
        }

        static Dictionary<string, Dictionary<ElementAttribute.Attribute, ElementAttribute>> m_mpElementAttribute;

        static void parseElementConfig(string strElementId)
        {
            JsonData.Element_Config.Element tConfigElement = getConfig_element(strElementId);
            if (tConfigElement == null)
            {
                return;
            }
            Dictionary<ElementAttribute.Attribute, ElementAttribute> arrElementAttribute = new Dictionary<ElementAttribute.Attribute, ElementAttribute>();
            tryParseElementStringValue(arrElementAttribute, ElementAttribute.Attribute.id, tConfigElement.id);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.type, tConfigElement.type);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.level, tConfigElement.level);
            tryParseElementStringValue(arrElementAttribute, ElementAttribute.Attribute.name, tConfigElement.name);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.color, tConfigElement.color);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.canMove, tConfigElement.canMove);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.moveType, tConfigElement.moveType);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.m_bIsCellOccupy, tConfigElement.m_bIsCellOccupy);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.bIsRandomElementId, tConfigElement.m_bIsRandomElement);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.involvedCompose, tConfigElement.involvedCompose);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.nWidth, tConfigElement.nWidth);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.nHeight, tConfigElement.nHeight);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.fixType, tConfigElement.fixType);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.eliminateTransmit, tConfigElement.eliminateTransmit);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.dirEliminateTransmit, tConfigElement.dirEliminateTransmit);
            tryParseElementStringValue(arrElementAttribute, ElementAttribute.Attribute.inBasketType, tConfigElement.inBasketType);
            tryParseElementStringValue(arrElementAttribute, ElementAttribute.Attribute.destroyTo, tConfigElement.destroyTo);
            tryParseElementStringValue(arrElementAttribute, ElementAttribute.Attribute.hypotaxis, tConfigElement.hypotaxis);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.followBasic, tConfigElement.followBasic);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.forbiddenMove, tConfigElement.forbiddenMove);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.forbiddenCompose, tConfigElement.forbiddenCompose);
            tryParseElementIntValue(arrElementAttribute, ElementAttribute.Attribute.eliminateTime, tConfigElement.eliminateTime);
            tryParseElementStringValue(arrElementAttribute, ElementAttribute.Attribute.exitid, tConfigElement.exitid);
            tryParseElementStringValue(arrElementAttribute, ElementAttribute.Attribute.reputationId, tConfigElement.reputationId);
            {
                PassCreate tPassCreateDestroy = new PassCreate();
                try
                {
                    tPassCreateDestroy.m_tElementDestroy = new ElementDestroy();
                    tPassCreateDestroy.m_tElementDestroy.addDestroyType(int.Parse(tConfigElement.passCreateDestroy.CdestroyType));
                    tPassCreateDestroy.m_strConditionId = tConfigElement.passCreateDestroy.condition;
                }
                catch (System.Exception)
                {
                    tPassCreateDestroy = null;
                }
                arrElementAttribute.Add(ElementAttribute.Attribute.passCreateDestroy, tPassCreateDestroy);
            }
            {
                ElementDestroy tElementDestroy = new ElementDestroy();
                foreach (var strDestoryedType in tConfigElement.destroyedType)
                {
                    try
                    {
                        tElementDestroy.addDestroyType(int.Parse(strDestoryedType));
                    }
                    catch (System.Exception) { }
                }
                arrElementAttribute.Add(ElementAttribute.Attribute.destroyType, tElementDestroy);
            }
            {
                ElementEliminateCreate tElementEliminateCreate = new ElementEliminateCreate();
                try
                {
                    int eDestroyType = int.Parse(tConfigElement.eliminateCreateDestroy.mine.CdestroyType);
                    tElementEliminateCreate.m_nEliminateMine = new ElementDestroy();
                    tElementEliminateCreate.m_nEliminateMine.addDestroyType(eDestroyType);
                }
                catch (System.Exception) { }
                try
                {
                    int eDestroyType = int.Parse(tConfigElement.eliminateCreateDestroy.left.CdestroyType);
                    tElementEliminateCreate.m_nEliminateLeft = new ElementDestroy();
                    tElementEliminateCreate.m_nEliminateLeft.addDestroyType(eDestroyType);
                }
                catch (System.Exception) { }
                try
                {
                    int eDestroyType = int.Parse(tConfigElement.eliminateCreateDestroy.right.CdestroyType);
                    tElementEliminateCreate.m_nEliminateRight = new ElementDestroy();
                    tElementEliminateCreate.m_nEliminateRight.addDestroyType(eDestroyType);
                }
                catch (System.Exception) { }
                try
                {
                    int eDestroyType = int.Parse(tConfigElement.eliminateCreateDestroy.up.CdestroyType);
                    tElementEliminateCreate.m_nEliminateUp = new ElementDestroy();
                    tElementEliminateCreate.m_nEliminateUp.addDestroyType(eDestroyType);
                }
                catch (System.Exception) { }
                try
                {
                    int eDestroyType = int.Parse(tConfigElement.eliminateCreateDestroy.down.CdestroyType);
                    tElementEliminateCreate.m_nEliminateDown = new ElementDestroy();
                    tElementEliminateCreate.m_nEliminateDown.addDestroyType(eDestroyType);
                }
                catch (System.Exception) { }
                arrElementAttribute.Add(ElementAttribute.Attribute.eliminateCreateDestroy, tElementEliminateCreate);
            }
            {
                ElementDestroy tElementDestroy = new ElementDestroy();
                foreach (var strDestoryedType in tConfigElement.stopOtherDestroyType)
                {
                    try
                    {
                        tElementDestroy.addDestroyType(int.Parse(strDestoryedType));
                    }
                    catch (System.Exception) { }
                }
                arrElementAttribute.Add(ElementAttribute.Attribute.stopOtherDestroyType, tElementDestroy);
            }
            {
                ElementDestroy tElementDestroy = new ElementDestroy();
                foreach (var strDestoryedType in tConfigElement.stopMineDestroyType)
                {
                    try
                    {
                        tElementDestroy.addDestroyType(int.Parse(strDestoryedType));
                    }
                    catch (System.Exception) { }
                }
                arrElementAttribute.Add(ElementAttribute.Attribute.stopMineDestroyType, tElementDestroy);
            }
            m_mpElementAttribute.Add(strElementId, arrElementAttribute);
        }
        static void parseElementConfig()
        {
            m_mpElementAttribute = new Dictionary<string, Dictionary<ElementAttribute.Attribute, ElementAttribute>>();
            foreach (var tElement in JsonManager.element_config.root.game.element)
            {
                parseElementConfig(tElement.id);
            }
            return;
        }
        public static void initConfig()
        {
            parseElementConfig();
        }
        public static Dictionary<ElementAttribute.Attribute, ElementAttribute> getElementAttribute(ref string strElementId)
        {
            UnityEngine.Profiling.Profiler.BeginSample("GetElementAttribute");
            if (m_mpElementAttribute.ContainsKey(strElementId) == true)
            {
                return m_mpElementAttribute[strElementId];
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return null;
        }
    }
}