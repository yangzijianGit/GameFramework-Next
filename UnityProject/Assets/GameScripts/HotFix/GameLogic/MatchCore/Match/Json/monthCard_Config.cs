using System;
using System.Collections.Generic;

namespace JsonData.MonthCard_Config
{
    public class MonthCard_Config
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
        public string moveMax;
        public string buffUpPercent;
    }
}