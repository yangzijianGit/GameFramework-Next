/*
        author      :       yangzijian
        time        :       2019-12-16 15:06:30
        function    :       StageData for step and other info
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    public class StageData
    {
        // config info 
        // use info 
        public int m_nStep;
        public string m_strName;
        StageRunningStatus m_StageRunningStatus = StageRunningStatus.DetectingReset;
        bool m_bIsWaitAni = false;

        public bool RunningAni
        {
            get
            {
                return m_bIsWaitAni;
            }
            set
            {
                m_bIsWaitAni = value;
            }
        }

        public StageRunningStatus RunningStatus
        {
            set
            {
                m_bIsWaitAni = true;
                m_StageRunningStatus = value;
            }
            get
            {
                return m_StageRunningStatus;
            }
        }
    }
}