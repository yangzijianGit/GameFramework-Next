using System;
using System.Collections.Generic;

namespace JsonData.Path_Config
{
    public class Path_Config
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
        public List<Path> path;
    }

    [Serializable]
    public class Path
    {
        public string id;
        public List<string> point;
        public List<BirthPoint> birthPoint;
    }

    [Serializable]
    public class BirthPoint
    {
        public string pos;
        public string buildGroup;
    }
}