/*
        author      :       yangzijian
        time        :       2019-12-27 11:46:09
        function    :       util
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public static partial class ExtentionMethod
{
    /// <summary>
    /// 获取子对象变换集合
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<Transform> GetChildCollection(this Transform obj)
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < obj.childCount; i++)
        {
            list.Add(obj.GetChild(i));
        }
        return list;
    }

    /// <summary>
    /// 获取子对象集合
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<GameObject> GetChildCollection(this GameObject obj)
    {
        var list = obj.transform.GetChildCollection();
        return list.ConvertAll(T => T.gameObject);
    }

    public static Transform GetRootParent(this Transform obj)
    {
        Transform Root = obj.parent;
        while (Root.parent != null)
        {
            //Root = Root.root;   //transform.root,方法可以直接获取最上父节点。
            Root = Root.parent;
        }
        return Root;
    }

    /// <summary>
    /// 把源对象身上的所有组件，添加到目标对象身上
    /// </summary>
    /// <param name="origin">源对象</param>
    /// <param name="target">目标对象</param>
    public static void CopyComponent(GameObject origin, GameObject target)
    {
        var originComs = origin.GetComponents<Component>();
        foreach (var item in originComs)
        {
            target.AddComponent(item.GetType());
        }
    }

    /// <summary>
    /// 改变游戏脚本
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    public static void ChangeScriptTo(this MonoBehaviour origin, MonoBehaviour target)
    {
        target.enabled = true;
        origin.enabled = false;
    }

    /// <summary>
    /// 从当前对象的子对象中查找，返回一个用tag做标识的活动的游戏物体的链表.如果没有找到则为空. 
    /// </summary>
    /// <param name="obj">对象Transform</param>
    /// <param name="tag">标签</param>
    /// <param name="transList">结果Transform集合</param> // 对一个父对象进行递归遍历，如果有子对象的tag和给定tag相符合时，则把该子对象存到 链表数组中
    public static void FindGameObjectsWithTagRecursive(this Transform obj, string tag, ref List<Transform> transList)
    {
        foreach (var item in obj.transform.GetChildCollection())
        {
            // 如果子对象还有子对象，则再对子对象的子对象进行递归遍历
            if (item.childCount > 0)
            {
                item.FindGameObjectsWithTagRecursive(tag, ref transList);
            }

            if (item.tag == tag)
            {
                transList.Add(item);
            }
        }
    }

    public static void FindGameObjectsWithTagRecursive(this GameObject obj, string tag, ref List<GameObject> objList)
    {
        List<Transform> list = new List<Transform>();
        obj.transform.FindGameObjectsWithTagRecursive(tag, ref list);

        objList.AddRange(list.ConvertAll(T => T.gameObject));
    }

    /// <summary>
    /// 从父对象中查找组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="com">物体组件</param>
    /// <param name="parentLevel">向上查找的级别，使用 1 表示与本对象最近的一个级别</param>
    /// <param name="searchDepth">查找深度</param>
    /// <returns>查找成功返回相应组件对象，否则返回null</returns>
    public static T GetComponentInParent<T>(this Component com, int parentLevel = 1, int searchDepth = int.MaxValue) where T : Component
    {
        searchDepth--;

        if (com != null && searchDepth > 0)
        {
            var component = com.transform.parent.GetComponent<T>();
            if (component != null)
            {
                parentLevel--;
                if (parentLevel == 0)
                {
                    return component;
                }
            }

            return com.transform.parent.GetComponentInParent<T>(parentLevel, searchDepth);
        }

        return null;
    }

    public static void attachTransform(this Transform transform, Transform transformParent)
    {
        var scale = transform.localScale;
        var rotation = transform.localRotation;
        transform.SetParent(transformParent);
        transform.localScale = scale;
        transform.localPosition = Vector3.zero;
        transform.localRotation = rotation;
    }

    public static void attachRectTransform(this RectTransform rectTransform, Transform rectTransformParent)
    {
        var scale = rectTransform.localScale;
        var rotation = rectTransform.localRotation;
        rectTransform.SetParent(rectTransformParent);
        rectTransform.localScale = scale;
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.localRotation = rotation;
    }
    public static void attachObj(this GameObject obj, GameObject parent)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            attachTransform(obj.transform, parent.transform);
        }
        else
        {
            attachRectTransform(rectTransform, parent.transform);
        }
    }
    public static void setText(this GameObject obj, string strKey, params string[] arr)
    {
        // MultiLanguage tMultiLanguage = obj.GetComponent<MultiLanguage>();
        // if (tMultiLanguage != null)
        // {
        //     tMultiLanguage.setText(strKey, arr);
        //     return;
        // }
        // Text tText = obj.GetComponent<Text>();
        // if (tText == null)
        // {
        //     return;
        // }
        // string strText = jc.LanguageManager.Instance.GetLanguage(strKey, arr);
        // if (strText == null)
        // {
        //     strText = strKey;
        // }
        // tText.text = strText;
    }

    /**
     * @Author: yangzijian
     * @description: 
     * @param {type} obj have Image Component, strImagePath is releative to the offect Assert/
     * @return: 
     */
    public static void setImage(this GameObject obj, string strImagePath)
    {
        var tImage = obj.GetComponent<Image>();
        if (tImage == null)
        {
            return;
        }
        jc.ResourceManager.Instance.LoadSprite(strImagePath, (Sprite tSprite) =>
        {
            if (tSprite == null)
            {
                return;
            }
            tImage.sprite = tSprite;
        });

    }

    public static void setTextParam(this GameObject obj, params string[] arr)
    {
        // MultiLanguage tMultiLanguage = obj.GetComponent<MultiLanguage>();
        // if (tMultiLanguage != null)
        // {
        //     tMultiLanguage.setParam(arr);
        //     return;
        // }
        // Text tText = obj.GetComponent<Text>();
        // if (tText == null)
        // {
        //     return;
        // }
        // string strText = jc.LanguageManager.Instance.GetLanguage(tText.text, arr);
        // if (strText == null)
        // {
        //     return;
        // }
        // tText.text = strText;
    }

    public static void ClearChild(this GameObject obj, int nBeginIndex = 0)
    {
        for (; nBeginIndex < obj.transform.childCount; nBeginIndex++)
        {
            GameObject.Destroy(obj.transform.GetChild(nBeginIndex).gameObject);
        }
    }

    public static Rect rectPosition(this RectTransform tTransform)
    {
        return new Rect(tTransform.anchoredPosition, tTransform.rect.size);
    }

    public static bool isIntersect(this Rect rect1, Rect rect2)
    {
        return !(rect1.x > rect2.x + rect2.width || rect1.y > rect2.y + rect2.height || rect2.x > rect1.x + rect1.width || rect2.y > rect1.y + rect1.height);
    }

    public static List<T> cloneSelf<T>(this List<T> arrList)
    {
        var arrRList = new List<T>();
        arrList.ForEach(i => arrRList.Add(i));
        return arrRList;
    }

    /// <summary>
    /// 判断输入的字符串只包含汉字
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsChineseCh(string input)
    {
        Regex regex = new Regex("^[\u4e00-\u9fa5]+$");
        return regex.IsMatch(input);
    }

}
public enum IdentityResult
{
    True,
    LengthWrong,
    IsNotNumber,
    ProvinceWrong,
    BirthdayWrong,
    VerifyWrong,
}