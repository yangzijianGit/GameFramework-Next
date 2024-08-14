/*
 * @Description: ENateAni
 * @Author: yangzijian
 * @Date: 2020-07-03 18:09:14
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-29 18:01:16
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    namespace ENateAnimation
    {

        public class ENateEventArg
        {
            public string strAnimationId
            {
                get;
                set;
            }
            public ENateAniArg tENateAniArg
            {
                get;
                set;
            }
            public Action pCallBack
            {
                get;
                set;
            }

            private bool m_bIsAddLockQueue = true;
            public bool isAddLockQueue
            {
                set
                {
                    m_bIsAddLockQueue = value;
                }
                get
                {
                    return m_bIsAddLockQueue;
                }
            }
        }
        public class ENateAniManager : MonoBehaviour
        {
            Stage m_tStage;
            string m_strAnimationId;
            float m_fDuration = -1;
            Counter m_tCounter;

            public Counter counter
            {
                get
                {
                    return m_tCounter;
                }
            }
            jc.EventManager.EventObj m_tEventObj = new jc.EventManager.EventObj();

            List<ENateAni> m_arrENateAni = new List<ENateAni>();

            private void Awake()
            {
                m_tCounter = new Counter();
                m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Init, init_stage);
                m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_Ani_Play, event_play);
            }
            private void OnDestroy()
            {
                m_tEventObj.clear();
            }

            void addENateAni(ENateAni tENateAni)
            {
                m_arrENateAni.Add(tENateAni);
            }
            void removeENateAni(ENateAni tENateAni)
            {
                m_arrENateAni.Remove(tENateAni);
            }
            public bool isAnyAni()
            {
                return m_arrENateAni.Count > 0;
            }

            public ENateAni play(string strAnimationId, ENateAniArg tENateAniArg = null, Action pCallBack = null, bool isAddLockQueue = true)
            {
                var tConfigAni = Config.ENateAniConfig.getENateAni(strAnimationId);
                if (tConfigAni == null)
                {
                    Debug.LogError( "tConfigAni == null by ID:       "); // .MoreStringFormat(strAnimationId));
                    if (pCallBack != null) pCallBack();
                    return null;
                }
                ENateAni tENateAni = new ENateAni(this, tConfigAni, tENateAniArg);
                if (isAddLockQueue == true)
                    addENateAni(tENateAni);
                tENateAni.play(this, () =>
                {
                    if (isAddLockQueue == true)
                        removeENateAni(tENateAni);
                    if (pCallBack != null) pCallBack();
                });
                return tENateAni;
            }

            ///////////////////////////////////////////////
            public void init_stage(object o)
            {
                m_tStage = o as Stage;
            }

            void event_play(object o)
            {
                ENateEventArg tENateEventArg = o as ENateEventArg;
                play(tENateEventArg.strAnimationId, tENateEventArg.tENateAniArg, tENateEventArg.pCallBack, tENateEventArg.isAddLockQueue);
            }
        }

    }
}