using System;
using System.Collections.Generic;

namespace JsonData.Active_NpcInteraction
{
    public class Active_NpcInteraction
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
        public Period period;
        public List<SampleInfo> sampleInfo;
    }

    [Serializable]
    public class Period
    {
        public List<WaveAreaLimit> waveAreaLimit;
        public Ani ani;
    }

    [Serializable]
    public class WaveAreaLimit
    {
        public string areaUnlock;
        public List<string> eventPoint;
        public List<string> refreshTime;
        public string waveCount;
        public string stayTime;
        public List<string> sample;
    }

    [Serializable]
    public class Ani
    {
        public string ani1;
        public string ani2;
        public string ani3;
        public string ani4;
        public string ani5;
        public string ani6;
        public string ani7;
        public string ani8;
    }

    [Serializable]
    public class SampleInfo
    {
        public string id;
        public string single;
        public List<string> img;
        public Ani ani;
        public ImgInfo imgInfo;
    }

    [Serializable]
    public class ImgInfo
    {
        public List<string> group1;
        public List<string> group2;
    }
}