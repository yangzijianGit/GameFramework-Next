using System;
using System.Collections.Generic;

namespace JsonData.Eliminate_Config
{
    public class Eliminate_Config
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
        public string doubleTime;
        public string dragDistance;
        public string m_fMaximumSpeed;
        public string m_fAcceleratedSpeed;
        public string m_fExchangeMaximumSpeed;
        public string m_fExchangeAcceleratedSpeed;
        public string fx_startTime;
        public string fx_comboTime;
        public string fx_endTime;
        public List<Combo> combo;
        public string flyTime;
    }

    [Serializable]
    public class Combo
    {
        public string index;
        public string anim;
    }
}