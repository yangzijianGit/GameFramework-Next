/*
 * @Description: counter 
 * @Author: yangzijian
 * @Date: 2019-12-16 15:02:02
 * @LastEditors  : yangzijian
 * @LastEditTime : 2019-12-30 14:51:30
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter
{
    private int m_nCounter = 0; // 计数器 

    public int count()
    {
        return m_nCounter++;

    }

}
