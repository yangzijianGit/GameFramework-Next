/*
 * @Description: ENateSpine
 * @Author: yangzijian
 * @Date: 2020-07-09 16:57:04
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-28 19:10:45
 */

using System;

using Spine.Unity;
using UnityEngine;

namespace ENate
{
    public class ENateSpine : MonoBehaviour
    {
        public static SkeletonDataAsset load_SkeletonDataAsset(string strSpineSkeletonDataName)
        {
            // return AssetsManager.Load<SkeletonDataAsset>("Assets/_Spine/" + strSpineSkeletonDataName + ".asset", null, false);
            return null;
        }
        public static void change_SpineAni(GameObject tObj, string strSpineSkeletonDataName)
        {
            if (tObj == null)
            {
                return;
            }
            // var tSpineAniGraphic = tObj.GetComponent<SpineAniGraphic>();
            // tSpineAniGraphic.SkeletonDataName = strSpineSkeletonDataName;
        }

        public static GameObject load_SpineAni(GameObject tObj, string strSpineSkeletonDataName)
        {
            if (tObj == null)
            {
                return null;
            }

            return null;
            // var tElementLShowObj = AssetsManager.Load<GameObject>("Assets/_Prefabs/ENate/Prefabs/Element/Battle_ElementPrefab/ElementShow.prefab", null, false);
            // var tElementShowObj = GameObject.Instantiate(tElementLShowObj);
            // tElementShowObj.name = "Spine";
            // tElementShowObj.attachObj(tObj);
            // change_SpineAni(tElementShowObj, strSpineSkeletonDataName);
            // return tElementShowObj;
        }

    }
}