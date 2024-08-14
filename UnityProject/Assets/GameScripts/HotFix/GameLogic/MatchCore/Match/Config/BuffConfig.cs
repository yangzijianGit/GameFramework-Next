/*
 * @Description: MissionConfig
 * @Author: yangzijian
 * @Date: 2020-01-14 17:40:09
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-06-11 15:47:28
 */

using System.Collections.Generic;

namespace Config
{
    public class BuffConfig
    {
        public static JsonData.Buff_Config.Buff getBuffConfig(string strBuffId)
        {
            foreach (var tBuffConfig in JsonManager.buff_config.root.game.buff)
            {
                if (tBuffConfig.id == strBuffId)
                {
                    return tBuffConfig;
                }
            }
            return null;
        }
    }
}