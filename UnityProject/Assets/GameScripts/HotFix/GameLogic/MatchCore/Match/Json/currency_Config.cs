using System;
using System.Collections.Generic;

namespace JsonData.Currency_Config
{
    public class Currency_Config
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
        public List<Currency> currency;
    }

    [Serializable]
    public class Currency
    {
        public string id;
        public string code;
        public string pic;
        public string Tips;
        public string name;
        public string des;
        public string charge;
        public string max;
        public string diamond;
    }
}