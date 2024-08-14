/*
 * @Description: 
 * @Author: yangzijian
 * @Date: 2020-04-08 15:43:16
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-04-28 15:18:36
 */
public abstract class ENateDispose<SubClass> : System.IDisposable where SubClass : ENateDispose<SubClass>
{
    private bool alreadyDisposed = false;
    public delegate void DisposeCallBack();
    event DisposeCallBack m_pDisposeCallBack;
    //供程序员显式调用的Dispose方法

    public void addDisposeCallback(DisposeCallBack pDisposeCallBack)
    {
        m_pDisposeCallBack += pDisposeCallBack;
    }
    public void Dispose()
    {
        //调用带参数的Dispose方法, 释放托管和非托管资源
        Dispose(true);
        //手动调用了Dispose释放资源，那么析构函数就是不必要的了, 这里阻止GC调用析构函数
        System.GC.SuppressFinalize(this);
    }

    //protected的Dispose方法, 保证不会被外部调用。
    //传入bool值disposing以确定是否释放托管资源
    void Dispose(bool disposing)
    {
        if (alreadyDisposed) return; //保证不重复释放

        if (disposing)
        {
            ///TODO:在这里加入清理"托管资源"的代码, 应该是xxx.Dispose();
            if (m_pDisposeCallBack != null) m_pDisposeCallBack();
        }
        ///TODO:在这里加入清理"非托管资源"的代码

        alreadyDisposed = true;
    }

    //供GC调用的析构函数
    ~ENateDispose()
    {
        Dispose(false); //释放非托管资源
    }
}