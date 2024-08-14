using System;
using System.Collections.Generic;

namespace JsonData.Item_Config
{
    public class Item_Config
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
        public List<Item> item;
    }

    [Serializable]
    public class Item
    {
        public string id;
        public string skin;
        public string Tips;
        public string name;
        public string des;
        public string skill;
        public string startSkill;
        public Reward reward;
    }

    [Serializable]
    public class Reward
    {
        public string type;
        public string id;
        public string pram1;
        public string pram2;
        public string value;
    }
}