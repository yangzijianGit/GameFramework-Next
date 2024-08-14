using System.Collections;
using UnityEngine;
public class testCor : MonoBehaviour
{
    IEnumerator test3()
    {
        consoleFrameCount("test3 begin");
        yield break;
    }
    IEnumerator test2()
    {
        for (int i = 0; i < 10; i++)
        {
            consoleFrameCount("test2 begin");
            yield return test3();
            consoleFrameCount("test2 end");
        }
    }

    IEnumerator test1()
    {
        for (int i = 0; i < 10; i++)
        {
            consoleFrameCount("test1 begin");
            yield return test2();
            consoleFrameCount("test1 end");
        }
    }
    private void Start()
    {
        StartCoroutine(test1());
    }
    public static int nFrameCount = 0;
    private void Update()
    {
        consoleFrameCount("Update");
        nFrameCount++;
    }
    public static void consoleFrameCount(string strLog)
    {
        Debug.Log("frame: " + nFrameCount + " " + strLog);
    }
}