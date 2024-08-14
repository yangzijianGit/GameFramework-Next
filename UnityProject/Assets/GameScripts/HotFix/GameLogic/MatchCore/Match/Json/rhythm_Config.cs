using System;
using System.Collections.Generic;

namespace JsonData.Rhythm_Config
{
    public class Rhythm_Config
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
        public string tolerance;
        public string maxTolerance;
        public string rhythmThinkTime;
        public string rhythmStayTime;
        public List<SonglList> songlList;
    }

    [Serializable]
    public class SonglList
    {
        public string id;
        public string sound;
        public string name;
        public string namelog;
        public string speed;
        public string widthPerSecond;
        public string imgIcon;
        public List<Rhythm> rhythm;
    }

    [Serializable]
    public class Rhythm
    {
        public string time;
        public string type;
    }
}