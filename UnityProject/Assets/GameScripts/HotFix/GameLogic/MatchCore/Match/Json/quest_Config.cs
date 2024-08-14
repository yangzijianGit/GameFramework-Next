using System;
using System.Collections.Generic;

namespace JsonData.Quest_Config
{
    public class Quest_Config
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
        public string id;
        public List<Reward> reward;
        public string imgIcon;
        public string des;
        public Require require;
        public string effect;
        public string exp;
        public List<string> preQuest;
    }

    [Serializable]
    public class Reward
    {
        public string type;
        public string value;
        public string ui;
        public string param1;
    }

    [Serializable]
    public class Require
    {
        public string type;
        public string value;
    }
}