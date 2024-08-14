using System;
using System.Collections.Generic;

namespace JsonData.Blog_Config
{
    public class Blog_Config
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
        public string incomeRecordMaxDay;
        public string maxAdvise;
        public AdviseDes adviseDes;
        public List<GoodsPosition> goodsPosition;
        public List<GoodsQuality> goodsQuality;
        public string partSpace;
        public List<NeedBd> needBd;
        public string minNews;
        public string maxNews;
        public List<News> news;
    }

    [Serializable]
    public class AdviseDes
    {
        public string buildingLevel;
        public string unCreateBuilding;
        public string goodPosition;
        public List<string> goodsQuality;
        public string partSpace;
    }

    [Serializable]
    public class GoodsPosition
    {
        public string position;
        public string rate;
    }

    [Serializable]
    public class GoodsQuality
    {
        public string quality;
        public string rate;
    }

    [Serializable]
    public class NeedBd
    {
        public string build;
        public string rate;
    }

    [Serializable]
    public class News
    {
        public string des;
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
}