/*
 * @Description: SkillPlay 
 * @Author: yangzijian
 * @Date: 2020-01-09 14:40:11
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-08 11:47:26
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ENate
{
    public class SkillPlayer
    {
        Stage m_tStage;
        Dictionary<string, string> m_mpCache = new Dictionary<string, string>();
        int m_nSkillOperatorId;
        JsonData.Skill_Config.SkillList m_tSkillInfo;
        ChessBoard m_tChessBoard;
        GridCoord m_tRootGridCoord;
        float m_fExcuteBeginTime;
        public Counter m_tCounter;
        Action<int> m_pOverCallback;

        Dictionary<int, SkillGroup> m_arrSkillGroup;
        public SkillPlayer(int nSkillOperatorId, JsonData.Skill_Config.SkillList tSkillInfo, ChessBoard tChessBoard, GridCoord tRootGridCoord, Counter tCounter = null)
        {
            m_tStage = tChessBoard.m_tStage;
            m_nSkillOperatorId = nSkillOperatorId;
            m_tSkillInfo = tSkillInfo;
            m_tChessBoard = tChessBoard;
            m_tRootGridCoord = tRootGridCoord;
            m_arrSkillGroup = new Dictionary<int, SkillGroup>();
            if (tCounter == null)
            {
                m_tCounter = new Counter();
            }
            else
            {
                m_tCounter = tCounter;
            }
        }
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return m_tStage.StartCoroutine(routine);
        }

        void addOperatorCache(string strKey, string strValue)
        {
            m_mpCache[strKey] = strValue;
        }

        string getOperatorCache(int nOperatorId, string strKey)
        {
            if (m_mpCache.ContainsKey(strKey) == true)
            {
                return m_mpCache[strKey];
            }
            return "";
        }
        void event_skillGroupEndCall(int nSkillGroupId)
        {
            m_arrSkillGroup.Remove(nSkillGroupId);
            if (m_arrSkillGroup.Count <= 0)
            {
                if (m_pOverCallback != null)
                    m_pOverCallback(m_nSkillOperatorId);
            }
        }
        public IEnumerator play(GridCoord tTriggerGridCoord, ConditionConfig.MapArg mpArg, Action<int> pOverCallBack)
        {
            m_fExcuteBeginTime = Time.time;
            m_pOverCallback = pOverCallBack;
            ENateCoroutine tENateCoroutine = new ENateCoroutine();
            foreach (var tConfigSkillGroup in m_tSkillInfo.group)
            {
                int nSkillGroupId = m_tCounter.count();
                var tSkillGroup = new SkillGroup(m_nSkillOperatorId, this, nSkillGroupId, tConfigSkillGroup, m_tChessBoard, m_tRootGridCoord);
                m_arrSkillGroup.Add(nSkillGroupId, tSkillGroup);
                tENateCoroutine.Add(tSkillGroup.play(tTriggerGridCoord, mpArg, event_skillGroupEndCall));
            }
            return tENateCoroutine.play();
        }

    }
}