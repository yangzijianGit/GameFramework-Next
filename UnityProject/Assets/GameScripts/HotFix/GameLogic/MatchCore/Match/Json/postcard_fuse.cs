using System;
using System.Collections.Generic;

namespace JsonData.Postcard_fuse
{
    public class Postcard_fuse
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
        public string HelpDes;
        public List<BonusEffect> bonusEffect;
        public string shopRetime;
        public string refreshItem;
        public List<CostRetime> costRetime;
        public List<ShopLv> shopLv;
        public List<Shop> shop;
    }

    [Serializable]
    public class BonusEffect
    {
        public string grade;
        public string kumamon;
        public string effect;
    }

    [Serializable]
    public class CostRetime
    {
        public string dust;
        public string ticketWelcome;
    }

    [Serializable]
    public class ShopLv
    {
        public string parkLv;
        public string shopId;
        public string des;
    }

    [Serializable]
    public class Shop
    {
        public string id;
        public List<Merchandise> merchandise;
    }

    [Serializable]
    public class Merchandise
    {
        public string merchandiseId;
        public List<Reward> reward;
        public List<Off> off;
    }

    [Serializable]
    public class Reward
    {
        public List<Reward> reward;
        public string type;
        public string pram2;
        public string pram1;
        public string id;
        public string value;
        public string rate;
        public List<Cost> cost;
    }

    [Serializable]
    public class Cost
    {
        public string type;
        public string id;
        public string value;
    }

    [Serializable]
    public class Off
    {
        public string percent;
        public string rate;
    }
}