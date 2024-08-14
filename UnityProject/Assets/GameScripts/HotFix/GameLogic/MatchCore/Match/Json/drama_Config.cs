using System;
using System.Collections.Generic;

namespace JsonData.Drama_Config
{
    public class Drama_Config
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
        public List<DramaChapter> dramaChapter;
    }

    [Serializable]
    public class DramaChapter
    {
        public string id;
        public string name;
        public List<HappenData> happenData;
        public DramaData dramaData;
        public List<string> blockName;
        public List<Move> move;
        public string nextID;
        public string plotPath;
        public List<TipsBubble> tipsBubble;
    }

    [Serializable]
    public class HappenData
    {
        public string happenCondition;
        public string happenValue;
    }

    [Serializable]
    public class DramaData
    {
        public List<NpcData> npcData;
        public View view;
        public string time;
    }

    [Serializable]
    public class NpcData
    {
        public string id;
        public string position;
        public string anima;
        public string direction;
    }

    [Serializable]
    public class View
    {
        public string height;
        public string position;
    }

    [Serializable]
    public class Move
    {
        public string block;
        public string npc;
        public string type;
        public string anima;
        public string position;
        public string speed;
        public string duration;
    }

    [Serializable]
    public class TipsBubble
    {
        public string block;
        public string npc;
        public string bubbleType;
        public string kw_string;
        public string direction;
    }
}