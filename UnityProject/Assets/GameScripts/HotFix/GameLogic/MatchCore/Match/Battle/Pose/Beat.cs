/*
 * @Description: Beat for pose skill
 * @Author: yangzijian
 * @Date: 2020-03-03 15:11:24
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-03-05 16:28:59
 */

using System;
using System.Collections;
using System.Collections.Generic;
using ENate;

using UnityEngine;
public class Beat : MonoBehaviour, IDisposable
{
    float m_fTime;
    short m_eType;
    public short type
    {
        get
        {
            return m_eType;
        }
        set
        {
            m_eType = value;
            ui_refreshType();
        }
    }
    Pose m_tPose;
    int m_nMultiplier;
    RectTransform m_tTransform;
    CanvasGroup m_tCanvasGroup;
    public GameObject RJNote_light;
    public GameObject RJNote_weight;

    void ui_refreshType()
    {
        if (type == 0)
        {
            RJNote_light.SetActive(true);
            RJNote_weight.SetActive(false);
        }
        else
        {
            RJNote_light.SetActive(false);
            RJNote_weight.SetActive(true);
        }
    }
    /**
     * @Author: yangzijian
     * @description: 
     * @return: whether to delete itself.
     */
    public bool UpdatePosition()
    {
        float fPlayTime = m_tPose.PlayTime;
        float fX = m_tPose.fWidth / 2 + (m_fTime - (fPlayTime - m_nMultiplier * m_tPose.fDuration)) * m_tPose.nWidthPerSecond;
        m_tTransform.anchoredPosition = new Vector2(fX, m_tTransform.anchoredPosition.y);
        if (fX < -20.0f)
        {
            GameObject.Destroy(gameObject);
            return false;
        }
        return true;
    }

    void Awake()
    {
        m_tTransform = GetComponent<RectTransform>();
        m_tCanvasGroup = GetComponent<CanvasGroup>();
    }
    public void Dispose()
    {
        LogUtil.AddLog("battle", "beat destroy ... ");
    }
    jc.EventManager.EventObj m_tEventObj;

    void OnEnable()
    {
        m_tEventObj = new jc.EventManager.EventObj();
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Handling, check);
        m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Trigger_Beat_Position_Notice, show_position);
    }

    void OnDisable()
    {
        m_tEventObj.clear();
        m_tEventObj = null;
    }
    public void check(object o)
    {
        // o -> Stage
        Pose.ComboType eComboType = m_tPose.checkPoint(m_tTransform.rectPosition());
        switch (eComboType)
        {
            case Pose.ComboType.Perfect:
                {
                    jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Combo_Perfect);
                    perfect(null);
                }
                break;
            case Pose.ComboType.Good:
                {
                    jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_POSE_Combo_Good);
                    good(null);
                }
                break;
        }
    }

    void show_position(object o)
    {

        if (m_tPose.isBeatInArea(m_tTransform.anchoredPosition.x))
        {
            m_tPose.bIsHaveBeatInArea = true;
            m_tCanvasGroup.alpha = 1;
        }
        else
        {
            m_tCanvasGroup.alpha = 0.6f;
        }
    }

    void perfect(object o)
    {
        Destroy(gameObject);
    }

    void good(object o)
    {
        Destroy(gameObject);
    }
    public static void create(Pose tPose, float fTime, short nType, int nMultiplier)
    {
        var obj = GameObject.Instantiate(tPose.UI_BattleRJNoteCell);
        obj.attachObj(tPose.UI_BattleRJNotePanel);
        Beat tBeat = obj.GetComponent<Beat>();
        tBeat.m_tPose = tPose;
        tBeat.m_fTime = fTime;
        tBeat.type = nType;
        tBeat.m_nMultiplier = nMultiplier;
        obj.SetActive(true);
        tBeat.UpdatePosition();
    }

    void Update()
    {
        UpdatePosition();
    }

}