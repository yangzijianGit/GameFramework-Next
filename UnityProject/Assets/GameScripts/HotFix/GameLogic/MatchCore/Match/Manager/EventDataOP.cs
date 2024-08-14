#if UNITY_EDITOR

//事件枚举类型
public enum EventTypeOP
{
    //点击创建关卡
    ET_CLICK_CREATE_LEVEL,
    //点击编辑关卡
    ET_CLICK_EDITOR_LEVEL,
    //点击预览关卡
    ET_CLICK_VIEW_LEVEL,
    //点击编辑关卡单元
    ET_CLICK_EDITOR_LEVEL_CELL,
    //点击自动生成
    ET_CLICK_AUTO_BUILD,
    //准备显示对话框(可以不显示)
    ET_PRE_SHOW_DIALOG,
    //关闭对话框完成
    ET_CLOSE_DIALOG_FINISH,
    //显示设置场景
    ET_SHOW_SET_SCENE,
    //更新棋盘号列表
    ET_UPDATE_BOARD_LIST,
    //点击保存关卡
    ET_CLICK_SAVE_LEVEL,
    //切换布局事件
    ET_CHANGE_LAYOUT,
    //添加新关卡
    ET_ADD_LEVEL,
    //删除关卡
    ET_DEL_LEVEL
}


//准备显示对话框(可以不显示)事件参数
public sealed class ETPreShowDialog
{
    //对话框类型
    public DialogType enumType = DialogType.DT_NONE; 
    //关卡文件名
    public string strLevelFileName = string.Empty;

    //构造
    public ETPreShowDialog() {}
    public ETPreShowDialog(DialogType type) 
    {
        this.enumType = type;
    }
    public ETPreShowDialog(DialogType type, string name) 
    {
        this.enumType = type;
        this.strLevelFileName = name;
    }
}

//点击编辑关卡事件参数
public sealed class ETClickEditorLevelCell
{
    //关卡文件名
    public string strLevelFileName = string.Empty;

    //构造
    public ETClickEditorLevelCell() {}
    public ETClickEditorLevelCell(string fileName)
    {
        this.strLevelFileName = fileName;
    }
}

//关闭对话框完成事件参数
public sealed class ETCloseDialogFinish
{
    //对话框类型
    public DialogType enumType = DialogType.DT_NONE; 
    //按钮类型
    public DialogButtonType enumButtonType = DialogButtonType.DBT_NONE;

    //构造
    public ETCloseDialogFinish() {}
    public ETCloseDialogFinish(DialogType type, DialogButtonType btnType)
    {
        this.enumType = type;
        this.enumButtonType = btnType;
    }
}

//更新棋盘号列表事件参数
public sealed class ETUpdateBoardList
{
    //棋盘数量
    public int nBoardCount = 0;

    //构造
    public ETUpdateBoardList() {}
    public ETUpdateBoardList(int count) 
    {
        this.nBoardCount = count;
    }
}

//切换布局事件参数
public sealed class ETChangeLayout
{
    //旧布局，新布局
    public int nOldLayout = 0;
    public int nNewLayout = 0;

    //构造
    public ETChangeLayout() {}
    public ETChangeLayout(int nOld, int nNew) 
    {
        this.nOldLayout = nOld;
        this.nNewLayout = nNew;
    }
}

//关卡事件参数
public sealed class ETLevelParam
{
    //关卡事件
    public LevelDataOP objLevel = null;

    //构造
    public ETLevelParam() {}
    public ETLevelParam(LevelDataOP obj) 
    {
        this.objLevel = obj;
    }
}


//事件数据类(无实例)
public sealed class EventDataOP
{
    /** 构造函数 **/
    private EventDataOP() {}


    //初始话
    public static void Init()
    {
        EventOP.RegisterEvent((int)EventTypeOP.ET_CLICK_CREATE_LEVEL);
        EventOP.RegisterEvent((int)EventTypeOP.ET_CLICK_EDITOR_LEVEL);
        EventOP.RegisterEvent((int)EventTypeOP.ET_CLICK_VIEW_LEVEL);
        EventOP.RegisterEvent((int)EventTypeOP.ET_CLICK_AUTO_BUILD);
        EventOP.RegisterEvent((int)EventTypeOP.ET_CLICK_EDITOR_LEVEL_CELL);
        EventOP.RegisterEvent((int)EventTypeOP.ET_PRE_SHOW_DIALOG);
        EventOP.RegisterEvent((int)EventTypeOP.ET_CLOSE_DIALOG_FINISH);
        EventOP.RegisterEvent((int)EventTypeOP.ET_SHOW_SET_SCENE);
        EventOP.RegisterEvent((int)EventTypeOP.ET_UPDATE_BOARD_LIST);
        EventOP.RegisterEvent((int)EventTypeOP.ET_CLICK_SAVE_LEVEL);
        EventOP.RegisterEvent((int)EventTypeOP.ET_CHANGE_LAYOUT);
        EventOP.RegisterEvent((int)EventTypeOP.ET_ADD_LEVEL);
        EventOP.RegisterEvent((int)EventTypeOP.ET_DEL_LEVEL);
    }
}

#endif