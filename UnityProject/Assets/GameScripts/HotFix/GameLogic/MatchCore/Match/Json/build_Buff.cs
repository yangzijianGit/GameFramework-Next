using System;
using System.Collections.Generic;

namespace JsonData.Build_Buff
{
    public class Build_Buff
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
        public List<Buff> buff;
    }

    [Serializable]
    public class Buff
    {
        public string id;
        public string name;
        public string des;
        public string imgIcon;
        public string time;
        public string toTime;
        public string offlineTiming;
        public string offlineEffect;
        public string offlineClear;
        public List<Effect> effect;
    }

    [Serializable]
    public class Effect
    {
        public string type;
        public string value;
        public string percent;
    }
}