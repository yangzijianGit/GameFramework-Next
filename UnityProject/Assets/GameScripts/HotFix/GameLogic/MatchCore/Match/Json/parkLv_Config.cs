using System;
using System.Collections.Generic;

namespace JsonData.ParkLv_Config
{
    public class ParkLv_Config
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
        public string income;
        public string moveCount;
        public string moveMax;
        public string bdLvMax;
        public string welcome;
        public List<Reward> reward;
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