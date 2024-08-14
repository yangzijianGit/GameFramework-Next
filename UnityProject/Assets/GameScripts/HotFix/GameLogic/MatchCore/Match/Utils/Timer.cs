using UnityEngine;
using System.Collections;

public class Timer{
    private static MonoBehaviour behaviour;
    public delegate void Task();

    public static void Schedule(MonoBehaviour _behaviour, float delay, Task task)
    {
        behaviour = _behaviour;
        behaviour.StartCoroutine(DoTask(task, delay));
    }

    /**
     * @Author: yangzijian
     * @description: for public  
     */    
    public static void Schedule(float delay, Task task)
    {
        // Schedule(Launcher.Instance, delay, task);
    }

    private static IEnumerator DoTask(Task task, float delay)
    {
        yield return new WaitForSeconds(delay);
        task();
    }
}
