using System;
using System.Collections.Generic;

namespace JsonData.Ads_Config
{
    public class Ads_Config
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
        public List<Ads> ads;
    }

    [Serializable]
    public class Ads
    {
        public string type;
        public string times;
    }
}