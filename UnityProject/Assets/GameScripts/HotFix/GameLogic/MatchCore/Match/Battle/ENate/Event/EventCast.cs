/*
 * @Description: EventCast
 * @Author: yangzijian
 * @Date: 2020-03-24 16:19:55
 * @LastEditors: yangzijian
 * @LastEditTime: 2024-08-15 01:42:45
 */
using System;
using System.Collections;
using System.Collections.Generic;
using ENate;


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class EventCast : GameBase.Singleton<EventCast>
{
    public delegate bool pointerCallBack(Vector3 vec);

    Dictionary<GameObject, pointerCallBack> m_arrPointer = new Dictionary<GameObject, pointerCallBack>();

    public void addPointerDown(GameObject obj, pointerCallBack pPointerDown)
    {
        if (m_arrPointer.ContainsKey(obj) == true)
        {
            m_arrPointer[obj] += pPointerDown;
        }
        else
        {
            m_arrPointer.Add(obj, pPointerDown);
        }
    }
    public void removePointerDown(GameObject obj, pointerCallBack pPointerDown)
    {
        if (m_arrPointer.ContainsKey(obj) == true)
        {
            m_arrPointer[obj] -= pPointerDown;
        }
    }
    bool noticePointerDown(GameObject obj, Vector3 vec)
    {
        if (m_arrPointer.ContainsKey(obj) == true)
        {
            return m_arrPointer[obj](vec);
        }
        return false;
    }
    void rayCast(Vector2 vector, ref List<RaycastResult> results)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = vector;

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
    }

    public void Update()
    {
        List<RaycastResult> results = new List<RaycastResult>();
#if !UNITY_EDITOR
        var arrTouch = Input.touches;
        foreach (var tTouch in arrTouch)
        {
            if (tTouch.phase == TouchPhase.Began)
            {
                rayCast(tTouch.position, ref results);
            }
        }
#elif UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) == true)
        {
            rayCast(new Vector2(Input.mousePosition.x, Input.mousePosition.y), ref results);
        }
#endif
        foreach (var tResult in results)
        {
            //LogUtil.AddLog("raycast ", tResult.gameObject.name);
            bool bIsDeal = noticePointerDown(tResult.gameObject, tResult.worldPosition);
            if (bIsDeal == true)
            {
                break;
            }
        }
    }
}