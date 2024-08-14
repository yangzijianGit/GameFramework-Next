/*
 * @Description: element
 * @Author: yangzijian
 * @Date: 2020-02-24 10:54:45
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-29 14:57:12
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{

    public class ElementAttribute
    {
        public enum Attribute
        {
            id,
            name,
            level,
            type,
            eliminateTransmit,
            dirEliminateTransmit,
            destroyType,
            color,
            eliminateCreateDestroy,
            stopMineDestroyType,
            stopOtherDestroyType,
            m_bIsCellOccupy,
            moveType,
            involvedCompose,
            forbiddenCompose,
            canMove,
            forbiddenMove,
            followBasic,
            fixType,
            destroyTo,
            hypotaxis,
            nWidth,
            nHeight,
            bIsRandomElementId,
            inBasketType,
            passCreateDestroy,
            exitid,
            reputationId,
            eliminateTime,
            isLock,
            changeCount,
            Count
        }
        public static void initDefaultValue()
        {
            m_tDefaultValue = new ElementAttribute[(int) ElementAttribute.Attribute.Count];
            m_tDefaultValue[(int) ElementAttribute.Attribute.level] = new ElementValue<int>(0);
            m_tDefaultValue[(int) ElementAttribute.Attribute.eliminateTransmit] = new ElementValue<int>(1);
            m_tDefaultValue[(int) ElementAttribute.Attribute.dirEliminateTransmit] = new ElementValue<int>(0);
            m_tDefaultValue[(int) ElementAttribute.Attribute.involvedCompose] = new ElementValue<int>(0);
            m_tDefaultValue[(int) ElementAttribute.Attribute.nWidth] = new ElementValue<int>(1);
            m_tDefaultValue[(int) ElementAttribute.Attribute.nHeight] = new ElementValue<int>(1);
            m_tDefaultValue[(int) ElementAttribute.Attribute.eliminateTime] = new ElementValue<int>(1);
            // 其他默认值为空
        }
        static private ElementAttribute[] m_tDefaultValue;
        // static private Dictionary<ElementAttribute.Attribute, ElementAttribute> m_tDefaultValue;
        static public ElementAttribute getDefaultValue(ElementAttribute.Attribute eAttribute)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getDefaultValue");
            ElementAttribute value = null;
            if (m_tDefaultValue != null)
            {
                value = m_tDefaultValue[(int) eAttribute];
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return value;
        }

    }

    public class ElementValue<T> : ElementAttribute
    {
        private T m_tValue;
        public T Value
        {
            get
            {
                return m_tValue;
            }
            set
            {
                m_tValue = value;
            }
        }
        public ElementValue(T value)
        {
            m_tValue = value;
        }

        public override bool Equals(object obj)
        {
            UnityEngine.Profiling.Profiler.BeginSample("ElementValue<T>.Equals");
            bool bIsEqual = false;
            ElementValue<T> p = obj as ElementValue<T>;
            if (p != null)
            {
                bIsEqual = this.m_tValue.Equals(p.Value);
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return bIsEqual;
        }

    }

    public class ElementDestroy : ElementAttribute
    {
        ulong m_eDestroyValue = 0;
        public void addDestroyType(int eDestroyType)
        {
            m_eDestroyValue |= (1uL << eDestroyType);
        }
        public void addDestroyType(ElementDestroy tElementDestroy)
        {
            if (tElementDestroy == null)
            {
                return;
            }
            m_eDestroyValue |= tElementDestroy.m_eDestroyValue;
        }

        public override bool Equals(object obj)
        {
            ElementDestroy p = obj as ElementDestroy;
            if (p == null)
            {
                return false;
            }
            return (this.m_eDestroyValue & p.m_eDestroyValue) > 0;
        }

        public bool isNull()
        {
            return m_eDestroyValue == 0;
        }
        public ElementDestroy clone()
        {
            return new ElementDestroy()
            {
                m_eDestroyValue = this.m_eDestroyValue
            };
        }

        public void filterOut(ElementDestroy tElementDestroy)
        {
            if (tElementDestroy == null)
            {
                return;
            }
            tElementDestroy.m_eDestroyValue &= ~m_eDestroyValue;
        }

    }

    public class ElementEliminateCreate : ElementAttribute
    {
        public ElementDestroy m_nEliminateMine;
        public ElementDestroy m_nEliminateUp;
        public ElementDestroy m_nEliminateDown;
        public ElementDestroy m_nEliminateLeft;
        public ElementDestroy m_nEliminateRight;

    }
    public class PassCreate : ElementAttribute
    {
        public ElementDestroy m_tElementDestroy;
        public string m_strConditionId;
    }

    /*
        Imageing : a series of prefabs for each representation.
    */
    public class Element : MonoBehaviour
    {
        public enum EShowType
        {
            normal,
            drop,
            remove,
            dropGenerate,
            generate,
            collect
        }
        private string m_strElementId;
        public Grid m_tGrid;

        public void changeGrid(Grid tGrid)
        {
            if (tGrid == m_tGrid)
            {
                return;
            }
            m_tGrid = tGrid;
        }

        private int m_nID;
        public int ID
        {
            get { return m_nID; }
        }
        public string ElementId
        {
            get { return m_strElementId; }
        }
        ElementValue<bool> m_bIsLock = new ElementValue<bool>(false);
        void refreshLock()
        {
            m_bIsLock.Value = m_nLockCount > 0;
        }
        int m_nLockCount = 0;
        public void addLockCount()
        {
            m_nLockCount++;
            refreshLock();
        }
        public void delLockCount()
        {
            m_nLockCount--;
            refreshLock();
        }

        public bool IsLock
        {
            get
            {
                return m_bIsLock.Value;
            }
        }

        public bool playAni(string strAniId, bool bIsLockElement, Action pCallback = null, float delay = 0, Vector3? tSrc = null, Vector3? tAim = null)
        {
            if (bIsLockElement == true)
                addLockCount();
            Action pOverCallback = () =>
            {
                if (bIsLockElement == true)
                    delLockCount();
                if (pCallback != null) pCallback();
            };
            if (string.IsNullOrEmpty(strAniId) == true)
            {
                pOverCallback();
                return false;
            }
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_CreateTask, new KeyValuePair<float, Action>(delay, () =>
            {
                Util.playENateAni(strAniId, gameObject, tSrc, tAim, pOverCallback);
            }));

            return true;
        }

        public bool playAniWithBehaviorId(string strBehaviorId, bool bIsLockElement = false, Action pCallback = null, float delay = 0, Vector3? tSrc = null, Vector3? tAim = null)
        {
            string strAniId = this.showTime(strBehaviorId);
            return playAni(strAniId, bIsLockElement, pCallback, delay);
        }
        private int m_nChangeCount = 0;

        public int ChangeCount
        {
            get
            {
                return m_nChangeCount;
            }
        }

        void event_clearChangeCount(object o)
        {
            m_nChangeCount = 0;
            //console.log("event_clearChangeCount ElementId:", ElementId, "m_nChangeCount", m_nChangeCount);
        }

        void changeCountAdd()
        {
            m_nChangeCount++;
            //console.log("changeCountAdd ElementId:", ElementId, "m_nChangeCount", m_nChangeCount);
        }

        public bool isCanChangeLevel()
        {
            var tEliminateTime = getElementAttribute(ElementAttribute.Attribute.eliminateTime) as ElementValue<int>;
            if (tEliminateTime == null)
            {
                return false;
            }
            return tEliminateTime.Value > m_nChangeCount;
        }
        public bool changeElementId(string strElementId)
        {
            //playAniWithBehaviorId("normal", true);
            string strSrcElementId = m_strElementId;
            m_strElementId = strElementId;
            bool bIsRefresh = refreshConfig();
            changeCountAdd();
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ElementChange, new KeyValuePair<string, Element>(strSrcElementId, this));
            playAniWithBehaviorId("generate", true);
            return bIsRefresh;
        }

        public bool skillChangeElementId(string strElementId)
        {
            string strSrcElementId = m_strElementId;
            m_strElementId = strElementId;
            bool bIsRefresh = refreshConfig();
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_SkillElementChange, new KeyValuePair<string, Element>(strSrcElementId, this));
            playAniWithBehaviorId("generate", true);
            return bIsRefresh;
        }

        public string getHypotaxisId()
        {
            ElementValue<string> tElementValue = getElementAttribute(ElementAttribute.Attribute.hypotaxis) as ElementValue<string>;
            do
            {
                if (tElementValue == null)
                {
                    break;
                }
                if (tElementValue.Value == null || tElementValue.Value == "")
                {
                    break;
                }
                return tElementValue.Value;
            } while (false);
            return m_strElementId;
        }
        bool refreshConfig()
        {
            m_mpElementAttribute = new ElementAttribute[(int) ElementAttribute.Attribute.Count];
            Dictionary<ElementAttribute.Attribute, ElementAttribute> mpElementAttribute = Config.ElementConfig.getElementAttribute(ref m_strElementId);
            if (mpElementAttribute == null)
            {
                return false;
            }
            foreach (var itAttribute in mpElementAttribute)
            {
                addElementAttribute(itAttribute.Key, itAttribute.Value);
            }
            var tWidth = getElementAttribute(ElementAttribute.Attribute.nWidth) as ElementValue<int>;
            var tHeight = getElementAttribute(ElementAttribute.Attribute.nHeight) as ElementValue<int>;
            GetComponent<RectTransform>().sizeDelta = new Vector2(m_tSize.x * tWidth.Value, m_tSize.y * tHeight.Value);
            return true;
        }
        Vector2 m_tSize;
        public void init(int nId, string strElementId)
        {
            m_nID = nId;
            m_strElementId = strElementId;
            m_tSize = GetComponent<RectTransform>().sizeDelta;
            refreshConfig();
        }
        // 元素属性
        private ElementAttribute[] m_mpElementAttribute;
        // private Dictionary<ElementAttribute.Attribute, ElementAttribute> m_mpElementAttribute;

        public ElementAttribute getElementAttribute(ElementAttribute.Attribute eAtrribute)
        {
            UnityEngine.Profiling.Profiler.BeginSample("getElementAttribute");
            if (eAtrribute == ElementAttribute.Attribute.isLock)
            {
                return m_bIsLock;
            }
            ElementAttribute tAttribute = null;
            try
            {
                tAttribute = m_mpElementAttribute[(int) eAtrribute];
            }
            catch (System.Exception) { }
            if (tAttribute != null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return tAttribute;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return ElementAttribute.getDefaultValue(eAtrribute);
        }

        public bool compareElementAttribute(ElementAttribute.Attribute eAttribute, ElementAttribute tCompareElementAttribute)
        {
            UnityEngine.Profiling.Profiler.BeginSample("compareElementAttribute");
            bool bIsOk = false;
            ElementAttribute tElementAttribute = getElementAttribute(eAttribute);
            if (tElementAttribute == null)
            {
                if (tCompareElementAttribute == null)
                {
                    bIsOk = true;
                }
            }
            else if (tElementAttribute.Equals(tCompareElementAttribute))
            {
                bIsOk = true;
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return bIsOk;
        }

        public void addElementAttribute(ElementAttribute.Attribute eAtrribute, ElementAttribute tElementAttribute)
        {
            m_mpElementAttribute[(int) eAtrribute] = tElementAttribute;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        jc.EventManager.EventObj m_tEventObj;

        // 表现
        void Awake()
        {
            m_tEventObj = new jc.EventManager.EventObj();
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_DROP_DROPOVERCHECK_Prefix, event_clearChangeCount);
        }

        void OnDestroy()
        {
            m_tEventObj.clear();
            m_tEventObj = null;
        }
    }
}