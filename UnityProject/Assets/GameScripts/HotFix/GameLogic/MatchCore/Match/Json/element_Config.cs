using System;
using System.Collections.Generic;

namespace JsonData.Element_Config
{
    public class Element_Config
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
        public List<Element> element;
    }

    [Serializable]
    public class Element
    {
        public string id;
        public string hypotaxis;
        public string type;
        public string level;
        public string name;
        public string Log;
        public string skin;
        public string color;
        public string canMove;
        public string involvedCompose;
        public string moveType;
        public List<string> destroyedType;
        public EliminateCreateDestroy eliminateCreateDestroy;
        public string m_bIsCellOccupy;
        public string m_bIsRandomElement;
        public string destroyTo;
        public string forbiddenMove;
        public string forbiddenCompose;
        public List<string> stopOtherDestroyType;
        public List<string> stopMineDestroyType;
        public string nWidth;
        public string nHeight;
        public string canGrow;
        public PassCreateDestroy passCreateDestroy;
        public string eliminateTransmit;
        public string dirEliminateTransmit;
        public string eliminateTime;
        public string reputationId;
        public string followBasic;
        public string fixType;
        public string inBasketType;
        public string exitid;
    }

    [Serializable]
    public class EliminateCreateDestroy
    {
        public Mine mine;
        public Left left;
        public Right right;
        public Up up;
        public Down down;
    }

    [Serializable]
    public class Mine
    {
        public string CdestroyType;
    }

    [Serializable]
    public class Left
    {
        public string CdestroyType;
    }

    [Serializable]
    public class Right
    {
        public string CdestroyType;
    }

    [Serializable]
    public class Up
    {
        public string CdestroyType;
    }

    [Serializable]
    public class Down
    {
        public string CdestroyType;
    }

    [Serializable]
    public class PassCreateDestroy
    {
        public string condition;
        public string CdestroyType;
    }
}