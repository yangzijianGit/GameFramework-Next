/*
 * @Description: ENateAniConfig
 * @Author: yangzijian
 * @Date: 2020-07-08 17:26:39
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-08 17:30:40
 */

namespace Config
{
    public static class ENateAniConfig
    {
        public static JsonData.ENateAni_Config.Ani getENateAni(string strAnimationId)
        {
            foreach (var tAniConfig in JsonManager.enateani_config.root.game.animation.ani)
            {
                if (tAniConfig.id == strAnimationId)
                {
                    return tAniConfig;
                }
            }
            return null;
        }

    }
}