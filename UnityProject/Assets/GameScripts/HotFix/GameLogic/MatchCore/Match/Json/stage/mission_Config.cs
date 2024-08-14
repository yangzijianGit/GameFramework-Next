using System;
using System.Collections.Generic;

namespace JsonData.Stage_mission_Config
{
    public class Mission_Config
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
        public AddStep addStep;
        public string energyCost;
    }

    [Serializable]
    public class AddStep
    {
        public Cost cost;
        public string times;
        public string steps;
    }

    [Serializable]
    public class Cost
    {
        public string type;
        public string value;
    }
}