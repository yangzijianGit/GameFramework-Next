using System;
using System.Collections.Generic;

namespace JsonData.Build_main
{
    public class Build_main
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
        public string developBdLvUp;
        public string developGoodsNew;
        public List<BuildMain> buildMain;
    }

    [Serializable]
    public class BuildMain
    {
        public string area;
        public string name;
        public string des;
        public List<string> quest;
        public List<Develop> develop;
    }

    [Serializable]
    public class Develop
    {
        public string count;
        public string lv;
        public string skinId;
        public List<DevelopStep> developStep;
        public Effect effect;
    }

    [Serializable]
    public class DevelopStep
    {
        public string count;
        public List<Reward> reward;
    }

    [Serializable]
    public class Reward
    {
        public string type;
        public string id;
        public string pram1;
        public string pram2;
        public string value;
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
}