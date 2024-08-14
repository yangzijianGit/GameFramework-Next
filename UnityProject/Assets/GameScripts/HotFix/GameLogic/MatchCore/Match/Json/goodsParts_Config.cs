using System;
using System.Collections.Generic;

namespace JsonData.GoodsParts_Config
{
    public class GoodsParts_Config
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
        public List<GoodsParts> goodsParts;
    }

    [Serializable]
    public class GoodsParts
    {
        public string id;
        public string type;
        public string pram1;
        public List<PartsPos> partsPos;
    }

    [Serializable]
    public class PartsPos
    {
        public string skinId;
        public string point;
    }
}