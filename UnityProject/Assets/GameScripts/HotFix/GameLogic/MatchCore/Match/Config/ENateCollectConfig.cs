/*
 * @Description: ENateCollectConfig
 * @Author: yangzijian
 * @Date: 2020-07-27 16:38:44
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-27 16:56:49
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Config
{
    public static class ENateCollectConfig
    {
        public static JsonData.ENateCollect_Config.Trigger getTriggers()
        {
            return JsonManager.enatecollect_config.root.game.trigger;
        }
    }
}