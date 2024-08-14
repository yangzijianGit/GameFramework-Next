using System;
using System.Collections.Generic;

namespace JsonData.Title_Config
{
    public class Title_Config
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
        public List<Title> title;
    }

    [Serializable]
    public class Title
    {
        public string index;
        public string id;
        public string name;
        public string pic;
    }
}