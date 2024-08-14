using System;
using System.Collections.Generic;

namespace JsonData.Stamp_Config
{
    public class Stamp_Config
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
        public List<Stamp> stamp;
    }

    [Serializable]
    public class Stamp
    {
        public string id;
        public string name;
        public string des;
        public string type;
        public List<Target> target;
    }

    [Serializable]
    public class Target
    {
        public string id;
        public List<From> from;
        public string name;
        public string des;
        public string pic;
    }

    [Serializable]
    public class From
    {
        public string des;
    }
}