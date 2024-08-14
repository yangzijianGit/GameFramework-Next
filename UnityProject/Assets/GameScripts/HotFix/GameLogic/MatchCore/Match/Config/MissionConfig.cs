/*
 * @Description: MissionConfig
 * @Author: yangzijian
 * @Date: 2020-01-14 17:40:09
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-04-01 14:39:05
 */

namespace Config
{

    public static class MissionConfig
    {
        static string m_tMissionId = "";
        static JsonData.Stage_mission_Client.Mission m_tCacheMission;
        public static JsonData.Stage_mission_Client.Mission getMissionConfig(string strMissionId)
        {
            if (m_tMissionId == strMissionId)
            {
                return m_tCacheMission;
            }
            foreach (var tMissionConfig in JsonManager.mission_client.root.game.mission)
            {
                if (tMissionConfig.missionId == strMissionId)
                {
                    m_tCacheMission = tMissionConfig;
                    m_tMissionId = strMissionId;
                    return tMissionConfig;
                }
            }
            return null;
        }
        public static string getNextMissionId(string strMissionId)
        {
            for (int i = 0; i < JsonManager.mission_client.root.game.mission.Count; i++)
            {
                var tMissionConfig = JsonManager.mission_client.root.game.mission[i];
                if (tMissionConfig.missionId == strMissionId)
                {
                    try
                    {
                        return JsonManager.mission_client.root.game.mission[i + 1].missionId;
                    }
                    catch (System.Exception) { }
                    return "";
                }
            }
            return "";
        }

        public static bool isBossMission(string strMissionType)
        {
            return int.Parse(strMissionType) == 1;
        }

    }
}