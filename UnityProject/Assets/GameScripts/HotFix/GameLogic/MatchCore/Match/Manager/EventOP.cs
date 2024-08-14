#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//事件类(无实例)
public sealed class EventOP
{
    /** 属性变量 **/
    //事件回调函数委托
    public delegate void CallbackEvent(object e);

    //事件回调映射表<事件id、回调结构>
    private static Dictionary<int, List<CallbackEvent>> m_mapEventCall = new Dictionary<int, List<CallbackEvent>>();

    //外部文件事件回调函数委托
    public delegate void ExternFileCallbackEvent(string path, object o);

    //外部文件事件结构
    public class ExternFile
    {
        //游戏对象，回调函数，参数对象，状态值
        public GameObject obj = null;
        public ExternFileCallbackEvent func = null;
        public object param = null;
        public bool status = false;
    }

    //光标、外部文件对象、路径
    private static DragAndDropVisualMode m_enumCursor = DragAndDropVisualMode.None;
    private static ExternFile m_objEF = null;
    private static string m_strPath = string.Empty;

    //外部文件回调映射列表<GameObject的id值，回调结构>
    private static Dictionary<int, ExternFile> m_mapFileDragEventCall = new Dictionary<int, ExternFile>();

    /** 构造函数 **/
    private EventOP() { }

    /** 公有函数 **/
    /*  
     * 描  述：绑定外部文件拖拽接收事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindFileDrag(GameObject obj, ExternFileCallbackEvent func, object param = null)
    {
        int id = obj.GetHashCode();
        if (!EventOP.m_mapFileDragEventCall.ContainsKey(id))
        {
            ExternFile ef = new ExternFile();
            ef.obj = obj;
            ef.func = func;
            ef.param = param;
            ef.status = false;

            EventOP.m_mapFileDragEventCall[id] = ef;
        }
        else
        {
            Debug.LogWarning(string.Format("EventOP - BindFileDrag - GameObject Is Bind! Name: {0}", obj.name));
        }
    }

    /*  
     * 描  述：绑定按钮点击事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindBtnClick(GameObject obj, UnityAction<object> func, object param = null)
    {
        Button btn = obj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(delegate() { func(null == param ? obj : param); });
        }
        else
        {
            Debug.LogError(string.Format("EventOP - BindBtnClick - Not Found Button Component! \"{0}\"", obj.name));
        }
    }

    /*  
     * 描  述：绑定编辑框结束事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindInputFieldEdit(GameObject obj, UnityAction<object> func, object param = null)
    {
        InputField com = obj.GetComponent<InputField>();
        if (com != null)
        {
            com.onEndEdit.AddListener(delegate { func(null == param ? obj : param); });
        }
        else
        {
            Debug.LogError(string.Format("EventOP - BindInputFieldEdit - Not Found InputField Component! \"{0}\"", obj.name));
        }
    }

    /*  
     * 描  述：绑定编辑框变化事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindInputFieldChange(GameObject obj, UnityAction<object> func, object param = null)
    {
        InputField com = obj.GetComponent<InputField>();
        if (com != null)
        {
            com.onValueChanged.AddListener(delegate { func(null == param ? obj : param); });
        }
        else
        {
            Debug.LogError(string.Format("EventOP - BindInputFieldChange - Not Found InputField Component! \"{0}\"", obj.name));
        }
    }

    /*  
     * 描  述：绑定toggle(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindToggle(GameObject obj, UnityAction<bool> func)
    {
        Toggle com = obj.GetComponent<Toggle>();
        if (com != null)
        {
            com.onValueChanged.AddListener(delegate(bool bIsSelect) { func(bIsSelect); });
        }
        else
        {
            Debug.LogError(string.Format("EventOP - BindInputFieldChange - Not Found InputField Component! \"{0}\"", obj.name));
        }
    }

    /*  
     * 描  述：绑定卷动列变化事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindScrollBarChange(GameObject obj, UnityAction<object> func, object param = null)
    {
        Scrollbar com = obj.GetComponent<Scrollbar>();
        if (com != null)
        {
            com.onValueChanged.AddListener(delegate { func(null == param ? obj : param); });
        }
        else
        {
            Debug.LogError(string.Format("EventOP - BindScrollBarChange - Not Found Scrollbar Component! \"{0}\"", obj.name));
        }
    }

    /*  
     * 描  述：绑定下拉框变化事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindDropdownChange(GameObject obj, UnityAction<object> func, object param = null)
    {
        Dropdown com = obj.GetComponent<Dropdown>();
        if (com != null)
        {
            com.onValueChanged.AddListener(delegate { func(null == param ? obj : param); });
        }
        else
        {
            Debug.LogError(string.Format("EventOP - BindDropdownChange - Not Found Dropdown Component! \"{0}\"", obj.name));
        }
    }

    /*  
     * 描  述：绑定点击事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindUIClick(GameObject obj, UnityAction<object> func, object param = null)
    {
        EventOP.BindUIEvent(obj, func, EventTriggerType.PointerClick, param);
    }

    /*  
     * 描  述：绑定按下事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindUIDown(GameObject obj, UnityAction<object> func, object param = null)
    {
        EventOP.BindUIEvent(obj, func, EventTriggerType.PointerDown, param);
    }

    /*  
     * 描  述：绑定拖动事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindUIDrag(GameObject obj, UnityAction<object> func, object param = null)
    {
        EventOP.BindUIEvent(obj, func, EventTriggerType.Drag, param);
    }

    /*  
     * 描  述：绑定拖动进入事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindUIDragEnter(GameObject obj, UnityAction<object> func, object param = null)
    {
        EventOP.BindUIEvent(obj, func, EventTriggerType.PointerEnter, param);
    }

    /*  
     * 描  述：绑定移动事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindUIMove(GameObject obj, UnityAction<object> func, object param = null)
    {
        EventOP.BindUIEvent(obj, func, EventTriggerType.Move, param);
    }

    /*  
     * 描  述：绑定丢失选中事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托
     * 返回值：无
     */
    public static void BindUIDeselect(GameObject obj, UnityAction<object> func, object param = null)
    {
        EventOP.BindUIEvent(obj, func, EventTriggerType.Deselect, param);
    }

    /*  
     * 描  述：注册事件
     * 参  数：事件id
     * 返回值：无
     */
    public static void RegisterEvent(int id)
    {
        if (!EventOP.m_mapEventCall.ContainsKey(id))
        {
            EventOP.m_mapEventCall[id] = new List<CallbackEvent>();
        }
        else
        {
            Debug.LogWarning(string.Format("EventOP - RegisterEvent - Event Is Registered! Id: {0}", id));
        }
    }

    /*  
     * 描  述：注销事件
     * 参  数：事件id
     * 返回值：无
     */
    public static void UnRegisterEvent(int id)
    {
        if (EventOP.m_mapEventCall.ContainsKey(id))
        {
            EventOP.m_mapEventCall.Remove(id);
        }
        else
        {
            Debug.LogWarning(string.Format("EventOP - UnRegisterEvent - Not Found Event! Id: {0}", id));
        }
    }

    /*  
     * 描  述：绑定事件回调函数
     * 参  数：事件id、回调函数
     * 返回值：无
     */
    public static void BindEvent(int id, CallbackEvent func)
    {
        if (!EventOP.m_mapEventCall.ContainsKey(id))
        {
            Debug.LogError(string.Format("EventOP - BindEvent - Not Found Event! Id: {0}", id));
            return;
        }

        if (!EventOP.m_mapEventCall[id].Contains(func))
        {
            EventOP.m_mapEventCall[id].Add(func);
        }
        else
        {
            Debug.LogWarning(string.Format("EventOP - BindEvent - Event Func Has Been Bind! Id: {0}", id));
        }
    }

    /*  
     * 描  述：解绑事件回调函数
     * 参  数：事件id、回调函数
     * 返回值：无
     */
    public static void UnBindEvent(int id, CallbackEvent func)
    {
        if (!EventOP.m_mapEventCall.ContainsKey(id))
        {
            Debug.LogWarning(string.Format("EventOP - UnBindEvent - Not Found! Id: {0}", id));
            return;
        }

        if (EventOP.m_mapEventCall[id].Contains(func))
        {
            EventOP.m_mapEventCall[id].Remove(func);
        }
        else
        {
            Debug.LogWarning(string.Format("EventOP - UnBindEvent - Event Func Not Found! Id: {0}", id));
        }
    }

    /*  
     * 描  述：解绑事件所有回调函数
     * 参  数：事件id
     * 返回值：无
     */
    public static void UnBindAllEvent(int id)
    {
        if (!EventOP.m_mapEventCall.ContainsKey(id))
        {
            Debug.LogWarning(string.Format("EventOP - UnBindAllEvent - Not Found! Id: {0}", id));
            return;
        }

        EventOP.m_mapEventCall[id].Clear();
    }

    /*  
     * 描  述：解绑所有事件
     * 参  数：事件id
     * 返回值：无
     */
    public static void UnBindAll()
    {
        foreach (KeyValuePair<int, List<CallbackEvent>> objList in EventOP.m_mapEventCall)
        {
            objList.Value.Clear();
        }
        EventOP.m_mapEventCall.Clear();
    }

    /*  
     * 描  述：禁用外部文件拖拽事件
     * 参  数：无
     * 返回值：无
     */
    public static void DisableExternDrag()
    {
        foreach (var kv in EventOP.m_mapFileDragEventCall)
        {
            EventOP.m_mapFileDragEventCall[kv.Key].status = false;
        }
    }

    /*  
     * 描  述：启用外部文件拖拽事件
     * 参  数：无
     * 返回值：无
     */
    public static void EnableExternDrag()
    {
        foreach (var kv in EventOP.m_mapFileDragEventCall)
        {
            EventOP.m_mapFileDragEventCall[kv.Key].status = true;
        }
    }

    /*  
     * 描  述：更新所有事件
     * 参  数：无
     * 返回值：无
     */
    public static void UpdateEvent()
    {
        if (DragAndDrop.paths.Length > 0)
        {
            string path = DragAndDrop.paths[0];
            bool isTriggle = false;
            if (Function.IsPNG(path))
            {
                foreach (var kv in EventOP.m_mapFileDragEventCall)
                {
                    if (kv.Value.status)
                    {
                        Rect rt = Function.GameObject2Rect(kv.Value.obj);
                        if (rt.Contains(Event.current.mousePosition))
                        {
                            isTriggle = true;
                            EventOP.m_objEF = kv.Value;
                            break;
                        }
                    }
                }
            }

            if (isTriggle)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                EventOP.m_enumCursor = DragAndDropVisualMode.Generic;
                EventOP.m_strPath = path;
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
                EventOP.m_enumCursor = DragAndDropVisualMode.None;
                EventOP.m_strPath = string.Empty;
                EventOP.m_objEF = null;
            }
        }
        else
        {
            if (DragAndDrop.visualMode == DragAndDropVisualMode.Generic && EventOP.m_objEF != null)
            {
                EventOP.m_objEF.func(EventOP.m_strPath, null == EventOP.m_objEF.param ? EventOP.m_objEF.obj : EventOP.m_objEF.param);

                DragAndDrop.visualMode = DragAndDropVisualMode.None;
                EventOP.m_objEF = null;
                EventOP.m_strPath = string.Empty;
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
                EventOP.m_objEF = null;
                EventOP.m_strPath = string.Empty;
            }
        }
    }

    /*  
     * 描  述：通知事件
     * 参  数：事件id、事件参数[默认]
     * 返回值：无
     */
    public static void NoticeEvent(int id, object objEvent = null)
    {
        if (EventOP.m_mapEventCall.ContainsKey(id))
        {
            List<CallbackEvent> callList = EventOP.m_mapEventCall[id];

            foreach (CallbackEvent callFunc in callList)
            {
                callFunc(objEvent);
            }
        }
        else
        {
            Debug.LogError("EventOP - NoticeEvent - Not Found Event! Id: " + id);
        }
    }

    /** 私有函数 **/
    /*  
     * 描  述：绑定点击事件(每个GameObject一种事件最多绑定一次)
     * 参  数：游戏对象、回调函数委托、事件类型
     * 返回值：无
     */
    private static void BindUIEvent(GameObject obj, UnityAction<object> func, EventTriggerType eventType, object param = null)
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
    public static Dictionary<int, List<CallbackEvent>> _EventCallMap
    {
        get { return EventOP.m_mapEventCall; }
    }
}

#endif