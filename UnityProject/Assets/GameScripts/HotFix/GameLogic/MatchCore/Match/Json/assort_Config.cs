using System;
using System.Collections.Generic;

namespace JsonData.Assort_Config
{
    public class Assort_Config
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
        public List<AssortLv> assortLv;
        public List<Assort> assort;
    }

    [Serializable]
    public class AssortLv
    {
        public string order;
        public string lv;
        public string image;
    }

    [Serializable]
    public class Assort
    {
        public string id;
        public string order;
        public string name;
        public string des;
        public string image;
        public string color;
        public List<Reward> reward;
        public Effect effect;
        public string maxCount;
        public List<AssortCondition> assortCondition;
    }

    [Serializable]
    public class Reward
    {
        public string type;
        public string pram1;
        public string pram2;
        public string id;
        public string value;
    }

    [Serializable]
    public class Effect
    {
        public string type;
        public string scope;
        public string value;
        public string percent;
        public string des;
    }

    [Serializable]
    public class AssortCondition
    {
        public string type;
        public string id;
        public string value;
        public string name;
    }
}