using System;
using System.Collections.Generic;

namespace JsonData.Stage_Config
{
    public class Stage_Config
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
        public string defaultAni;
        public List<Stage> stage;
    }

    [Serializable]
    public class Stage
    {
        public string index;
        public string condition;
        public string anim;
        public string rhythm;
        public List<ControlAni> controlAni;
    }

    [Serializable]
    public class ControlAni
    {
        public string tag;
        public string ctrAniId;
    }
}