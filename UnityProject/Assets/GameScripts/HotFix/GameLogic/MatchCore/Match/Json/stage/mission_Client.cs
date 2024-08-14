using System;
using System.Collections.Generic;

namespace JsonData.Stage_mission_Client
{
    public class Mission_Client
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
        public List<Mission> mission;
    }

    [Serializable]
    public class Mission
    {
        public string missionId;
        public string levelNumber;
        public string type;
        public string feverCount;
        public string stgId;
        public string bgm;
        public string country;
        public string icon;
        public string missionName;
        public string background;
        public List<DropType> dropType;
        public List<string> clothesTips;
    }

    [Serializable]
    public class DropType
    {
        public string hypotaxis;
        public Number number;
    }

    [Serializable]
    public class Number
    {
        public string min;
        public string max;
        public string total;
    }
}