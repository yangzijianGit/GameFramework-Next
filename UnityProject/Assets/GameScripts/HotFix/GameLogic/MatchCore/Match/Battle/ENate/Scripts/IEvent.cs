
using UnityEngine;

public class EventArg<Producer>
{
    public Producer m_tProducer;

    public EventArg()
    {

    }
}


public interface IEvent
{
    void Register<Producer>(int key, EventArg<Producer> ptr);

}
