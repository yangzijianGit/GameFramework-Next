using System;
using System.Collections.Generic;

namespace JsonData.DropConfig
{
    public class DropConfig
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
        public string prefab;
        public List<DropNode> DropNode;
    }

    [Serializable]
    public class DropNode
    {
        public string id;
        public string skin;
        public string m_bIsShowSkin;
        public List<M_pDropStrategy> m_pDropStrategy;
    }

    [Serializable]
    public class M_pDropStrategy
    {
        public List<string> m_strElementId;
        public string m_nPower;
    }
}