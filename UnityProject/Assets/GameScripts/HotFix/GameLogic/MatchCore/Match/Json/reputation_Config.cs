using System;
using System.Collections.Generic;

namespace JsonData.Reputation_Config
{
    public class Reputation_Config
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
        public List<Group> group;
    }

    [Serializable]
    public class Group
    {
        public string id;
        public List<Reputation> reputation;
    }

    [Serializable]
    public class Reputation
    {
        public string index;
        public string id;
        public string name;
        public string min;
        public string max;
        public string log;
    }
}