using System;
using System.Collections.Generic;

namespace JsonData.ElementBehavior_Config
{
    public class ElementBehavior_Config
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
        public AniArg aniArg;
        public List<Element> element;
    }

    [Serializable]
    public class AniArg
    {
        public List<Tag> tag;
    }

    [Serializable]
    public class Tag
    {
        public string id;
        public string value;
    }

    [Serializable]
    public class Element
    {
        public string id;
        public List<Behavior> behavior;
    }

    [Serializable]
    public class Behavior
    {
        public string id;
        public Show show;
    }

    [Serializable]
    public class Show
    {
        public List<string> ani;
    }
}