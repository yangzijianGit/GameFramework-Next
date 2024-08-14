using System;
using System.Collections.Generic;

namespace JsonData.String_all_Language
{
    public class All_Language
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
        public List<KW_string> kw_string;
    }

    [Serializable]
    public class KW_string
    {
        public string id;
        public string zh_CN;
        public string zh_TW;
        public string en_US;
        public string ko_KR;
        public string ja_JP;
        public string ru_RU;
    }
}