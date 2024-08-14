using System;
using System.Collections.Generic;

namespace JsonData.Build_Config
{
    public class Build_Config
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
        public List<BuildGroup> buildGroup;
        public List<Build> build;
    }

    [Serializable]
    public class BuildGroup
    {
        public string id;
        public List<string> buildId;
        public string minArae;
        public string areaIndex;
    }

    [Serializable]
    public class Build
    {
        public string id;
        public string name;
        public string des;
        public string log;
        public string coordinate;
        public string stopTime;
        public string title;
        public string type;
        public string appearanceId;
        public string animationFinish;
        public string animationDelete;
        public string animationBuild;
        public AttachBld attachBld;
    }

    [Serializable]
    public class AttachBld
    {
        public List<string> attachBldId;
    }
}