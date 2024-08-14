#if UNITY_EDITOR

using System.Collections.Generic;
using System.Xml;
using UnityEngine;

//配置类(无实例)
public sealed class ConfigOP
{
    //元素映射列表<名字，皮肤>(暂时停用)
    public static Dictionary<string, string> m_mapElement = new Dictionary<string, string>();

    //道具映射列表<名字，皮肤>(暂时停用)
    public static Dictionary<string, string> m_mapProps = new Dictionary<string, string>();

    //角色映射列表<名字，皮肤>(暂时停用)
    public static Dictionary<string, string> m_mapRole = new Dictionary<string, string>();

    //元素结构
    public sealed class ElementConfig
    {
        //元素id 元素层级 元素皮肤
        public string m_strId = string.Empty;
        public int m_nLevel = 0;
        public string m_strSkin = string.Empty;

        public ElementConfig() { }
        public ElementConfig(string id, int level, string skin)
        {
            this.m_strId = id;
            this.m_nLevel = level;
            this.m_strSkin = skin;
        }
    }

    //掉落元素结构
    public sealed class DropElementConfig
    {
        //掉落元素id，掉落元素皮肤
        public string m_strId = string.Empty;
        public string m_strSkin = string.Empty;

        public DropElementConfig() { }
        public DropElementConfig(string id, string skin)
        {
            this.m_strId = id;
            this.m_strSkin = skin;
        }
    }

    //元素配置映射列表<id, 元素属性结构>
    public static Dictionary<string, ElementConfig> m_mapElementConfig = new Dictionary<string, ElementConfig>();

    public static bool checkElementExist(string strElementId)
    {
        return m_mapElementConfig.ContainsKey(strElementId);
    }

    public static ElementConfig getElementConfig(string strElementId)
    {
        if (m_mapElementConfig.ContainsKey(strElementId) == true)
        {
            return m_mapElementConfig[strElementId];
        }
        return null;
    }

    //掉落元素配置映射列表<id, 掉落元素属性结构>
    public static Dictionary<string, DropElementConfig> m_mapDropElementConfig = new Dictionary<string, DropElementConfig>();

    //关卡输出路径、元素配置文件路径、掉落元素配置文件路径
    public static string m_strLevelOutputPath = string.Empty;
    private static string m_strElementConfigPath = string.Empty;
    private static string m_strDropElementConfigPath = string.Empty;

    //元素等级列表
    public static List<int> m_listElementGrade = new List<int>();

    /** 构造函数 **/
    private ConfigOP() { }

    //获取路径前缀
    private static string getPathPrefix()
    {
        string strSub = "artist/201911_kumamon/Assets";
        int index = Application.dataPath.IndexOf(strSub);
        string strPrefix = Application.dataPath.Substring(0, index);

        return strPrefix;
    }

    //加载配置
    public static void Load()
    {
        //读取编辑器配置
        ConfigOP.loadEditorConfig();

        //读取元素配置
        ConfigOP.loadElementConfig();

        //读取掉落元素配置
        ConfigOP.loadDropElementConfig();

        //筛选元素等级列表
        ConfigOP.filterElementGrade();
    }

    //保存配置
    public static void Save() { }

    //筛选元素等级列表
    private static void filterElementGrade()
    {
        foreach (var element in ConfigOP.m_mapElementConfig)
        {
            if (!ConfigOP.m_listElementGrade.Contains(element.Value.m_nLevel))
            {
                ConfigOP.m_listElementGrade.Add(element.Value.m_nLevel);
            }
        }

        ConfigOP.m_listElementGrade.Sort((x, y) => x.CompareTo(y));
    }

    //加载编辑器配置
    private static void loadEditorConfig()
    {
        string strPrefix = ConfigOP.getPathPrefix();
        string filePath = strPrefix + GlobalDefine.PATH_LEVEL_EDITOR_CONFIG;

        XmlTextReader reader = new XmlTextReader(filePath);
        if (null == reader)
        {
            Debug.LogError(string.Format("ConfigOP - loadEditorConfig - Read XML Failed! \"{0}\"", filePath));
            return;
        }
        while (reader.Read())
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode node = xmlDocument.ReadNode(reader);

            XmlNodeList rootChild = node.ChildNodes;
            foreach (XmlNode rc in rootChild)
            {
                if (rc.Name == "game")
                {
                    XmlNodeList gameChild = rc.ChildNodes;
                    foreach (XmlNode gc in gameChild)
                    {
                        switch (gc.Name)
                        {
                            case "output":
                                {
                                    ConfigOP.m_strLevelOutputPath = strPrefix + gc.InnerText;
                                    break;
                                }
                            case "elementPath":
                                {
                                    ConfigOP.m_strElementConfigPath = strPrefix + gc.InnerText;
                                    break;
                                }
                            case "dropConfigPath":
                                {
                                    ConfigOP.m_strDropElementConfigPath = strPrefix + gc.InnerText;
                                    break;
                                }
                        }
                    }
                }
            }
        }

        reader.Close();
    }

    //加载元素配置文件
    private static void loadElementConfig()
    {
        XmlTextReader reader = new XmlTextReader(ConfigOP.m_strElementConfigPath);
        if (null == reader)
        {
            Debug.LogError(string.Format("ConfigOP - loadElementConfig - Read XML Failed! \"{0}\"", ConfigOP.m_strElementConfigPath));
            return;
        }
        while (reader.Read())
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode node = xmlDocument.ReadNode(reader);

            XmlNodeList rootChild = node.ChildNodes;
            foreach (XmlNode rc in rootChild)
            {
                if (rc.Name == "game")
                {
                    XmlNodeList gameChild = rc.ChildNodes;
                    foreach (XmlNode gc in gameChild)
                    {
                        if (gc.Name == "element")
                        {
                            string id = string.Empty;
                            string skin = string.Empty;
                            int level = -1;

                            XmlNodeList elementChild = gc.ChildNodes;
                            foreach (XmlNode _ec in elementChild)
                            {
                                switch (_ec.Name)
                                {
                                    case "id":
                                        {
                                            id = _ec.InnerText;
                                            break;
                                        }
                                    case "skin":
                                        {
                                            skin = _ec.InnerText;
                                            break;
                                        }
                                    case "level":
                                        {
                                            level = int.Parse(_ec.InnerText);
                                            break;
                                        }
                                }
                            }

                            if (id != string.Empty)
                            {
                                if (ConfigOP.m_mapElementConfig.ContainsKey(id))
                                {
                                    Debug.LogWarning(string.Format("ConfigOP - loadElementConfig - Element Already Exists! Id:{0}", id));
                                }
                                else
                                {
                                    ConfigOP.m_mapElementConfig[id] = new ElementConfig(id, level, skin);
                                }
                            }
                            else
                            {
                                Debug.LogError("ConfigOP - loadElementConfig - Element Struct Error!");
                            }
                        }
                    }
                }
            }
        }

        reader.Close();
    }

    //加载掉落元素配置文件
    private static void loadDropElementConfig()
    {
        XmlTextReader reader = new XmlTextReader(ConfigOP.m_strDropElementConfigPath);
        if (null == reader)
        {
            Debug.LogError(string.Format("ConfigOP - loadDropElementConfig - Read XML Failed! \"{0}\"", ConfigOP.m_strDropElementConfigPath));
            return;
        }
        while (reader.Read())
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode node = xmlDocument.ReadNode(reader);

            XmlNodeList rootChild = node.ChildNodes;
            foreach (XmlNode rc in rootChild)
            {
                if (rc.Name == "game")
                {
                    XmlNodeList gameChild = rc.ChildNodes;
                    foreach (XmlNode gc in gameChild)
                    {
                        if (gc.Name == "DropNode")
                        {
                            string id = string.Empty;
                            string skin = string.Empty;

                            XmlNodeList elementChild = gc.ChildNodes;
                            foreach (XmlNode _ec in elementChild)
                            {
                                switch (_ec.Name)
                                {
                                    case "id":
                                        {
                                            id = _ec.InnerText;
                                            break;
                                        }
                                    case "skin":
                                        {
                                            skin = _ec.InnerText;
                                            break;
                                        }
                                }
                            }

                            if (id != string.Empty)
                            {
                                if (ConfigOP.m_mapElementConfig.ContainsKey(id))
                                {
                                    Debug.LogWarning(string.Format("ConfigOP - loadDropElementConfig - Element Already Exists! Id:{0}", id));
                                }
                                else
                                {
                                    ConfigOP.m_mapDropElementConfig[id] = new DropElementConfig(id, skin);
                                }
                            }
                            else
                            {
                                Debug.LogError("ConfigOP - loadDropElementConfig - Element Struct Error!");
                            }
                        }
                    }
                }
            }
        }

        reader.Close();
    }

    //加载旧版配置(暂时停用)
    public static void loadConfig()
    {
        string strPrefix = ConfigOP.getPathPrefix();
        string filePath = strPrefix + GlobalDefine.PATH_CONFIG_LEVEL_EDITOR;

        XmlTextReader reader = new XmlTextReader(filePath);
        if (null == reader)
        {
            Debug.LogError(string.Format("ConfigOP - LoadConfig - Read XML Failed! \"{0}\"", filePath));
            return;
        }
        while (reader.Read())
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode node = xmlDocument.ReadNode(reader);

            XmlNodeList rootChild = node.ChildNodes;
            foreach (XmlNode rc in rootChild)
            {
                if (rc.Name == "game")
                {
                    XmlNodeList gameChild = rc.ChildNodes;
                    foreach (XmlNode gc in gameChild)
                    {
                        switch (gc.Name)
                        {
                            case "element":
                                {
                                    //元素
                                    XmlNodeList elementChild = gc.ChildNodes;
                                    foreach (XmlNode _ec in elementChild)
                                    {
                                        if (_ec.Name == "option")
                                        {
                                            XmlNodeList optionChild = _ec.ChildNodes;
                                            string name = string.Empty, skin = string.Empty;
                                            foreach (XmlNode __oc in optionChild)
                                            {
                                                if (__oc.Name == "name")
                                                {
                                                    name = __oc.InnerText;
                                                }
                                                else if (__oc.Name == "skin")
                                                {
                                                    skin = __oc.InnerText;
                                                }
                                            }
                                            if (name != string.Empty && skin != string.Empty)
                                            {
                                                if (ConfigOP.m_mapElement.Count < GlobalDefine.MAX_ELEMENT_COUNT)
                                                {
                                                    ConfigOP.m_mapElement[name] = skin;
                                                }
                                                else
                                                {
                                                    Debug.LogError(string.Format("ConfigOP - LoadConfig - Element Count Too Many! {0}", GlobalDefine.MAX_ELEMENT_COUNT));
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                            case "props":
                                {
                                    //道具
                                    XmlNodeList propsChild = gc.ChildNodes;
                                    foreach (XmlNode _pc in propsChild)
                                    {
                                        if (_pc.Name == "option")
                                        {
                                            XmlNodeList optionChild = _pc.ChildNodes;
                                            string name = string.Empty, skin = string.Empty;
                                            foreach (XmlNode __oc in optionChild)
                                            {
                                                if (__oc.Name == "name")
                                                {
                                                    name = __oc.InnerText;
                                                }
                                                else if (__oc.Name == "skin")
                                                {
                                                    skin = __oc.InnerText;
                                                }
                                            }
                                            if (name != string.Empty && skin != string.Empty)
                                            {
                                                if (ConfigOP.m_mapProps.Count < GlobalDefine.MAX_PROPS_COUNT)
                                                {
                                                    ConfigOP.m_mapProps[name] = skin;
                                                }
                                                else
                                                {
                                                    Debug.LogError(string.Format("ConfigOP - LoadConfig - Props Count Too Many! {0}", GlobalDefine.MAX_PROPS_COUNT));
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                            case "role":
                                {
                                    //角色
                                    XmlNodeList roleChild = gc.ChildNodes;
                                    foreach (XmlNode _rc in roleChild)
                                    {
                                        if (_rc.Name == "option")
                                        {
                                            XmlNodeList optionChild = _rc.ChildNodes;
                                            string name = string.Empty, skin = string.Empty;
                                            foreach (XmlNode __oc in optionChild)
                                            {
                                                if (__oc.Name == "name")
                                                {
                                                    name = __oc.InnerText;
                                                }
                                                else if (__oc.Name == "skin")
                                                {
                                                    skin = __oc.InnerText;
                                                }
                                            }
                                            if (name != string.Empty && skin != string.Empty)
                                            {
                                                if (ConfigOP.m_mapRole.Count < GlobalDefine.MAX_ROLE_COUNT)
                                                {
                                                    ConfigOP.m_mapRole[name] = skin;
                                                }
                                                else
                                                {
                                                    Debug.LogError(string.Format("ConfigOP - LoadConfig - Role Count Too Many! {0}", GlobalDefine.MAX_ROLE_COUNT));
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                        }
                    }
                }
            }
        }

        reader.Close();
    }

    //保存旧版配置(暂时停用)
    public static void SaveConfig()
    {
        string strPrefix = ConfigOP.getPathPrefix();
        string filePath = strPrefix + GlobalDefine.PATH_CONFIG_LEVEL_EDITOR;

        string xml = string.Empty;
        xml += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        xml += "<root>";
        xml += "<client>1</client>";
        xml += "<game gameId=\"-99\">";

        //写入元素配置
        xml += "<element>";
        foreach (var kv in ConfigOP.m_mapElement)
        {
            xml += "<option>";
            xml += "<name>" + kv.Key + "</name>";
            xml += "<skin>" + kv.Value + "</skin>";
            xml += "</option>";
        }
        xml += "</element>";

        //写入道具配置
        xml += "<props>";
        foreach (var kv in ConfigOP.m_mapProps)
        {
            xml += "<option>";
            xml += "<name>" + kv.Key + "</name>";
            xml += "<skin>" + kv.Value + "</skin>";
            xml += "</option>";
        }
        xml += "</props>";

        //写入角色配置
        xml += "<role>";
        foreach (var kv in ConfigOP.m_mapRole)
        {
            xml += "<option>";
            xml += "<name>" + kv.Key + "</name>";
            xml += "<skin>" + kv.Value + "</skin>";
            xml += "</option>";
        }
        xml += "</role>";

        xml += "</game>";
        xml += "</root>";

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        doc.Save(filePath);
    }
}

#endif