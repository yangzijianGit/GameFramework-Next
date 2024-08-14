using System;
using System.Collections.Generic;

namespace JsonData.Map_Config
{
    public class Map_Config
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
        public ViewLevel viewLevel;
        public List<string> birthPoint;
        public List<NavObstacle> navObstacle;
        public List<Area> area;
    }

    [Serializable]
    public class ViewLevel
    {
        public string lv;
        public string min;
        public string max;
    }

    [Serializable]
    public class NavObstacle
    {
        public string text;
        public string id;
        public List<Point> point;
    }

    [Serializable]
    public class Point
    {
        public string element;
    }

    [Serializable]
    public class Area
    {
        public string id;
        public string name;
        public string mapArea;
        public string gridWidth;
        public string gridHeight;
        public string mapWidth;
        public string mapHeight;
        public string resPath;
        public Unlock unlock;
        public string pathId;
        public string coreBdlId;
        public string openQuest;
    }

    [Serializable]
    public class Unlock
    {
        public Effect effect;
    }

    [Serializable]
    public class Effect
    {
        public string prefab;
        public List<Cloud> cloud;
    }

    [Serializable]
    public class Cloud
    {
        public string pos;
        public string tex;
        public string disappearTime;
    }
}