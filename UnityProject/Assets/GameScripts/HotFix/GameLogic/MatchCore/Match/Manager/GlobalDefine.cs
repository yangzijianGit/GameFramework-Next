#if UNITY_EDITOR

public class GlobalDefine
{
    //预设根路径、精灵根路径、配置文件路径(暂时停用)、关卡输出根路径(暂时停用)、关卡编辑器配置路径
    public static string PATH_PREFAB_ROOT = "Assets/_Prefab";
    public static string PATH_SPRITE_ROOT = "Assets/_Sprites";
    public static string PATH_CONFIG_LEVEL_EDITOR = "resource/201911_kumamon/Config/editor/level.xml";
    public static string PATH_OUTPUT_LEVEL_ROOT = "resource/201911_kumamon/Config/level";
    public static string PATH_LEVEL_EDITOR_CONFIG = "resource/201911_kumamon/Config/editor/levelEditor.xml";


    //元素最大数量(暂时停用)、道具最大数量(暂时停用)、角色最大数量(暂时停用)
    public static int MAX_ELEMENT_COUNT = 30;
    public static int MAX_PROPS_COUNT = 30;
    public static int MAX_ROLE_COUNT = 30;


    //创建面板（参数面板标题、布局面板标题）
    public static string CREATE_PANEL_LAYOUT_TITLE = "第一步：棋盘布局";
    public static string CREATE_PANEL_PARAM_TITLE = "第二步：参数编辑";

    //提示面板（提示内容，按钮类型）
    public static string TIP_PANEL_CONTENT_SAVE_LEVEL = "当前关卡已经发生改动但并未保存，是否返回？";
    public static string TIP_PANEL_CONTENT_DELETE_LEVEL = "是否删除 {0} 关卡";
    public static string TIP_PANEL_CONTENT_OPERATOR_FINISH = "操作完成";
    public static string TIP_PANEL_CONTENT_SAVE_INCOMPLETE = "无法保存，关卡信息不完整，是否离开";
    public static string TIP_PANEL_CONTENT_NO_SAVE = "无法保存，关卡信息不完整";
    public static string TIP_PANEL_CONTENT_LEAVE = "当前有内容已修改，是否离开";
    public static string TIP_PANEL_CONTENT_SAVE_ERROR = "无法保存，关卡信息存在错误";
    public static string TIP_PANEL_BUTTON_YES = "是";
    public static string TIP_PANEL_BUTTON_NO = "否";
    public static string TIP_PANEL_BUTTON_OK = "确定";

    //关卡创建方式
    public static string LEVEL_CREATE_WAY_AUTO = "[自动]";
    public static string LEVEL_CREATE_WAY_MANUAL = "[手动]";
}


//对话框枚举
public enum DialogType
{
    //空
    DT_NONE,

    //是否删除xxx关卡的对话框
    DT_DELETE_LEVEL,
    //操作完成的对话框
    DT_OPERATOR_FINISH,
    //当前关卡已修改，是否保存的对话框
    DT_SAVE_LEVEL,
    //无法保存，关卡信息不完整，是否离开的对话框
    DT_SAVE_INCOMPLETE,
    //无法保存，关卡信息存在错误的对话框
    DT_SAVE_ERROR,
    //无法保存，关卡信息不完整
    DT_NO_SAVE
}


//对话框按钮枚举
public enum DialogButtonType
{
    //空
    DBT_NONE,

    //左按钮(是)
    DBT_LEFT,
    //右按钮(否)
    DBT_RIGHT,
    //中按钮(确定)
    DBT_CENTER
}

#endif