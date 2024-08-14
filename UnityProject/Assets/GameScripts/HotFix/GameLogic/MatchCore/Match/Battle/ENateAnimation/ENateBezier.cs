/*
 * @Description: ENateBezier 0 到 1 的点
 * @Author: yangzijian
 * @Date: 2020-07-09 14:58:19
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-10 15:54:58
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    public class ENateBezier
    {
        List<Vector3> m_ctrl = new List<Vector3>();
        public ENateBezier()
        {
            m_ctrl.Add(Vector3.zero);
            m_ctrl.Add(Vector3.one);
        }
        public void addCtrlPoint(Vector3 ctrlPoint)
        {
            m_ctrl.Insert(m_ctrl.Count - 1, ctrlPoint);
        }
        public Vector3 getInterpolatePos(float t)
        {
            Vector3 pt = Vector3.zero;
            int n = m_ctrl.Count - 1;
            for (int i = 0; i <= n; i++)
            {
                pt.x += getKanbudong(n, i) * Mathf.Pow((1 - t), n - i) * Mathf.Pow(t, i) * m_ctrl[i].x;
                pt.y += getKanbudong(n, i) * Mathf.Pow((1 - t), n - i) * Mathf.Pow(t, i) * m_ctrl[i].y;
                pt.z += getKanbudong(n, i) * Mathf.Pow((1 - t), n - i) * Mathf.Pow(t, i) * m_ctrl[i].z;
            }
            return pt;
        }
        float getKanbudong(int n, int i)
        {
            return getJieCheng(n) / (getJieCheng(i) * getJieCheng(n - i));
        }
        float getJieCheng(int n)
        {
            float result = 1.0f;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

    };
}