using System;
using System.Collections.Generic;

namespace JsonData.Skill_Config
{
    public class Skill_Config
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
        public List<SkillList> skillList;
    }

    [Serializable]
    public class SkillList
    {
        public string id;
        public string name;
        public string namelog;
        public string imgIcon;
        public List<Group> group;
    }

    [Serializable]
    public class Group
    {
        public List<Skill> skill;
    }

    [Serializable]
    public class Skill
    {
        public Target target;
        public string SkillAnimationId;
        public string time;
        public string count;
        public List<string> destroyType;
        public List<string> area;
        public string animationId;
        public List<string> subSkill;
        public List<Reputation> reputation;
        public List<string> prefixCondition;
        public List<Ex_Reputation> Ex_Reputation;
        public List<TargetGridReputation> TargetGridReputation;
        public List<Direction> direction;
        public WaitDrop waitDrop;
        public Ray ray;
        public ChangeElement changeElement;
        public string effectPrefab;
        public SubTractTarget subTractTarget;
        public Move move;
        public string costStep;
    }

    [Serializable]
    public class Target
    {
        public string type;
        public List<string> point;
        public List<string> condition;
    }

    [Serializable]
    public class Reputation
    {
        public string id;
        public string value;
    }

    [Serializable]
    public class Ex_Reputation
    {
        public string id;
        public string value;
    }

    [Serializable]
    public class TargetGridReputation
    {
        public string id;
        public string value;
    }

    [Serializable]
    public class Direction
    {
        public string dir;
        public string delay;
        public string flyAni;
        public string offect;
    }

    [Serializable]
    public class WaitDrop
    {
        public string isWait;
        public string endTime;
    }

    [Serializable]
    public class Ray
    {
        public RayAni rayAni;
        public string RayDelay;
        public string StandDelay;
    }

    [Serializable]
    public class RayAni
    {
        public List<string> rayAniId;
    }

    [Serializable]
    public class ChangeElement
    {
        public string id;
        public string type;
        public List<string> condition;
    }

    [Serializable]
    public class SubTractTarget
    {
        public string hypotaxisId;
        public string num;
    }

    [Serializable]
    public class Move
    {
        public string level;
        public string moveType;
    }
}