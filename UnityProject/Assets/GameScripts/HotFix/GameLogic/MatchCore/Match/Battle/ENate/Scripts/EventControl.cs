using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventControl : GameBase.Singleton<EventControl>, IEvent
{
    public delegate void EventControlEvent(object tEventProducer);

    private Dictionary<string, EventControlEvent> m_mpEventControl;

    public void Register<Producer>(int key, EventArg<Producer> tt)
    {

    }

}
