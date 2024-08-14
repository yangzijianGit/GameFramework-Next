using System;
using System.Collections.Generic;

namespace JsonData.Exp_Config
{
    public class Exp_Config
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
        public List<Level> level;
    }

    [Serializable]
    public class Level
    {
        public string lv;
        public string exp;
        public Function function;
        public Reward reward;
        public List<Display> display;
    }

    [Serializable]
    public class Function
    {
        public string type;
        public string value;
    }

    [Serializable]
    public class Reward
    {
        public string type;
        public string id;
        public string pram1;
        public string pram2;
        public string value;
    }

    [Serializable]
    public class Display
    {
        public string item;
        public string count;
    }
}