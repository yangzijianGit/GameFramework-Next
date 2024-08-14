/*
 * @Description: plane B 
 * @Author: yangzijian
 * @Date: 2020-03-19 14:53:28
 * @LastEditors: yangzijian
 * @LastEditTime: 2024-08-15 02:14:57
 */
using System;
using System.Collections;
using System.Collections.Generic;
using ENate;

using UnityEngine;

public class Pose_PlaneB : MonoBehaviour
{
    public GameObject fx_dianji_blue;
    public GameObject fx_dianji_red;
    public GameObject fx_dianji_xiaosan_blue;
    public GameObject fx_dianji_xiaosan_red;
    public GameObject fx_quan_red;
    public GameObject fx_quan_blue;
    
    
    public GameObject RJ_effect;

    public GameObject BAKumaPose_p;

    JsonData.Rhythm_Config.SonglList m_tCurrentSoundConfig;
    public static float sm_fToleranceTime = 0;
    public static float sm_fRhythmThinkTime = 0;
    public static float sm_fRhythmStayTime = 0;
    LinkedList<JsonData.Rhythm_Config.Rhythm> m_linkRhythm = new LinkedList<JsonData.Rhythm_Config.Rhythm>();
    LinkedListNode<JsonData.Rhythm_Config.Rhythm> m_linkRhythmNode;
    int m_nLoopTick = 0;
    float m_fDuration = 0;
    public float fDuration
    {
        get { return m_fDuration; }
    }
    float m_fPlayTime;
    public float PlayTime
    {
        get
        {
            return m_fPlayTime + m_nLoopTick * fDuration;
        }
    }
    int m_nRhyIndex = 0;
    int m_nMultiplier = 0;
    public int nMultiplier
    {
        get
        {
            return m_nMultiplier;
        }
    }
    bool m_bIsStopSound = false;
    public Stage m_tStage;

    jc.EventManager.EventObj m_tEventObj;
    void OnEnable()
    {
        m_tEventObj = new jc.EventManager.EventObj();
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Init, event_initStage);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE_ROUND, event_triggerEliminateElement);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSEPLANEA_Resonance_trigger_succeed, event_triggerOperatorSucceed);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSEPLANEA_Resonance_trigger_failed, event_triggerOperatorFailed);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Beat_Position_NoticeEnd, triggerBeatAreaTips);

        initUI();
    }

    void OnDisable()
    {
        m_tEventObj.clear();
        m_tEventObj = null;
        stopSound();
    }
 
    void initUI()
    {
        RJ_effect.SetActive(false);
        BAKumaPose_p.SetActive(false);
    }

    void updatePlayTime()
    {
        var tSoundList = getSoundConfigWithMission();
        if (tSoundList == null)
        {
            return;
        }
        // m_fPlayTime = //SoundManager.GetPlayTime(tSoundList.id);
    }
    bool m_bIsSucceed = false;
    int m_nBeatComboCount = 0;
    void BeatComboCountChange()
    {
        if (m_nBeatComboCount >= 3)
        {
            BAKumaPose_p.SetActive(true);
            m_nBeatComboCount = 0;
            Timer.Schedule(1.0f, () =>
            {
                BAKumaPose_p.SetActive(false);
            });
        }
    }
    int nBeatComboCount
    {
        set
        {
            m_nBeatComboCount = value;
            BeatComboCountChange();
        }
        get
        {
            return m_nBeatComboCount;
        }
    }

    void OperatorPrepare()
    {
        m_bIsSucceed = false;
    }

    void OperatorCheck()
    {
        if (m_bIsSucceed == true)
        {
            nBeatComboCount++;

        }
        else
        {
            nBeatComboCount = 0;
        }
    }

    void Update()
    {
        updatePlayTime();
        noticeBeats();
    }

    public static JsonData.Rhythm_Config.SonglList getSoundConfig(string strSoundId)
    {

        foreach (var tSongList in JsonManager.rhythm_config.root.game.songlList)
        {
            if (tSongList.id == strSoundId)
            {
                return tSongList;
            }
        }
        return null;
    }

    public static void iniiConfig()
    {
        sm_fToleranceTime = float.Parse(JsonManager.rhythm_config.root.game.tolerance);
        sm_fRhythmStayTime = float.Parse(JsonManager.rhythm_config.root.game.rhythmStayTime);
        sm_fRhythmThinkTime = float.Parse(JsonManager.rhythm_config.root.game.rhythmThinkTime);
    }

    void initRhythmListBegin()
    {
        m_linkRhythmNode = m_linkRhythm.First;
    }

    void initRhythmList()
    {
        foreach (var tRhythm in m_tCurrentSoundConfig.rhythm)
        {
            if (float.Parse(tRhythm.time) > m_fDuration)
            {
                break;
            }
            m_linkRhythm.AddLast(tRhythm);
        }
        initRhythmListBegin();
        m_nMultiplier = 0;
    }
    JsonData.Rhythm_Config.SonglList getSoundConfigWithMission()
    {
        string strSoundId = BattleArg.Instance.m_tStageArg.m_tMission.bgm;
        if (m_tCurrentSoundConfig != null)
        {
            if (m_tCurrentSoundConfig.id == strSoundId)
            {
                return m_tCurrentSoundConfig;
            }
        }
        m_tCurrentSoundConfig = getSoundConfig(strSoundId);
        return m_tCurrentSoundConfig;
    }

    void playCallBack(object o)
    {
        // m_fDuration = //SoundManager.GetDuration(m_tCurrentSoundConfig.id);
        initRhythmList();
    }

    void loopMusic(object obj)
    {
        if (this == null || m_bIsStopSound == true)
        {
            return;
        }
        var tSoundList = getSoundConfigWithMission();
        if (tSoundList == null)
        {
            return;
        }
        m_nLoopTick++;
        //SoundManager.Play(tSoundList.id, false, null, loopMusic);
    }

    void init()
    {
        var tSoundList = getSoundConfigWithMission();
        if (tSoundList == null)
        {
            return;
        }
        m_fPlayTime = 0.0f;
        m_nRhyIndex = 0;
        m_nMultiplier = 0;
        m_nLoopTick = 0;
        nBeatComboCount = 0;
        //SoundManager.Add(tSoundList.sound, tSoundList.id, false);
        m_bIsStopSound = false;
        //SoundManager.Play(tSoundList.id, false, playCallBack, loopMusic);
    }

    void stopSound()
    {
        m_bIsStopSound = true;
        //SoundManager.Stop(m_tCurrentSoundConfig.id);
        //SoundManager.Remove(m_tCurrentSoundConfig.id);
    }
    public void Dispose()
    {
        stopSound();
    }

    ///////
    public void event_initStage(object obj)
    {
        m_tStage = obj as Stage;
        init();
    }

    Vector3 computerCenter(ElementContainer arrElement)
    {
        if (arrElement.Count <= 0)
        {
            return Vector2.zero;
        }
        Vector3 tVec3 = Vector3.zero;
        foreach (var itElement in arrElement)
        {
            var tElement = itElement.Value;
            RectTransform tRectTransform = tElement.GetComponent<RectTransform>();
            tVec3 += tRectTransform.position;
        }
        return tVec3 / (float) arrElement.Count;
    }

    public class BeatInfo
    {
        public float m_fPlayTime;
        public Pose_PlaneB_Beat.BeatType m_eBeatType;
    }

    BeatInfo getNextRhyBeat()
    {
        float fPlayTime = PlayTime - m_fDuration * m_nMultiplier + sm_fToleranceTime;
        int nRhyIndex = m_nRhyIndex;
        while (true)
        {
            for (; nRhyIndex < m_tCurrentSoundConfig.rhythm.Count; nRhyIndex++)
            {
                var rhy = m_tCurrentSoundConfig.rhythm[nRhyIndex];
                float fRhyTime = float.Parse(rhy.time);
                if (fPlayTime < fRhyTime)
                {
                    m_nRhyIndex = nRhyIndex + 1;
                    return new BeatInfo()
                    {
                        m_fPlayTime = fRhyTime - fPlayTime,
                            m_eBeatType = (Pose_PlaneB_Beat.BeatType) int.Parse(rhy.type)
                    };
                }
            }
            nRhyIndex = 0;
            fPlayTime -= m_fDuration;
            m_nMultiplier++;
        }
    }
    public void event_triggerOperatorSucceed(object obj)
    {
        nBeatComboCount++;
        // m_bIsSucceed = true;
    }

    public void event_triggerOperatorFailed(object obj)
    {
        nBeatComboCount = 0;
        // m_bIsSucceed = false;
    }
    public void event_triggerEliminateElement(object obj)
    {
        ElementContainer arrElement = obj as ElementContainer;
        if (arrElement == null || arrElement.Count <= 0)
        {
            return;
        }
        Element tElement = arrElement.first();
        Vector3 vec = computerCenter(arrElement);
        //LogUtil.AddLog("battle", "computerCenter "); // .MoreStringFormat(vec));
        BeatInfo tBeatInfo = getNextRhyBeat();
        var tBeat = Pose_PlaneB_Beat.create(this, tBeatInfo.m_eBeatType, vec, gameObject);
        tBeat.setTarget(tBeatInfo.m_fPlayTime);
    }
    bool m_bIsHaveBeatInArea = false;
    public bool bIsHaveBeatInArea
    {
        set
        {
            if (m_bIsHaveBeatInArea == true)
            {
                return;
            }
            m_bIsHaveBeatInArea = value;
        }
        get
        {
            return m_bIsHaveBeatInArea;
        }
    }
    void noticeBeats()
    {
        m_bIsHaveBeatInArea = false;
        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Beat_Position_Notice);
        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Beat_Position_NoticeEnd);
    }
    void triggerBeatAreaTips(object o)
    {
        RJ_effect.SetActive(bIsHaveBeatInArea);
    }


}