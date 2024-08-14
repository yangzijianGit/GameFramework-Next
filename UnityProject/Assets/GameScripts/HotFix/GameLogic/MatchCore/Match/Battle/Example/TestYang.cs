using System.Collections;
using ENate;
using UnityEngine;

public class yii : CustomYieldInstruction
{
    static public bool m_bIsOver = true;
    public override bool keepWaiting
    {
        get
        {
            return m_bIsOver;
        }
    }
    public new object Current
    {
        get
        {
            TestYang.consoleFrameCount("Current");
            return 2;
        }
    }

    public new bool MoveNext()
    {
        TestYang.consoleFrameCount("MoveNext");
        return base.MoveNext();
    }

    public new void Reset()
    {
        TestYang.consoleFrameCount("Reset");
        base.Reset();
    }

}

public class yii2 : IEnumerator
{
    static public bool m_bIsOver = true;
    public object Current
    {
        get
        {
            TestYang.consoleFrameCount("Current");
            return 2;
        }
    }

    public bool MoveNext()
    {
        TestYang.consoleFrameCount("MoveNext");
        return m_bIsOver;
    }

    public void Reset()
    {
        TestYang.consoleFrameCount("Reset");
    }

}
public class TestYang : MonoBehaviour
{
    public static int nFrameCount = 0;
    private void Start()
    {
        // StartCoroutine(ttttt1());
        //StartCoroutine(aaa1().InternalRoutine());
        // StartCoroutine(aaa1());
        StartCoroutine(bbb1());
        TestYang.consoleFrameCount("new yii2");
    }
    private void Update()
    {
        // consoleFrameCount("Update");
        nFrameCount++;
        if (nFrameCount > 100)
        {
            yii2.m_bIsOver = false;
        }
    }
    public static void consoleFrameCount(string strLog)
    {
        // LogUtil.AddLog("battle","frame: "); // .MoreStringFormat(nFrameCount , " " , strLog));
    }
    public IEnumerator ttttt1()
    {
        TestYang.consoleFrameCount("ttttt1");
        // yield return StartCoroutine(ttttt2());
        yield return ttttt2();
        TestYang.consoleFrameCount("ttttt1 end");
    }
    public IEnumerator ttttt2()
    {
        TestYang.consoleFrameCount("ttttt2");
        yield return new yii2();
        TestYang.consoleFrameCount("ttttt2 end");
    }

    public IEnumerator aaa1()
    {
        TestYang.consoleFrameCount("aaa1");
        var tvalue = aaa2();
        yield return null;
        yield return aaa2();
        TestYang.consoleFrameCount("aaa1 end");
    }
    public IEnumerator aaa2()
    {
        TestYang.consoleFrameCount("aaa2");
        yield return null;
        yield return ENateYield.WaitForSeconds(10);
        TestYang.consoleFrameCount("waitend");
        yield return aaa3();
        TestYang.consoleFrameCount("aaa2 end");
    }
    public IEnumerator aaa3()
    {
        TestYang.consoleFrameCount("aaa3");
        TestYang.consoleFrameCount("aaa3 end");
        if (false)
        {
            yield break;
        }
    }

    public IEnumerator bbb1()
    {
        ENate.ENateCoroutine tee = new ENate.ENateCoroutine();
        tee.Add(aaa1());
        tee.Add(aaa3());
        tee.Add(aaa3());
        return tee.play();
    }
}