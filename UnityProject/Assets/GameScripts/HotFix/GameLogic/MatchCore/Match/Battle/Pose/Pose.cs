/*
 * @Description: pose skill
 * @Author: yangzijian
 * @Date: 2020-03-02 11:41:40
 * @LastEditors: yangzijian
 * @LastEditTime: 2024-08-15 02:16:15
 */

using System;
using System.Collections;
using System.Collections.Generic;
using ENate;

using UnityEngine;

public class Pose : MonoBehaviour
{
    public GameObject Rhythm_p; /// 节奏整体面板
    public GameObject RJ_p; // 节奏判断面板
    public GameObject RJ_list; // 判断格列表

    public GameObject RK_p; // 技能准备阶段提示面板
    public GameObject RK_table; // 熊本熊的台子，有5种品质
    public GameObject RK_pic; // 熊本熊，有5种品质
    public GameObject RK_num; // 熊本熊技能的连击数量显示：{0}/{1}Hits（0/5Hits）

    public GameObject RKHit_p; // 连击数面板
    public GameObject RKHit_frame; // 背景，有5中品质
    public GameObject RKHit_result_fail; // 失败提示图片或特效
    public GameObject RKHit_result_success; // 成功提示图片或特效
    public GameObject RKHit_pic; // 熊本熊，有5种品质
    public GameObject RKHit_num; // 熊本熊技能的连击数量显示：{0}/{1}Hits（0/5Hits），失败但可以从新计数颜色改为#AC0C0C
    public GameObject RKHit_fail_text; // 失败提示文字
    public GameObject RJ_effect; // 音符节奏(渐现渐隐)提示特效，有两种优、良（快慢只分）。动画机制好了，我这可以做动画。

    public GameObject BAKumaPose_p; //技能开始特效提示
    public GameObject Hit_p; //连击提示面板
    public GameObject Hit_num; //连击数显示：{0}Hits（5Hits）

    public List<GameObject> arrPerfect;
    public List<GameObject> arrGood;

    public GameObject UI_BattleRJNotePanel;
    public GameObject UI_BattleRJNoteCell;

    JsonData.Rhythm_Config.SonglList m_tCurrentSoundConfig;
    LinkedList<JsonData.Rhythm_Config.Rhythm> m_linkRhythm = new LinkedList<JsonData.Rhythm_Config.Rhythm>();
    LinkedListNode<JsonData.Rhythm_Config.Rhythm> m_linkRhythmNode;
    int m_nMultiplier = 0;
    public Stage m_tStage;

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
    void initUI()
    {
        UI_BattleRJNoteCell.SetActive(false);
        RK_p.SetActive(false);
        RKHit_p.SetActive(false);
        Hit_p.SetActive(false);
        RKHit_result_success.SetActive(false);
        RKHit_num.SetActive(false);
        RKHit_result_fail.SetActive(false);
        RKHit_fail_text.SetActive(false);
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
    /**
     * @Author: yangzijian
     * @description: create a list of notes and initializes the positions.
     */

    void stopSound()
    {
        m_bIsStopSound = true;
        //SoundManager.Stop(m_tCurrentSoundConfig.id);
        //SoundManager.Remove(m_tCurrentSoundConfig.id);
    }

    public void OnDisable()
    {
        stopSound();
        m_tEventObj.clear();
        m_tEventObj = null;
    }

    /**
     * @Author: yangzijian
     * @description: when the board is calm, trigger detection -can trigger pose ablilty
     */
    static int sm_nInPosePercent;
    static int sm_nMisTime;
    static List<JsonData.Pose_Config.PoseWeight> sm_arrPoseWeight;
    static int sm_nTotalPower;

    public static void initPoseConfig()
    {
        sm_nInPosePercent = (int) (float.Parse(JsonManager.pose_config.root.game.inPosePercent) * 1000);
        sm_nMisTime = int.Parse(JsonManager.pose_config.root.game.misTime);
        sm_nTotalPower = 0;
        foreach (var tPoseWeight in JsonManager.pose_config.root.game.poseWeight)
        {
            sm_nTotalPower += int.Parse(tPoseWeight.weight);
        }
        sm_arrPoseWeight = JsonManager.pose_config.root.game.poseWeight;
    }

    public static JsonData.Pose_Config.SkillList getPoseSkillConfig(string strPoseId)
    {
        foreach (var tPoseSkill in JsonManager.pose_config.root.game.skillList)
        {
            if (tPoseSkill.index == strPoseId)
            {
                return tPoseSkill;
            }
        }
        return null;
    }
    public class PoseSkill
    {
        public Stage m_tStage;
        /**
         * @Author: yangzijian
         * @description: pose skill statue
         */
        public enum PoseSkillStatue
        {
            wait,
            Combo,
            Failed
        }
        PoseSkillStatue m_ePoseSkillStatue;
        public PoseSkillStatue ePoseSkillStatue
        {
            get
            {
                return m_ePoseSkillStatue;
            }
        }

        public void changePoseSkillStatue(PoseSkillStatue ePoseSkillStatue)
        {
            switch (ePoseSkillStatue)
            {
                case PoseSkillStatue.Combo:
                    {
                        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Combo);
                    }
                    break;
            }
            m_ePoseSkillStatue = ePoseSkillStatue;
        }

        public JsonData.Pose_Config.SkillList m_tPoseSkillConfig;

        public void triggerDetection_Calmness(object o)
        {
            if (ePoseSkillStatue == PoseSkillStatue.Combo)
            {
                return;
            }
            var lValue = Stage.m_tComputeRandom.random(0, 1000);
            /**
             * @Author: yangzijian
             * @description: detect with sm_nInPosePercent, if random number less than sm_nInPosePercent, failed and return.
             */
            if (lValue > sm_nInPosePercent)
            {
                return;
            }
            /**
             * @Author: yangzijian
             * @description: Come up with a pose skill at random.
             */
            var lRandomPower = Stage.m_tComputeRandom.random(0, sm_nTotalPower);
            string strIndex = null;
            foreach (var tPoseWeight in sm_arrPoseWeight)
            {
                lRandomPower -= int.Parse(tPoseWeight.weight);
                if (lRandomPower < 0)
                {
                    strIndex = tPoseWeight.index;
                    break;
                }
            }
            if (string.IsNullOrEmpty(strIndex))
            {
                return;
            }
            var tPoseSkillConfig = getPoseSkillConfig(strIndex);
            if (tPoseSkillConfig == null)
            {
                return;
            }
            m_tPoseSkillConfig = tPoseSkillConfig;
            changePoseSkillStatue(PoseSkillStatue.Combo);
        }

    }
    PoseSkill m_tPoseSkill;
    float m_fDuration;
    public float fDuration
    {
        get { return m_fDuration; }
    }
    float m_nWidthPerSecond;
    public float nWidthPerSecond
    {
        get { return m_nWidthPerSecond; }
    }
    float m_fWidth;
    public float fWidth
    {
        get { return m_fWidth; }
    }

    float m_fPlayTime;
    int m_nLoopTick = 0;
    public float PlayTime
    {
        get
        {
            return m_fPlayTime + m_nLoopTick * fDuration;
        }
    }
    public int nMultiplier
    {
        get
        {
            return m_nMultiplier;
        }
    }
    float m_fHalfTime;
    bool m_bIsStopSound = false;
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

    void playCallBack(object o)
    {
        // m_fDuration = //SoundManager.GetDuration(m_tCurrentSoundConfig.id);
        initRhythmList();
    }
    void init()
    {
        m_tPoseSkill = new PoseSkill() { m_tStage = m_tStage };
        var tSoundList = getSoundConfigWithMission();
        if (tSoundList == null)
        {
            return;
        }
        m_nLoopTick = 0;
        m_nWidthPerSecond = int.Parse(tSoundList.widthPerSecond);
        //SoundManager.Add(tSoundList.sound, tSoundList.id, false);
        m_bIsStopSound = false;
        //SoundManager.Play(tSoundList.id, false, playCallBack, loopMusic);
        var tRectTransform = UI_BattleRJNotePanel.GetComponent<RectTransform>();
        m_fWidth = tRectTransform.rect.width;
        m_fHalfTime = (m_fWidth / 2) / m_nWidthPerSecond;
    }

    public void initStage(object obj)
    {
        m_tStage = obj as Stage;
        init();
    }

    /**
     * @Author: yangzijian
     * @description: check to see if the next beat can be created each time.
     */
    LinkedListNode<JsonData.Rhythm_Config.Rhythm> getNextBeatGenerator()
    {
        var tRhythm = m_linkRhythmNode;
        if (tRhythm != null)
        {
            LinkedListNode<JsonData.Rhythm_Config.Rhythm> tNext = tRhythm.Next;
            if (tNext != null)
            {
                m_linkRhythmNode = tNext;
                return tNext;
            }
        }
        return null;
    }

    bool beatGeneratorAim(float fPlayTime, int nMultiplier)
    {
        float currentOkTime = fPlayTime + (m_fWidth / 2 + 10 /*fix*/ ) / m_nWidthPerSecond;
        LinkedListNode<JsonData.Rhythm_Config.Rhythm> tRhythm = m_linkRhythmNode;
        while (true)
        {
            if (tRhythm == null)
            {
                return true;
            }
            if (float.Parse(tRhythm.Value.time) < currentOkTime)
            {
                Beat.create(this, float.Parse(tRhythm.Value.time), short.Parse(tRhythm.Value.type), nMultiplier);
            }
            else
            {
                break;
            }
            tRhythm = getNextBeatGenerator();
        }
        return false;
    }
    void beatGenerator(float fPlayTime)
    {
        fPlayTime = fPlayTime - m_fDuration * m_nMultiplier;
        if (beatGeneratorAim(fPlayTime, m_nMultiplier))
        {
            initRhythmListBegin();
            ++m_nMultiplier;
            //beatGenerator(fPlayTime);
        }
    }

    void Update()
    {
        var tSoundList = getSoundConfigWithMission();
        if (tSoundList == null)
        {
            return;
        }
        // m_fPlayTime = //SoundManager.GetPlayTime(tSoundList.id);
        beatGenerator(PlayTime);
        noticeBeats();
    }

    jc.EventManager.EventObj m_tEventObj;

    void OnEnable()
    {
        m_tEventObj = new jc.EventManager.EventObj();
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Init, initStage);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_CALMNESS_PREPARE, triggerCalmness);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Combo, triggerComb);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Combo_Failed, triggerComboFailed);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Handle, triggerHandle);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Handle_Prepare, trggerHandleComboPrepare);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Handle_EndCheck, trggerHandleComboEnd);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Combo_Perfect, triggerComboPerfect);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Combo_Good, triggerComboGood);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_ComboSkill, triggerPoseSkill);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Beat_Position_NoticeEnd, triggerBeatAreaTips);

        initUI();
    }

    public enum ComboType
    {
        None,
        Perfect,
        Good
    }

    public ComboType checkPoint(Rect tRect)
    {
        ComboType eCombomType = ComboType.None;
        foreach (var tCombo in arrPerfect)
        {
            bool bIsIn = tCombo.GetComponent<RectTransform>().rectPosition().isIntersect(tRect);
            if (bIsIn)
            {
                eCombomType = ComboType.Perfect;
                break;
            }
        }
        foreach (var tCombo in arrGood)
        {
            bool bIsIn = tCombo.GetComponent<RectTransform>().rectPosition().isIntersect(tRect);
            if (bIsIn)
            {
                eCombomType = ComboType.Good;
                break;
            }
        }
        return eCombomType;
    }

    /**
     * @Author: yangzijian
     * @description: trigger combo state
     */
    public bool m_bIsTriggerCombo = false;
    public int m_nComboCount = 0; // comb count
    public int m_nHitComboCount = 0; // comb count
    public int m_nMisTime = 0; // number of fault tolerance

    void triggerComb(object o)
    {
        m_nComboCount = 0;
        m_nMisTime = 0;
        RK_p.SetActive(true);
        RK_num.setTextParam(m_nComboCount.ToString(), m_tPoseSkill.m_tPoseSkillConfig.comboNumber);
        Timer.Schedule(3.0f, () => { RK_p.SetActive(false); });
    }

    void triggerCalmness(object o)
    {
        m_tPoseSkill.triggerDetection_Calmness(o);
    }
    void triggerHandle(object o)
    {
        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_Handle_Prepare);
        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_Handling);
        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_Handle_EndCheck);
    }

    void trggerHandleComboPrepare(object o)
    {
        m_bIsTriggerCombo = false;
    }
    void trggerHandleComboEnd(object o)
    {
        if (m_bIsTriggerCombo == false)
        {
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Combo_Failed);
        }
    }
    /**
     * @Author: yangzijian
     * @description: trigger combo failure state
     */

    void showHit()
    {
        Hit_p.SetActive(true);
        Hit_num.setTextParam(m_nHitComboCount.ToString());
        Timer.Schedule(3.0f, () => { Hit_p.SetActive(false); });
    }

    void showSucceedUI()
    {
        #if UNITY_ANDROID || UNITY_IPHONE
        Handheld.Vibrate();
#endif
        RKHit_p.SetActive(true);
        RKHit_result_fail.SetActive(false);
        RKHit_fail_text.SetActive(false);
        RKHit_result_success.SetActive(true);
        RKHit_num.SetActive(true);
        RKHit_num.setTextParam(m_nComboCount.ToString(), m_tPoseSkill.m_tPoseSkillConfig.comboNumber);

        Timer.Schedule(3.0f, () => { RKHit_p.SetActive(false); });
    }

    void showFailedUI()
    {
        RKHit_p.SetActive(true);
        RKHit_result_fail.SetActive(true);
        RKHit_fail_text.SetActive(true);
        RKHit_result_success.SetActive(false);
        RKHit_num.SetActive(false);
        Timer.Schedule(3.0f, () => { RKHit_p.SetActive(false); });
    }

    void checkMisCount()
    {
        if (m_nMisTime >= sm_nMisTime)
        {
            m_nComboCount = 0;
            m_nMisTime = 0;
            m_tPoseSkill.changePoseSkillStatue(PoseSkill.PoseSkillStatue.wait);
        }
    }
    void triggerComboFailed(object o)
    {
        m_nHitComboCount = 0;
        showHit();
        m_nMisTime++;
        showFailedUI();
        checkMisCount();
    }

    void checkComboCount()
    {
        if (m_nComboCount >= int.Parse(m_tPoseSkill.m_tPoseSkillConfig.comboNumber))
        {
            // trigger success.
            jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_ComboSkill);
        }
    }

    void triggerComboPerfect(object o)
    {
        m_nHitComboCount++;
        m_bIsTriggerCombo = true;
        showHit();
        if (m_tPoseSkill.ePoseSkillStatue != PoseSkill.PoseSkillStatue.Combo)
        {
            return;
        }
        m_nComboCount++;
        showSucceedUI();
        checkComboCount();
    }

    void triggerComboGood(object o)
    {
        m_nHitComboCount++;
        m_bIsTriggerCombo = true;
        showHit();
        if (m_tPoseSkill.ePoseSkillStatue != PoseSkill.PoseSkillStatue.Combo)
        {
            return;
        }
        m_nComboCount++;
        showSucceedUI();
        checkComboCount();
    }

    void triggerPoseSkill(object o)
    {
        m_nComboCount = 0;
        m_nMisTime = 0;
        m_tPoseSkill.changePoseSkillStatue(PoseSkill.PoseSkillStatue.wait);
        BAKumaPose_p.SetActive(true);
        Timer.Schedule(1.0f, () =>
        {
            BAKumaPose_p.SetActive(false);
        });
        foreach (var tSkillConfig in m_tPoseSkill.m_tPoseSkillConfig.skill)
        {
            m_tStage.m_tSkillManager.excuteSkill(m_tStage.CurrentChessBoard, tSkillConfig.id, GridCoord.NULL, null);
        }
    }

    ///////////////////////////////////
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

    public bool isBeatInArea(float fx)
    {
        if (fx > m_fWidth / 2 - 20 * 3 && fx < m_fWidth / 2 + 20 * 3)
        {
            return true;
        }
        return false;
    }
    void triggerBeatAreaTips(object o)
    {
        RJ_effect.SetActive(bIsHaveBeatInArea);
    }

    void noticeBeats()
    {
        m_bIsHaveBeatInArea = false;
        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Beat_Position_Notice);
        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Beat_Position_NoticeEnd);
    }
}