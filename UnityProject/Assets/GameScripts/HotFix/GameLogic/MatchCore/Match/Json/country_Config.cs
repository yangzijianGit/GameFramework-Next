using System;
using System.Collections.Generic;

namespace JsonData.Country_Config
{
    public class Country_Config
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
        public List<Country> country;
    }

    [Serializable]
    public class Country
    {
        public string id;
        public string index;
        public string name;
        public string des;
        public string imgIcon;
        public string backIcon;
        public string smallIcon;
    }
}