/*
 * @Description: Pose_PlaneA
 * @Author: yangzijian
 * @Date: 2020-03-16 11:09:02
 * @LastEditors: yangzijian
 * @LastEditTime: 2024-08-15 02:14:51
 */

using System;
using System.Collections;
using System.Collections.Generic;
using ENate;

using UnityEngine;

public class Pose_PlaneA : MonoBehaviour, IDisposable
{

    public GameObject fx_xin_red;
    public GameObject fx_shanguang_red;
    public GameObject fx_shanguang_red_1;

    public GameObject fx_xin_blue;
    public GameObject fx_shanguang_blue;
    public GameObject fx_shanguang_blue_1;

    public GameObject BSNSkill_pro;

    public GameObject RJ_effect;
    public GameObject BSNSkill_sign; // 音符普通状态
    public GameObject BSNSkill_sign_Light; // 为音符移动到这个地方显示0.1秒左右，并隐藏BSNSkill_sign。
    public GameObject BSNSkill_Effect; // 踩到点才显示一下0.1秒左右
    public GameObject NoticeText_p; // 消除数字提示面板，已加动画。
    public GameObject NoticeText_combo; // 提示combo数。已配置String，字符串中赋值。
    public GameObject BAKumaPose_p;

    JsonData.Rhythm_Config.SonglList m_tCurrentSoundConfig;
    public static float sm_fToleranceTime = 0;
    public static float sm_fMaxToleranceTime = 0;
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
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ADDSTEP, event_resumeMusic);

        initUI();
        m_bIsStopSound = false;
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
        BSNSkill_sign.SetActive(true);
        BSNSkill_sign_Light.SetActive(false);
        BSNSkill_Effect.SetActive(false);
        NoticeText_p.SetActive(false);
        NoticeText_combo.SetActive(false);
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
    int m_nBeatComboCount = 0;
    void BeatComboCountChange()
    {
        NoticeText_p.SetActive(false);
        NoticeText_combo.SetActive(false);
        if (m_nBeatComboCount > 1)
        {
            NoticeText_p.SetActive(true);
            NoticeText_combo.SetActive(true);
            NoticeText_combo.setTextParam(m_nBeatComboCount.ToString());
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

    void updateController()
    {
        if (Input.GetMouseButtonDown(0))
        {
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSEPLANEA_Screen_Operator);
        }
    }

    void Update()
    {
        updatePlayTime();
        updateController();
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
        sm_fMaxToleranceTime = float.Parse(JsonManager.rhythm_config.root.game.maxTolerance);
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
        if (isActiveAndEnabled == false)
        {
            return;
        }
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

    public class BeatInfo
    {
        public float m_fPlayTime;
        public Pose_PlaneA_Beat.BeatType m_eBeatType;
    }

    BeatInfo getNextRhyBeat()
    {
        float fPlayTime = PlayTime - m_fDuration * m_nMultiplier;
        float fMaxPlayTime = PlayTime - m_fDuration * m_nMultiplier + sm_fMaxToleranceTime;
        int nRhyIndex = m_nRhyIndex;
        while (true)
        {
            for (; nRhyIndex < m_tCurrentSoundConfig.rhythm.Count; nRhyIndex++)
            {
                var rhy = m_tCurrentSoundConfig.rhythm[nRhyIndex];
                float fRhyTime = float.Parse(rhy.time);
                if (fMaxPlayTime < fRhyTime)
                {
                    return null;
                }
                if (fPlayTime + sm_fToleranceTime < fRhyTime)
                {
                    m_nRhyIndex = nRhyIndex + 1;
                    return new BeatInfo()
                    {
                        m_fPlayTime = fRhyTime - fPlayTime,
                            m_eBeatType = (Pose_PlaneA_Beat.BeatType) int.Parse(rhy.type)
                    };
                }

            }
            nRhyIndex = 0;
            fPlayTime -= m_fDuration;
            fMaxPlayTime -= m_fDuration;
            m_nMultiplier++;
        }
    }

    int m_nEffectOperatorCount = 0;
    public void event_triggerOperatorSucceed(object obj)
    {
        nBeatComboCount++;
        BSNSkill_Effect.SetActive(false);
        int nEffectOperatorCount = ++m_nEffectOperatorCount;
        Timer.Schedule(this, 0.0f, () =>
        {
            if (nEffectOperatorCount != m_nEffectOperatorCount)
            {
                return;
            }
            BSNSkill_Effect.SetActive(true);
            Timer.Schedule(this, 0.1f, () =>
            {
                if (nEffectOperatorCount != m_nEffectOperatorCount)
                {
                    return;
                }
                BSNSkill_Effect.SetActive(false);
            });
        });
    }

    public void event_triggerOperatorFailed(object obj)
    {
        nBeatComboCount = 0;
        BSNSkill_Effect.SetActive(false);
        ++m_nEffectOperatorCount;
    }
    public void event_triggerEliminateElement(object obj)
    {
        if (string.IsNullOrEmpty(Data.PlayerData.Instance.CurrentClothId) == true)
        {
            return;
        }
        ElementContainer arrElement = obj as ElementContainer;
        if (arrElement == null || arrElement.Count <= 0)
        {
            return;
        }
        Element tElement = arrElement.first();
        Vector3 vec = arrElement.computerCenter();
        //LogUtil.AddLog("battle", "computerCenter "); // .MoreStringFormat(vec));
        BeatInfo tBeatInfo = getNextRhyBeat();
        if (tBeatInfo == null)
        {
            return;
        }
        Pose_PlaneA_Beat tPose_PlaneA_Beat = Pose_PlaneA_Beat.create(this, tBeatInfo.m_eBeatType, vec, gameObject);
        tPose_PlaneA_Beat.setTarget(tBeatInfo.m_fPlayTime, BSNSkill_pro.transform.position);
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
        // RJ_effect.SetActive(bIsHaveBeatInArea);
        BSNSkill_sign.SetActive(!bIsHaveBeatInArea);
        BSNSkill_sign_Light.SetActive(bIsHaveBeatInArea);
    }
    void event_resumeMusic(object o)
    {
        init();
    }
}