using System;
using System.Collections.Generic;

namespace JsonData.Collect_Config
{
    public class Collect_Config
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
        public List<Quest> quest;
    }

    [Serializable]
    public class Quest
    {
        public string quality;
        public string count;
        public string id;
        public string name;
        public string des;
        public List<Mission> mission;
        public List<Reward> reward;
        public Effect effect;
    }

    [Serializable]
    public class Mission
    {
        public string type;
        public string value;
        public string img;
    }

    [Serializable]
    public class Reward
    {
        public string type;
        public string pram2;
        public string pram1;
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
}