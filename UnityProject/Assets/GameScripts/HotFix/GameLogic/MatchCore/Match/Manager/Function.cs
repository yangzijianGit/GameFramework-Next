#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//公共类（无实例）
public sealed class Function
{
    /** 构造函数 **/
    private Function() { }

    //获取时间
    public static string GetTimeString()
    {
        string strTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return strTime;
    }

    //设置文本
    public static void SetText(GameObject obj, string text)
    {
        Text objText = obj.GetComponent<Text>();
        if (null == objText)
        {
            Debug.LogError(string.Format("Function - SetText - Not Found Text Component! \"{0}\"", obj.name));
            return;
        }

        objText.text = text;
    }

    //获取文本
    public static string GetText(GameObject obj)
    {
        Text objText = obj.GetComponent<Text>();
        if (null == objText)
        {
            Debug.LogError(string.Format("Function - GetText - Not Found Text Component! \"{0}\"", obj.name));
            return string.Empty;
        }

        return objText.text;
    }

    //设置编辑框文本
    public static void SetFieldText(GameObject obj, string text)
    {
        InputField objText = obj.GetComponent<InputField>();
        if (null == objText)
        {
            Debug.LogError(string.Format("Function - SetFieldText - Not Found InputField Component! \"{0}\"", obj.name));
            return;
        }

        objText.text = text;
    }

    //获取编辑框文本
    public static string GetFieldText(GameObject obj)
    {
        InputField objText = obj.GetComponent<InputField>();
        if (null == objText)
        {
            Debug.LogError(string.Format("Function - GetFieldText - Not Found InputField Component! \"{0}\"", obj.name));
            return string.Empty;
        }

        return objText.text;
    }

    //设置编辑框背景颜色
    public static void SetFieldBgColor(GameObject obj, Color rgb)
    {
        InputField objText = obj.GetComponent<InputField>();
        if (null == objText)
        {
            Debug.LogError(string.Format("Function - SetFieldBgColor - Not Found InputField Component! \"{0}\"", obj.name));
            return;
        }

        objText.targetGraphic.color = rgb;
    }

    //设置编辑框文字颜色
    public static void SetFieldTextColor(GameObject obj, Color rgb)
    {
        InputField objText = obj.GetComponent<InputField>();
        if (null == objText)
        {
            Debug.LogError(string.Format("Function - SetFieldTextColor - Not Found InputField Component! \"{0}\"", obj.name));
            return;
        }

        objText.textComponent.color = rgb;
    }

    //设置皮肤
    public static void SetSkin(GameObject obj, string path)
    {
        Image objImage = obj.GetComponent<Image>();
        if (null == objImage)
        {
            Debug.LogError(string.Format("Function - SetSkin - Not Found Image Component! \"{0}\"", obj.name));
            return;
        }

        Sprite objSprite = ResOP.LoadSprite(path);
        objImage.sprite = objSprite;
    }

    //设置皮肤
    public static void SetSkin(GameObject obj, Sprite skin)
    {
        Image objImage = obj.GetComponent<Image>();
        if (null == objImage)
        {
            Debug.LogError(string.Format("Function - SetSkin - Not Found Image Component! \"{0}\"", obj.name));
            return;
        }

        objImage.sprite = skin;
    }

    public static void setColor(GameObject obj, Color tColor)
    {
        Image objImage = obj.GetComponent<Image>();
        if (null == objImage)
        {
            Debug.LogError(string.Format("Function - SetSkin - Not Found Image Component! \"{0}\"", obj.name));
            return;
        }

        objImage.color = tColor;
    }

    //获取皮肤
    public static Sprite GetSkin(GameObject obj)
    {
        Image objImage = obj.GetComponent<Image>();
        if (null == objImage)
        {
            Debug.LogError(string.Format("Function - GetSkin - Not Found Image Component! \"{0}\"", obj.name));
            return null;
        }

        return objImage.sprite;
    }

    //清理皮肤
    public static void ClearSkin(GameObject obj)
    {
        Image objImage = obj.GetComponent<Image>();
        if (null == objImage)
        {
            Debug.LogError(string.Format("Function - ClearSkin - Not Found Image Component! \"{0}\"", obj.name));
            return;
        }

        objImage.sprite = null;
    }

    //判断字符串是否全为不可见字符
    public static bool IsNoVisible(string str)
    {
        foreach (char ch in str)
        {
            if (!(ch == ' ' || ch == '\r' || ch == '\r' || ch == '\t'))
            {
                return false;
            }
        }

        return true;
    }

    //判读字符串是否全是数字串
    public static bool IsNumber(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }

        foreach (char ch in str)
        {
            if (ch < '0' || ch > '9')
            {
                return false;
            }
        }

        return true;
    }

    //世界坐标转屏幕坐标
    public static Vector2 WorldPos2Screen(Vector3 worldPos)
    {
        Camera camera = SceneDataOP.objUICamera.GetComponent<Camera>();
        Vector3 pos = camera.WorldToScreenPoint(worldPos);

        return new Vector2(pos.x, Screen.height - pos.y);
    }

    //对象的UI屏幕尺寸
    public static Rect GameObject2Rect(GameObject obj)
    {
        RectTransform comRT = obj.GetComponent<RectTransform>();
        Vector2 size = comRT.sizeDelta;
        Camera camera = SceneDataOP.objUICamera.GetComponent<Camera>();
        Vector3 pos = camera.WorldToScreenPoint(obj.transform.position);
        Vector2 uiPOS = new Vector2(pos.x, Screen.height - pos.y);

        return new Rect(uiPOS.x - size.x / 2, uiPOS.y - size.y / 2, size.x, size.y);
    }

    //判断文件是否是PNG
    public static bool IsPNG(string path)
    {
        string strSuffix = Path.GetExtension(path);
        if (strSuffix.ToLower() == ".png")
        {
            return true;
        }

        return false;
    }

    //过滤掉精灵根路径
    public static string FilterSpritePath(string path)
    {
        string str = string.Empty;
        int index = path.IndexOf(GlobalDefine.PATH_SPRITE_ROOT);
        string fileName = Path.GetFileNameWithoutExtension(path);

        if (-1 == index)
        {
            str = Path.GetDirectoryName(path) + "/" + fileName;
        }
        else
        {
            string temp = path.Substring(index + GlobalDefine.PATH_SPRITE_ROOT.Length + 1);
            str = Path.GetDirectoryName(temp) + "/" + fileName;
        }
        str = str.Replace("\\", "/");

        return str;
    }

    //获取下拉列表选中值
    public static int GetDropdownSelect(GameObject obj)
    {
        Dropdown objDD = obj.GetComponent<Dropdown>();
        if (null == objDD)
        {
            Debug.LogError(string.Format("Function - GetDropdownSelect - Not Found Dropdown Component! \"{0}\"", obj.name));
            return -1;
        }

        return objDD.value;
    }

    //获取下拉列表选中选项内容
    public static string GetDropdownSelectOption(GameObject obj)
    {
        Dropdown objDD = obj.GetComponent<Dropdown>();
        if (null == objDD)
        {
            Debug.LogError(string.Format("Function - GetDropdownSelectOption - Not Found Dropdown Component! \"{0}\"", obj.name));
            return string.Empty;
        }

        int index = objDD.value;
        string strText = objDD.options[index].text;

        return strText;
    }

    //获取下拉列表选项数量
    public static int GetDropdownOptionCount(GameObject obj)
    {
        Dropdown objDD = obj.GetComponent<Dropdown>();
        if (null == objDD)
        {
            Debug.LogError(string.Format("Function - GetDropdownOptionCount - Not Found Dropdown Component! \"{0}\"", obj.name));
            return 0;
        }

        return objDD.options.Count;
    }

    //设置下拉列表选中值
    public static void SetDropdownSelect(GameObject obj, int value)
    {
        Dropdown objDD = obj.GetComponent<Dropdown>();
        if (null == objDD)
        {
            Debug.LogError(string.Format("Function - SetDropdownSelect - Not Found Dropdown Component! \"{0}\"", obj.name));
            return;
        }

        objDD.value = value;
    }

    //添加下拉列表选项值
    public static void AddDropdownOption(GameObject obj, string text)
    {
        Dropdown objDD = obj.GetComponent<Dropdown>();
        if (null == objDD)
        {
            Debug.LogError(string.Format("Function - AddDropdownOption - Not Found Dropdown Component! \"{0}\"", obj.name));
            return;
        }

        Dropdown.OptionData opt = new Dropdown.OptionData(text);
        List<Dropdown.OptionData> optList = new List<Dropdown.OptionData>();
        optList.Add(opt);
        objDD.AddOptions(optList);
    }

    //清空下拉列表选项值
    public static void ClearDropdownOption(GameObject obj)
    {
        Dropdown objDD = obj.GetComponent<Dropdown>();
        if (null == objDD)
        {
            Debug.LogError(string.Format("Function - ClearDropdownOption - Not Found Dropdown Component! \"{0}\"", obj.name));
            return;
        }

        objDD.ClearOptions();
    }

    //设置下拉列表状态
    public static void SetDropdownStatus(GameObject obj, bool status)
    {
        Dropdown objDD = obj.GetComponent<Dropdown>();
        if (null == objDD)
        {
            Debug.LogError(string.Format("Function - AddDropdownOption - Not Found Dropdown Component! \"{0}\"", obj.name));
            return;
        }

        objDD.enabled = status;
    }

    //设置卷动列偏移量
    public static void SetScrollBarOffset(GameObject obj, float value)
    {
        if (value < 0 || value > 1.0f)
        {
            Debug.LogError(string.Format("Function - SetScrollBarOffset - Offset Value Error! \"{0}\"", value));
            return;
        }

        Scrollbar objSB = obj.GetComponent<Scrollbar>();
        if (null == objSB)
        {
            Debug.LogError(string.Format("Function - SetScrollBarOffset - Not Found Scrollbar Component! \"{0}\"", obj.name));
            return;
        }

        objSB.value = value;
    }

    //设置对象事件穿透状态
    public static void SetObjPenetrateStatus(GameObject obj, bool status)
    {
        Image objImage = obj.GetComponent<Image>();
        if (null == objImage)
        {
            Debug.LogError(string.Format("Function - SetObjPenetrateStatus - Not Found Image Component! \"{0}\"", obj.name));
            return;
        }

        objImage.raycastTarget = status;
    }

    //设置颜色
    public static void SetObjColor(GameObject obj, int i)
    {
        Image objImage = obj.GetComponent<Image>();
        if (null == objImage)
        {
            Debug.LogError(string.Format("Function - SetObjColor - Not Found Image Component! \"{0}\"", obj.name));
            return;
        }

        switch (i)
        {
            case 0:
                objImage.color = Color.white;
                break;
            case 1:
                objImage.color = Color.magenta;
                break;
            case 2:
                objImage.color = Color.red;
                break;
            case 3:
                objImage.color = Color.cyan;
                break;
            case 4:
                objImage.color = Color.blue;
                break;
            case 5:
                objImage.color = Color.gray;
                break;
            case 6:
                objImage.color = Color.white;
                break;
            case 7:
                objImage.color = Color.yellow;
                break;
            case 8:
                objImage.color = Color.green;
                break;
            default:
                objImage.color = Color.white;
                break;
        }
        objImage.gameObject.SetActive(true);
    }
    //获取格子方向描述
    public static void GetGridDirection(GameObject obj,int index)
    {
        Text objText = obj.GetComponent<Text>();
        switch (index)
        {
            case 0:
                objText.text = "Mine";
                break;
            case 1:
                objText.text = "UL";
                break;
            case 2:
                objText.text = "U";
                break;
            case 3:
                objText.text = "UR";
                break;
            case 4:
                objText.text = "R";
                break;
            case 5:
                objText.text = "DR";
                break;
            case 6:
                objText.text = "D";
                break;
            case 7:
                objText.text = "DL";
                break;
            case 8:
                objText.text = "L";
                break;
            default:
                objText.text = "NO";
                break;
        }
        objText.gameObject.SetActive(true);
    }
}

#endif