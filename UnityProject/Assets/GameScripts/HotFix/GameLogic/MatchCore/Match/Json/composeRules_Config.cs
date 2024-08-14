using System;
using System.Collections.Generic;

namespace JsonData.ComposeRules_Config
{
    public class ComposeRules_Config
    {
        public Root root;
    }

    [Serializable]
    public class Root
    {
        public string client;
        public Game game;
    }

    [Serializable]
    public class Game
    {
        public string gameId;
        public ComposeTemplate ComposeTemplate;
    }

    [Serializable]
    public class ComposeTemplate
    {
        public List<Compose> Compose;
        public List<ExchangeHypotaxisSkillRules> ExchangeHypotaxisSkillRules;
        public List<ExchangeSkillRules> ExchangeSkillRules;
    }

    [Serializable]
    public class Compose
    {
        public string m_eComposeElementType;
        public List<Accord> accord;
        public string m_nPriority;
        public Create create;
    }

    [Serializable]
    public class Accord
    {
        public string m_nLine;
        public string m_nColumns;
    }

    [Serializable]
    public class Create
    {
        public string m_strCreateElementId;
    }

    [Serializable]
    public class ExchangeHypotaxisSkillRules
    {
        public string m_strExchangeMainId;
        public string m_strExchangeSecondId;
        public List<string> skill;
    }

    [Serializable]
    public class ExchangeSkillRules
    {
        public string m_strExchangeMainId;
        public string m_strExchangeSecondId;
        public List<string> skill;
    }
}