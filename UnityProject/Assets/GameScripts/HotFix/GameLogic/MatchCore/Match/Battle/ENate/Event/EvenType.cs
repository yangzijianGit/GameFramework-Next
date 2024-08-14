namespace jc
{
    /*
     * 描述：系统事件类型枚举(用于接收服务器数据)[>= 1 && < 100]
     * 说明：每个事件都需要有一个枚举值，如果事件有参数，请在EventParam定义事件的参数对象
     */
    public enum SysEventType
    {
        //保留值（用于起点）
        ET_NONE = 0,

        //单机测试
        ET_STAND_ALONE_TEST
    }

    /*
     * 描述：网络事件类型枚举(用于接收服务器数据)[>= 101 && < 400]
     * 说明：每个事件都需要有一个枚举值，如果事件有参数，请在EventParam定义事件的参数对象
     * 注意：事件命名根据协议指令是一一对应，请务必遵循
     */
    public enum NetEventType
    {
        //保留值（用于起点）
        ET_NONE = 100,

        //ping
        ET_PING,
        //登录结果
        ET_LOGIN_RESULT,
        //服务器数据发送完毕
        ET_SERVERSENDDATAFINISH,
        //防沉迷
        ET_ANTIINDULGE,
        //玩家信息
        ET_PLAYER_INFO,
        //更新玩家属性结果
        ET_PLAYER_PROPERTY_RESULT,
        //解锁建筑结果
        ET_UNLOCK_BUILDING_RESULT,
        //收益建筑收益结果
        ET_COLLECT_BUILDING_INCOME_RESULT,
        //建築數據初始化
        ET_BUILDINGINIT,
        //更新金币
        ET_CURRENCYCHANGE,
        //全部货币
        ET_ALLCURRENCY,
        // 所有item
        ET_ITEMCHANGE,
        ET_ITEM_USE_RESULT,
        ET_ALLITEM,
        //更新建筑
        ET_UPGRADEBUILDING,
        //移除建筑
        ET_REMOVEINGBUILDING,
        //增加建筑
        ET_ADDBUILDING,
        ET_QUESTCOMPLETE,
        ET_STAGE_STAGERECORD,
        ET_STAGE_START_PREPARE,
        ET_STAGE_START_Send,
        ET_STAGE_START,
        ET_STAGE_END,
        ET_STAGE_BUYHAMMER,
        ET_STAGE_USEHAMMER,
        //更新 所有建筑的收入信息
        ET_BUILDINCOMEUPDATE,

        ET_GOLDUPDATE,
        ET_ALL_POSTCARD,
        ET_POSTCARD_CHANGE,
        ET_INFINITE_ENERGY_ENDTIME,
        ET_FUSION_MERGE_SHOWRESULT,
        ET_FUSION_MERGE_RESULT,
        ET_STAGE_BUYSTEP,
        ET_CURRENCYREFRESH,
        ET_ENERGY_RECOVER_REMAIN_TIME,
        ET_BUILDING_GOODS_UPDATE,
        ET_APPEARANCEBUY,
        ET_APPEARANC_UNLOCK,
        ET_Appearance_Switch,
        ET_S_PartsInfo,
        S_AddPart,
        S_PartSpaceUnlock,
        S_EquipPart,
        S_RemovePart,
        S_ParkLevelUp,
        S_MainBuildingUpdate,
        S_ClothesInit,
        S_ClothesUpdate,
        S_EquipClothes,
        S_LotteryInfo,
        S_Lottery,
        S_ShopInfo,
        S_ShopRefresh,
        S_ShopBuy,
        S_MailData, //邮件相关
        S_AchievementInfo,
        S_AcquireAchievementReward,
        S_AchievementUpdate,
        S_ParadeInfo,
        S_ParadeAdd,
        S_ParadeBaseActive,
        S_ExchangeParade,
        S_ParadeDepart,
        S_ParadeDropAcquire,
        S_ParadeBubbleUpdate,
        ET_Count,
    }

    /*
     * 描述：UI事件类型枚举(用于接收服务器数据)[>= 401 && < 1000]
     * 说明：每个事件都需要有一个枚举值，如果事件有参数，请在EventParam定义事件的参数对象
     */
    public enum UIEventType
    {
        //保留值（用于起点）
        ET_NONE = 400,

        //打开building的换装
        ET_OPEN_BUILDING_SURFACE,
        //建筑面板打开物品信息
        ET_OPEN_BUILDING_GOODSINFO,
        //建筑buff
        ET_BUILDING_BUFF,
        //建筑面板打开关闭物品信息
        ET_OPEN_BUILDING_CLOSEGOODSINFO,

        ET_COUNT
    }

    /*
     * 描述：普通事件类型枚举[> 1000]
     * 说明：每个事件都需要有一个枚举值，如果事件有参数，请在EventParam定义事件的参数对象
     */
    public enum CommonEventType
    {
        //保留值（用于起点）
        ET_NONE = 1000,

        //进入大陆场景
        ET_ENTER_MAINLAND_SCENE,
        ET_buildInitComplete,
        //离开大陆场景
        ET_LEAVE_MAINLAND_SCENE,
        //点击登录
        ET_CLICK_LOGIN,
        //资源加载完成之前
        ET_LOAD_RES_FINISH_PRE,
        //加载配置完成
        ET_LOAD_CONFIG_FINISH,
        //打开登陆UI
        ET_OPEN_UI_LOGIN,
        //登陆加载完成
        ET_LOGIN_LOAD_FINISH,
        //打开UI完成
        ET_OPEN_UI_FINISH,
        //关闭UI完成
        ET_CLOSE_UI_FINISH,
        //更新服务器地址
        ET_UPDATE_SERVER_ADDR,
        //创建实体完成
        ET_CREATE_ENTITY_FINISH,
        //升级建筑
        ET_UPGRADE_BUILDING,
        //更新建筑解锁状态
        ET_UPDATE_BUILDING_UNLOCK_STATUS,
        //打开建筑外观
        ET_OPEN_BUILDING_SURFACE,
        //更新建筑皮肤,
        ET_UPDATE_BUILDING_SKIN,
        //解锁建筑
        ET_UNLOCK_BUILDING,
        //解锁建筑组
        ET_UNLOCK_BUILDING_GROUP,
        //解锁大陆
        ET_UNLOCK_BUILDING_LAND,
        //初始化世界完成
        ET_INIT_WORLD_FINISH,
        //添加buff
        ET_ADD_BUFF,
        //移除buff
        ET_REMOVE_BUFF,
        //添加玩家属性
        ET_ADD_PLAYER_PROPERTY,
        //减少玩家属性
        ET_SUB_PLAYER_PROPERTY,
        //添加玩家属性值上限
        ET_ADD_PLAYER_PROPERTY_CAP,
        //减少玩家属性值下限
        ET_SUB_PLAYER_PROPERTY_LOWER,
        //特效逻辑结束
        ET_EFFECT_LOGIC_END,
        //收集建筑收益
        ET_COLLECT_BUILDING_INCOME,
        //请求单位时间NPC
        ET_REQUEST_UNIT_TIME_NPC,
        //响应单位时间NPC
        ET_RESPONSE_UNIT_TIME_NPC,
        //获取建筑信息
        ET_BUILDINGDATA,
        //在線時長
        ET_ANTI,
        ET_UPDATE_HAPPINESS,
        ET_CLICKBUILD,
        ET_SELECTLEVEL,
        ET_INITSTART,
        ET_SDKLOGINCOMPLETE,
        ET_AddBuildingFromeServer,
        ET_RemoveBuildingFromServer,
        ET_SHOW_POSTCARD_INFO,
        ET_CLOSE_POSTCARDFUSION_INFO,
        ET_POSTCARD_CHANGECOUNTRY,
        ET_POSTCARD_CHANGE,
        ET_UPDATE_FUSION_ADDITEM,
        ET_BUILD_TO_FUSION,
        ET_BUILDING_GOODS_UPDATE,
        ET_APPEARANC_UNLOCK,
        ET_Appearance_Switch,
        ET_APPEARANCEBUY,
        ET_Open_building_Part, //打开显建筑的装饰
        ET_Close_building_Part, //打开显建筑的装饰
        ET_show_type_part, //显示特定类型的装饰
        ET_building_part_click,
        ET_ParkLevelUp,
        ET_ITEMCHANGE,
        ET_updateGold,
        ET_updateCurrency,
        ET_updateViewGold, //更新营业额的ui显示
        ET_UpdateBuildingPart,
        ET_Building_Part_watch,
        ET_UpdateBuildingQuestState,
        ET_move_camera,
        ET_scale_camera,
        ET_return_move_camera,
        S_MainBuildingUpdate,
        ET_MoveObj_end, //游客出园了
        ET_Building_open_function,
        ET_BuildingShowUnlockEffect,
        ET_BuildingShowAward,
        ET_BuildingHideClear,
        ET_BuildingHideUnlock,
        ET_BuildingShowQuest,
        ET_BuildingGroupShowUnlockPrice,
        ET_BuildingGroupHideUnlock,
        ET_LotteryInfo,
        S_ShopInfo,
        S_MailData_Changed, //邮件数据发生了变化
        ET_AssortReward,
        ET_PlotOpenAnim,
        ET_PlotLevel,
        ET_PlotQuest,
        S_ShopRefresh,

        ET_splotSpriteComplete,//剧情需要的精灵已经加载完毕了。add by dcg at 2020.8.5

        ET_COUNT,
    }

    public enum STAGEEVENTTYPE
    {
        ET_NONE = 1500,
        ET_DEBUG_CTRL0,
        ET_DEBUG_CTRL1,
        ET_DEBUG_CTRL2,
        ET_DEBUG_CTRL3,
        ET_DEBUG_CTRL4,
        ET_DEBUG_CTRL5,
        ET_DEBUG_CTRL6,
        ET_DEBUG_CTRL7,
        ET_DEBUG_CTRL8,
        ET_DEBUG_CTRL9,
        ET_DEBUG_SPACE,
        ET_DEBUG_ALT1,
        ET_DEBUG_ALT2,
        ET_DEBUG_ALT3,
        ET_DEBUG_ALT4,
        ET_DEBUG_ALT5,
        ET_DEBUG_ALT6,
        ET_DEBUG_ALT7,
        ET_DEBUG_ALT8,
        ET_DEBUG_ALT9,
        ET_STAGE_Init,
        ET_STAGE_CreateTask,
        ET_STAGE_ADDSTEP,
        ET_STAGE_STEPFRESH,
        ET_STAGE_ChangeStatuesToCheckWin,
        ET_STAGE_WIN,
        ET_STAGE_FAILED,
        ET_STAGE_ExitStage,
        ET_STAGE_Reset,
        ET_STAGE_DROP_DROPOVERCHECK_Prefix,
        ET_STAGE_DROP_DROPOVERCHECK,
        ET_STAGE_CALMNESS_PREPARE,
        ET_STAGE_ELEMENT_CREATE,
        ET_STAGE_ELEMENT_DROPDOWN,
        ET_STAGE_ELEMENT_ELIMINATE,
        ET_STAGE_ELEMENT_ElementChange,
        ET_STAGE_ELEMENT_SkillElementChange,
        ET_STAGE_ELEMENT_ELIMINATE_ROUND,
        ET_STAGE_ELEMENT_ELIMINATE_CALMNESS,
        ET_STAGE_Handle,
        ET_STAGE_Handle_SelectGrid,
        ET_STAGE_Handle_Prepare,
        ET_STAGE_Handling,
        ET_STAGE_Handle_EndCheck,
        ET_STAGE_POSE_Combo_Perfect,
        ET_STAGE_POSE_Combo_Good,
        ET_STAGE_POSE_Combo_Failed,
        ET_STAGE_POSE_Trigger_Combo,
        ET_STAGE_POSE_Trigger_ComboSkill,
        ET_STAGE_POSE_Trigger_Beat_Position_Notice,
        ET_STAGE_POSE_Trigger_Beat_Position_NoticeEnd,

        ET_STAGE_POSEPLANEA_Screen_Operator,
        ET_STAGE_POSEPLANEA_Resonance_trigger_succeed,
        ET_STAGE_POSEPLANEA_Resonance_trigger_failed,
        ET_STAGE_POSEPLANEA_Resonance_trigger_skill,

        ET_STAGE_CLOTHESSKILL_POWERADD,
        ET_STAGE_CLOTHESSKILL_POWERFULL_OPERATING,
        ET_STAGE_CLOTHESSKILL_POWERFULL_ProChange,
        ET_STAGE_REPUTATION_CHANGE,

        ET_STAGE_FEVER_SHOWFEVER,
        ET_STAGE_FEVER_CHANGEFEVERPRO,
        ET_STAGE_FEVER_ADDPOWER,
        ET_STAGE_FEVER_BEGINAni,
        ET_STAGE_FEVER_BEGIN,
        ET_STAGE_FEVER_END,

        ET_UI_BATTLEREADYCLOTHES_skilltype_select,
        ET_UI_BATTLEREADYCLOTHES_cloth_select,
        ET_UI_BATTLEREADYCLOTHES_cloth_change,
        ET_UI_BATTLE_MASK,
        ET_UI_BATTLE_MASK_click,

        ET_UI_BATTLE_hanmer_infoChange,

        ET_Ani_Play,

        ET_TIPS_Add,
        ET_TIPS_Hide,
        ET_TIPS_Remove,
        ET_TIPS_Show,

        ET_KUMMUN_PLAYANI,

        ET_DROP_STOP,
        ET_DROP_OPEN,


        ET_ENTER_STAGE_START,
        ET_NUM

    }
}