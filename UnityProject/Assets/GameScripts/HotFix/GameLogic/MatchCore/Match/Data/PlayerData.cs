using System;
using System.Collections;
using System.Collections.Generic;
using jc;


namespace Data
{
    //玩家数据类（单例）
    public sealed class PlayerData : GameBase.Singleton<PlayerData>
    {
        /** 属性变量 **/
        //等级、名字
        private int m_nLevel = 0;
        private string m_strName = string.Empty;
        private long m_strId;

        //乐园等级
        private int m_nParkLevel = 0;

        //钞票、金币、幸福星(升级建筑)
        private long m_Diamond = 0;
        private long m_nDust = 0;
        private long m_nHappinessStar = 0;
        private long m_mEnergy = 0;
        private long m_move = 0;

        public long lCheckFeverCount
        {
            set;
            get;
        }
        private long m_lFeverPowerNum;
        public long lFeverPowerNum
        {
            set
            {
                m_lFeverPowerNum = value;
                jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_STAGE_FEVER_CHANGEFEVERPRO, (float) Data.PlayerData.Instance.lFeverPowerNum / lCheckFeverCount);
            }
            get
            {
                return m_lFeverPowerNum;
            }
        }

        //扭蛋时间
        private long Banner_time;
        public long BannerTime
        {
            get { return Banner_time; }
            set { Banner_time = value; }
        }

        private Dictionary<string, int> m_mpItem = new Dictionary<string, int>();
        public class ClothesInfo
        {
            public int m_nLevel;
            public int m_nExp; // 不知道是什么， 但是服务器发了 
        }
        private Dictionary<string, ClothesInfo> m_mpClothes = new Dictionary<string, ClothesInfo>();
        private string m_strCurrentClothId;
        private string m_strClothSkillType;
        public string CurrentClothId
        {
            set
            {
                m_strCurrentClothId = value;
                m_strClothSkillType = Config.ClothesConfig.getClothSkillIndexType(value);
                jc.EventManager.Instance.NoticeEvent((int) jc.STAGEEVENTTYPE.ET_UI_BATTLEREADYCLOTHES_cloth_change, value);
            }
            get
            {
                return m_strCurrentClothId;
            }
        }

        public string CurrentSkillType
        {
            get
            {
                return m_strClothSkillType;
            }
        }

        /// <summary>
        /// 营业额
        /// </summary>
        private long m_gold = 0;
        /// <summary>
        /// 改變的营业额
        /// </summary>
        private long m_changeGold = 0;

        /// <summary>
        /// 历史最高营业额
        /// </summary>
        private long m_historyMaxGold = 0;

        //无限体力剩余时间
        private long m_infinityEnergyTime;
        public long InifinityEnergyTime
        {
            get
            {
                return m_infinityEnergyTime;
            }
            set
            {
                m_infinityEnergyTime = value;
            }
        }

        ///反沉迷等级
        private double m_anti = 0;
        public double Anti
        {
            get { return m_anti; }
            set { m_anti = value; }
        }
        private string m_strCurrentMissionId;
        JsonData.Stage_mission_Client.Mission m_tCurrentMission;

        /** 构造函数 **/
        public PlayerData() { }

        public void Init()
        {
            
        }

        private void S_ParkLevelUp(object e)
        {
            // m_nParkLevel = (e as S_ParkLevelUp).ParkLevel;
            // EventManager.Instance.NoticeEvent((int) CommonEventType.ET_ParkLevelUp);
        }
        public int getItemCount(string strItemId)
        {
            int nCount = 0;
            if (m_mpItem.ContainsKey(strItemId) == false)
            {
                UnityEngine.Debug.LogError("ERROR: this item is not in map."+strItemId);
            }
            else
            {
                nCount = m_mpItem[strItemId];
            }
            return nCount;
        }

        /// <summary>
        /// 根据货币种类和数量，判断数量是否足够
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
      

        /// <summary>
        /// 获得货币数量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long GetCurrencyCount(string type)
        {
            switch (type)
            {
                case CurrencyType.Diamond:
                    return m_Diamond;
                case CurrencyType.dust:
                    return m_nDust;
                case CurrencyType.energy:
                    return m_mEnergy;
                case CurrencyType.move:
                    return m_move;
                case CurrencyType.star:
                    return m_nHappinessStar;
                default:
                    break;
            }
            return 0;
        }

        /** 操作属性变量 **/
        public int _Level
        {
            get { return this.m_nLevel; }
            set { this.m_nLevel = value; }
        }

        public string _Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        /// <summary>
        /// 营业额数据
        /// </summary>
        public long _Gold
        {
            get { return m_gold; }
            set
            {
                m_changeGold = value - m_gold;
                m_gold = value;
            }
        }

        /// <summary>
        /// 历史最高营业额.
        /// </summary>
        public long _historyMaxGold
        {
            get { return m_historyMaxGold; }
            set { m_historyMaxGold = value; }
        }

        public int _ParkLevel
        {
            get { return m_nParkLevel; }
            set { m_nParkLevel = value; }
        }

        public long _ID
        {
            get { return this.m_strId; }
            set { this.m_strId = value; }
        }
        public long _Diamond
        {
            get { return m_Diamond; }
            set { m_Diamond = value; }
        }

        public long _Dust
        {
            get { return this.m_nDust; }
            set { this.m_nDust = value; }
        }

        public long _HappinessStar
        {
            get { return this.m_nHappinessStar; }
            set
            {
                this.m_nHappinessStar = value;
            }
        }

        public long _Move
        {
            get { return m_move; }
            set { m_move = value; }
        }

        public long _Energy { get => m_mEnergy; set => m_mEnergy = value; }

        public string CurrentMissionId
        {
            get
            {
                return m_strCurrentMissionId;
            }
            set
            {
                m_strCurrentMissionId = value;
                m_tCurrentMission = Config.MissionConfig.getMissionConfig(m_strCurrentMissionId);
            }
        }

        public JsonData.Stage_mission_Client.Mission CurrentMissionData
        {
            get
            {
                return m_tCurrentMission;
            }
        }

        public long ChangeGold { get => m_changeGold; }

        public void addCloth(string strClothId, int nLevel, int nExp)
        {
            m_mpClothes[strClothId] = new ClothesInfo() { m_nExp = nExp, m_nLevel = nLevel };
        }

        public void refreshCloth(string strClothId, int nLevel, int nExp)
        {
            if (m_mpClothes.ContainsKey(strClothId) == true)
            {
                m_mpClothes[strClothId].m_nLevel = nLevel;
                m_mpClothes[strClothId].m_nExp = nExp;
            }
            else
            {
                addCloth(strClothId, nLevel, nExp);
            }
        }

        public int getClothLevel(string strClothId)
        {
            if (m_mpClothes.ContainsKey(strClothId) == true)
            {
                return m_mpClothes[strClothId].m_nLevel;
            }
            return -1;
        }

        public int getClothExp(string strClothId)
        {
            if (m_mpClothes.ContainsKey(strClothId) == true)
            {
                return m_mpClothes[strClothId].m_nExp;
            }
            return -1;
        }

        public bool isClothExist(string strClothId)
        {
            return getClothLevel(strClothId) != -1;
        }

    }
}