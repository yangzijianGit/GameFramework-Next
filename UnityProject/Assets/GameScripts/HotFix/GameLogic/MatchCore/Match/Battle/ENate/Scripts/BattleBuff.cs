/*
 * @Description: BattleBuff
 * @Author: yangzijian
 * @Date: 2020-06-11 14:27:14
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-06-15 10:05:08
 */
using System.Collections.Generic;
using UnityEngine;
public class BattleBuff
{
    public enum BuffType
    {
        skillPower_trigger_addSpeedPercent,
        skillPower_stageBegin_addSpeedPercent,
        step_stageBegin_addStepNum
    }

    BuffType m_eBuffType;

    public BuffType EBuffType
    {
        get
        {
            return m_eBuffType;
        }
    }
    public List<string> m_arrParam;
    int m_nTriggerCount;
    public BattleBuff(BuffType eBuffType, int nTriggerCount, List<string> arrParam)
    {
        m_eBuffType = eBuffType;
        m_arrParam = arrParam.cloneSelf();
    }

    static public BattleBuff create(string strType, int nTriggerCount, List<string> arrParam)
    {
        if (strType == "addSteps")
        {
            return new BattleBuff(BuffType.step_stageBegin_addStepNum, nTriggerCount, arrParam);
        }
        else if (strType == "energySpeed")
        {
            return new BattleBuff(BuffType.skillPower_trigger_addSpeedPercent, nTriggerCount, arrParam);
        }
        else if (strType == "tankBegin")
        {
            return new BattleBuff(BuffType.skillPower_stageBegin_addSpeedPercent, nTriggerCount, arrParam);
        }
        return null;
    }

    static public BattleBuff create(string strBuffId)
    {
        var tBuffConfig = Config.BuffConfig.getBuffConfig(strBuffId);
        if (tBuffConfig == null)
        {
            return null;
        }
        return create(tBuffConfig.effectType, int.Parse(tBuffConfig.time), tBuffConfig.value);
    }

}