using System;
using System.Collections.Generic;

namespace JsonData.Clothes_Config
{
    public class Clothes_Config
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
        public Trigger trigger;
        public List<SkillType> skillType;
        public List<Clothes> clothes;
    }

    [Serializable]
    public class Trigger
    {
        public string energyTotal;
    }

    [Serializable]
    public class SkillType
    {
        public string index;
        public string name;
        public string imgIcon;
        public List<string> clothesGroup;
    }

    [Serializable]
    public class Clothes
    {
        public string id;
        public string name;
        public string skillDes;
        public string kw_class;
        public string quality;
        public string targetChoose;
        public string imgIcon;
        public List<Attribute> attribute;
        public string targetChooseCondition;
    }

    [Serializable]
    public class Attribute
    {
        public string level;
        public string exp;
        public List<string> des;
        public BuffServer buffServer;
        public List<StageBehaviorID> stageBehaviorID;
        public List<StartBehaviorID> startBehaviorID;
    }

    [Serializable]
    public class BuffServer
    {
        public string timeExpression;
        public List<string> buffid;
    }

    [Serializable]
    public class StageBehaviorID
    {
        public List<SkillGroup> skillGroup;
        public string weight;
    }

    [Serializable]
    public class SkillGroup
    {
        public List<string> skill;
        public string probability;
        public string buff;
    }

    [Serializable]
    public class StartBehaviorID
    {
        public List<SkillGroup> skillGroup;
        public string weight;
    }
}