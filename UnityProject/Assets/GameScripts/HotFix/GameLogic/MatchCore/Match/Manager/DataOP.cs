#if UNITY_EDITOR

//数据类(无实例)
public sealed class DataOP
{
    //是否有未保存的修改
    public static bool isNoSaveModify = false;


    /** 构造函数 **/
    private DataOP() {}
}

#endif