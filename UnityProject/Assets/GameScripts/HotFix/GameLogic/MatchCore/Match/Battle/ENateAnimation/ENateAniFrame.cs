/*
 * @Description: ENateAniFrame
 * @Author: yangzijian
 * @Date: 2020-07-06 10:52:24
 * @LastEditors: yangzijian
 * @LastEditTime: 2024-08-15 02:21:51
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{

    namespace ENateAnimation
    {
        public class ENateAniFrame
        {
            ENateAni m_tENateAni;
            public ENateAni tENateAni
            {
                get
                {
                    return m_tENateAni;
                }
            }
            public ENateAniFrame(ENateAni tENateAni)
            {
                m_tENateAni = tENateAni;
            }
            public float m_fTriggerTime;
            public float fTriggerTime
            {
                get
                {
                    return m_fTriggerTime;
                }
            }
            public enum ESubFramePlayType
            {
                CreateObj = 0,
                DelObj,
                SpineAni,
                Particle,
                Position,
                Count
            }
            public abstract class ISubFramePlay
            {
                ENateAniFrame m_tENateAniFrame;
                public ENateAniFrame tENateAniFrame
                {
                    get
                    {
                        return m_tENateAniFrame;
                    }
                }
                public abstract IEnumerator play();
                public abstract ESubFramePlayType getFramePlayType();
                public ISubFramePlay(ENateAniFrame tENateAniFrame)
                {
                    m_tENateAniFrame = tENateAniFrame;
                }
            }

            public class CreateObj : ISubFramePlay
            {
                public int m_nId = -1;
                public string m_strCreatePrefab;
                public int? m_nParentId;
                public CreateObj(ENateAniFrame tENateAniFrame) : base(tENateAniFrame) { }
                public void parse(JsonData.ENateAni_Config.Create tConfig)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(tConfig.objId) == true)
                        {
                            return;
                        }
                        m_nId = int.Parse(tConfig.objId);
                        m_strCreatePrefab = tConfig.prefab;
                        m_nParentId = tConfig.parent.parseInt();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError( ex.ToString());
                    }
                }
                public override IEnumerator play()
                {
                    if (string.IsNullOrEmpty(m_strCreatePrefab))
                    {
                        yield break;
                    }
                    var tCreatePrefab = jc.ResourceManager.Instance.LoadPrefab(m_strCreatePrefab, null, false);
                    if (tCreatePrefab == null)
                    {
                        yield break;
                    }
                    tENateAniFrame.tENateAni.createObj(m_nId, tCreatePrefab);
                    if (m_nParentId != null)
                    {
                        var tParent = tENateAniFrame.tENateAni.GetObject(m_nParentId.Value);
                        if (tParent != null)
                        {
                            tCreatePrefab.attachObj(tParent);
                        }
                    }
                    yield break;
                }
                public override ESubFramePlayType getFramePlayType()
                {
                    return ESubFramePlayType.CreateObj;
                }
            }

            public class DelObj : ISubFramePlay
            {
                public int m_nId = -1;
                public DelObj(ENateAniFrame tENateAniFrame) : base(tENateAniFrame) { }
                public void parse(JsonData.ENateAni_Config.Del tConfig)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(tConfig.objId) == true)
                        {
                            return;
                        }
                        m_nId = int.Parse(tConfig.objId);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError( ex.ToString());
                    }
                }
                public override IEnumerator play()
                {
                    tENateAniFrame.tENateAni.delObj(m_nId);
                    yield break;
                }
                public override ESubFramePlayType getFramePlayType()
                {
                    return ESubFramePlayType.DelObj;
                }
            }
            public class Position : ISubFramePlay
            {
                public int m_nId = -1;
                public Vector3? m_v3fWorldPosition;
                public Vector3? m_v3fPosition;
                public Vector4? m_v2fAnchors;
                public Vector2? m_v2fPivot;
                public Vector3? m_v3fScale;
                public Vector3? m_v3fRoation;
                public Bezier m_tBezier;
                public Position(ENateAniFrame tENateAniFrame) : base(tENateAniFrame) { }
                public class Bezier
                {
                    public float? m_fDuration;
                    public bool? m_bWithRotation = true;
                    ENateBezier m_tENateBezier = new ENateBezier();

                    public void parse(JsonData.ENateAni_Config.Bezier tConfig)
                    {
                        m_fDuration = tConfig.duration.parsefloat();
                        m_bWithRotation = tConfig.withRotation.parsebool();
                        if (tConfig.infPoint != null)
                        {
                            foreach (var strInfluencePoint in tConfig.infPoint)
                            {
                                var v3fInfluencePoint = strInfluencePoint.parseVector3();
                                if (v3fInfluencePoint != null)
                                {
                                    m_tENateBezier.addCtrlPoint(v3fInfluencePoint.Value);
                                }
                            }
                        }
                    }
                    public Vector3 getCurrentPosition(float fTime)
                    {
                        return m_tENateBezier.getInterpolatePos(fTime);
                    }
                }
                public void parse(JsonData.ENateAni_Config.RectTransform tConfig)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(tConfig.objId) == true)
                        {
                            return;
                        }
                        m_nId = int.Parse(tConfig.objId);
                        if (tConfig.pos == "target")
                        {
                            m_v3fWorldPosition = tENateAniFrame.tENateAni.tENateAniArg.tTriggerPos;
                        }
                        else if (tConfig.pos == "src")
                        {
                            m_v3fWorldPosition = tENateAniFrame.tENateAni.tENateAniArg.tRootPos;
                        }
                        else
                        {
                            m_v3fPosition = tConfig.pos.parseVector3();
                        }
                        m_v2fAnchors = tConfig.anchors.parseVector2();
                        m_v2fPivot = tConfig.pivot.parseVector2();
                        m_v3fScale = tConfig.scale.parseVector3();
                        m_v3fRoation = tConfig.rotation.parseVector3();
                        if (tConfig.bezier != null)
                        {
                            m_tBezier = new Bezier();
                            m_tBezier.parse(tConfig.bezier);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError( ex.ToString());
                    }
                }
                public override IEnumerator play()
                {
                    float fStartTime = tENateAniFrame.tENateAni.fTriggerTime + tENateAniFrame.fTriggerTime;
                    var tObj = tENateAniFrame.tENateAni.GetObject(m_nId);
                    if (tObj == null)
                        yield break;
                    //LogUtil.AddLog("enateAni", "Position -> tObj.name:"); // .MoreStringFormat(tObj.name));
                    var tRectTransform = tObj.GetComponent<RectTransform>();
                    //LogUtil.AddLog("enateAni", "begin tNateAniFram:"); // .MoreStringFormat(tENateAniFrame.fTriggerTime, " position :", tRectTransform.anchoredPosition3D));
                    if (m_v2fAnchors != null)
                    {
                        tRectTransform.anchorMin = new Vector2(m_v2fAnchors.Value.x, m_v2fAnchors.Value.y);
                        tRectTransform.anchorMax = new Vector2(m_v2fAnchors.Value.x, m_v2fAnchors.Value.y);
                    }
                    if (m_v2fPivot != null)
                    {
                        tRectTransform.pivot = m_v2fPivot.Value;
                    }
                    Vector3? v3fOffect_worldPosition = null;
                    Vector3? v3fOffect_SWorldPosition = null;
                    Vector3? v3fOffect_Position = null;
                    Vector3? v3fOffect_SPosition = null;
                    if (m_v3fWorldPosition != null)
                    {
                        v3fOffect_SWorldPosition = tRectTransform.position;
                        v3fOffect_worldPosition = m_v3fWorldPosition.Value - tRectTransform.position;
                    }
                    else
                    {
                        if (m_v3fPosition != null)
                        {
                            v3fOffect_SPosition = tRectTransform.anchoredPosition3D;
                            v3fOffect_Position = m_v3fPosition.Value;
                        }
                    }
                    Action pEndSet = () =>
                    {
                        if (m_v3fWorldPosition != null)
                        {
                            tRectTransform.position = m_v3fWorldPosition.Value;
                        }
                        else if (m_v3fPosition != null)
                        {
                            tRectTransform.anchoredPosition3D = v3fOffect_SPosition.Value + m_v3fPosition.Value;
                        }
                        if (m_v3fScale != null)
                        {
                            tRectTransform.localScale = m_v3fScale.Value;
                        }
                        if (m_v3fRoation != null)
                        {
                            tRectTransform.localEulerAngles = m_v3fRoation.Value;
                        }
                    };
                    float fDuration = 0;
                    if (m_tBezier != null && m_tBezier.m_fDuration != null)
                    {
                        fDuration = m_tBezier.m_fDuration.Value;
                    }
                    else { }

                    Vector3? v3fOffect_Scale = null;
                    Vector3? v3fOffect_SScale = null;
                    if (m_v3fScale != null)
                    {
                        v3fOffect_SScale = tRectTransform.localScale;
                        v3fOffect_Scale = m_v3fScale.Value - tRectTransform.localScale;
                    }
                    Vector3? v3fOffect_Roation = null;
                    Vector3? v3fOffect_SRoation = null;
                    if (m_v3fRoation != null)
                    {
                        v3fOffect_SRoation = tRectTransform.localEulerAngles;
                        v3fOffect_Roation = m_v3fRoation.Value - tRectTransform.localEulerAngles;
                    }
                    while (true)
                    {
                        if (fDuration <= 0)
                        {
                            pEndSet();
                            break;
                        }
                        float fPercent = (Time.time - fStartTime) / fDuration;
                        if (fPercent >= 1)
                        {
                            pEndSet();
                            break;
                        }
                        var v3fBezierPercent = m_tBezier.getCurrentPosition(fPercent);
                        //LogUtil.AddLog("enateAni", "fPercent:"); // .MoreStringFormat(fPercent, ",Position -> v3fBezierPercent:", v3fBezierPercent));

                        if (v3fOffect_Position != null)
                        {
                            tRectTransform.anchoredPosition3D = v3fOffect_SPosition.Value + v3fOffect_Position.Value.mulEverySub(v3fBezierPercent);
                            //LogUtil.AddLog("enateAni", "Position -> tRectTransform.anchoredPosition3D"); // .MoreStringFormat(tRectTransform.anchoredPosition3D));
                        }
                        if (v3fOffect_worldPosition != null)
                        {
                            tRectTransform.position = v3fOffect_SWorldPosition.Value + v3fOffect_worldPosition.Value.mulEverySub(v3fBezierPercent);
                            //LogUtil.AddLog("enateAni", "Position -> tRectTransform.position"); // .MoreStringFormat(tRectTransform.position));
                        }
                        if (v3fOffect_Scale != null)
                        {
                            tRectTransform.localScale = v3fOffect_SScale.Value + v3fOffect_Scale.Value * v3fBezierPercent.magnitude / Vector3.one.magnitude;
                            //LogUtil.AddLog("enateAni", "Position -> tRectTransform.localScale"); // .MoreStringFormat(tRectTransform.localScale));
                        }
                        if (v3fOffect_Roation != null)
                        {
                            tRectTransform.localEulerAngles = v3fOffect_SRoation.Value + v3fOffect_Roation.Value.mulEverySub(v3fBezierPercent);
                            //LogUtil.AddLog("enateAni", "Position -> tRectTransform.localEulerAngles"); // .MoreStringFormat(tRectTransform.localEulerAngles));
                        }
                        //LogUtil.AddLog("enateAni", "frame end local position :"); // .MoreStringFormat(tRectTransform.anchoredPosition3D));
                        yield return null;
                    }
                    //LogUtil.AddLog("battle", "end tNateAniFram:"); // .MoreStringFormat(tENateAniFrame.fTriggerTime, " position :"); // .MoreStringFormat(tRectTransform.anchoredPosition3D)));

                }
                public override ESubFramePlayType getFramePlayType()
                {
                    return ESubFramePlayType.Position;
                }
            }

            public class SpineAni : ISubFramePlay
            {
                public int m_nId = -1;
                public string m_strAniName;
                public bool m_bIsLoop = false;
                public string m_strSkeletonDataName;
                public float m_fTimeScale = 1;
                public string m_strSkin;
                public float m_fStartTime = 0;
                public Vector3? m_v3fPosition;
                public SpineAni(ENateAniFrame tENateAniFrame) : base(tENateAniFrame) { }
                public void parse(JsonData.ENateAni_Config.SpineAni tConfig)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(tConfig.objId) == true)
                        {
                            return;
                        }
                        m_nId = int.Parse(tConfig.objId);
                        m_strAniName = tConfig.aniName;
                        if (string.IsNullOrEmpty(tConfig.loop) == false)
                        {
                            m_bIsLoop = bool.Parse(tConfig.loop);
                        }
                        m_strSkeletonDataName = tConfig.skeletonDataName;
                        m_strSkin = tConfig.skin;
                        if (string.IsNullOrEmpty(tConfig.timeScale) == false)
                        {
                            m_fTimeScale = float.Parse(tConfig.timeScale);
                        }
                        m_v3fPosition = tConfig.pos.parseVector3();
                        // if (string.IsNullOrEmpty(tConfig.startTime) == false)
                        // {
                        //     m_fStartTime = float.Parse(tConfig.startTime);
                        // }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError( ex.ToString());
                    }
                }
                public override IEnumerator play()
                {
                    var tObj = tENateAniFrame.tENateAni.GetObject(m_nId);
                    if (tObj == null)
                    {
                        yield break;
                    }
                    //LogUtil.AddLog("enateAni", "play:"); // .MoreStringFormat(tObj.name, " m_nId", m_nId, " m_strAniName:", m_strAniName, " m_strSkeletonDataName:" + m_strSkeletonDataName, " m_fTimeScale:", m_fTimeScale));
                    var tSpineTransform = tObj.transform.Find("Spine");
                    GameObject tSpineObj = null;
                    if (tSpineTransform == null)
                    {
                        if (string.IsNullOrEmpty(m_strSkeletonDataName) == true)
                        {
                            Debug.LogError( "Ani Error doesn't exist spine resource :m_nId "); // .MoreStringFormat(m_nId, " m_strAniName:", m_strAniName, " m_strSkeletonDataName:" + m_strSkeletonDataName, " m_fTimeScale:", m_fTimeScale));
                            yield break;
                        }
                        else
                        {
                            tSpineObj = ENateSpine.load_SpineAni(tObj, m_strSkeletonDataName);
                        }
                    }
                    else
                    {
                        tSpineObj = tSpineTransform.gameObject;
                    }
                    // var tSpineAniGraphic = tSpineObj.GetComponent<SpineAniGraphic>();
                    // if (tSpineAniGraphic == null)
                    // {
                    //     yield break;
                    // }
                    // if (m_v3fPosition != null)
                    // {
                    //     tSpineObj.GetComponent<RectTransform>().anchoredPosition3D = m_v3fPosition.Value;
                    // }
                    // tSpineAniGraphic.SkeletonDataName = m_strSkeletonDataName;
                    // tSpineAniGraphic.Skin = m_strSkin;
                    // tSpineAniGraphic.playAni(m_strAniName, m_bIsLoop);
                    // tSpineAniGraphic.TimeScale = m_fTimeScale;
                    // tSpineAniGraphic.update(m_fStartTime);
                }
                public override ESubFramePlayType getFramePlayType()
                {
                    return ESubFramePlayType.SpineAni;
                }
            }

            public class Particle : ISubFramePlay
            {
                public int m_nId = -1;
                public int? m_nLoopCount = 1;
                public Particle(ENateAniFrame tENateAniFrame) : base(tENateAniFrame) { }
                public void parse(JsonData.ENateAni_Config.Particle tConfig)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(tConfig.objId) == true)
                        {
                            return;
                        }
                        m_nId = int.Parse(tConfig.objId);
                        m_nLoopCount = tConfig.loopCount.parseInt();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError( ex.ToString());
                    }
                }
                public override IEnumerator play()
                {
                    var tObj = tENateAniFrame.tENateAni.GetObject(m_nId);
                    if (tObj == null)
                    {
                        yield break;
                    }
                    yield break;
                }
                public override ESubFramePlayType getFramePlayType()
                {
                    return ESubFramePlayType.Particle;
                }
            }
            ISubFramePlay[] m_arrSubFramePlay;

            public void parse(JsonData.ENateAni_Config.FrameAni tConfigFrameAni)
            {
                m_fTriggerTime = float.Parse(tConfigFrameAni.triggerTime);
                m_arrSubFramePlay = new ISubFramePlay[(int) ESubFramePlayType.Count];
                if (tConfigFrameAni.rectTransform != null)
                {
                    var tPosition = new Position(this);
                    tPosition.parse(tConfigFrameAni.rectTransform);
                    m_arrSubFramePlay[(int) ESubFramePlayType.Position] = tPosition;
                }
                if (tConfigFrameAni.spineAni != null)
                {
                    var tSpineAni = new SpineAni(this);
                    tSpineAni.parse(tConfigFrameAni.spineAni);
                    m_arrSubFramePlay[(int) ESubFramePlayType.SpineAni] = tSpineAni;
                }
                if (tConfigFrameAni.particle != null)
                {
                    var tParticle = new Particle(this);
                    tParticle.parse(tConfigFrameAni.particle);
                    m_arrSubFramePlay[(int) ESubFramePlayType.Particle] = tParticle;
                }
                if (tConfigFrameAni.create != null)
                {
                    var tCreateObj = new CreateObj(this);
                    tCreateObj.parse(tConfigFrameAni.create);
                    m_arrSubFramePlay[(int) ESubFramePlayType.CreateObj] = tCreateObj;
                }
                if (tConfigFrameAni.del != null)
                {
                    var tDelObj = new DelObj(this);
                    tDelObj.parse(tConfigFrameAni.del);
                    m_arrSubFramePlay[(int) ESubFramePlayType.DelObj] = tDelObj;
                }
            }

            public static ENateAniFrame create(ENateAni tENateAni, JsonData.ENateAni_Config.FrameAni tConfigFrameAni)
            {
                try
                {
                    ENateAniFrame tENateAniFrame = new ENateAniFrame(tENateAni);
                    tENateAniFrame.parse(tConfigFrameAni);
                    return tENateAniFrame;
                }
                catch (Exception ex)
                {
                    Debug.LogError( ex.ToString());
                }
                return null;
            }

            public bool tryTrigger(float fStartDuration)
            {
                return fTriggerTime <= fStartDuration;
            }

            public IEnumerator play()
            {
                while (tryTrigger(Time.time - m_tENateAni.fTriggerTime) == false)
                {
                    yield return null;
                }
                ENateCoroutine tENateCoroutine = new ENateCoroutine();
                foreach (var tSubFramePlay in m_arrSubFramePlay)
                {
                    tENateCoroutine.Add(tSubFramePlay.play());
                }
                yield return tENateCoroutine.play();
            }
        }
    }
}