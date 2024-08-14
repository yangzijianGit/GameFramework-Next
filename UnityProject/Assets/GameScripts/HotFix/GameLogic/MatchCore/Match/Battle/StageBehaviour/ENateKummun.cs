/*
 * @Description: 
 * @Author: yangzijian
 * @Date: 2020-07-28 19:38:37
 * @LastEditors: yangzijian
 * @LastEditTime: 2024-08-15 02:08:19
 */
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
// UI_Battle界面有个熊的基本动作状态机
namespace ENate
{

    public class ENateKummun : MonoBehaviour
    {
        jc.EventManager.EventObj m_tEventObj;
        Stage m_tStage;
        string m_strDefaultAniId;
        public GameObject BSBg_p;
        int m_nRunIndex = -1;
        ConditionConfig.MapArg m_tMapArg;
        string m_strCurrentMusic;

        void playAni(string strAniId)
        {
            Util.playENateAni(strAniId, gameObject, null, null, changeDefault, false);
        }

        public static void init()
        {
            foreach (var tStageConfig in JsonManager.stage_config.root.game.stage)
            {
                //SoundManager.Add(tStageConfig.rhythm, tStageConfig.rhythm, true);
            }
        }
        void playDefaultAni(string strAniId)
        {
            m_strDefaultAniId = strAniId;
            Util.playENateAni(strAniId, gameObject);
        }

        void changeDefault()
        {
            playDefaultAni(m_strDefaultAniId);
        }

        void stopBackground()
        {
            if (string.IsNullOrEmpty(m_strCurrentMusic) == false)
            {
                //SoundManager.Stop(m_strCurrentMusic);
                m_strCurrentMusic = "";
            }
        }
        void playBackground(string strMusic)
        {
            stopBackground();
            m_strCurrentMusic = strMusic;
            //SoundManager.Play(strMusic, true);
        }

        void playContrlAni(string strControlId, string strAni)
        {
            var tChildTransform = transform.parent.Find(strControlId);
            if (tChildTransform == null)
            {
                return;
            }
            var tAniController = tChildTransform.GetComponent<Animation>();
            if (tAniController == null)
            {
                return;
            }
            tAniController.Play(strAni);
        }
        private void OnEnable()
        {
            m_tEventObj = new jc.EventManager.EventObj();
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Init, event_initStage);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_ELEMENT_ELIMINATE, event_checkStatus);
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_KUMMUN_PLAYANI, event_comboAni);
        }
        private void OnDisable()
        {
            m_tEventObj.clear();
            m_tEventObj = null;
            stopBackground();
        }
        void event_initStage(object o)
        {
            m_tStage = o as Stage;
            playDefaultAni(JsonManager.stage_config.root.game.defaultAni);
            m_nRunIndex = 0;
            m_tMapArg = new ConditionConfig.MapArg();
            m_tMapArg.Stage = m_tStage;
            m_tStage.waitCallback(0, () =>
            {
                event_checkStatus();
            });
        }

        void event_checkStatus(object o = null)
        {
            foreach (var tStageConfig in JsonManager.stage_config.root.game.stage)
            {
                int nCheckIndex = int.Parse(tStageConfig.index);
                if (nCheckIndex <= m_nRunIndex)
                {
                    continue;
                }

                bool bIsOk = ConditionConfig.checkCondition(tStageConfig.condition, m_tMapArg);
                if (bIsOk == true)
                {
                    m_nRunIndex = nCheckIndex;
                    playDefaultAni(tStageConfig.anim);
                    playBackground(tStageConfig.rhythm);
                    foreach (var tControlAni in tStageConfig.controlAni)
                    {
                        playContrlAni(tControlAni.tag, tControlAni.ctrAniId);
                    }
                }
            }
        }

        void event_comboAni(object o)
        {
            string strAni = o as string;
            playAni(strAni);
        }
    }
}