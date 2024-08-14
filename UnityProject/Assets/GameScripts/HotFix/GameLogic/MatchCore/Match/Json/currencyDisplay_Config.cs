using System;
using System.Collections.Generic;

namespace JsonData.CurrencyDisplay_Config
{
    public class CurrencyDisplay_Config
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
        public List<CurrencyDisplay> CurrencyDisplay;
    }

    [Serializable]
    public class CurrencyDisplay
    {
        public string UI;
        public string diamond;
        public string energy;
        public string star;
        public string move;
        public string dust;
        public string gold;
        public string gold1;
        public string diamond1;
        public string energy1;
        public string star1;
        public string move1;
        public string dust1;
    }
}