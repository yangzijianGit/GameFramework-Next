/*
 * @Description: MissionConfig
 * @Author: yangzijian
 * @Date: 2020-01-14 17:40:09
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-06-09 14:45:15
 */

using System.Collections.Generic;

namespace Config
{
    public class ClothesConfig
    {
        static Dictionary<string, string> sm_mpClothIdAIndexType;
        static public void initConfig()
        {
            sm_mpClothIdAIndexType = new Dictionary<string, string>();
            foreach (var tSkillConfig in JsonManager.clothes_config.root.game.skillType)
            {
                foreach (var strClothId in tSkillConfig.clothesGroup)
                {
                    sm_mpClothIdAIndexType.Add(strClothId, tSkillConfig.index);
                }
            }
        }
        static public string getClothSkillIndexType(string strClothId)
        {
            if (sm_mpClothIdAIndexType.ContainsKey(strClothId) == true)
            {
                return sm_mpClothIdAIndexType[strClothId];
            }
            return "";
        }
        public static JsonData.Clothes_Config.Clothes getClothesConfig(string strClothesId)
        {
            foreach (var tClothesConfig in JsonManager.clothes_config.root.game.clothes)
            {
                if (tClothesConfig.id == strClothesId)
                {
                    return tClothesConfig;
                }
            }
            return null;
        }
        public static JsonData.Clothes_Config.SkillType getClothesSkillTypeConfig(string strSkillType)
        {
            foreach (var tConfig in JsonManager.clothes_config.root.game.skillType)
            {
                if (tConfig.index == strSkillType)
                {
                    return tConfig;
                }
            }
            return null;
        }
    }
}