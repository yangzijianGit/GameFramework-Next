/*
 * @Description: animation class 
 * @Author: yangzijian
 * @Date: 2020-03-10 15:53:51
 * @LastEditors: yangzijian
 * @LastEditTime: 2020-06-01 11:08:05
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENate
{
    public class ENateTask
    {
        public ENateTask(int nTaskId)
        {
            m_nId = nTaskId;
        }
        int m_nId;
        public int ID
        {
            get
            {
                return m_nId;
            }
        }
        bool m_bIsForceQuit = false;
        public bool IsForceQuit
        {
            set
            {
                m_bIsForceQuit = value;
            }
            get
            {
                return m_bIsForceQuit;
            }
        }

        bool m_bIsOver = false;
        public bool IsOver
        {
            get
            {
                return m_bIsOver;
            }
        }
        List<Action<ENateTask>> m_arrCallBack = new List<Action<ENateTask>>();
        public IEnumerator run(Func<bool> pTaskFunction)
        {
            bool bIsOk = false;
            while (bIsOk == false)
            {
                try
                {
                    if (IsForceQuit == true)
                    {
                        yield break;
                    }
                    bIsOk = pTaskFunction();
                }
                catch (Exception)
                {
                    break;
                }
                yield return null;
            }
            foreach (var pCallBack in m_arrCallBack)
            {
                try
                {
                    pCallBack(this);
                }
                catch (Exception) { }
            }
            m_bIsOver = true;
        }
        public void addCallBack(Action<ENateTask> pCallBack)
        {
            m_arrCallBack.Add(pCallBack);
        }

    }
    public class ENateTaskManagement : MonoBehaviour
    {
        Dictionary<int, ENateTask> m_mpTask = new Dictionary<int, ENateTask>();
        public Counter m_tCounter = new Counter();
        void delTask(ENateTask tTask)
        {
            m_mpTask.Remove(tTask.ID);
        }
        public int createTask(Func<bool> pTaskFunction, Action<ENateTask> pCallBack = null)
        {
            int nTaskId = m_tCounter.count();
            ENateTask tTask = new ENateTask(nTaskId);
            tTask.addCallBack(pCallBack);
            tTask.addCallBack(delTask);
            m_mpTask.Add(nTaskId, tTask);
            StartCoroutine(tTask.run(pTaskFunction));
            return nTaskId;
        }

        public bool isAnyAni()
        {
            return m_mpTask.Count > 0;
        }

        public bool isTaskRunning(int nTaskId)
        {
            return getTask(nTaskId) != null;
        }

        public void clear()
        {
            foreach (var itTask in m_mpTask)
            {
                itTask.Value.IsForceQuit = true;
            }
            m_mpTask.Clear();
        }

        public void remove(int nTaskId)
        {
            try
            {
                m_mpTask[nTaskId].IsForceQuit = true;
                m_mpTask.Remove(nTaskId);
            }
            catch (Exception)
            {

            }
        }

        ENateTask getTask(int nTaskId)
        {
            try
            {
                return m_mpTask[nTaskId];
            }
            catch (Exception)
            {

            }
            return null;
        }

        IEnumerator waitTask(List<int> arrWaitTaskId, Action pCallBack)
        {
            List<ENateTask> arrTask = new List<ENateTask>();
            try
            {

                foreach (var nTaskId in arrWaitTaskId)
                {
                    ENateTask tENateTask = getTask(nTaskId);
                    if (tENateTask == null)
                    {
                        continue;
                    }
                    arrTask.Add(tENateTask);
                }
            }
            catch (Exception)
            {

            }
            bool bIsOk = false;
            while (bIsOk == false)
            {
                bIsOk = true;
                foreach (var tENateTask in arrTask)
                {
                    if (tENateTask.IsOver == false)
                    {
                        bIsOk = false;
                        break;
                    }
                }
                yield return null;
            }
            try
            {
                pCallBack();
            }
            catch (Exception) { }
        }

        public void WaitTask(List<int> arrWaitTaskId, Action pCallBack)
        {
            StartCoroutine(waitTask(arrWaitTaskId, pCallBack));
        }

    }
}