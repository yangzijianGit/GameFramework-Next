/*
 * @Description: plane A beat
 * @Author: yangzijian
 * @Date: 2020-03-17 16:30:45
 * @LastEditors: yangzijian
 * @LastEditTime: 2024-08-15 02:15:35
 */

using System;
using System.Collections;
using System.Collections.Generic;
using ENate;

using UnityEngine;
using UnityEngine.EventSystems;

public class Pose_PlaneB_Beat : MonoBehaviour
{
    public enum BeatType
    {
        red,
        blue
    }

    Pose_PlaneB m_tPose;
    BeatType m_eBeatType;
    float m_fPlayTime;
    float m_fBeginTime;
    bool m_bIsOver = false;
    GameObject fx_dianji_Effect = null;

    static public Pose_PlaneB_Beat create(Pose_PlaneB tPose, BeatType eBeatType, Vector3 vWorldPosition, GameObject parent)
    {
        GameObject obj = null;
        switch (eBeatType)
        {
            case BeatType.red:
                obj = GameObject.Instantiate(tPose.fx_quan_red);
                break;
            case BeatType.blue:
                obj = GameObject.Instantiate(tPose.fx_quan_blue);
                break;
        }
        // 
        if (parent != null)
        {
            obj.attachObj(parent);
        }
        obj.transform.position = vWorldPosition;
        obj.transform.localPosition += new Vector3(40, -40, 0);
        Pose_PlaneB_Beat tPose_PlaneB_Beat = obj.GetComponent<Pose_PlaneB_Beat>();
        tPose_PlaneB_Beat.m_tPose = tPose;
        tPose_PlaneB_Beat.m_eBeatType = eBeatType;
        tPose_PlaneB_Beat.m_bIsOver = true;
        obj.SetActive(true);
        return tPose_PlaneB_Beat;
    }

    public void setTarget(float fTime)
    {
        // m_fBeginTime = Time.time;
        // m_fPlayTime = fTime;
        // var tUIParticle = gameObject.GetComponent<Framework.Components.UIParticle>();
        // if (tUIParticle == null)
        // {
        //     return;
        // }
        // ParticleSystem[] arrParticleSystems = tUIParticle.GetParticleSystems();
        // foreach (var tParticleSystem in arrParticleSystems)
        // {
        //     var tMainModule = tParticleSystem.main;
        //     tMainModule.startLifetime = fTime - Pose_PlaneA.sm_fRhythmThinkTime;
        // }
        // tUIParticle.Play();
        // m_bIsOver = false;
    }

    jc.EventManager.EventObj m_tEventObj;

    void OnEnable()
    {
        m_tEventObj = new jc.EventManager.EventObj();
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ExitStage, destroy);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Beat_Position_Notice, show_position);
        EventCast.Instance.addPointerDown(gameObject, pointerDown);
    }
    void OnDisable()
    {
        m_tEventObj.clear();
        m_tEventObj = null;
        EventCast.Instance.removePointerDown(gameObject, pointerDown);
    }

    bool check()
    {
        float fDis = Time.time - m_fBeginTime - m_fPlayTime;
        return Math.Abs(fDis) <= Pose_PlaneA.sm_fRhythmThinkTime;
    }

    void checkPlayEffect()
    {
        if (fx_dianji_Effect != null)
        {
            return;
        }
        if (check() == false)
        {
            return;
        }
        switch (m_eBeatType)
        {
            case BeatType.red:
                fx_dianji_Effect = GameObject.Instantiate(m_tPose.fx_dianji_red);
                break;
            case BeatType.blue:
                fx_dianji_Effect = GameObject.Instantiate(m_tPose.fx_dianji_blue);
                break;
        }
        fx_dianji_Effect.attachObj(gameObject);
    }

    bool operatorCheck()
    {
        float fDis = Time.time - m_fBeginTime - m_fPlayTime;
        if (Math.Abs(fDis) <= Pose_PlaneA.sm_fRhythmThinkTime)
        {
            destroySelf(true);
            return true;
        }
        return false;
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
                    tEffect = GameObject.Instantiate(m_tPose.fx_dianji_xiaosan_red);
                    break;
                case BeatType.blue:
                    tEffect = GameObject.Instantiate(m_tPose.fx_dianji_xiaosan_blue);
                    break;
            }
            #if UNITY_ANDROID || UNITY_IPHONE
            Handheld.Vibrate();
#endif
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSEPLANEA_Resonance_trigger_succeed);
        }
        else
        {
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSEPLANEA_Resonance_trigger_failed);
        }
        if (tEffect != null)
        {
            tEffect.attachObj(gameObject.transform.parent.gameObject);
            tEffect.transform.position = gameObject.transform.position;
            Timer.Schedule(1, ()=>{
                GameObject.Destroy(tEffect);
            });
        }
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

    void show_position(object o)
    {
        if (check())
        {
            m_tPose.bIsHaveBeatInArea = true;
        }
    }
    void Update()
    {
        if (m_bIsOver == true)
        {
            return;
        }
        checkPlayEffect();
        float fPercent = getPercent();
        if (fPercent >= 1.0f)
        {
            StartCoroutine(waitDestroy());
        }
    }

    public bool pointerDown(Vector3 vec)
    {
        return operatorCheck();
    }

}