using System;
using System.Collections.Generic;

namespace JsonData.Build_Appearance
{
    public class Build_Appearance
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
        public Space space;
        public List<AppearanceGroup> appearanceGroup;
    }

    [Serializable]
    public class Space
    {
        public string skinImg;
    }

    [Serializable]
    public class AppearanceGroup
    {
        public string id;
        public BldState bldState;
    }

    [Serializable]
    public class BldState
    {
        public List<SkinGroup> skinGroup;
    }

    [Serializable]
    public class SkinGroup
    {
        public string skinId;
        public string skinImg;
        public List<ClickPoint> clickPoint;
        public List<Split> split;
        public List<Action> action;
        public List<Nav> nav;
        public List<DoolPoint> doolPoint;
        public string unlockDes;
        public GoTo goTo;
        public Effect effect;
        public UnlockPrice unlockPrice;
    }

    [Serializable]
    public class ClickPoint
    {
        public string element;
    }

    [Serializable]
    public class Split
    {
        public string Img;
        public string type;
        public string tier;
        public string coordinate;
        public string animation;
        public string aniType;
        public string spinePart;
    }

    [Serializable]
    public class Action
    {
        public string element;
        public string type;
        public string tier;
        public string direction;
        public List<ActVistor> actVistor;
        public List<AnimationVistor> animationVistor;
    }

    [Serializable]
    public class ActVistor
    {
        public string Id;
        public string rate;
    }

    [Serializable]
    public class AnimationVistor
    {
        public string Id;
        public string rate;
    }

    [Serializable]
    public class Nav
    {
        public List<NavObstacle> navObstacle;
    }

    [Serializable]
    public class NavObstacle
    {
        public string element;
    }

    [Serializable]
    public class DoolPoint
    {
        public string element;
    }

    [Serializable]
    public class GoTo
    {
        public string type;
        public List<string> para;
    }

    [Serializable]
    public class Effect
    {
        public string type;
        public string scope;
        public string value;
        public string des;
    }

    [Serializable]
    public class UnlockPrice
    {
        public string currency;
        public string value;
    }
}