#if UNITY_EDITOR

using UnityEngine;


//场景数据管理(无实例)
public sealed class SceneDataOP
{
    //ui根节点，ui相机根节点 
    public static GameObject objUI = null;
    public static GameObject objUICamera = null;


    /** 构造函数 **/
    private SceneDataOP() {}


    //初始化
    public static void Init()
    {
        SceneDataOP.objUI = GameObject.Find("UI");
        SceneDataOP.objUICamera = GameObject.Find("UICamera");
    }
}

#endif