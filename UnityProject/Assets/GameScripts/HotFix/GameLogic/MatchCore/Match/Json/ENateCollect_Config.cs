using System;
using System.Collections.Generic;

namespace JsonData.ENateCollect_Config
{
    public class ENateCollect_Config
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
        public string elementType;
        public Trigger trigger;
    }

    [Serializable]
    public class Trigger
    {
        public List<Node> node;
    }

    [Serializable]
    public class Node
    {
        public string type;
        public string condition;
        public string basePercent;
        public string energyPer;
        public string waitTime;
        public string disappearTime;
        public string disappearAni;
        public Show show;
        public List<string> ani;
        public string collectAni;
    }

    [Serializable]
    public class Show
    {
        public string isCurrent;
        public string prefab;
    }
}