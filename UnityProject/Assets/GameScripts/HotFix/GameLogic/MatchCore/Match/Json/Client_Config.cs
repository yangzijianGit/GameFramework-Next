using System;
using System.Collections.Generic;

namespace JsonData.Client_Config
{
    public class Client_Config
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
        public string gameid;
        public List<ConditionFunc> conditionFunc;
        public List<ConditionStruct> conditionStruct;
        public List<Condition> condition;
    }

    [Serializable]
    public class ConditionFunc
    {
        public string description;
        public string type;
        public List<Para> para;
    }

    [Serializable]
    public class Para
    {
        public string description;
        public string text;
    }

    [Serializable]
    public class ConditionStruct
    {
        public string description;
        public string type;
        public List<Para> para;
    }

    [Serializable]
    public class Condition
    {
        public string description;
        public string clientId;
        public string type;
        public List<Para> para;
    }
}