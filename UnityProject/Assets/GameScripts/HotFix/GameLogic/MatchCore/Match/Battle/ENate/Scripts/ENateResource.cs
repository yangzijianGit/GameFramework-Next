/*
 * @Description: 
 * @Author: yangzijian
 * @Date: 2020-01-03 15:51:19
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-03-05 16:45:32
 */
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    public static class ENateResource
    {
        static Dictionary<string, GameObject> m_mpResourcePrefabCache = new Dictionary<string, GameObject>();

        public static GameObject loadPrefab(string strPrefabPath, Action<GameObject> callback = null, bool isAsync = true)
        {
            return jc.ResourceManager.Instance.LoadPrefab(strPrefabPath, callback, isAsync);
            // GameObject obj = null;
            // try
            // {
            //     obj = m_mpResourcePrefabCache[strPrefabPath];
            //     obj = (GameObject) GameObject.Instantiate(obj);
            // }
            // catch (System.Exception)
            // {
            //     Action<GameObject> pCallback = (GameObject callBackObj) =>
            //     {
            //         if(callBackObj == null)
            //         {
            //             return;
            //         }
            //         m_mpResourcePrefabCache[strPrefabPath] = callBackObj;
            //         obj = (GameObject) GameObject.Instantiate(callBackObj);
            //         if (callback != null)
            //         {
            //             callback(obj);
            //         }
            //     };
            //     obj = jc.ResourceManager.Instance.LoadPrefab(strPrefabPath, pCallback, isAsync);
            //     if(isAsync == false)
            //     {
            //         pCallback(obj);
            //     }
            // }
            // return obj;
        }

        public static void clearAll()
        {
            foreach(var tDel in m_mpResourcePrefabCache)
            {
                GameObject.Destroy(tDel.Value);
            }
            m_mpResourcePrefabCache.Clear();
        }

    }
}