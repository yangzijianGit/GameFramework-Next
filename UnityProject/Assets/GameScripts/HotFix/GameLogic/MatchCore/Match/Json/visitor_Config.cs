using System;
using System.Collections.Generic;

namespace JsonData.Visitor_Config
{
    public class Visitor_Config
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
        public string quest;
        public string rollRange;
        public string rollCount;
        public string rollTime;
        public string visitorMax;
        public string speed1;
        public string speed2;
        public string walkPercent;
        public List<UnhappyPercent> unhappyPercent;
        public string walkRange;
        public List<Visitor> visitor;
        public List<VisitorSplot> visitorSplot;
        public List<ActionList> actionList;
        public List<VisitorAction> visitorAction;
        public string bubbleCD1;
        public string bubbleCD2;
        public string bubblePercent;
        public string bubbleTime;
        public VisitorBubble visitorBubble;
        public List<LoveGoods> loveGoods;
        public string appraiseMaxCount;
        public string appraiseCD1;
        public string appraiseCD2;
        public List<AppraiseStar> appraiseStar;
        public string visitorCount;
        public string appearTime;
        public string spaceVisitor;
        public string spaceBuild;
        public string spaceSmall;
        public string stopTime;
        public List<Bubble> bubble;
        public string unhappy;
    }

    [Serializable]
    public class UnhappyPercent
    {
        public string unhappy;
        public string percent;
    }

    [Serializable]
    public class Visitor
    {
        public string id;
        public string model;
        public string sex;
        public string name;
    }

    [Serializable]
    public class VisitorSplot
    {
        public string mapid;
        public string model;
    }

    [Serializable]
    public class ActionList
    {
        public string id;
        public string actName;
    }

    [Serializable]
    public class VisitorAction
    {
        public string type;
        public List<Actmaps> actmaps;
    }

    [Serializable]
    public class Actmaps
    {
        public string unhappy;
        public List<Action> action;
    }

    [Serializable]
    public class Action
    {
        public string Id;
        public string rate;
        public string loopTime;
    }

    [Serializable]
    public class VisitorBubble
    {
        public string type;
        public List<Act> act;
    }

    [Serializable]
    public class Act
    {
        public string unhappy;
        public List<Bubble> bubble;
    }

    [Serializable]
    public class Bubble
    {
        public string img;
        public string rate;
        public string type;
        public List<SkinId> skinId;
    }

    [Serializable]
    public class LoveGoods
    {
        public string parkLv;
        public List<RateGoods> rateGoods;
    }

    [Serializable]
    public class RateGoods
    {
        public string area;
        public string rateGoods1;
        public string rateGoods2;
        public string rateGoods3;
    }

    [Serializable]
    public class AppraiseStar
    {
        public string appraiseType;
        public string rate;
        public List<AppraiseDes> appraiseDes;
        public List<StarCount> starCount;
    }

    [Serializable]
    public class AppraiseDes
    {
        public List<string> star;
        public List<string> des;
    }

    [Serializable]
    public class StarCount
    {
        public string buildCountMin;
        public string buildCountMax;
        public string star1Rate;
        public string star2Rate;
        public string star3Rate;
        public string star4Rate;
        public string star5Rate;
        public string buildLvMin;
        public string buildLvMax;
        public string goodsCountMin;
        public string goodsCountMax;
        public string goodsQuality;
    }

    [Serializable]
    public class SkinId
    {
        public string skinImg;
        public string rate;
    }
}