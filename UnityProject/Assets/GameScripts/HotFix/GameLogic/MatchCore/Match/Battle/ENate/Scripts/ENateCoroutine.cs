/*
 * @Description: ENateCoroutine
 * @Author: yangzijian
 * @Date: 2020-07-07 10:33:30
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-07-20 14:37:50
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ENate
{

    public class ENateCoroutine
    {
        List<IEnumerator> m_arrEnumerator = new List<IEnumerator>();
        public void Add(IEnumerator tIEnumerator)
        {
            m_arrEnumerator.Add(tIEnumerator);
        }

        public IEnumerator play()
        {
            var iter = m_arrEnumerator.InternalRoutine();
            iter.MoveNext();
            return iter;
        }

        public void clear()
        {
            m_arrEnumerator.Clear();
        }

    }
}