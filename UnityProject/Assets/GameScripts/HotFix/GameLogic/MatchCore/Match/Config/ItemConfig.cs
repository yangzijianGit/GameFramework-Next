namespace Config
{
    static public class ItemConfig
    {
        public static JsonData.Item_Config.Item getItemConfig(string strId)
        {
            foreach (var tItemConfig in JsonManager.item_config.root.game.item)
            {
                if (tItemConfig.id == strId)
                {
                    return tItemConfig;
                }
            }
            return null;
        }
    }
}