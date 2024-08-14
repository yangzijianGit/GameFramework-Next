using System;
using System.Collections.Generic;

namespace JsonData.System_Config
{
    public class System_Config
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
        public string fansDayTimes;
        public string fansPercent;
        public string fansPercentMax;
        public FreeMoveInfo freeMoveInfo;
        public string visitors;
        public string visitorsRTime;
        public string standardTime;
        public string buttonPeriod;
        public List<Avatar> avatar;
        public EnergyInfo energyInfo;
        public KW_default kw_default;
        public string energyCost;
        public string neoShowProbability;
        public string neoShowTargetNum;
        public CurrencyRefresh currencyRefresh;
        public string assistId;
        public string assistCost;
        public string mailOutTimeDay;
        public string clothesBegin;
    }

    [Serializable]
    public class FreeMoveInfo
    {
        public string noMoveDes;
        public string dayFreeUpLvTimes;
        public string dayFreeRollTimes;
        public string windowDes01;
        public string windowDes02;
        public string bottonName;
        public string freeTimesDes;
    }

    [Serializable]
    public class Avatar
    {
        public string id;
        public string imgIcon;
    }

    [Serializable]
    public class EnergyInfo
    {
        public string max;
        public string recoverTime;
        public string rechargeCostItemId1;
    }

    [Serializable]
    public class KW_default
    {
        public string energy;
        public string diamond;
        public string star;
        public string move;
    }

    [Serializable]
    public class CurrencyRefresh
    {
        public string currencyType;
        public string costCount;
    }
}