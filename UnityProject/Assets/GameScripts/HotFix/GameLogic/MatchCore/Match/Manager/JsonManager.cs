using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class JsonManager
{
    public static JsonData.Stage_mission_Client.Mission_Client mission_client = null;

    public static JsonData.Stage_mission_Config.Mission_Config mission_config = null;

    public static Dictionary<string, JsonData.String_all_Language.All_Language> all_language = new Dictionary<string, JsonData.String_all_Language.All_Language>();
    private static List<string> list_all_language = new List<string>();

    public static Dictionary<string, JsonData.Ui_UIData.UIData> uidata = new Dictionary<string, JsonData.Ui_UIData.UIData>();
    private static List<string> list_uidata = new List<string>();

    public static JsonData.Active_NpcInteraction.Active_NpcInteraction active_npcinteraction = null;

    public static JsonData.Activity_Config.Activity_Config activity_config = null;

    public static JsonData.Addiction_Config.Addiction_Config addiction_config = null;

    public static JsonData.Ads_Config.Ads_Config ads_config = null;

    public static JsonData.Animation_Config.Animation_Config animation_config = null;

    public static JsonData.Assort_Config.Assort_Config assort_config = null;

    public static JsonData.Blog_Config.Blog_Config blog_config = null;

    public static JsonData.Buff_Config.Buff_Config buff_config = null;

    public static JsonData.Build.Build build = null;

    public static JsonData.Build_Appearance.Build_Appearance build_appearance = null;

    public static JsonData.Build_Buff.Build_Buff build_buff = null;

    public static JsonData.Build_Config.Build_Config build_config = null;

    public static JsonData.Build_main.Build_main build_main = null;

    public static JsonData.Build_Parade.Build_Parade build_parade = null;

    public static JsonData.Client_Config.Client_Config client_config = null;

    public static JsonData.Clothes_Config.Clothes_Config clothes_config = null;

    public static JsonData.Collect_Config.Collect_Config collect_config = null;

    public static JsonData.ComposeRules_Config.ComposeRules_Config composerules_config = null;

    public static JsonData.Country_Config.Country_Config country_config = null;

    public static JsonData.CurrencyDisplay_Config.CurrencyDisplay_Config currencydisplay_config = null;

    public static JsonData.Currency_Config.Currency_Config currency_config = null;

    public static JsonData.Drama_Config.Drama_Config drama_config = null;

    public static JsonData.DropConfig.DropConfig dropconfig = null;

    public static JsonData.Effect_Config.Effect_Config effect_config = null;

    public static JsonData.ElementBehavior_Config.ElementBehavior_Config elementbehavior_config = null;

    public static JsonData.Element_Config.Element_Config element_config = null;

    public static JsonData.Eliminate_Config.Eliminate_Config eliminate_config = null;

    public static JsonData.ENateAni_Config.ENateAni_Config enateani_config = null;

    public static JsonData.ENateCollect_Config.ENateCollect_Config enatecollect_config = null;

    public static JsonData.Events_Config.Events_Config events_config = null;

    public static JsonData.Exp_Config.Exp_Config exp_config = null;

    public static JsonData.Fever_Config.Fever_Config fever_config = null;

    public static JsonData.Gacha_Config.Gacha_Config gacha_config = null;

    public static JsonData.GoodsParts_Config.GoodsParts_Config goodsparts_config = null;

    public static JsonData.Goods_Config.Goods_Config goods_config = null;

    public static JsonData.Item_Config.Item_Config item_config = null;

    public static JsonData.Map_Config.Map_Config map_config = null;

    public static JsonData.MonthCard_Config.MonthCard_Config monthcard_config = null;

    public static JsonData.Npc_group.Npc_group npc_group = null;

    public static JsonData.ParkLv_Config.ParkLv_Config parklv_config = null;

    public static JsonData.Parts_Config.Parts_Config parts_config = null;

    public static JsonData.Path_Config.Path_Config path_config = null;

    public static JsonData.Popular_Config.Popular_Config popular_config = null;

    public static JsonData.Pose_Config.Pose_Config pose_config = null;

    public static JsonData.Postcard_Config.Postcard_Config postcard_config = null;

    public static JsonData.Postcard_fuse.Postcard_fuse postcard_fuse = null;

    public static JsonData.Quality_Config.Quality_Config quality_config = null;

    public static JsonData.Quest_Config.Quest_Config quest_config = null;

    public static JsonData.Reputation_Config.Reputation_Config reputation_config = null;

    public static JsonData.Rhythm_Config.Rhythm_Config rhythm_config = null;

    public static JsonData.Skill_Config.Skill_Config skill_config = null;

    public static JsonData.Stage_Config.Stage_Config stage_config = null;

    public static JsonData.Stamp_Config.Stamp_Config stamp_config = null;

    public static JsonData.System_Config.System_Config system_config = null;

    public static JsonData.Title_Config.Title_Config title_config = null;

    public static JsonData.Visitor_Config.Visitor_Config visitor_config = null;


    private static void register()
    {
        JsonManager.list_all_language.Add("string/string_Clothes");
        JsonManager.list_all_language.Add("string/string_System");

        JsonManager.list_uidata.Add("ui/Building/BuildingGroupPanel");
        JsonManager.list_uidata.Add("ui/Building/BuildingPanel");
        JsonManager.list_uidata.Add("ui/Building/BuildScreenClearRuin");
        JsonManager.list_uidata.Add("ui/Building/BuildScreenFunctionBtn");
        JsonManager.list_uidata.Add("ui/Building/BuildScreenGetGold");
        JsonManager.list_uidata.Add("ui/Building/BuildScreenMain");
        JsonManager.list_uidata.Add("ui/Building/BuildScreenReward");
        JsonManager.list_uidata.Add("ui/Building/BuildScreenTaskbtn");
        JsonManager.list_uidata.Add("ui/Building/BuildScreenUnlock");
        JsonManager.list_uidata.Add("ui/Building/UI_Building");
        JsonManager.list_uidata.Add("ui/Building/UI_BuildingMain");
        JsonManager.list_uidata.Add("ui/Building/UI_BuildingMainLvUp");
        JsonManager.list_uidata.Add("ui/Building/UI_BuildingPart");
        JsonManager.list_uidata.Add("ui/Building/UI_BuildingRent");
        JsonManager.list_uidata.Add("ui/Building/UI_BuildingSurface");
        JsonManager.list_uidata.Add("ui/Parade/UI_BuildingParade");
        JsonManager.list_uidata.Add("ui/Parade/UI_BuildingParadeStart");
        JsonManager.list_uidata.Add("ui/Parade/UI_BuildingParadeVehicle");
        JsonManager.list_uidata.Add("ui/UI_Anti");
        JsonManager.list_uidata.Add("ui/UI_Assort");
        JsonManager.list_uidata.Add("ui/UI_Battle");
        JsonManager.list_uidata.Add("ui/UI_BattleLostNotice");
        JsonManager.list_uidata.Add("ui/UI_BattleReady");
        JsonManager.list_uidata.Add("ui/UI_BattleReadyClothes");
        JsonManager.list_uidata.Add("ui/UI_BattleResult");
        JsonManager.list_uidata.Add("ui/UI_Bubble");
        JsonManager.list_uidata.Add("ui/UI_Energy");
        JsonManager.list_uidata.Add("ui/UI_Fusion");
        JsonManager.list_uidata.Add("ui/UI_FusionResult");
        JsonManager.list_uidata.Add("ui/UI_Gacha");
        JsonManager.list_uidata.Add("ui/UI_GachaReward");
        JsonManager.list_uidata.Add("ui/UI_Level");
        JsonManager.list_uidata.Add("ui/UI_Mail");
        JsonManager.list_uidata.Add("ui/UI_Main");
        JsonManager.list_uidata.Add("ui/UI_Park");
        JsonManager.list_uidata.Add("ui/UI_ParkLv");
        JsonManager.list_uidata.Add("ui/UI_PlotStory");
        JsonManager.list_uidata.Add("ui/UI_Postcard");
        JsonManager.list_uidata.Add("ui/UI_Quest");
        JsonManager.list_uidata.Add("ui/UI_Recharge");
        JsonManager.list_uidata.Add("ui/UI_Reward");
        JsonManager.list_uidata.Add("ui/UI_Start");
        JsonManager.list_uidata.Add("ui/UI_StartService");
        JsonManager.list_uidata.Add("ui/UI_Store");
        JsonManager.list_uidata.Add("ui/UI_SysCurrency");
        JsonManager.list_uidata.Add("ui/UI_SysTips");
        JsonManager.list_uidata.Add("ui/WorldMap_WorldMap");

    }

    public static IEnumerator Init()
    {
        yield return null;
        // JsonManager.register();

        // ResourceRequest mission_client = AssetsManager.LoadJsonResource("stage/mission_Client");
        // yield return mission_client;
        // JsonManager.mission_client = JsonUtility.FromJson<JsonData.Stage_mission_Client.Mission_Client>((mission_client.asset as TextAsset).ToString());

        // ResourceRequest mission_config = AssetsManager.LoadJsonResource("stage/mission_Config");
        // yield return mission_config;
        // JsonManager.mission_config = JsonUtility.FromJson<JsonData.Stage_mission_Config.Mission_Config>((mission_config.asset as TextAsset).ToString());

        // foreach (string fn in JsonManager.list_all_language) {
        //     ResourceRequest all_language = AssetsManager.LoadJsonResource(fn);
        //     yield return all_language;
        //     JsonData.String_all_Language.All_Language temp = null;
        //     temp = JsonUtility.FromJson<JsonData.String_all_Language.All_Language>((all_language.asset as TextAsset).ToString());
        //     JsonManager.all_language[fn] = temp;
        // }

        // foreach (string fn in JsonManager.list_uidata) {
        //     ResourceRequest uidata = AssetsManager.LoadJsonResource(fn);
        //     yield return uidata;
        //     JsonData.Ui_UIData.UIData temp = null;
        //     temp = JsonUtility.FromJson<JsonData.Ui_UIData.UIData>((uidata.asset as TextAsset).ToString());
        //     JsonManager.uidata[fn] = temp;
        // }

        // ResourceRequest active_npcinteraction = AssetsManager.LoadJsonResource("active_NpcInteraction");
        // yield return active_npcinteraction;
        // JsonManager.active_npcinteraction = JsonUtility.FromJson<JsonData.Active_NpcInteraction.Active_NpcInteraction>((active_npcinteraction.asset as TextAsset).ToString());

        // ResourceRequest activity_config = AssetsManager.LoadJsonResource("activity_Config");
        // yield return activity_config;
        // JsonManager.activity_config = JsonUtility.FromJson<JsonData.Activity_Config.Activity_Config>((activity_config.asset as TextAsset).ToString());

        // ResourceRequest addiction_config = AssetsManager.LoadJsonResource("addiction_Config");
        // yield return addiction_config;
        // JsonManager.addiction_config = JsonUtility.FromJson<JsonData.Addiction_Config.Addiction_Config>((addiction_config.asset as TextAsset).ToString());

        // ResourceRequest ads_config = AssetsManager.LoadJsonResource("ads_Config");
        // yield return ads_config;
        // JsonManager.ads_config = JsonUtility.FromJson<JsonData.Ads_Config.Ads_Config>((ads_config.asset as TextAsset).ToString());

        // ResourceRequest animation_config = AssetsManager.LoadJsonResource("animation_Config");
        // yield return animation_config;
        // JsonManager.animation_config = JsonUtility.FromJson<JsonData.Animation_Config.Animation_Config>((animation_config.asset as TextAsset).ToString());

        // ResourceRequest assort_config = AssetsManager.LoadJsonResource("assort_Config");
        // yield return assort_config;
        // JsonManager.assort_config = JsonUtility.FromJson<JsonData.Assort_Config.Assort_Config>((assort_config.asset as TextAsset).ToString());

        // ResourceRequest blog_config = AssetsManager.LoadJsonResource("blog_Config");
        // yield return blog_config;
        // JsonManager.blog_config = JsonUtility.FromJson<JsonData.Blog_Config.Blog_Config>((blog_config.asset as TextAsset).ToString());

        // ResourceRequest buff_config = AssetsManager.LoadJsonResource("buff_Config");
        // yield return buff_config;
        // JsonManager.buff_config = JsonUtility.FromJson<JsonData.Buff_Config.Buff_Config>((buff_config.asset as TextAsset).ToString());

        // ResourceRequest build = AssetsManager.LoadJsonResource("build");
        // yield return build;
        // JsonManager.build = JsonUtility.FromJson<JsonData.Build.Build>((build.asset as TextAsset).ToString());

        // ResourceRequest build_appearance = AssetsManager.LoadJsonResource("build_Appearance");
        // yield return build_appearance;
        // JsonManager.build_appearance = JsonUtility.FromJson<JsonData.Build_Appearance.Build_Appearance>((build_appearance.asset as TextAsset).ToString());

        // ResourceRequest build_buff = AssetsManager.LoadJsonResource("build_Buff");
        // yield return build_buff;
        // JsonManager.build_buff = JsonUtility.FromJson<JsonData.Build_Buff.Build_Buff>((build_buff.asset as TextAsset).ToString());

        // ResourceRequest build_config = AssetsManager.LoadJsonResource("build_Config");
        // yield return build_config;
        // JsonManager.build_config = JsonUtility.FromJson<JsonData.Build_Config.Build_Config>((build_config.asset as TextAsset).ToString());

        // ResourceRequest build_main = AssetsManager.LoadJsonResource("build_main");
        // yield return build_main;
        // JsonManager.build_main = JsonUtility.FromJson<JsonData.Build_main.Build_main>((build_main.asset as TextAsset).ToString());

        // ResourceRequest build_parade = AssetsManager.LoadJsonResource("build_Parade");
        // yield return build_parade;
        // JsonManager.build_parade = JsonUtility.FromJson<JsonData.Build_Parade.Build_Parade>((build_parade.asset as TextAsset).ToString());

        // ResourceRequest client_config = AssetsManager.LoadJsonResource("Client_Config");
        // yield return client_config;
        // JsonManager.client_config = JsonUtility.FromJson<JsonData.Client_Config.Client_Config>((client_config.asset as TextAsset).ToString());

        // ResourceRequest clothes_config = AssetsManager.LoadJsonResource("clothes_Config");
        // yield return clothes_config;
        // JsonManager.clothes_config = JsonUtility.FromJson<JsonData.Clothes_Config.Clothes_Config>((clothes_config.asset as TextAsset).ToString());

        // ResourceRequest collect_config = AssetsManager.LoadJsonResource("collect_Config");
        // yield return collect_config;
        // JsonManager.collect_config = JsonUtility.FromJson<JsonData.Collect_Config.Collect_Config>((collect_config.asset as TextAsset).ToString());

        // ResourceRequest composerules_config = AssetsManager.LoadJsonResource("composeRules_Config");
        // yield return composerules_config;
        // JsonManager.composerules_config = JsonUtility.FromJson<JsonData.ComposeRules_Config.ComposeRules_Config>((composerules_config.asset as TextAsset).ToString());

        // ResourceRequest country_config = AssetsManager.LoadJsonResource("country_Config");
        // yield return country_config;
        // JsonManager.country_config = JsonUtility.FromJson<JsonData.Country_Config.Country_Config>((country_config.asset as TextAsset).ToString());

        // ResourceRequest currencydisplay_config = AssetsManager.LoadJsonResource("currencyDisplay_Config");
        // yield return currencydisplay_config;
        // JsonManager.currencydisplay_config = JsonUtility.FromJson<JsonData.CurrencyDisplay_Config.CurrencyDisplay_Config>((currencydisplay_config.asset as TextAsset).ToString());

        // ResourceRequest currency_config = AssetsManager.LoadJsonResource("currency_Config");
        // yield return currency_config;
        // JsonManager.currency_config = JsonUtility.FromJson<JsonData.Currency_Config.Currency_Config>((currency_config.asset as TextAsset).ToString());

        // ResourceRequest drama_config = AssetsManager.LoadJsonResource("drama_Config");
        // yield return drama_config;
        // JsonManager.drama_config = JsonUtility.FromJson<JsonData.Drama_Config.Drama_Config>((drama_config.asset as TextAsset).ToString());

        // ResourceRequest dropconfig = AssetsManager.LoadJsonResource("dropConfig");
        // yield return dropconfig;
        // JsonManager.dropconfig = JsonUtility.FromJson<JsonData.DropConfig.DropConfig>((dropconfig.asset as TextAsset).ToString());

        // ResourceRequest effect_config = AssetsManager.LoadJsonResource("effect_Config");
        // yield return effect_config;
        // JsonManager.effect_config = JsonUtility.FromJson<JsonData.Effect_Config.Effect_Config>((effect_config.asset as TextAsset).ToString());

        // ResourceRequest elementbehavior_config = AssetsManager.LoadJsonResource("elementBehavior_Config");
        // yield return elementbehavior_config;
        // JsonManager.elementbehavior_config = JsonUtility.FromJson<JsonData.ElementBehavior_Config.ElementBehavior_Config>((elementbehavior_config.asset as TextAsset).ToString());

        // ResourceRequest element_config = AssetsManager.LoadJsonResource("element_Config");
        // yield return element_config;
        // JsonManager.element_config = JsonUtility.FromJson<JsonData.Element_Config.Element_Config>((element_config.asset as TextAsset).ToString());

        // ResourceRequest eliminate_config = AssetsManager.LoadJsonResource("eliminate_Config");
        // yield return eliminate_config;
        // JsonManager.eliminate_config = JsonUtility.FromJson<JsonData.Eliminate_Config.Eliminate_Config>((eliminate_config.asset as TextAsset).ToString());

        // ResourceRequest enateani_config = AssetsManager.LoadJsonResource("ENateAni_Config");
        // yield return enateani_config;
        // JsonManager.enateani_config = JsonUtility.FromJson<JsonData.ENateAni_Config.ENateAni_Config>((enateani_config.asset as TextAsset).ToString());

        // ResourceRequest enatecollect_config = AssetsManager.LoadJsonResource("ENateCollect_Config");
        // yield return enatecollect_config;
        // JsonManager.enatecollect_config = JsonUtility.FromJson<JsonData.ENateCollect_Config.ENateCollect_Config>((enatecollect_config.asset as TextAsset).ToString());

        // ResourceRequest events_config = AssetsManager.LoadJsonResource("events_Config");
        // yield return events_config;
        // JsonManager.events_config = JsonUtility.FromJson<JsonData.Events_Config.Events_Config>((events_config.asset as TextAsset).ToString());

        // ResourceRequest exp_config = AssetsManager.LoadJsonResource("exp_Config");
        // yield return exp_config;
        // JsonManager.exp_config = JsonUtility.FromJson<JsonData.Exp_Config.Exp_Config>((exp_config.asset as TextAsset).ToString());

        // ResourceRequest fever_config = AssetsManager.LoadJsonResource("fever_Config");
        // yield return fever_config;
        // JsonManager.fever_config = JsonUtility.FromJson<JsonData.Fever_Config.Fever_Config>((fever_config.asset as TextAsset).ToString());

        // ResourceRequest gacha_config = AssetsManager.LoadJsonResource("gacha_Config");
        // yield return gacha_config;
        // JsonManager.gacha_config = JsonUtility.FromJson<JsonData.Gacha_Config.Gacha_Config>((gacha_config.asset as TextAsset).ToString());

        // ResourceRequest goodsparts_config = AssetsManager.LoadJsonResource("goodsParts_Config");
        // yield return goodsparts_config;
        // JsonManager.goodsparts_config = JsonUtility.FromJson<JsonData.GoodsParts_Config.GoodsParts_Config>((goodsparts_config.asset as TextAsset).ToString());

        // ResourceRequest goods_config = AssetsManager.LoadJsonResource("goods_Config");
        // yield return goods_config;
        // JsonManager.goods_config = JsonUtility.FromJson<JsonData.Goods_Config.Goods_Config>((goods_config.asset as TextAsset).ToString());

        // ResourceRequest item_config = AssetsManager.LoadJsonResource("item_Config");
        // yield return item_config;
        // JsonManager.item_config = JsonUtility.FromJson<JsonData.Item_Config.Item_Config>((item_config.asset as TextAsset).ToString());

        // ResourceRequest map_config = AssetsManager.LoadJsonResource("map_Config");
        // yield return map_config;
        // JsonManager.map_config = JsonUtility.FromJson<JsonData.Map_Config.Map_Config>((map_config.asset as TextAsset).ToString());

        // ResourceRequest monthcard_config = AssetsManager.LoadJsonResource("monthCard_Config");
        // yield return monthcard_config;
        // JsonManager.monthcard_config = JsonUtility.FromJson<JsonData.MonthCard_Config.MonthCard_Config>((monthcard_config.asset as TextAsset).ToString());

        // ResourceRequest npc_group = AssetsManager.LoadJsonResource("npc_group");
        // yield return npc_group;
        // JsonManager.npc_group = JsonUtility.FromJson<JsonData.Npc_group.Npc_group>((npc_group.asset as TextAsset).ToString());

        // ResourceRequest parklv_config = AssetsManager.LoadJsonResource("parkLv_Config");
        // yield return parklv_config;
        // JsonManager.parklv_config = JsonUtility.FromJson<JsonData.ParkLv_Config.ParkLv_Config>((parklv_config.asset as TextAsset).ToString());

        // ResourceRequest parts_config = AssetsManager.LoadJsonResource("parts_Config");
        // yield return parts_config;
        // JsonManager.parts_config = JsonUtility.FromJson<JsonData.Parts_Config.Parts_Config>((parts_config.asset as TextAsset).ToString());

        // ResourceRequest path_config = AssetsManager.LoadJsonResource("path_Config");
        // yield return path_config;
        // JsonManager.path_config = JsonUtility.FromJson<JsonData.Path_Config.Path_Config>((path_config.asset as TextAsset).ToString());

        // ResourceRequest popular_config = AssetsManager.LoadJsonResource("popular_Config");
        // yield return popular_config;
        // JsonManager.popular_config = JsonUtility.FromJson<JsonData.Popular_Config.Popular_Config>((popular_config.asset as TextAsset).ToString());

        // ResourceRequest pose_config = AssetsManager.LoadJsonResource("pose_Config");
        // yield return pose_config;
        // JsonManager.pose_config = JsonUtility.FromJson<JsonData.Pose_Config.Pose_Config>((pose_config.asset as TextAsset).ToString());

        // ResourceRequest postcard_config = AssetsManager.LoadJsonResource("postcard_Config");
        // yield return postcard_config;
        // JsonManager.postcard_config = JsonUtility.FromJson<JsonData.Postcard_Config.Postcard_Config>((postcard_config.asset as TextAsset).ToString());

        // ResourceRequest postcard_fuse = AssetsManager.LoadJsonResource("postcard_fuse");
        // yield return postcard_fuse;
        // JsonManager.postcard_fuse = JsonUtility.FromJson<JsonData.Postcard_fuse.Postcard_fuse>((postcard_fuse.asset as TextAsset).ToString());

        // ResourceRequest quality_config = AssetsManager.LoadJsonResource("quality_Config");
        // yield return quality_config;
        // JsonManager.quality_config = JsonUtility.FromJson<JsonData.Quality_Config.Quality_Config>((quality_config.asset as TextAsset).ToString());

        // ResourceRequest quest_config = AssetsManager.LoadJsonResource("quest_Config");
        // yield return quest_config;
        // JsonManager.quest_config = JsonUtility.FromJson<JsonData.Quest_Config.Quest_Config>((quest_config.asset as TextAsset).ToString());

        // ResourceRequest reputation_config = AssetsManager.LoadJsonResource("reputation_Config");
        // yield return reputation_config;
        // JsonManager.reputation_config = JsonUtility.FromJson<JsonData.Reputation_Config.Reputation_Config>((reputation_config.asset as TextAsset).ToString());

        // ResourceRequest rhythm_config = AssetsManager.LoadJsonResource("rhythm_Config");
        // yield return rhythm_config;
        // JsonManager.rhythm_config = JsonUtility.FromJson<JsonData.Rhythm_Config.Rhythm_Config>((rhythm_config.asset as TextAsset).ToString());

        // ResourceRequest skill_config = AssetsManager.LoadJsonResource("skill_Config");
        // yield return skill_config;
        // JsonManager.skill_config = JsonUtility.FromJson<JsonData.Skill_Config.Skill_Config>((skill_config.asset as TextAsset).ToString());

        // ResourceRequest stage_config = AssetsManager.LoadJsonResource("stage_Config");
        // yield return stage_config;
        // JsonManager.stage_config = JsonUtility.FromJson<JsonData.Stage_Config.Stage_Config>((stage_config.asset as TextAsset).ToString());

        // ResourceRequest stamp_config = AssetsManager.LoadJsonResource("stamp_Config");
        // yield return stamp_config;
        // JsonManager.stamp_config = JsonUtility.FromJson<JsonData.Stamp_Config.Stamp_Config>((stamp_config.asset as TextAsset).ToString());

        // ResourceRequest system_config = AssetsManager.LoadJsonResource("system_Config");
        // yield return system_config;
        // JsonManager.system_config = JsonUtility.FromJson<JsonData.System_Config.System_Config>((system_config.asset as TextAsset).ToString());

        // ResourceRequest title_config = AssetsManager.LoadJsonResource("title_Config");
        // yield return title_config;
        // JsonManager.title_config = JsonUtility.FromJson<JsonData.Title_Config.Title_Config>((title_config.asset as TextAsset).ToString());

        // ResourceRequest visitor_config = AssetsManager.LoadJsonResource("visitor_Config");
        // yield return visitor_config;
        // JsonManager.visitor_config = JsonUtility.FromJson<JsonData.Visitor_Config.Visitor_Config>((visitor_config.asset as TextAsset).ToString());
    }
}