using System;
using System.Collections.Generic;

namespace JsonData.ENateAni_Config
{
    public class ENateAni_Config
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
        public string gameid;
        public Animation animation;
    }

    [Serializable]
    public class Animation
    {
        public List<Ani> ani;
    }

    [Serializable]
    public class Ani
    {
        public string id;
        public List<FrameAni> frameAni;
        public string lockTime;
    }

    [Serializable]
    public class FrameAni
    {
        public string triggerTime;
        public SpineAni spineAni;
        public RectTransform rectTransform;
        public Create create;
        public Del del;
        public Particle particle;
    }

    [Serializable]
    public class SpineAni
    {
        public string objId;
        public string aniName;
        public string skin;
        public string skeletonDataName;
        public string pos;
        public string loop;
        public string timeScale;
    }

    [Serializable]
    public class RectTransform
    {
        public string objId;
        public string pos;
        public string rotation;
        public string scale;
        public Bezier bezier;
        public string anchors;
        public string pivot;
    }

    [Serializable]
    public class Bezier
    {
        public string duration;
        public string withRotation;
        public List<string> infPoint;
    }

    [Serializable]
    public class Create
    {
        public string prefab;
        public string objId;
        public string parent;
    }

    [Serializable]
    public class Del
    {
        public string objId;
    }

    [Serializable]
    public class Particle
    {
        public string objId;
        public string loopCount;
    }
}