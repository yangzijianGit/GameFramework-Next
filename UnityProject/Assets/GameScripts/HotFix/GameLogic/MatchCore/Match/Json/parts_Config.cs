using System;
using System.Collections.Generic;

namespace JsonData.Parts_Config
{
    public class Parts_Config
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
        public List<TypeEnum> typeEnum;
        public List<Part> part;
        public List<PartSpaceType> partSpaceType;
        public List<PartSpace> partSpace;
        public List<Build> build;
        public List<OpenCondition> openCondition;
    }

    [Serializable]
    public class TypeEnum
    {
        public string typeId;
        public string name;
    }

    [Serializable]
    public class Part
    {
        public string partId;
        public string appearanceId;
        public string name;
        public string des;
        public string type;
        public string pic;
        public List<Effect> effect;
    }

    [Serializable]
    public class Effect
    {
        public string timeCondition;
        public string type;
        public string scope;
        public string value;
        public string percent;
        public string des;
        public Condition condition;
    }

    [Serializable]
    public class PartSpaceType
    {
        public string type;
        public string pic;
        public string light;
    }

    [Serializable]
    public class PartSpace
    {
        public string partSpaceId;
        public string coordinate;
        public string type;
    }

    [Serializable]
    public class Build
    {
        public string buildId;
        public List<string> partSpaceIdGroup;
    }

    [Serializable]
    public class OpenCondition
    {
        public string partId;
        public string name;
        public string des;
        public string type;
        public string pic;
        public List<Effect> effect;
    }

    [Serializable]
    public class Condition
    {
        public string type;
        public List<string> para;
    }
}