using System;
using System.Collections.Generic;

namespace JsonData.Gacha_Config
{
    public class Gacha_Config
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
        public string fansCome;
        public string costWelcome;
        public string fansBuff;
        public string fansDes1;
        public string fansDes2;
        public List<FansBDRate> fansBDRate;
        public string targetMovie;
        public string targetMovieN;
        public List<Gacha> gacha;
    }

    [Serializable]
    public class FansBDRate
    {
        public string buildId;
        public string rate;
    }

    [Serializable]
    public class Gacha
    {
        public string id;
        public string name;
        public string type;
        public string banner;
        public string img;
        public string specialDes;
        public Single single;
        public Multi multi;
        public HelpConfig helpConfig;
        public string timeCondition;
        public string specialImage;
    }

    [Serializable]
    public class Single
    {
        public List<Cost> cost;
        public List<Target> target;
        public List<ExtraTarget> extraTarget;
        public string freeTime;
    }

    [Serializable]
    public class Cost
    {
        public string type;
        public string id;
        public string value;
    }

    [Serializable]
    public class Target
    {
        public string parkLv;
        public List<Reward> reward;
        public string count;
        public string specialMax;
        public string specialMin;
    }

    [Serializable]
    public class Reward
    {
        public List<Reward> reward;
        public string type;
        public string pram2;
        public string pram1;
        public string id;
        public string value;
        public string rate;
        public string special;
        public string unique;
    }

    [Serializable]
    public class ExtraTarget
    {
        public string type;
        public List<Reward> reward;
        public string rate;
        public string unique;
    }

    [Serializable]
    public class Multi
    {
        public List<Cost> cost;
        public List<Target> target;
        public List<ExtraTarget> extraTarget;
    }

    [Serializable]
    public class HelpConfig
    {
        public string imageCount;
        public string titleDes;
        public string helpDes;
        public List<string> typeDes;
        public List<HelpDisplay> helpDisplay;
    }

    [Serializable]
    public class HelpDisplay
    {
        public string name;
        public string type;
        public string quality;
        public string percent;
        public string special;
    }
}