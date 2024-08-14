/*
 * @Description: ENateCollect
 * @Author: yangzijian
 * @Date: 2020-07-24 17:06:31
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-31 15:33:14
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    public class ENateCollect : MonoBehaviour
    {
        jc.EventManager.EventObj m_tEventObj;
        Stage m_tStage;
        public RectTransform m_tTargetAreaRectTransform;
        public RectTransform m_tTtargetAimRectTransform;
        float m_fAreaHeight;
        public List<ENateCollect_FlyObj> arrCreateObj = new List<ENateCollect_FlyObj>();

        public GameObject m_tFeverTarget;
        public GameObject m_tClothesSkillTarget;
        public ConditionConfig.MapArg m_tMapArg;

        public GameObject FlyObjParent;

        void addCreateObj(ENateCollect_FlyObj tENateCollect_FlyObj)
        {
            arrCreateObj.Add(tENateCollect_FlyObj);
        }

        public void removeObj(ENateCollect_FlyObj tENateCollect_FlyObj)
        {
            arrCreateObj.Remove(tENateCollect_FlyObj);
        }

        void clear()
        {
            foreach (var tObj in arrCreateObj)
            {
                if (tObj != null)
                    tObj.destroy(false);
            }
            arrCreateObj.Clear();
        }

        private void OnEnable()
        {
            m_tEventObj = new jc.EventManager.EventObj();
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Init, event_initStage);
            m_fAreaHeight = m_tTargetAreaRectTransform.rect.height;
        }

        private void OnDisable()
        {
            m_tEventObj.clear();
            m_tEventObj = null;
            clear();
        }

        private void OnDestroy()
        {
            clear();
        }

        Vector3 getTargetPosition(Element tElement)
        {
            m_tTtargetAimRectTransform.position = tElement.transform.position;
            m_tTtargetAimRectTransform.anchoredPosition3D = new Vector3(m_tTtargetAimRectTransform.anchoredPosition3D.x, -Stage.m_tENateRandom.random(0, 100) * m_fAreaHeight / 100f + 40, m_tTtargetAimRectTransform.anchoredPosition3D.z);
            return m_tTtargetAimRectTransform.position;
        }

        bool trigger(Element tElement, JsonData.ENateCollect_Config.Node tTriggerNode)
        {
            ENateCollect_FlyObj tCreateObj = null;
            if (bool.Parse(tTriggerNode.show.isCurrent))
            {
                tCreateObj = ENateCollect_FlyObj.create(tTriggerNode, tElement.gameObject, FlyObjParent, this);
            }
            else
            {
                tCreateObj = ENateCollect_FlyObj.create(tTriggerNode, tTriggerNode.show.prefab, FlyObjParent, this);
            }
            if (tCreateObj == null)
            {
                return false;
            }
            addCreateObj(tCreateObj);
            tCreateObj.transform.position = tElement.transform.position;
            string strAniId = "";
            if (tTriggerNode.ani.Count > 0)
            {
                int nIndex = (int) Stage.m_tENateRandom.random(0, tTriggerNode.ani.Count);
                strAniId = tTriggerNode.ani[nIndex];
            }
            Util.playENateAni(strAniId, tCreateObj.gameObject, tElement.transform.position, getTargetPosition(tElement), null, false);
            return true;
        }
        bool trigger(Element tElement)
        {
            ElementValue<int> type = tElement.getElementAttribute(ElementAttribute.Attribute.type) as ElementValue<int>;
            if (type.Value != int.Parse(JsonManager.enatecollect_config.root.game.elementType))
            {
                return false;
            }
            var tTrigger = Config.ENateCollectConfig.getTriggers();
            foreach (var tTriggerNode in tTrigger.node)
            {
                if (ConditionConfig.checkCondition(tTriggerNode.condition, m_tMapArg) == false)
                {
                    continue;
                }
                var lRandomValue = Stage.m_tENateRandom.random(0, 100);
                if (lRandomValue <= int.Parse(tTriggerNode.basePercent))
                {
                    trigger(tElement, tTriggerNode);
                    return true;
                }
            }
            return false;
        }

        //////////////////////////////////////////

        void event_initStage(object o)
        {
            m_tStage = o as Stage;
            ENate.BattleArg.Instance.setTriggerPowerFunc(trigger);
            m_tMapArg = new ConditionConfig.MapArg();
            m_tMapArg.Stage = m_tStage;
        }
        // // 尝试触发产生一个小球
        // void event_trigger(object o)
        // {
        //     trigger(o as Element);
        // }

    }
}