using System;
using System.Collections.Generic;

namespace JsonData.Addiction_Config
{
    public class Addiction_Config
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
        public string health;
        public List<OnlineTime> onlineTime;
        public string offlineTimeHours;
    }

    [Serializable]
    public class OnlineTime
    {
        public string onlineTimeHours;
        public string factor;
    }
}