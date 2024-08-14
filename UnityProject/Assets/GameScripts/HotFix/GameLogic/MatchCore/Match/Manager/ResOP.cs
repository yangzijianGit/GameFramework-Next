#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;


//资源类
public sealed class ResOP
{
    /** 构造函数 **/
    private ResOP() {}


    //加载精灵资源(相对路径，不带后缀名.png)
    public static Sprite LoadSprite(string name)
    {
        string resPath = GlobalDefine.PATH_SPRITE_ROOT + "/" + name + ".png";
        Sprite objSprite = AssetDatabase.LoadAssetAtPath(resPath, typeof(Sprite)) as Sprite;

        if (null == objSprite) {
            Debug.LogError(string.Format("ResOP - LoadSprite - Load Sprite \"{0}\" Failed!", resPath));
        }

        return objSprite;
    }

    //加载预设资源(相对路径，不带后缀名.prefab)
    public static GameObject LoadPrefab(string name)
    {
        string resPath = GlobalDefine.PATH_PREFAB_ROOT + "/" + name + ".prefab";
        GameObject objPrefab = AssetDatabase.LoadAssetAtPath(resPath, typeof(GameObject)) as GameObject;
        GameObject goInstance = (GameObject)GameObject.Instantiate(objPrefab);

        if (null == goInstance) {
            Debug.Log(string.Format("ResOP - LoadPrefab - Load Prefab \"{0}\" Failed!", resPath));
        }

        return goInstance;
    }
}

#endif