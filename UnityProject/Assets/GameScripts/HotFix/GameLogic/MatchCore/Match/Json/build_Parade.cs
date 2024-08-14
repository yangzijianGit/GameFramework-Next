using System;
using System.Collections.Generic;

namespace JsonData.Build_Parade
{
    public class Build_Parade
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
        public HelpMail helpMail;
        public string paradeTimes;
        public string vipParadeTimes;
        public string helpTimes;
        public string vipHelpTimes;
        public string paradeCD;
        public string countBySelf;
        public string countByHelp;
        public List<Way> way;
        public Drop drop;
        public Bubble bubble;
        public List<Box> box;
        public List<Parade> parade;
    }

    [Serializable]
    public class HelpMail
    {
        public string des;
        public string dayTimes;
        public List<Reward> reward;
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
    public class Way
    {
        public string build;
        public string areaId;
        public string openQuest;
        public string time;
        public string defaultParade;
        public List<string> wayPoint;
    }

    [Serializable]
    public class Drop
    {
        public string cd;
        public string rate;
        public string compensateRate;
        public string saveTime;
    }

    [Serializable]
    public class Bubble
    {
        public string chatDes;
        public string cd;
        public string rate;
        public string compensateRate;
        public string saveTime;
    }

    [Serializable]
    public class Box
    {
        public string id;
        public string pic;
        public string rate;
        public List<BoxDrop> boxDrop;
    }

    [Serializable]
    public class BoxDrop
    {
        public string rate;
        public List<Reward> reward;
    }

    [Serializable]
    public class Parade
    {
        public string id;
        public string respath;
        public string name;
        public string des;
        public string speed;
        public string script;
        public string sound;
        public Effect effect;
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