using System;
using System.Collections.Generic;

namespace JsonData.Fever_Config
{
    public class Fever_Config
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
        public string inFeverTime;
        public Trigger trigger;
        public Effect effect;
    }

    [Serializable]
    public class Trigger
    {
        public string energyTotal;
    }

    [Serializable]
    public class Effect
    {
        public string continueTime;
        public string costStep;
        public string playDelay;
    }
}