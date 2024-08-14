using System;
using System.Collections.Generic;

namespace JsonData.Ui_UIData
{
    public class UIData
    {
        public Root root;
    }

    [Serializable]
    public class Root
    {
        public string group;
        public string client;
        public Game game;
    }

    [Serializable]
    public class Game
    {
        public string gameId;
        public string prefab;
        public string type;
        public string dontdestroy;
        public string IsCloseCurrentWindow;
    }
}