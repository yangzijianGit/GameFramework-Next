using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ENate
{
    public static class ENateYield
    {

        public static IEnumerator InternalRoutine(this IEnumerator coroutine)
        {
            bool more = true;
            while (more)
            {
                try
                {
                    more = coroutine.MoveNext();
                }
                catch (Exception ex)
                {
                    Debug.LogError( ex.ToString());
                }
                if (more)
                    yield return coroutine.Current;
            }
        }

        // public static bool MoveNext(IEnumerator subTask)
        // {
        //     bool bIsOk = subTask != null && subTask.MoveNext();
        //     bool bIsSubOk = subTask != null && subTask.Current != null && subTask.Current is IEnumerator && MoveNext(subTask.Current as IEnumerator);
        //     return bIsOk || bIsSubOk;
        // }
        public static bool MoveNext(IEnumerator subTask)
        {　　
            var child = subTask.Current;　　 //yield return另一个协程：递归MoveNext
            if (child != null && child is IEnumerator && MoveNext(child as IEnumerator))
                return true;　　　
            if (subTask.MoveNext() == true)
            {
                child = subTask.Current;
                if (child != null && child is IEnumerator)
                {
                    MoveNext(child as IEnumerator);
                }
                return true;
            }
            return false;
        }

        public static IEnumerator InternalRoutine(this List<IEnumerator> arrCoroutine)
        {
            var arrDel = new List<IEnumerator>();
            while (arrCoroutine.Count > 0)
            {
                arrDel.Clear();
                foreach (var tCoroutine in arrCoroutine)
                {
                    try
                    {
                        if (MoveNext(tCoroutine) == false)
                        {
                            arrDel.Add(tCoroutine);
                        }
                    }
                    catch (Exception ex)
                    {
                        arrDel.Add(tCoroutine);
                        Debug.LogError( ex.ToString());
                    }
                }
                foreach (var tCoroutine in arrDel)
                {
                    arrCoroutine.Remove(tCoroutine);
                }
                if (arrCoroutine.Count > 0)
                    yield return null;
            }
        }

        public static IEnumerator WaitForSeconds(float fTime, Action pCallBack = null)
        {
            float m_fReadTime = Time.time + fTime;
            while (Time.time < m_fReadTime)
            {
                yield return null;
            }
            if (pCallBack != null) pCallBack();
        }

    }
}