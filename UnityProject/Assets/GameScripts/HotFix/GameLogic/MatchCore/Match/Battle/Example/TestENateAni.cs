using System.Collections;
using System.Collections.Generic;
using ENate;
using ENate.ENateAnimation;

using UnityEngine;

public class TestENateAni : ENateAniManager
{
    public static JsonData.ENateAni_Config.ENateAni_Config enateani_config = null;
    public string strAniId;

    public Transform tAim;

    public GameObject aimElement;
    public Canvas m_tCanvas;
    ENateAni tENateAni;
    IEnumerator load()
    {
        // ResourceRequest tenateani_config = AssetsManager.LoadJsonResource("ENateAni_Config");
        // yield return enateani_config;
        // enateani_config = JsonUtility.FromJson<JsonData.ENateAni_Config.ENateAni_Config>((tenateani_config.asset as TextAsset).ToString());
        yield return null;
    }

    public static JsonData.ENateAni_Config.Ani getENateAni(string strAnimationId)
    {
        foreach (var tAniConfig in enateani_config.root.game.animation.ani)
        {
            if (tAniConfig.id == strAnimationId)
            {
                return tAniConfig;
            }
        }
        return null;
    }

    private void Awake()
    {
        jc.ResourceManager.Instance.SetResPath(jc.ResType.RT_SCRIPT, "Assets/_Scripts");
        jc.ResourceManager.Instance.SetResPath(jc.ResType.RT_SPRITE, "Assets/_Sprites");
        jc.ResourceManager.Instance.SetResPath(jc.ResType.RT_PREFAB, "Assets/_Prefabs");
        StartCoroutine(load());
    }

    public void playAni()
    {
        ENateAniArg tEnateAniArg = new ENateAniArg();
        tEnateAniArg.tObj = aimElement;
        tEnateAniArg.tRootPos = aimElement.transform.position;
        tEnateAniArg.tTriggerPos = tAim.position;
        //LogUtil.AddLog("battle", "tSrcPos"); // .MoreStringFormat(tEnateAniArg.tTriggerPos));
        //LogUtil.AddLog("battle", "tRootPos"); // .MoreStringFormat(tEnateAniArg.tRootPos));
        //LogUtil.AddLog("battle", "tAim1"); // .MoreStringFormat(tAim.parent.InverseTransformPoint(tAim.position)));
        //LogUtil.AddLog("battle", "tAim2"); // .MoreStringFormat(tAim.InverseTransformPoint(tAim.position)));
        //LogUtil.AddLog("battle", "tAim local 1"); // .MoreStringFormat(tAim.parent.localPosition));
        //LogUtil.AddLog("battle", "tAim local 2"); // .MoreStringFormat(tAim.localPosition));
        //LogUtil.AddLog("battle", "tAim canvas offect 1"); // .MoreStringFormat(m_tCanvas.GetComponent<RectTransform>().InverseTransformPoint(tAim.position)));
        var tAni = getENateAni(strAniId);
        tENateAni = new ENateAni(this, tAni, tEnateAniArg);
        //tENateAni.play(this, () =>{});
    }

    public void stop()
    {
        tENateAni.stop();
    }

}