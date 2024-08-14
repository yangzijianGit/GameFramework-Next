using System;
using System.Collections.Generic;

namespace JsonData.Postcard_Config
{
    public class Postcard_Config
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
        public List<ShopQuality> shopQuality;
        public List<Postcard> postcard;
    }

    [Serializable]
    public class ShopQuality
    {
        public string quality;
        public string rate;
    }

    [Serializable]
    public class Postcard
    {
        public string id;
        public string name;
        public string des;
        public string order;
        public string country;
        public string quality;
        public string imgIcon;
        public string dustCount;
        public List<string> title;
    }
}