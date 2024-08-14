using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LogUtil))]
public class DeuggerEditor : Editor
{
    LogUtil debugger;
    Dictionary<string, bool> dicLogChanged = new Dictionary<string, bool>();
    Color m_pGreen = new Color(0, 1, 0);
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        debugger = (LogUtil)this.target;
        Dictionary<string, LogCache> DicLogCache = LogUtil.m_dicLogCache;

        if (DicLogCache==null)
        {
            return;
        }

        dicLogChanged.Clear();
        GUI.skin.label.normal.textColor = m_pGreen;

        foreach (var item in DicLogCache)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(item.Key,GUILayout.Width(100));
            GUILayout.Space(50);
            bool b = GUILayout.Toggle(item.Value.Console, string.Empty);

            if (b != item.Value.Console)
            {
                dicLogChanged.Add(item.Value.m_szLogType, b);
            }
            GUILayout.EndHorizontal();
        }

        foreach (var item in dicLogChanged)
        {
            DicLogCache[item.Key].Console = item.Value;
        }

        if (dicLogChanged.Count > 0)
        {
            LogUtil.OnLogStateChanged();
        }
        EditorUtility.SetDirty(target);
    }
}
