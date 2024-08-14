/*
 * @Description: ENateAni
 * @Author: yangzijian
 * @Date: 2020-07-03 18:09:14
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-30 18:39:39
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    namespace ENateAnimation
    {
        public class ENateAni
        {
            string m_strAnimationId;
            Dictionary<int, GameObject> m_mpCreateObj;
            ENateAniArg m_tENateAniArg;
            public ENateAniArg tENateAniArg
            {
                get
                {
                    return m_tENateAniArg;
                }
            }
            JsonData.ENateAni_Config.Ani m_tConfigAni;

            List<ENateAniFrame> m_arrExcuteFrameList;
            ENateCoroutine m_tENateCoroutine = new ENateCoroutine();
            ENateAniManager m_tENateAniManager;
            float m_fTriggerTime;
            public float fTriggerTime
            {
                get
                {
                    return m_fTriggerTime;
                }
            }
            Dictionary<MonoBehaviour, Dictionary<IEnumerator, Action>> m_mpPlayer = new Dictionary<MonoBehaviour, Dictionary<IEnumerator, Action>>();

            void addPlayerCache(MonoBehaviour tPlayer, IEnumerator tCoroutine, Action pCallback)
            {
                if (m_mpPlayer.ContainsKey(tPlayer) == false)
                {
                    m_mpPlayer[tPlayer] = new Dictionary<IEnumerator, Action>();
                }
                if (m_mpPlayer[tPlayer].ContainsKey(tCoroutine) == false)
                {
                    m_mpPlayer[tPlayer] = new Dictionary<IEnumerator, Action>();
                }
                if (pCallback != null)
                {
                    m_mpPlayer[tPlayer][tCoroutine] = pCallback;
                }
            }

            void removePlayerCache(MonoBehaviour tPlayer, IEnumerator tCoroutine)
            {
                m_mpPlayer[tPlayer].Remove(tCoroutine);
                if (m_mpPlayer[tPlayer].Count <= 0)
                {
                    m_mpPlayer.Remove(tPlayer);
                }
            }

            public ENateAni(ENateAniManager tManager, JsonData.ENateAni_Config.Ani tConfigAni, ENateAniArg tENateAniArg)
            {
                m_tConfigAni = tConfigAni;
                m_tENateAniArg = tENateAniArg;
                m_mpCreateObj = new Dictionary<int, GameObject>();
                m_arrExcuteFrameList = new List<ENateAniFrame>();
                m_tENateAniManager = tManager;
                foreach (var tConfigFrame in tConfigAni.frameAni)
                {
                    var tENateAniFrame = ENateAniFrame.create(this, tConfigFrame);
                    if (tENateAniFrame == null)
                    {
                        continue;
                    }
                    m_arrExcuteFrameList.Add(tENateAniFrame);
                }
            }

            IEnumerator play(Action pCallback)
            {
                m_fTriggerTime = Time.time;
                bool bIsWaitLock = string.IsNullOrEmpty(m_tConfigAni.lockTime) == true;
                if (bIsWaitLock == false)
                {
                    float fLockTime = float.Parse(m_tConfigAni.lockTime);
                    if (fLockTime <= 0)
                    {
                        fLockTime = 0;
                    }
                    m_tENateCoroutine.Add(ENateYield.WaitForSeconds(fLockTime, pCallback));
                }
                foreach (var tFrameAni in m_arrExcuteFrameList)
                {
                    m_tENateCoroutine.Add(tFrameAni.play());
                }
                yield return m_tENateCoroutine.play();
                if (bIsWaitLock == true)
                    pCallback();
            }

            public void play(MonoBehaviour tPlayer, Action pCallback = null)
            {
                IEnumerator tCallbackValue = null;
                Action pAniPlayCallback = () =>
                {
                    if (pCallback != null)
                    {
                        pCallback();
                    }
                    removePlayerCache(tPlayer, tCallbackValue);
                };
                tCallbackValue = play(pAniPlayCallback);
                addPlayerCache(tPlayer, tCallbackValue, pCallback);
                tPlayer.StartCoroutine(tCallbackValue);
            }

            public void stop()
            {
                foreach (var tPlayerCache in m_mpPlayer)
                {
                    foreach (var tCoroutine in tPlayerCache.Value)
                    {
                        tPlayerCache.Key.StopCoroutine(tCoroutine.Key);
                        tCoroutine.Value();
                    }
                }
                m_mpPlayer.Clear();
            }

            public bool isAniPlaying()
            {
                return m_mpPlayer.Count > 0;
            }

            //////////////////////////////////////////////////////////////
            public void createObj(int nObjId, GameObject tObj)
            {
                if (tObj == null)
                {
                    return;
                }
                if (tObj.GetComponent<RectTransform>() == null)
                {
                    tObj.AddComponent<RectTransform>();
                }
                tObj.attachObj(m_tENateAniManager.gameObject);
                m_mpCreateObj.Add(nObjId, tObj);
            }

            public GameObject GetObject(int nObjId)
            {
                if (nObjId == 0)
                {
                    return m_tENateAniArg.tObj;
                }
                if (m_mpCreateObj.ContainsKey(nObjId) == true)
                {
                    return m_mpCreateObj[nObjId];
                }
                return null;
            }

            public bool delObj(int nObjId)
            {
                var tObj = GetObject(nObjId);
                if (tObj != null)
                {
                    GameObject.Destroy(tObj);
                }
                return m_mpCreateObj.Remove(nObjId);
            }

        }
    }
}