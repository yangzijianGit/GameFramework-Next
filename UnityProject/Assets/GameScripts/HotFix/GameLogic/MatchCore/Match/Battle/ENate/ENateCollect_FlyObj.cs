using System;
using System.Collections;
using System.Collections.Generic;
using ENate;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ENateCollect_FlyObj : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    ENateCollect m_tENateCollect;
    JsonData.ENateCollect_Config.Node m_tConfig;
    bool m_bIsEffect = false;
    public static ENateCollect_FlyObj create(JsonData.ENateCollect_Config.Node tConfig, GameObject tObj, GameObject tParent, ENateCollect tENateCollect)
    {
        GameObject tNew = new GameObject();
        var tRectTransform = tNew.AddComponent<RectTransform>();
        tRectTransform.sizeDelta = new Vector2(80, 80);
        tNew.name = "collectAniElement";
        // tNew.AddComponent<Image>().color = Color.blue;
        var tENateCollect_FlyObj = tNew.AddComponent<ENateCollect_FlyObj>();
        tENateCollect_FlyObj.m_tConfig = tConfig;
        tENateCollect_FlyObj.m_tENateCollect = tENateCollect;
        for (int nChildIndex = 0; nChildIndex < tObj.transform.childCount; nChildIndex++)
        {
            var tChild = tObj.transform.GetChild(nChildIndex);
            tChild.gameObject.attachObj(tNew);
        }
        tNew.attachObj(tParent);
        return tENateCollect_FlyObj;
    }
    public static ENateCollect_FlyObj create(JsonData.ENateCollect_Config.Node tConfig, string strPrefabPath, GameObject tParent, ENateCollect tENateCollect)
    {
        GameObject tNew = new GameObject();
        var tRectTransform = tNew.AddComponent<RectTransform>();
        tRectTransform.sizeDelta = new Vector2(80, 80);
        tNew.name = "collectAniElement";
        var tENateCollect_FlyObj = tNew.AddComponent<ENateCollect_FlyObj>();
        tENateCollect_FlyObj.m_tConfig = tConfig;
        tENateCollect_FlyObj.m_tENateCollect = tENateCollect;
        tNew.attachObj(tParent);
        var tCreateObj = jc.ResourceManager.Instance.LoadPrefab(strPrefabPath, null, false);
        tCreateObj.attachObj(tNew);
        return tENateCollect_FlyObj;
    }

    private void Start()
    {
        m_bIsEffect = false;
        StartCoroutine(ENateYield.WaitForSeconds(float.Parse(m_tConfig.disappearTime), () =>
        {
            if (m_bIsEffect == true) return;
            Util.playENateAni(m_tConfig.disappearAni, gameObject, transform.position, null, () =>
            {
                m_bIsEffect = true;
            }, false);
        }));
        StartCoroutine(ENateYield.WaitForSeconds(float.Parse(m_tConfig.waitTime), destroy));
    }

    public void destroy(bool bIsRemoveCallBack)
    {
        destroy();
        if (bIsRemoveCallBack == true)
            m_tENateCollect.removeObj(this);
    }
    void destroy()
    {
        StopAllCoroutines();
        if (gameObject == null) return;
        GameObject.Destroy(gameObject);
    }
    private void Update()
    {

    }

    void trigger()
    {
        if (m_bIsEffect == true) return;
        m_bIsEffect = true;
        Vector3? v3fTriggerPos = null;
        Action pCallBack = null;
        switch (m_tConfig.type)
        {
            case "fever":
                {
                    jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_FEVER_SHOWFEVER);
                    v3fTriggerPos = m_tENateCollect.m_tFeverTarget.transform.position;
                    pCallBack = () =>
                    {
                        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_FEVER_ADDPOWER, int.Parse(m_tConfig.energyPer));
                        destroy();
                    };
                }
                break;
            case "clothes":
                {
                    v3fTriggerPos = m_tENateCollect.m_tClothesSkillTarget.transform.position;
                    pCallBack = () =>
                    {
                        jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_CLOTHESSKILL_POWERADD, int.Parse(m_tConfig.energyPer));
                        destroy();
                    };
                }
                break;
        }
        Util.playENateAni(m_tConfig.collectAni, gameObject, transform.position, v3fTriggerPos, pCallBack, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
#if !UNITY_EDITOR
        if (Input.touchCount <= 0) return;
#else
        if (Input.GetMouseButton(0) == false) return;
#endif
        trigger();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        trigger();
    }

}