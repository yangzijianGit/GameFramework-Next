/*
 * @Description: skill 
 * @Author: yangzijian
 * @Date: 2020-01-09 14:40:11
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-31 11:46:46
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{

    public class SkillManager : MonoBehaviour
    {
        Stage m_tStage;
        Counter m_tExcuteCounter;
        jc.EventManager.EventObj m_tEventObj = new jc.EventManager.EventObj();
        Dictionary<int, SkillPlayer> m_mpSkillPlayer;

        void addSkillPlayer(int nSkillId, SkillPlayer tSkillPlayer)
        {
            m_mpSkillPlayer[nSkillId] = tSkillPlayer;
        }

        SkillPlayer getSkillPlayer(int nSkillId)
        {
            if (m_mpSkillPlayer.ContainsKey(nSkillId) == true)
            {
                return m_mpSkillPlayer[nSkillId];
            }
            return null;
        }
        bool delSkillPlayer(int nSkillId)
        {
            return m_mpSkillPlayer.Remove(nSkillId);
        }

        void clearSkillPlayer()
        {
            m_mpSkillPlayer.Clear();
        }

        public bool isAnyPlaying()
        {
            return m_mpSkillPlayer.Count > 0;
        }

        private void Awake()
        {
            m_tEventObj.Add((int) jc.STAGEEVENTTYPE.ET_STAGE_Init, init_stage);
            m_mpSkillPlayer = new Dictionary<int, SkillPlayer>();
        }
        private void OnDestroy()
        {
            m_tEventObj.clear();
            clearSkillPlayer();
        }
        public void init_stage(object o)
        {
            m_tStage = o as Stage;
            m_tExcuteCounter = m_tStage.m_tExcuteCounter;
        }

        void event_skillOverCallBack(int nSkillOperatorId)
        {
            delSkillPlayer(nSkillOperatorId);
        }

        public void excuteSkill(ChessBoard tChessBoard, string strSkillId, GridCoord tRootGridCoord, ConditionConfig.MapArg mpArg)
        {
            if (strSkillId == "")
            {
                return;
            }
            var tSkillInfo = Config.SkillConfig.getSkillInfo(strSkillId);
            if (tSkillInfo == null)
            {
                return;
            }
            int nSkillOperatorId = m_tExcuteCounter.count();
            var tSkillPlayer = new SkillPlayer(nSkillOperatorId, tSkillInfo, tChessBoard, tRootGridCoord);
            addSkillPlayer(nSkillOperatorId, tSkillPlayer);
            StartCoroutine(tSkillPlayer.play(tRootGridCoord, mpArg, event_skillOverCallBack));
        }

        public void excuteSkill(ChessBoard tChessBoard, List<string> arrSkillId, GridCoord tRootGridCoord, ConditionConfig.MapArg mpArg)
        {
            if (arrSkillId == null)
            {
                return;
            }
            foreach (var strSkill in arrSkillId)
            {
                excuteSkill(tChessBoard, strSkill, tRootGridCoord, mpArg);
            }
        }
        public void excuteSkill(ChessBoard tChessBoard, JsonData.Events_Config.SkillNode tSkillNode, GridCoord tRootGridCoord, ConditionConfig.MapArg mpArg)
        {
            if (tSkillNode == null)
            {
                return;
            }
            if (ConditionConfig.checkCondition(tSkillNode.condition, mpArg) == false)
            {
                return;
            }
            excuteSkill(tChessBoard, tSkillNode.skill, tRootGridCoord, mpArg);
        }
        public void excuteSkill(ChessBoard tChessBoard, List<JsonData.Events_Config.SkillNode> arrSkillNode, GridCoord tRootGridCoord, ConditionConfig.MapArg mpArg)
        {
            if (arrSkillNode == null)
            {
                return;
            }
            foreach (var strSkill in arrSkillNode)
            {
                excuteSkill(tChessBoard, strSkill, tRootGridCoord, mpArg);
            }
        }
    }
}