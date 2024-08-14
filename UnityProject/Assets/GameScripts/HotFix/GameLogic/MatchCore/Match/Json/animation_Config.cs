using System;
using System.Collections.Generic;

namespace JsonData.Animation_Config
{
    public class Animation_Config
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
        public List<BuildSplitActionMap> buildSplitActionMap;
        public List<BuildSplitBehavior> buildSplitBehavior;
        public List<BuildSplitModel> buildSplitModel;
        public List<BuildAnimation> buildAnimation;
    }

    [Serializable]
    public class BuildSplitActionMap
    {
        public string id;
        public string actName;
    }

    [Serializable]
    public class BuildSplitBehavior
    {
        public string type;
        public List<ActionID> actionID;
    }

    [Serializable]
    public class ActionID
    {
        public string id;
    }

    [Serializable]
    public class BuildSplitModel
    {
        public string id;
        public string model;
    }

    [Serializable]
    public class BuildAnimation
    {
        public string id;
        public string name;
        public string path;
        public string IsLoop;
        public string Time;
        public string IsDestroy;
        public string scale;
        public string AnimationName;
    }
}