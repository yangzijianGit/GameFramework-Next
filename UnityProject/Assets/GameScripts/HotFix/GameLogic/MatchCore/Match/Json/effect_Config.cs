using System;
using System.Collections.Generic;

namespace JsonData.Effect_Config
{
    public class Effect_Config
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
        public List<Effect> effect;
    }

    [Serializable]
    public class Effect
    {
        public string id;
        public string res;
        public string LogicEndTime;
        public string isAutoDestroy;
        public string FinishTime;
    }
}