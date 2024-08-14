using System;
using System.Collections.Generic;

namespace JsonData.Goods_Config
{
    public class Goods_Config
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
        public List<GoodsInfo> goodsInfo;
    }

    [Serializable]
    public class GoodsInfo
    {
        public string id;
        public string goodsPartsID;
        public string order;
        public string type;
        public string name;
        public string des;
        public string imgIcon;
        public List<string> title;
        public List<Quality> quality;
        public List<CardsDisplay> cardsDisplay;
    }

    [Serializable]
    public class Quality
    {
        public string qualityId;
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

    [Serializable]
    public class CardsDisplay
    {
        public string cardsId;
        public string cardsCount;
    }
}