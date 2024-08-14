/*
        author      :       yangzijian
        time        :       2019-12-26 20:55:24
        function    :       victory rules
*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace ENate
{

    public interface Rule
    {
        bool check(Stage tStage, int nChessBoardIndex);
        int getShow(Stage tStage, int nChessBoardIndex);
        string getShowElementId();

        WinRules.WinCheckType getWinCheckType();
    }

    public class EliminateRule : Rule
    {
        private string m_strElementId;
        private int m_nNum;

        public EliminateRule(string strElementId, int nNum)
        {
            m_strElementId = strElementId;
            m_nNum = nNum;
        }
        public virtual bool check(Stage tStage, int nChessBoardIndex)
        {
            return tStage.m_tENateCollecter.getCollectNum(nChessBoardIndex, m_strElementId) >= m_nNum;
        }
        public virtual int getShow(Stage tStage, int nChessBoardIndex)
        {
            return m_nNum - tStage.m_tENateCollecter.getCollectNum(nChessBoardIndex, m_strElementId);
        }
        public virtual string getShowElementId()
        {
            return m_strElementId;
        }
        public WinRules.WinCheckType getWinCheckType()
        {
            return WinRules.WinCheckType.eliminate;
        }
    }
    public class NoOnetRule : Rule
    {
        private string m_strHypotaxisId;
        private int m_nNum;
        public NoOnetRule(string strHypotaxisId, int nNum)
        {
            m_strHypotaxisId = strHypotaxisId;
            m_nNum = nNum;
        }
        public virtual bool check(Stage tStage, int nChessBoardIndex)
        {
            int nAllCount = tStage.m_tENateCollecter.getHypotaxisIdBlockCount(m_strHypotaxisId, nChessBoardIndex);
            return nAllCount <= 0;
        }
        public virtual int getShow(Stage tStage, int nChessBoardIndex)
        {
            int nAllCount = tStage.m_tENateCollecter.getHypotaxisIdBlockCount(m_strHypotaxisId, nChessBoardIndex);
            return nAllCount;
        }
        public virtual string getShowElementId()
        {
            return m_strHypotaxisId;
        }
        public WinRules.WinCheckType getWinCheckType()
        {
            return WinRules.WinCheckType.noOne;
        }
    }
    public class NoElement : Rule
    {
        private string m_strElementId;
        public virtual bool check(Stage tStage, int nChessBoardIndex)
        {
            return true;
        }
        public virtual int getShow(Stage tStage, int nChessBoardIndex)
        {
            return 0;
        }
        public virtual string getShowElementId()
        {
            return m_strElementId;
        }
        public WinRules.WinCheckType getWinCheckType()
        {
            return WinRules.WinCheckType.none;
        }
    }
    public class TimeRule
    {
        private int m_nTime;
        public virtual bool check(Stage tStage, int nChessBoardIndex)
        {
            return true;
        }
        public virtual int getShow(Stage tStage, int nChessBoardIndex)
        {
            return 0;
        }
        public virtual string getShowElementId()
        {
            return "";
        }
        public WinRules.WinCheckType getWinCheckType()
        {
            return WinRules.WinCheckType.none;
        }

    }

    public class WinRules
    {
        public enum WinCheckType
        {
            none,
            eliminate,
            noOne
        }
        private Stage m_tStage;
        public Dictionary<int, List<Rule>> m_dtRules; // chessboardindex : rules

        public WinRules(Stage tStage)
        {
            m_tStage = tStage;
            m_dtRules = new Dictionary<int, List<Rule>>();
        }
        public void addRule(int nChessBoardIndex, Rule tRule)
        {
            List<Rule> arrRules = null;
            try
            {
                arrRules = m_dtRules[nChessBoardIndex];
            }
            catch (Exception) { }
            if (arrRules == null)
            {
                arrRules = new List<Rule>();
                m_dtRules[nChessBoardIndex] = arrRules;
            }
            arrRules.Add(tRule);
        }

        public bool check()
        {
            foreach (var it in m_dtRules)
            {
                foreach (Rule tRule in it.Value)
                {
                    if (tRule.check(m_tStage, it.Key) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        List<Rule> getRules(int nChessBoardIndex)
        {
            if (m_dtRules.ContainsKey(nChessBoardIndex) == true)
            {
                return m_dtRules[nChessBoardIndex];
            }
            return null;
        }

        public bool check(int nChessBoardIndex)
        {
            List<Rule> arrRules = getRules(nChessBoardIndex);
            if (arrRules == null)
            {
                return false;
            }
            foreach (Rule tRule in arrRules)
            {
                if (tRule.check(m_tStage, nChessBoardIndex) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public bool isTarget(string strHypotaxisId)
        {
            List<Rule> arrRules = getRules(-1);
            if (arrRules == null)
            {
                return false;
            }
            foreach (Rule tRule in arrRules)
            {
                if (tRule.getShowElementId() == strHypotaxisId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}