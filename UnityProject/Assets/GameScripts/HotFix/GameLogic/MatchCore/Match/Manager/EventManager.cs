using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace jc
{
    //事件管理类（单例）
    public sealed class EventManager : GameBase.Singleton<EventManager>
    {
        /** 属性变量 **/
        //事件回调函数委托
        public delegate void CallbackEvent(object e);

        //事件回调映射表<事件id、回调结构>
        private Dictionary<int, List<CallbackEvent>> m_mapEventCall = new Dictionary<int, List<CallbackEvent>>();

        /** 构造函数 **/
        public EventManager() { }

        /** 公有函数 **/
        /*  
         * 描  述：绑定按钮点击事件(每个GameObject一种事件最多绑定一次)
         * 参  数：游戏对象、回调函数委托
         * 返回值：无
         */
        public void BindBtnClick(GameObject obj, UnityAction<object> func, object param = null)
        {
            Button btn = obj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(delegate() { func(null == param ? obj : param); });
            }
            else
            {
                Debug.LogError(string.Format("EventManager - BindBtnClick - Not Found Button Component! \"{0}\"", obj.name));
            }
        }
        /*  
         * 描  述：绑定toggle(每个GameObject一种事件最多绑定一次)
         * 参  数：游戏对象、回调函数委托
         * 返回值：无
         */
        public static void BindToggle(GameObject obj, UnityAction<bool, object> func, object param = null)
        {
            Toggle com = obj.GetComponent<Toggle>();
            if (com != null)
            {
                com.onValueChanged.AddListener(delegate(bool bIsSelect) { func(bIsSelect, null == param ? obj : param); });
            }
            else
            {
                Debug.LogError( string.Format("EventOP - BindInputFieldChange - Not Found InputField Component! \"{0}\"", obj.name));
            }
        }

        /*  
         * 描  述：绑定点击事件(每个GameObject一种事件最多绑定一次)
         * 参  数：游戏对象、回调函数委托
         * 返回值：无
         */
        public void BindUIClick(GameObject obj, UnityAction<object> func, object param = null)
        {
            this.BindUIEvent(obj, func, EventTriggerType.PointerClick, param);
        }

        /*  
         * 描  述：绑定按下事件(每个GameObject一种事件最多绑定一次)
         * 参  数：游戏对象、回调函数委托
         * 返回值：无
         */
        public void BindUIDown(GameObject obj, UnityAction<object> func, object param = null)
        {
            this.BindUIEvent(obj, func, EventTriggerType.PointerDown, param);
        }

        public void BindUIUp(GameObject obj, UnityAction<object> func, object param = null)
        {
            this.BindUIEvent(obj, func, EventTriggerType.PointerUp, param);
        }

        /*  
         * 描  述：注册事件
         * 参  数：事件id
         * 返回值：无
         */
        public void RegisterEvent(int id)
        {
            if (!this.m_mapEventCall.ContainsKey(id))
            {
                this.m_mapEventCall[id] = new List<CallbackEvent>();
            }
            else
            {
                Debug.LogError( string.Format("EventManager - RegisterEvent - Event Is Registered! Id: {0}", id));
            }
        }

        /*  
         * 描  述：注销事件
         * 参  数：事件id
         * 返回值：无
         */
        public void UnRegisterEvent(int id)
        {
            if (this.m_mapEventCall.ContainsKey(id))
            {
                this.m_mapEventCall.Remove(id);
            }
            else
            {
                Debug.LogError( string.Format("EventManager - UnRegisterEvent - Not Found Event! Id: {0}", id));
            }
        }

        /*  
         * 描  述：绑定事件回调函数
         * 参  数：事件id、回调函数
         * 返回值：无
         */
        public void BindEvent(int id, CallbackEvent func)
        {
            if (!this.m_mapEventCall.ContainsKey(id))
            {
                Debug.LogError( string.Format("EventManager - BindEvent - Not Found Event! Id: {0}", id));
                return;
            }

            if (!this.m_mapEventCall[id].Contains(func))
            {
                this.m_mapEventCall[id].Add(func);
            }
            else
            {
                Debug.LogError( string.Format("EventManager - BindEvent - Event Func Has Been Bind! Id: {0}", id));
            }
        }

        /*  
         * 描  述：解绑事件回调函数
         * 参  数：事件id、回调函数
         * 返回值：无
         */
        public void UnBindEvent(int id, CallbackEvent func)
        {
            if (!this.m_mapEventCall.ContainsKey(id))
            {
                Debug.LogError( string.Format("EventManager - UnBindEvent - Not Found! Id: {0}", id));
                return;
            }

            if (this.m_mapEventCall[id].Contains(func))
            {
                this.m_mapEventCall[id].Remove(func);
            }
            else
            {
                Debug.LogError( string.Format("EventManager - UnBindEvent - Event Func Not Found! Id: {0}", id));
            }
        }

        /*  
         * 描  述：解绑事件所有回调函数
         * 参  数：事件id
         * 返回值：无
         */
        public void UnBindAllEvent(int id)
        {
            if (!this.m_mapEventCall.ContainsKey(id))
            {
                Debug.LogError( string.Format("EventManager - UnBindAllEvent - Not Found! Id: {0}", id));
                return;
            }

            this.m_mapEventCall[id].Clear();
        }

        /*  
         * 描  述：解绑所有事件
         * 参  数：事件id
         * 返回值：无
         */
        public void UnBindAll()
        {
            foreach (KeyValuePair<int, List<CallbackEvent>> objList in this.m_mapEventCall)
            {
                objList.Value.Clear();
            }
            this.m_mapEventCall.Clear();
        }

        /*  
         * 描  述：通知事件
         * 参  数：事件id、事件参数[默认]
         * 返回值：无
         */
        public void NoticeEvent(int id, object objEvent = null)
        {
            if (this.m_mapEventCall.ContainsKey(id))
            {
                List<CallbackEvent> callList = this.m_mapEventCall[id].cloneSelf();

                foreach (CallbackEvent callFunc in callList)
                {
                    try
                    {
                        callFunc(objEvent);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError( ex.ToString());
                    }
                }
            }
            else
            {
                Debug.LogError( "EventManager - NoticeEvent - Not Found Event! Id: " + id);
            }
        }

        /** 私有函数 **/
        /*  
         * 描  述：绑定点击事件(每个GameObject一种事件最多绑定一次)
         * 参  数：游戏对象、回调函数委托、事件类型
         * 返回值：无
         */
        private void BindUIEvent(GameObject obj, UnityAction<object> func, EventTriggerType eventType, object param = null)
        {
            EventTrigger et = obj.GetComponent<EventTrigger>();
            if (null == et)
            {
                et = obj.AddComponent<EventTrigger>();
            }

            //定义所要绑定的事件类型 
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(delegate(BaseEventData bed) { func(param != null ? param : obj); });
            et.triggers.Add(entry);
        }

        /** 操作属性变量 **/
        public Dictionary<int, List<CallbackEvent>> _EventCallMap
        {
            get { return this.m_mapEventCall; }
        }

        public class EventObj
        {
            class EventInfo
            {
                int m_eEventType;
                CallbackEvent m_pCallback;
                public EventInfo(int eEventType, CallbackEvent callback)
                {
                    m_eEventType = eEventType;
                    m_pCallback = callback;
                    // EventManager.Instance.BindEvent(eEventType, callback);
                }
                public void unBind()
                {
                    if (m_pCallback == null)
                    {
                        return;
                    }
                    // EventManager.Instance.UnBindEvent(m_eEventType, m_pCallback);
                    m_pCallback = null;
                }

                public bool equalsCallback(CallbackEvent callback)
                {
                    return m_pCallback == callback;
                }
            }
            Dictionary<int, List<EventInfo>> m_arrEventInfo = new Dictionary<int, List<EventInfo>>();
            public void Add(int eEventType, CallbackEvent callback)
            {
                if (callback == null)
                {
                    return;
                }
                EventInfo tEventInfo = new EventInfo(eEventType, callback);
                if (m_arrEventInfo.ContainsKey(eEventType) == false)
                {
                    m_arrEventInfo.Add(eEventType, new List<EventInfo>());
                }
                m_arrEventInfo[eEventType].Add(tEventInfo);
            }

            public void Del(int eEventType, CallbackEvent callback)
            {
                if (m_arrEventInfo.ContainsKey(eEventType))
                {
                    foreach (var tEventInfo in m_arrEventInfo[eEventType])
                    {
                        if (tEventInfo.equalsCallback(callback))
                        {
                            tEventInfo.unBind();
                            m_arrEventInfo[eEventType].Remove(tEventInfo);
                        }
                    }
                }
            }

            public void clear()
            {
                foreach (var arrEventInfo in m_arrEventInfo)
                {
                    foreach (var tEventInfo in arrEventInfo.Value)
                    {
                        tEventInfo.unBind();
                    }
                }
                m_arrEventInfo.Clear();
            }
            ~EventObj()
            {
                clear();
            }
        }
    }
}