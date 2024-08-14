/*
 * @Description: Target
 * @Author: yangzijian
 * @Date: 2020-04-10 12:11:44
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-06-23 17:39:40
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Target : MonoBehaviour
{
    float m_tBeginTime;
    float m_fFlytime;
    Vector3 m_tTarget;
    Vector3 m_tSrc;
    Action m_pCallback;
    BezierCurve m_tBezierCurve;

    static List<Target> sm_arrTargetObj = new List<Target>();

    static void addTargetObj(Target tTarget)
    {
        sm_arrTargetObj.Add(tTarget);
    }

    static void delTargetObj(Target tTarget)
    {
        sm_arrTargetObj.Remove(tTarget);
    }

    public static bool isTargetAniOver()
    {
        return sm_arrTargetObj.Count <= 0;
    }

    static public void create(string strFlyObjPath, Vector3 vWorldPosition, Vector3 vLockOffect, Vector3 vTarget, float fflyTime, Action pCallback = null, GameObject parent = null)
    {
        //LogUtil.AddLog("battle", "create        "); // .MoreStringFormat(strFlyObjPath));
        jc.ResourceManager.Instance.LoadPrefab(strFlyObjPath, (GameObject obj) =>
        {
            create(obj, vWorldPosition, vLockOffect, vTarget, fflyTime, pCallback, parent);
        }, true);
    }

    static public void create(GameObject tFlyObj, Vector3 vWorldPosition, Vector3 vLockOffect, Vector3 vTarget, float fflyTime, Action pCallback = null, GameObject parent = null)
    {
        //LogUtil.AddLog("battle", "create start        "); // .MoreStringFormat(tFlyObj.name));
        if (parent != null)
        {
            tFlyObj.attachObj(parent);
        }
        //LogUtil.AddLog("battle", "create end        "); // .MoreStringFormat(tFlyObj.name));
        tFlyObj.transform.position = vWorldPosition;
        tFlyObj.transform.localPosition += new Vector3(40, 40, 0);
        tFlyObj.SetActive(true);
        Target tTarget = tFlyObj.AddComponent<Target>();
        addTargetObj(tTarget);
        tTarget.fly(fflyTime, vTarget, pCallback);
    }

    IEnumerator flyWait()
    {
        while (true)
        {
            float fCurrentTime = Time.time;
            float fPassTime = fCurrentTime - m_tBeginTime;
            float fPercent = fPassTime / m_fFlytime;
            if (fPercent >= 1.0f)
            {
                m_pCallback();
                transform.position = m_tTarget;
                break;
            }

            transform.position = m_tBezierCurve.GetPointWorld(fPercent);
            // transform.position = (m_tTarget - m_tSrc) * fPercent + m_tSrc;
            var tCompute = (m_tTarget - m_tSrc) * fPercent + m_tSrc;
            //LogUtil.AddLog("battle", "flyWait() transform.position:"); // .MoreStringFormat(transform.position, "      tCompute       ", tCompute));
            yield return null;
        }
        delTargetObj(this);
        GameObject.Destroy(gameObject);
    }
    void fly(float fflyTime, Vector3 vTarget, Action pCallback)
    {
        m_tBeginTime = Time.time;
        m_fFlytime = fflyTime;
        m_tTarget = vTarget;
        m_tSrc = transform.position;
        m_pCallback = pCallback;
        m_tBezierCurve = gameObject.AddComponent<BezierCurve>();
        m_tBezierCurve.Init(m_tSrc, m_tTarget);
        m_tBezierCurve.AddPoint(new Vector3(0.05f, 0.1f, 0));
        StartCoroutine(flyWait());
    }

}