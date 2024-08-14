using System;
using System.Collections.Generic;

namespace JsonData.Pose_Config
{
    public class Pose_Config
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
        public string inPosePercent;
        public string misTime;
        public List<PoseWeight> poseWeight;
        public List<SkillList> skillList;
    }

    [Serializable]
    public class PoseWeight
    {
        public string index;
        public string weight;
    }

    [Serializable]
    public class SkillList
    {
        public string index;
        public string name;
        public string comboNumber;
        public string namelog;
        public string imgIcon;
        public List<Skill> skill;
    }

    [Serializable]
    public class Skill
    {
        public string id;
    }
}