public enum BuildFunctionType
{
    shop,
    parade,
    server,
    system,
}

//声音类型
public enum SoundType
{
    None,
    Home,
    Main_0,
};

public enum Direction
{
    Mine = 0,
    UpLeft = 1,
    Up = 2,
    UpRight = 3,
    Right = 4,
    DownRight = 5,
    Down = 6,
    DownLeft = 7,
    Left = 8,
    NULL
}
/// <summary>
/// 在世界地图场景，摄像机的距离
/// </summary>
public enum WorldDistanceType
{
    Near,
    Far,
}

public enum PlayerState
{
    NormalState,
}

/// <summary>
/// 消除类格子状态
/// </summary>
public enum GridState
{
    /// <summary>
    /// 初始状态，什么都没有
    /// </summary>
    InitState,
    /// <summary>
    /// 格子上面没东西，准备接受下落
    /// </summary>
    DownPreState,
    /// <summary>
    /// 格子上面没东西，正在有东西落下的状态
    /// </summary>
    DownState,
    /// <summary>
    /// 下落状态结束，格子上面有东西了
    /// </summary>
    DownEndState,
    /// <summary>
    /// 正常状态
    /// </summary>
    NormalState,
    /// <summary>
    /// 格子上面有东西，正在发生爆炸
    /// </summary>
    BombState,
    /// <summary>
    /// 爆炸结束
    /// </summary>
    BombEndState,
}

public enum ObjType
{
    ObjGrid,
    ObjBuild,
    ObjVisitor,
    ObjPartSpace,
    ObjPart,
    ObjBuildGroup,
    ObjMapArea
}

/// <summary>
/// 三消场景中的层级
/// </summary>
public enum BattleLayerType
{
    BattleGridLayer = 0,
    BattleFirstLayer = 1,
    BattleSecondLayer = 2,
}

public enum BuildingStateType
{
    ShowClear,
    ShowUnlock,
    None,
}
public enum Week
{
    SUN=1,
    MON,
    TUE,
    WED,
    THU,
    FRI,
    SAT,
}

public enum TouchMoveDir
{
    idle, left, right, up, down
}

public static class BuildIncomeType
{
    public static string incomeType = "income";
}

public class ItemType
{
    public const string item = "item";
    public const string itemgoods = "goods";
    public const string itempart = "parts";
    public const string itempostcard = "postcard";
    public const string itemappearence = "appearance";
}

public static class CountryType
{
    public const string CN ="CN";
    public const string JP = "JP";
    public const string Kr = "KR";
    public const string TH = "TH";
    public const string EG ="EG";
    public const string FR ="FR";
    public const string US ="US";
    public const string BR = "BR";
}

public static class QuestRewardType
{
    public const string build = "build";
    public const string removeBuild = "removeBuild";
    public const string buildGroup = "buildGroup";
}

//建筑类型枚举(shop店铺型、sever服务型、system系统功能型、parade彩车型)
public static class BuildingType
{
    public const string SHOP = "shop";
    public const string SERVER = "server";
    public const string SYSTEM = "system";
    public const string PARADE = "parade";
    public const string RUIN = "ruins";
    public const string MAIN = "main";
    public const string GACHA = "gacha";
    public const string SCENERY = "scenery";//休闲建筑
}

public enum CameraState
{
    Normal,
    Touch,
    BuildingMainMove,
    ScaleCamera,
}

/// <summary>
/// 建筑部分所处于的层级划分
/// </summary>
public static class BuildLayer
{
    /// <summary>
    /// 最底层，主要用于道路，不能挡住任何其他建筑和npc
    /// </summary>
    public const string low = "lowest";
    /// <summary>
    /// 中间层，和npc有遮挡关系
    /// </summary>
    public const string middle = "middle";
    /// <summary>
    /// 最高层，能挡住任何npc和其他建筑
    /// </summary>
    public const string high = "highest";
}

public static class BuildingAniType
{
    public const string always = "always";
    public const string usings = "using";
}

/// <summary>
/// 游戏内道具类型
/// </summary>
public static class RewardType
{
    /// <summary>
    /// 货币
    /// </summary>
    public const string currency = "currency"; 
    /// <summary>
    /// 物品，item_config
    /// </summary>
    public const string item = "item"; 
    /// <summary>
    /// 外观
    /// </summary>
    public const string appearance = "appearance";
    /// <summary>
    /// 明信片
    /// </summary>
    public const string postcard = "postcard";
    /// <summary>
    /// 建筑消费品
    /// </summary>
    public const string good = "good";
    /// <summary>
    /// 装扮
    /// </summary>
    public const string clothes = "clothes";
    /// <summary>
    /// 建筑装饰
    /// </summary>
    public const string part = "part";
    public const string buff = "buff";
}

public static class  PartType
{
    public const string all = "0";
    public const string small = "1";
    public const string middle = "2";
    public const string big = "3";
}

public static class BuildingSufaceUnlockGotoType
{
    public const string parkLv = "parkLv";
    public const string mission = "mission";
    public const string build = "build";
}

/// <summary>
/// 货币类型
/// </summary>
public static class CurrencyType
{
    public const string Diamond = "diamond";
    public const string star = "star";
    public const string energy = "energy";
    public const string move = "move";
    public const string dust = "dust";
}

public static class GoodsSurfaceAniType
{
    public const string pic = "pic";
    public const string ani = "ani";
}

//buff效果类型枚举(income收益效果)
public static class BuffEffectType
{
    public const string INCOME = "income";
}

public enum ButtonPressEffect
{
    None,
    scale,
    MoveUp,
}

public static class GachaRewardType
{
    public const string special = "special";
    public const string postcard = "postcard";
    public const string clothes = "clothes";
}

//游戏UI名类
public sealed class UIDefine
{
    /** 构造函数 **/
    private UIDefine() { }


    /** UI名 **/
    public static string LOGIN = "ui/UI_Start";                       //登陆UI
    public static string LOGIN_SERVER = "ui/UI_StartService";        //登陆服务器UI
    public static string UI_LEVEL = "ui/UI_Level";        //登陆服务器UI
    public static string UI_WORLDMAP = "WorldMap/WorldMap";
    public static string UI_Building = "ui/Building/UI_Building";        //建筑ui
    public static string UI_BuildingMain = "ui/Building/UI_BuildingMain";
    public static string UI_BuildingMainLvUp = "ui/Building/UI_BuildingMainLvUp";
    public static string UI_Quest = "ui/UI_Quest";        //建筑ui
    public static string UI_Main = "ui/UI_Main";//主界面
    public static string UI_Anti = "ui/UI_Anti";//主界面
    public static string UI_Assort = "ui/UI_Assort";//收藏柜
    public static string UI_Battle = "ui/UI_Battle";//消除界面
    public static string UI_BattleReadyClothes = "ui/UI_BattleReadyClothes";//消除界面
    public static string UI_BattleReady = "ui/UI_BattleReady";//消除准备界面
    public static string UI_BattleResult = "ui/UI_BattleResult";//消除界面
    public static string UI_Recharge = "ui/UI_Recharge";
    public static string UI_Reward = "ui/UI_Reward";
    public static string UI_Postcard = "ui/UI_Postcard";
    public static string UI_Fusion = "ui/UI_Fusion";
    public static string UI_BuildingPart = "ui/Building/UI_BuildingPart";
    public static string UI_BuildingSurface = "ui/Building/UI_BuildingSurface";
    public static string UI_FusionResult = "ui/UI_FusionResult";
    public static string UI_Energy = "ui/UI_Energy";
    public static string UI_Park = "ui/UI_Park";
    public static string UI_ParkLv = "ui/UI_ParkLv";
    public static string UI_PlotStory = "ui/UI_PlotStory";
    public static string UI_SysCurrency = "ui/UI_SysCurrency";
    public static string UI_Gacha = "ui/UI_Gacha";
    public static string UI_Store = "ui/UI_Store";
    public static string UI_Mail = "ui/UI_Mail";
    public static string UI_SysTips = "ui/UI_SysTips";
    public static string UI_Bubble = "ui/UI_Bubble";
    public static string UI_GachaReward = "ui/UI_GachaReward";
    public static string UI_BuildingRent = "ui/Building/UI_BuildingRent";
    public static string UI_BuildingGroupPanel = "ui/Building/BuildingGroupPanel";
    public static string UI_BuildScreenClearRuin = "ui/Building/BuildScreenClearRuin";
    public static string UI_BuildScreenUnlock = "ui/Building/BuildScreenUnlock";
    public static string UI_BuildScreenTaskbtn = "ui/Building/BuildScreenTaskbtn";
    public static string UI_BuildScreenGetGold = "ui/Building/BuildScreenGetGold"; 
    public static string UI_BuildScreenReward = "ui/Building/BuildScreenReward";
    public static string UI_BuildScreenFunctionBtn = "ui/Building/BuildScreenFunctionBtn";
    public static string UI_BuildingParade = "ui/Parade/UI_BuildingParade";
    public static string UI_BuildingParadeStart = "ui/Parade/UI_BuildingParadeStart";
    public static string UI_BuildingParadeVehicle = "ui/Parade/UI_BuildingParadeVehicle";
}

//储物柜套装类型(assort_config)
public static class AssortType
{
    public const string Build = "build";
    public const string Goods = "goods";
    public const string Postcard = "postcard";
    public const string Parts = "parts";
    public const string Appearance = "appearance";
    public const string BuildLv = "buildLv";
    public const string GoodsQuality = "goodsQuality";
    public const string ParkLv = "parkLv";
    public const string Ins = "ins";
    public const string Income = "income";
    public const string PostcardCountry = "postcardCountry";
    public const string GoodsTitle = "goodsTitle";
    public const string BuildTitle = "buildTitle";
    public const string Gacha = "gacha";
    public const string Fuse = "fuse";
    public const string Borrow = "borrow";
    public const string Parade = "parade";
    public const string Find = "find";
    public const string Activity = "activity";
    public const string Top10 = "top10";
}