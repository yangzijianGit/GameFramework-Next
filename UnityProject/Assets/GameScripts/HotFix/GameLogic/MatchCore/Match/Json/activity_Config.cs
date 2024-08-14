using System;
using System.Collections.Generic;

namespace JsonData.Activity_Config
{
    public class Activity_Config
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
        public List<Plan> plan;
        public Pass pass;
    }

    [Serializable]
    public class Plan
    {
        public string id;
        public string name;
        public string des;
        public string banner;
        public string icon;
        public string pic;
        public string day;
        public string price;
        public List<WeekReward> weekReward;
        public List<DayReward> dayReward;
        public List<Function> function;
    }

    [Serializable]
    public class WeekReward
    {
        public string type;
        public string id;
        public string value;
        public string pram1;
        public string pram2;
    }

    [Serializable]
    public class DayReward
    {
        public string type;
        public string id;
        public string value;
        public string pram1;
        public string pram2;
    }

    [Serializable]
    public class Function
    {
        public string des;
    }

    [Serializable]
    public class Pass
    {
        public string id;
        public string name;
        public string des;
        public string banner;
        public string icon;
        public string pic;
        public string day;
        public string price;
        public List<Reward> reward;
    }

    [Serializable]
    public class Reward
    {
        public string lv;
        public string exp;
        public FreeReward freeReward;
        public List<MoneyReward> moneyReward;
    }

    [Serializable]
    public class FreeReward
    {
        public string type;
        public string id;
        public string value;
        public string pram1;
        public string pram2;
    }

    [Serializable]
    public class MoneyReward
    {
        public string type;
        public string id;
        public string value;
        public string pram1;
        public string pram2;
    }
}