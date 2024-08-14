using System;
using System.Collections.Generic;

namespace JsonData.Quality_Config
{
    public class Quality_Config
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
        public List<PostcardQuality> postcardQuality;
        public List<ClothesQuality> clothesQuality;
        public List<PoseQuality> poseQuality;
        public List<GoodsQuality> goodsQuality;
        public List<SeverQuality> severQuality;
    }

    [Serializable]
    public class PostcardQuality
    {
        public string id;
        public string tip;
        public string name;
        public string des;
        public string imgIcon;
    }

    [Serializable]
    public class ClothesQuality
    {
        public string id;
        public string tip;
        public string name;
        public string des;
        public string imgIcon;
    }

    [Serializable]
    public class PoseQuality
    {
        public string id;
        public string tip;
        public string name;
        public string des;
        public string imgIcon;
    }

    [Serializable]
    public class GoodsQuality
    {
        public string id;
        public string tip;
        public string name;
        public string des;
        public string imgIcon;
    }

    [Serializable]
    public class SeverQuality
    {
        public string id;
        public string tip;
        public string name;
        public string des;
        public string imgIcon;
    }
}