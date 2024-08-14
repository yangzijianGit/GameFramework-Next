using System;
using System.Collections.Generic;

namespace JsonData.Popular_Config
{
    public class Popular_Config
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
        public string activityHelpDes;
        public string popularCD;
        public List<ConditionTime> conditionTime;
        public List<Popular> popular;
    }

    [Serializable]
    public class ConditionTime
    {
        public string time;
        public List<Populars> populars;
    }

    [Serializable]
    public class Populars
    {
        public string levelMin;
        public string levelMax;
        public List<PopularGoods> popularGoods;
    }

    [Serializable]
    public class PopularGoods
    {
        public string id;
        public string rate;
    }

    [Serializable]
    public class Popular
    {
        public string id;
        public string name;
        public string des;
        public List<string> titles;
        public string quality;
        public string qualityCount;
        public List<Display> display;
        public Reward reward;
    }

    [Serializable]
    public class Display
    {
        public string item;
        public string count;
    }

    [Serializable]
    public class Reward
    {
        public string type;
        public string id;
        public string value;
        public string pram1;
        public string pram2;
    }
}