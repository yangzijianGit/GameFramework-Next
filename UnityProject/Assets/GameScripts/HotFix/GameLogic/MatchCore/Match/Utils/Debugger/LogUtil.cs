using System;
using System.IO;
using System.Text;
using UnityEngine;
//using LuaFramework;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public class LogCache
{
    public string m_szLogType;
    public bool Console;
    public StringBuilder m_szLog = new StringBuilder();
    public int count = 1;
    public int width = 20;

    public void Setup(string szLogType)
    {
        m_szLogType = szLogType;
    }

    public void PushLog(string szLog)
    {
        count = 1;
        m_szLog.Append(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
        m_szLog.Append("\t");

        m_szLog.Append(szLog);
        m_szLog.Append("\n");

        //防止errorlog 太多，刷不出来
        if (m_szLogType.ToLower() == "error" && m_szLog.Length > 1000)
        {
            m_szLog.Remove(1000, m_szLog.Length - 1000);
        }
    }

    public string GetAllLog()
    {
        return m_szLog.ToString();
    }

    public void ClearLog()
    {
        m_szLog.Remove(0, m_szLog.Length);
    }
}

public class LogUtil : MonoBehaviour
{
    [Header("开关日志")]
    public bool isShowLog = true;

    public static LogUtil Instance;
    static bool _IsOpen = false;

    //private LuaManager _LuaMgr;
    //public LuaManager LuaMgr
    //{
    //    get
    //    {
    //        if (_LuaMgr == null)
    //        {
    //            _LuaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
    //        }
    //        return _LuaMgr;
    //    }
    //}

    /// <summary>
    /// 显示log日志开关
    /// </summary>
    private bool isLogShow = true;
    /// <summary>
    /// log日志输出文字大小
    /// </summary>
    private int fontSize = 26;
    /// <summary>
    /// 一个简单的开关没有什么含义
    /// </summary>
    private bool isBol = false;

    private LogCache m_pLogCache = null;

    #region 生命周期函数 ----------------------------
    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        Application.logMessageReceived += OnHandleLogEvent;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= OnHandleLogEvent;
    }

    void Update()
    {
        if (!isShowLog)
        {
            return;
        }

        if (Input.touchCount >= 3)
        {
            if (Input.touches[2].phase == TouchPhase.Began)
            {
                ///手机上开启Log界面
                _IsOpen = !_IsOpen;
            }
        }

        if (Input.GetKeyUp("`"))
        {
            ///pc上开启Log界面
            _IsOpen = !_IsOpen;
        }
    }

    #endregion --------------------------------------

    #region 事件返回 --------------------------------

    /// <summary>
    /// 错误堆栈时间返回
    /// </summary>
    /// <param name="logString">错误信息</param>
    /// <param name="stackTrace">跟踪堆栈</param>
    /// <param name="type">错误类型</param>
    private void OnHandleLogEvent(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            AddLog("Error", logString);
            //AddLog("ErrorDes", logString); // .MoreStringFormat(stackTrace));
        }
    }

    public void OnApplicationQuit()
    {
        m_dicLogCache.Clear();
        m_dicLogCache = null;
    }

    #endregion --------------------------------------

    #region 接口和方法 ------------------------------

    public static Dictionary<string, LogCache> m_dicLogCache = new Dictionary<string, LogCache>();

    private static LogCache GetLogCache(string szLogType)
    {
        LogCache value;
        if (m_dicLogCache != null)
        {
            m_dicLogCache.TryGetValue(szLogType, out value);
            return value;
        }
        return null;
    }

    private static void ClearLogCache(string szLogType)
    {
        LogCache value;
        m_dicLogCache.TryGetValue(szLogType, out value);
        value.ClearLog();
    }

    public static void AddLog(string szLogType, string szLog)
    {
#if UNITY_EDITOR
        // Debug.Log(szLogType); // .MoreStringFormat(":", szLog));
#endif        
        if (!LogUtil.Instance.isShowLog)
        {
            return;
        }

        LogCache pCache = GetLogCache(szLogType);
        if (pCache == null)
        {
            pCache = new LogCache();
            pCache.Setup(szLogType);
            m_dicLogCache.Add(szLogType, pCache);
        }
        pCache.PushLog(szLog);
    }

    #endregion --------------------------------------

    internal void OnGUI()
    {
        if (!_IsOpen)
        {
            return;
        }

        //显示功能操作按钮
        LogFunctionBtnShow();

        GUI.skin.label.normal.textColor = GetCurrColorValue;

        //显示日志标签按钮
        LogTabBtnShow();

        if (isLogShow)
        {
            //显示日志文字
            LogLabelShow();
        }
    }

    #region 创建功能操作按钮 ------------------------

    private void LogFunctionBtnShow()
    {
        //适配缩放比（宽）
        float btnScaleWigth = Screen.width / 1080.0f;
        //适配缩放比（高）
        float btnScaleHight = Screen.height / 1920.0f;
        //按钮的宽
        float btnWigth = Screen.width * 0.15f;
        //按钮的高
        float btnHight = Screen.height * 0.05f;

        //GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(btnScaleWigth, btnScaleHight, 1));
        GUIStyle style = new GUIStyle(GUI.skin.GetStyle("button"));
        style.fontSize = 26 * (int)btnScaleWigth;

        float btnPosX = 10;
        float indexY = 110;

        Rect rect = new Rect(btnPosX, btnHight + indexY * 0, btnWigth, btnHight);
        if (GUI.Button(rect, "显示log", style))
        {
            IsLogLabelShow();
        }

        rect = new Rect(btnPosX, btnHight + indexY * 1, btnWigth, btnHight);
        if (GUI.Button(rect, "清屏", style))
        {
            LogLabelClear();
        }

        rect = new Rect(btnPosX, btnHight + indexY * 2, btnWigth, btnHight);
        if (GUI.Button(rect, "切换颜色", style))
        {
            LogLabelChangeColor();
        }

        rect = new Rect(btnPosX, btnHight + indexY * 3, btnWigth, btnHight);
        if (GUI.Button(rect, "输出为txt", style))
        {
            LogLabelSaveToTxt();
        }

        rect = new Rect(btnPosX, btnHight + indexY * 4, btnWigth, btnHight);
        if (GUI.Button(rect, "放大字体", style))
        {
            LogLabelFontSize(1);
        }

        rect = new Rect(btnPosX, btnHight + indexY * 5, btnWigth, btnHight);
        if (GUI.Button(rect, "缩小字体", style))
        {
            LogLabelFontSize(0);
        }
    }

    #endregion --------------------------------------

    #region 创建log页签按钮 -------------------------

    private void LogTabBtnShow()
    {
        //适配缩放比（宽）
        float btnScaleWigth = Screen.width / 1080.0f;
        //适配缩放比（高）
        float btnScaleHight = Screen.height / 1920.0f;

        //计数
        int logIndex = 0;
        //按钮的宽
        float btnWigth = Screen.width * 0.15f;
        //按钮的高
        float btnHight = Screen.height * 0.05f;
        //按钮起始坐标X位置
        float posX = 220 * btnScaleWigth;
        //按钮起始坐标Y位置
        float posY = 18.0f * btnScaleHight;
        //一行按钮有几个
        int lineMax = 5;

        GUIStyle style = new GUIStyle(GUI.skin.GetStyle("button"));
        style.fontSize = 24 * (int)btnScaleWigth;

        if (m_dicLogCache.Count > 0)
        {
            foreach (var logCache in m_dicLogCache)
            {
                LogCache pCache = logCache.Value;

                int nRow = logIndex % lineMax;
                int nLine = logIndex / lineMax;
                logIndex++;
                Rect rect = new Rect(posX + btnWigth * nRow, posY + btnHight * nLine, btnWigth, btnHight);
                if (GUI.Button(rect, pCache.m_szLogType, style))
                {
                    m_pLogCache = pCache;
                }
            }
        }
    }

    #endregion --------------------------------------

    #region 显示log日志 -----------------------------

    private Vector2 m_scroll = new Vector2(0, 0);

    /// <summary>
    /// 是否显示log开关
    /// </summary>
    public bool IsLogLabelShow()
    {
        isLogShow = !isLogShow;
        return isLogShow;
    }

    /// <summary>
    /// GUI绘制log文字
    /// </summary>
    private void LogLabelShow()
    {
        GUIStyle Gsty = new GUIStyle();
        Gsty.normal.textColor = GetCurrColorValue;
        Gsty.fontSize = this.fontSize;
        Gsty.wordWrap = true;

        //总共有多少个标签
        int allCount = m_dicLogCache.Count;
        //标签的高度
        int btnHight = (int)(Screen.height * 0.06f);
        //文字显示的位置
        float lablePosY = btnHight * (allCount / 8.0f + 1.0f);

        GUILayout.BeginArea(new Rect(180, lablePosY, Screen.width / 1.2f, Screen.height - lablePosY));
        GUIStyle barStyle = new GUIStyle("verticalscrollbar");
        barStyle.fixedWidth = 50;
        m_scroll = GUILayout.BeginScrollView(m_scroll, false, false, null, barStyle);

        if (m_pLogCache != null)
        {
            GUILayout.Label(m_pLogCache.GetAllLog(), Gsty);
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    #endregion --------------------------------------

    #region 清屏 ------------------------------------

    /// <summary>
    /// 清屏
    /// </summary>
    public void LogLabelClear()
    {
        ClearLogCache(m_pLogCache.m_szLogType);
    }

    #endregion --------------------------------------

    #region 控制log文字的大小 -----------------------

    /// <summary>
    /// 控制log文字的大小
    /// sizeType【0】缩小【1】放大
    /// </summary>
    public void LogLabelFontSize(int sizeType)
    {
        if (sizeType == 0)
        {
            this.fontSize -= 2;
        }
        else
        {
            this.fontSize += 2;
        }
    }

    #endregion --------------------------------------

    #region 获取当前log文字颜色 ---------------------

    private int currColorId = 0;
    ///红
    private Color color01 = new Color(255.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);
    ///橙
    private Color color02 = new Color(249.0f / 255.0f, 111.0f / 255.0f, 0.0f / 255.0f);
    ///黄
    private Color color03 = new Color(255.0f / 255.0f, 255.0f / 255.0f, 0.0f / 255.0f);
    ///绿
    private Color color04 = new Color(0.0f / 255.0f, 128.0f / 255.0f, 0.0f / 255.0f);
    ///蓝
    private Color color05 = new Color(0.0f / 255.0f, 0.0f / 255.0f, 255.0f / 255.0f);
    ///紫
    private Color color06 = new Color(230.0f / 255.0f, 39.0f / 255.0f, 64.0f / 255.0f);
    ///黑
    private Color color07 = new Color(0.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);

    private Color GetCurrColorValue
    {
        get
        {
            Color color = color01;
            if (currColorId == 0)
            {
                color = color01;
            }
            else if (currColorId == 1)
            {
                color = color02;
            }
            else if (currColorId == 2)
            {
                color = color03;
            }
            else if (currColorId == 3)
            {
                color = color04;
            }
            else if (currColorId == 4)
            {
                color = color05;
            }
            else if (currColorId == 5)
            {
                color = color06;
            }
            else if (currColorId == 6)
            {
                color = color07;
            }

            return color;
        }
    }

    /// <summary>
    /// 切换文字颜色
    /// </summary>
    public void LogLabelChangeColor()
    {
        currColorId++;
        if (currColorId > 6)
        {
            currColorId = 0;
        }
    }

    #endregion --------------------------------------

    #region 将日志写入txt文件，保存到本地 -----------

    public void LogLabelSaveToTxt()
    {
        if (m_pLogCache != null)
        {
            string log = m_pLogCache.GetAllLog();

            string path = Application.persistentDataPath + "/Log/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(string.Format("{0}{1}.txt", path, m_pLogCache.m_szLogType), log, Encoding.Default);
        }
    }

    #endregion --------------------------------------

    #region Debugger-Editor -------------------------

    public static void OnLogStateChanged()
    {
        ClearConsole();
        foreach (var item in m_dicLogCache)
        {
            if (item.Value.Console)
            {
                PlayerPrefs.SetInt(item.Value.m_szLogType, 1);

                string[] logs = item.Value.GetAllLog().Split('\n');

                for (int i = 0; i < logs.Length; i++)
                {
                    Debug.Log(logs[i]);
                }
            }
            else
            {
                PlayerPrefs.SetInt(item.Value.m_szLogType, 0);
            }
        }
    }

    public static void ClearConsole()
    {
#if UNITY_EDITOR
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
#endif
    }

    #endregion --------------------------------------
}
