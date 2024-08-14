using System;
using System.Collections.Generic;

namespace JsonData.Build
{
    public class Build
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
        public List<StarLv> starLv;
        public List<BuildData> buildData;
    }

    [Serializable]
    public class StarLv
    {
        public string level;
        public string visitorCount;
        public string star;
    }

    [Serializable]
    public class BuildData
    {
        public string id;
        public List<LevelInfo> levelInfo;
        public List<Appraise> appraise;
        public List<string> goods;
    }

    [Serializable]
    public class LevelInfo
    {
        public string level;
        public string visitorCount;
        public List<Effect> effect;
        public string moveCost;
        public Reward reward;
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

    [Serializable]
    public class Reward
    {
        public string type;
        public string pram1;
        public string pram2;
        public string id;
        public string value;
    }

    [Serializable]
    public class Appraise
    {
        public string appraiseId;
        public string level;
        public string visitorCount;
        public string imgIcon;
    }
}