/*
 * @Description: plane A beat
 * @Author: yangzijian
 * @Date: 2020-03-17 16:30:45
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-05-28 10:55:57
 */

using System;
using System.Collections;
using System.Collections.Generic;
using ENate;

using UnityEngine;

public class Pose_PlaneA_Beat : MonoBehaviour
{
    public enum BeatType
    {
        red,
        blue
    }

    Pose_PlaneA m_tPose;
    BeatType m_eBeatType;

    Vector3 m_tTargetPosition;
    Vector3 m_tBeginPosition;
    float m_fPlayTime;
    float m_fBeginTime;
    bool m_bIsOver = false;

    static public Pose_PlaneA_Beat create(Pose_PlaneA tPose, BeatType eBeatType, Vector3 vWorldPosition, GameObject parent)
    {
        GameObject obj = null;
        switch (eBeatType)
        {
            case BeatType.red:
                obj = GameObject.Instantiate(tPose.fx_xin_red);
                break;
            case BeatType.blue:
                obj = GameObject.Instantiate(tPose.fx_xin_blue);
                break;
        }
        // 
        if (parent != null)
        {
            obj.attachObj(parent);
        }
        obj.transform.position = vWorldPosition;
        obj.transform.localPosition += new Vector3(40, 40, 0);
        Pose_PlaneA_Beat tPose_PlaneA_Beat = obj.AddComponent<Pose_PlaneA_Beat>();
        tPose_PlaneA_Beat.m_tPose = tPose;
        tPose_PlaneA_Beat.m_eBeatType = eBeatType;
        obj.SetActive(true);
        return tPose_PlaneA_Beat;
    }

    jc.EventManager.EventObj m_tEventObj;

    void OnEnable()
    {
        m_tEventObj = new jc.EventManager.EventObj();
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSEPLANEA_Screen_Operator, operatorCheck);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ExitStage, destroy);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Beat_Position_Notice, show_position);
    }
    void OnDisable()
    {
        m_tEventObj.clear();
        m_tEventObj = null;
    }

    bool check()
    {
        float fDis = Time.time - m_fBeginTime - m_fPlayTime;
        return Math.Abs(fDis) <= Pose_PlaneA.sm_fRhythmThinkTime;
    }

    void operatorCheck(object obj = null)
    {
        float fDis = Time.time - m_fBeginTime - m_fPlayTime;
        if (Math.Abs(fDis) <= Pose_PlaneA.sm_fRhythmThinkTime)
        {
            destroySelf(true);
        }
    }
    BezierCurve m_tBezierCurve;
    public void setTarget(float fTime, Vector3 vTarget)
    {
        m_fBeginTime = Time.time;
        m_fPlayTime = fTime;
        m_tTargetPosition = vTarget;
        m_tBeginPosition = transform.position;
        m_tBezierCurve = gameObject.AddComponent<BezierCurve>();
        m_tBezierCurve.Init(m_tBeginPosition, m_tTargetPosition);
        m_tBezierCurve.AddPoint(new Vector3(0.05f, 0.1f, 0));
    }

    void destroy(object obj)
    {
        destroySelf();
    }

    public void destroySelf(bool bIsShowWin = false)
    {
        if (gameObject == null)
        {
            return;
        }
        GameObject tEffect = null;
        if (bIsShowWin)
        {
            switch (m_eBeatType)
            {
                case BeatType.red:
                    tEffect = GameObject.Instantiate(m_tPose.fx_shanguang_red);
                    break;
                case BeatType.blue:
                    tEffect = GameObject.Instantiate(m_tPose.fx_shanguang_blue);
                    break;
            }
#if UNITY_ANDROID || UNITY_IPHONE
            Handheld.Vibrate();
#endif
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSEPLANEA_Resonance_trigger_succeed);
        }
        else
        {
            switch (m_eBeatType)
            {
                case BeatType.red:
                    tEffect = GameObject.Instantiate(m_tPose.fx_shanguang_red_1);
                    break;
                case BeatType.blue:
                    tEffect = GameObject.Instantiate(m_tPose.fx_shanguang_blue_1);
                    break;
            }
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSEPLANEA_Resonance_trigger_failed);
        }
        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_CLOTHESSKILL_POWERADD);
        tEffect.attachObj(gameObject.transform.parent.gameObject);
        tEffect.transform.position = gameObject.transform.position;
        Timer.Schedule(3.0f, () =>
        {
            GameObject.DestroyImmediate(tEffect);
        });
        GameObject.DestroyImmediate(gameObject);
    }

    IEnumerator waitDestroy()
    {
        yield return new WaitForSeconds(Pose_PlaneA.sm_fRhythmStayTime);
        destroySelf();
    }

    float getPercent()
    {
        float fPercent = (Time.time - m_fBeginTime) / m_fPlayTime;
        return fPercent > 1.0f ? 1.0f : fPercent;
    }

    void Update()
    {
        if (m_bIsOver == true)
        {
            return;
        }
        float fPercent = getPercent();
        if (fPercent >= 1.0f)
        {
            StartCoroutine(waitDestroy());
            fPercent = 1;
        }
        // transform.position = m_tBeginPosition + (m_tTargetPosition - m_tBeginPosition) * fPercent;
        transform.position = m_tBezierCurve.GetPointWorld(fPercent);
    }

    void show_position(object o)
    {

        if (check())
        {
            m_tPose.bIsHaveBeatInArea = true;
        }
    }

}