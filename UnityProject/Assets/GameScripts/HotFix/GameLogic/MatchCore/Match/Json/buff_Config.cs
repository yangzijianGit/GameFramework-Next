using System;
using System.Collections.Generic;

namespace JsonData.Buff_Config
{
    public class Buff_Config
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
        public List<Buff> buff;
    }

    [Serializable]
    public class Buff
    {
        public string id;
        public string time;
        public string effectType;
        public List<string> value;
    }
}