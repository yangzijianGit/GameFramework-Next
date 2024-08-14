/*
 * @Description: SkillConfig
 * @Author: yangzijian
 * @Date: 2020-07-06 15:49:30
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-06 15:51:19
 */
using System;
using System.Collections.Generic;

namespace Config
{
    public static class SkillConfig
    {
        static SortedDictionary<int, List<string>> m_mpRoundEndSkillPriority; // priority : array hypotaxid

        static public void initConfig()
        {
            m_mpRoundEndSkillPriority = new SortedDictionary<int, List<string>>();
            foreach (var tPriorityNode in JsonManager.events_config.root.game.roundEndPriority.PriorityNode)
            {
                int nPriority = int.Parse(tPriorityNode.priority);
                if (m_mpRoundEndSkillPriority.ContainsKey(nPriority) == false)
                {
                    m_mpRoundEndSkillPriority.Add(nPriority, new List<string>());
                }
                m_mpRoundEndSkillPriority[nPriority].Add(tPriorityNode.elementId);
            }
        }

        static public int getFirstRondEndPriority()
        {
            int nFirstPriority = int.Parse(JsonManager.events_config.root.game.roundEndPriority.nMinPriority);
            var it = m_mpRoundEndSkillPriority.GetEnumerator();
            if (it.MoveNext() == true)
            {
                return it.Current.Key;
            }
            return nFirstPriority;
        }
        static public int getNextRoundEndPriority(int nPriority)
        {
            int nNext = -1;
            bool bIsCurrent = false;
            var tKeyCollection = m_mpRoundEndSkillPriority.Keys;
            foreach (var tKeyIt in tKeyCollection)
            {
                if (bIsCurrent == true)
                {
                    nNext = tKeyIt;
                    break;
                }
                if (tKeyIt == nPriority)
                {
                    bIsCurrent = true;
                }
            }
            return nNext;
        }

        static public List<string> getRoundEndElementId(int nPriority)
        {
            if (m_mpRoundEndSkillPriority.ContainsKey(nPriority) == true)
            {
                return m_mpRoundEndSkillPriority[nPriority];
            }
            return null;
        }

        public static List<JsonData.Events_Config.SkillNode> getDestroySkillNode(string strElement)
        {
            foreach (var tEvent in JsonManager.events_config.root.game.events)
            {
                if (tEvent.id != JsonManager.events_config.root.game.eliminate)
                {
                    continue;
                }
                foreach (var tEliminateElementEvent in tEvent.eventSkill.index)
                {
                    if (tEliminateElementEvent.strElementId == strElement)
                    {
                        return tEliminateElementEvent.skillNode;
                    }
                }

            }
            return null;
        }
        public static List<JsonData.Events_Config.SkillNode> getGeneratorSkillNode(string strHypotaxisId)
        {
            foreach (var tEvent in JsonManager.events_config.root.game.events)
            {
                if (tEvent.id != JsonManager.events_config.root.game.generate)
                {
                    continue;
                }
                foreach (var tEliminateElementEvent in tEvent.eventSkill.index)
                {
                    if (tEliminateElementEvent.strElementId == strHypotaxisId)
                    {
                        return tEliminateElementEvent.skillNode;
                    }
                }

            }
            return null;
        }
        public static List<JsonData.Events_Config.SkillNode> getRoundEndSkillNode(string strHypotaxisId)
        {
            foreach (var tEvent in JsonManager.events_config.root.game.events)
            {
                if (tEvent.id != JsonManager.events_config.root.game.roundEnd)
                {
                    continue;
                }
                foreach (var tEliminateElementEvent in tEvent.eventSkill.index)
                {
                    if (tEliminateElementEvent.strElementId == strHypotaxisId)
                    {
                        return tEliminateElementEvent.skillNode;
                    }
                }

            }
            return null;
        }

        public static List<string> getExcuteSkillId(List<JsonData.Events_Config.SkillNode> arrSkillNode, ConditionConfig.MapArg mpArg)
        {
            List<string> arrSkillId = new List<string>();
            foreach (var tSkillNode in arrSkillNode)
            {
                if (ConditionConfig.checkCondition(tSkillNode.condition, mpArg) == false)
                {
                    continue;
                }
                arrSkillId.AddRange(tSkillNode.skill);
            }
            return arrSkillId;
        }

        public static JsonData.Skill_Config.SkillList getSkillInfo(string strSkillId)
        {
            foreach (var tSkill in JsonManager.skill_config.root.game.skillList)
            {
                if (tSkill.id == strSkillId)
                {
                    return tSkill;
                }
            }
            return null;
        }

    }
}