using System;
using System.Collections.Generic;

namespace JsonData.Npc_group
{
    public class Npc_group
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
        public List<Group> group;
    }

    [Serializable]
    public class Group
    {
        public string id;
        public Level level;
    }

    [Serializable]
    public class Level
    {
        public string min;
        public string max;
        public Attribute attribute;
    }

    [Serializable]
    public class Attribute
    {
        public string id;
        public string value;
        public string step;
    }
}