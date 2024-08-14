using System;
using System.Collections.Generic;

namespace JsonData.Events_Config
{
    public class Events_Config
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
        public string gameStart;
        public string roundEnd;
        public string trigger;
        public string generate;
        public string eliminate;
        public RoundEndPriority roundEndPriority;
        public List<Events> events;
    }

    [Serializable]
    public class RoundEndPriority
    {
        public string nMinPriority;
        public List<PriorityNode> PriorityNode;
    }

    [Serializable]
    public class PriorityNode
    {
        public string elementId;
        public string priority;
    }

    [Serializable]
    public class Events
    {
        public string id;
        public EventSkill eventSkill;
    }

    [Serializable]
    public class EventSkill
    {
        public List<Index> index;
    }

    [Serializable]
    public class Index
    {
        public string strElementId;
        public List<SkillNode> skillNode;
    }

    [Serializable]
    public class SkillNode
    {
        public List<string> skill;
        public List<string> condition;
    }
}