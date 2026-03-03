using System;
using System.Collections.Generic;
using GameBase;
using UnityEngine;

namespace GameMain
{
    //===========================================================
    // 图鉴数据类型
    //===========================================================

    /// <summary>
    /// 图鉴类型
    /// </summary>
    public enum CollectionType
    {
        /// <summary>服装图鉴</summary>
        Clothes = 0,
        
        /// <summary>元素图鉴</summary>
        Element = 1,
        
        /// <summary>技能图鉴</summary>
        Skill = 2,
        
        /// <summary>关卡图鉴</summary>
        Level = 3,
        
        /// <summary>成就图鉴</summary>
        Achievement = 4,
    }

    /// <summary>
    /// 图鉴项数据
    /// </summary>
    [Serializable]
    public class CollectionItemData
    {
        /// <summary>物品ID</summary>
        public string ItemId;
        
        /// <summary>物品名称</summary>
        public string Name;
        
        /// <summary>物品描述</summary>
        public string Description;
        
        /// <summary>图标</summary>
        public string Icon;
        
        /// <summary>品质</summary>
        public int Quality;
        
        /// <summary>是否已解锁</summary>
        public bool IsUnlocked;
        
        /// <summary>解锁条件</summary>
        public string UnlockCondition;
        
        /// <summary>解锁时间</summary>
        public long UnlockTime;
    }

    /// <summary>
    /// 图鉴收集数据
    /// </summary>
    [Serializable]
    public class CollectionData
    {
        /// <summary>收集类型</summary>
        public CollectionType Type;
        
        /// <summary>已解锁物品列表</summary>
        public List<string> UnlockedItems = new List<string>();
        
        /// <summary>收集进度</summary>
        public int Progress;
        
        /// <summary>总物品数</summary>
        public int TotalCount;
        
        /// <summary>是否已完成</summary>
        public bool IsCompleted;
    }

    //===========================================================
    // 图鉴系统接口
    //===========================================================

    /// <summary>
    /// 图鉴系统接口
    /// </summary>
    public interface ICollectionSystem
    {
        /// <summary>获取图鉴列表</summary>

        /// <summary>初始化系统</summary>
        void Initialize();

        /// <summary>获取图鉴列表</summary>
        /// <param name="type">图鉴类型</param>
        /// <returns>图鉴数据</returns>
        CollectionData GetCollectionData(CollectionType type);

        /// <summary>获取物品详情</summary>
        /// <param name="type">图鉴类型</param>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品数据</returns>
        CollectionItemData GetItemData(CollectionType type, string itemId);

        /// <summary>获取已解锁物品列表</summary>
        /// <param name="type">图鉴类型</param>
        /// <returns>物品列表</returns>
        List<CollectionItemData> GetUnlockedItems(CollectionType type);

        /// <summary>获取收集进度</summary>
        /// <param name="type">图鉴类型</param>
        /// <returns>进度(0-1)</returns>
        float GetProgress(CollectionType type);

        /// <summary>解锁物品</summary>
        /// <param name="type">图鉴类型</param>
        /// <param name="itemId">物品ID</param>
        /// <returns>是否成功</returns>
        bool UnlockItem(CollectionType type, string itemId);

        /// <summary>领取收集奖励</summary>
        /// <param name="type">图鉴类型</param>
        /// <returns>是否成功</returns>
        bool ClaimCollectionReward(CollectionType type);

        /// <summary>保存数据</summary>
        void SaveData();

        /// <summary>加载数据</summary>
        void LoadData();
    }

    //===========================================================
    // 图鉴系统实现
    //===========================================================

    /// <summary>
    /// 图鉴系统
    /// 职责：
    /// 1. 管理各类图鉴数据
    /// 2. 收集进度追踪
    /// 3. 物品解锁管理
    /// 4. 收集奖励发放
    /// 
    /// 扩展点：
    /// 1. 可扩展图鉴类型
    /// 2. 可自定义解锁条件
    /// </summary>
    public class CollectionSystem : BaseLogicSys<CollectionSystem>, ICollectionSystem
    {
        //========================== 常量 ==========================

        private const string DATA_KEY = "CollectionData";

        //========================== 私有变量 ==========================

        /// <summary>所有物品配置</summary>
        private Dictionary<CollectionType, Dictionary<string, CollectionItemData>> m_allItems = 
            new Dictionary<CollectionType, Dictionary<string, CollectionItemData>>();

        /// <summary>玩家收集数据</summary>
        private Dictionary<CollectionType, CollectionData> m_collections = 
            new Dictionary<CollectionType, CollectionData>();

        /// <summary>是否已初始化</summary>
        private bool m_isInitialized = false;

        //========================== 生命周期 ==========================

        /// <summary>
        /// 初始化系统
        /// </summary>
        public void Initialize()
        {
            OnInit();
        }

        public override bool OnInit()
        {
            base.OnInit();

            LoadItemConfigs();
            LoadData();
            UpdateCollectionProgress();

            m_isInitialized = true;
            Debug.Log("[CollectionSystem] Initialized");
            return true;
        }

        public override void OnDestroy()
        {
            SaveData();
            m_allItems.Clear();
            m_collections.Clear();
            base.OnDestroy();
        }

        //========================== 公开接口 ==========================

        /// <summary>
        /// 获取图鉴数据
        /// </summary>
        public CollectionData GetCollectionData(CollectionType type)
        {
            if (m_collections.TryGetValue(type, out var data))
                return data;
            return null;
        }

        /// <summary>
        /// 获取物品详情
        /// </summary>
        public CollectionItemData GetItemData(CollectionType type, string itemId)
        {
            if (m_allItems.TryGetValue(type, out var items))
            {
                if (items.TryGetValue(itemId, out var item))
                    return item;
            }
            return null;
        }

        /// <summary>
        /// 获取已解锁物品
        /// </summary>
        public List<CollectionItemData> GetUnlockedItems(CollectionType type)
        {
            var result = new List<CollectionItemData>();

            if (!m_collections.TryGetValue(type, out var collection))
                return result;

            foreach (var itemId in collection.UnlockedItems)
            {
                var item = GetItemData(type, itemId);
                if (item != null)
                    result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// 获取收集进度
        /// </summary>
        public float GetProgress(CollectionType type)
        {
            if (!m_collections.TryGetValue(type, out var collection))
                return 0f;

            if (collection.TotalCount == 0) return 0f;

            return (float)collection.Progress / collection.TotalCount;
        }

        /// <summary>
        /// 解锁物品
        /// </summary>
        public bool UnlockItem(CollectionType type, string itemId)
        {
            var item = GetItemData(type, itemId);
            if (item == null)
            {
                Debug.LogWarning($"[CollectionSystem] Item not found: {itemId}");
                return false;
            }

            if (item.IsUnlocked)
            {
                Debug.LogWarning($"[CollectionSystem] Already unlocked: {itemId}");
                return false;
            }

            // 解锁
            item.IsUnlocked = true;
            item.UnlockTime = TimeUtil.GetCurrentTimeMillis();

            // 更新收集数据
            if (!m_collections.TryGetValue(type, out var collection))
                return false;

            if (!collection.UnlockedItems.Contains(itemId))
            {
                collection.UnlockedItems.Add(itemId);
                collection.Progress = collection.UnlockedItems.Count;

                // 检查是否完成
                if (collection.Progress >= collection.TotalCount)
                {
                    collection.IsCompleted = true;
                }
            }

            SaveData();
            Debug.Log($"[CollectionSystem] Item unlocked: {type}, {itemId}");
            return true;
        }

        /// <summary>
        /// 领取收集奖励
        /// </summary>
        public bool ClaimCollectionReward(CollectionType type)
        {
            if (!m_collections.TryGetValue(type, out var collection))
            {
                Debug.LogWarning($"[CollectionSystem] Collection not found: {type}");
                return false;
            }

            // 发放奖励(根据完成进度)
            int reward = collection.Progress * 10;
            // PlayerData.Instance.AddCurrency("diamond", reward);

            Debug.Log($"[CollectionSystem] Collection reward claimed: {type}, diamond x {reward}");
            return true;
        }

        //========================== 数据持久化 ==========================

        public void SaveData()
        {
            var wrapper = new CollectionDataWrapper
            {
                Collections = m_collections,
                Items = SerializeItems()
            };
            var json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(DATA_KEY, json);
            Debug.Log("[CollectionSystem] Data saved");
        }

        public void LoadData()
        {
            if (!PlayerPrefs.HasKey(DATA_KEY))
            {
                return;
            }

            try
            {
                var json = PlayerPrefs.GetString(DATA_KEY);
                var wrapper = JsonUtility.FromJson<CollectionDataWrapper>(json);
                
                if (wrapper != null)
                {
                    m_collections = wrapper.Collections ?? new Dictionary<CollectionType, CollectionData>();
                    // 恢复物品解锁状态
                    RestoreItemStates();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[CollectionSystem] Failed to load data: {e.Message}");
            }
        }

        //========================== 私有方法 ==========================

        /// <summary>
        /// 加载物品配置
        /// </summary>
        private void LoadItemConfigs()
        {
            m_allItems.Clear();

            // 服装图鉴
            m_allItems[CollectionType.Clothes] = CreateClothesCollection();

            // 元素图鉴
            m_allItems[CollectionType.Element] = CreateElementCollection();

            // 技能图鉴
            m_allItems[CollectionType.Skill] = CreateSkillCollection();

            // 初始化收集数据
            foreach (var kvp in m_allItems)
            {
                var type = kvp.Key;
                var items = kvp.Value;

                m_collections[type] = new CollectionData
                {
                    Type = type,
                    UnlockedItems = new List<string>(),
                    Progress = 0,
                    TotalCount = items.Count,
                    IsCompleted = false
                };
            }
        }

        /// <summary>
        /// 创建服装图鉴
        /// </summary>
        private Dictionary<string, CollectionItemData> CreateClothesCollection()
        {
            var items = new Dictionary<string, CollectionItemData>();

            string[,] clothesData = {
                { "clothes_001", "初始服装", "默认服装", "1" },
                { "clothes_002", "休闲装", "舒适的休闲服装", "1" },
                { "clothes_003", "运动装", "充满活力的运动装", "1" },
                { "clothes_004", "礼服", "优雅的礼服", "2" },
                { "clothes_005", "公主装", "华丽的公主服装", "2" },
                { "clothes_006", "王子装", "帅气的王子服装", "2" },
                { "clothes_007", "英雄战甲", "英雄的象征", "3" },
                { "clothes_008", "传奇套装", "传说中的装备", "3" },
            };

            for (int i = 0; i < clothesData.GetLength(0); i++)
            {
                var item = new CollectionItemData
                {
                    ItemId = clothesData[i, 0],
                    Name = clothesData[i, 1],
                    Description = clothesData[i, 2],
                    Icon = $"icon_{clothesData[i, 0]}.png",
                    Quality = int.Parse(clothesData[i, 3]),
                    IsUnlocked = false,
                    UnlockCondition = "",
                    UnlockTime = 0
                };
                items[item.ItemId] = item;
            }

            return items;
        }

        /// <summary>
        /// 创建元素图鉴
        /// </summary>
        private Dictionary<string, CollectionItemData> CreateElementCollection()
        {
            var items = new Dictionary<string, CollectionItemData>();

            string[,] elementData = {
                { "element_001", "红元素", "红色消除元素", "1" },
                { "element_002", "蓝元素", "蓝色消除元素", "1" },
                { "element_003", "绿元素", "绿色消除元素", "1" },
                { "element_004", "黄元素", "黄色消除元素", "1" },
                { "element_005", "紫元素", "紫色消除元素", "1" },
                { "element_006", "特殊元素", "特殊消除元素", "2" },
            };

            for (int i = 0; i < elementData.GetLength(0); i++)
            {
                var item = new CollectionItemData
                {
                    ItemId = elementData[i, 0],
                    Name = elementData[i, 1],
                    Description = elementData[i, 2],
                    Icon = $"icon_{elementData[i, 0]}.png",
                    Quality = int.Parse(elementData[i, 3]),
                    IsUnlocked = false,
                    UnlockCondition = "",
                    UnlockTime = 0
                };
                items[item.ItemId] = item;
            }

            return items;
        }

        /// <summary>
        /// 创建技能图鉴
        /// </summary>
        private Dictionary<string, CollectionItemData> CreateSkillCollection()
        {
            var items = new Dictionary<string, CollectionItemData>();

            string[,] skillData = {
                { "skill_001", "火焰爆炸", "消除时产生爆炸", "1" },
                { "skill_002", "冰冻射线", "消除时冰冻目标", "1" },
                { "skill_003", "雷电打击", "消除时召唤雷电", "2" },
                { "skill_004", "治疗光环", "恢复生命值", "2" },
                { "skill_005", "彩虹球", "消除所有同色", "3" },
                { "skill_006", "核武器", "大规模消除", "3" },
            };

            for (int i = 0; i < skillData.GetLength(0); i++)
            {
                var item = new CollectionItemData
                {
                    ItemId = skillData[i, 0],
                    Name = skillData[i, 1],
                    Description = skillData[i, 2],
                    Icon = $"icon_{skillData[i, 0]}.png",
                    Quality = int.Parse(skillData[i, 3]),
                    IsUnlocked = false,
                    UnlockCondition = "",
                    UnlockTime = 0
                };
                items[item.ItemId] = item;
            }

            return items;
        }

        /// <summary>
        /// 序列化物品(用于保存)
        /// </summary>
        private Dictionary<string, bool> SerializeItems()
        {
            var result = new Dictionary<string, bool>();

            foreach (var kvp in m_allItems)
            {
                foreach (var item in kvp.Value)
                {
                    result[$"{kvp.Key}_{item.Key}"] = item.Value.IsUnlocked;
                }
            }

            return result;
        }

        /// <summary>
        /// 恢复物品状态
        /// </summary>
        private void RestoreItemStates()
        {
            // 从保存的数据恢复物品解锁状态
            // 这里简化处理，实际需要更复杂的序列化
        }

        /// <summary>
        /// 更新收集进度
        /// </summary>
        private void UpdateCollectionProgress()
        {
            foreach (var kvp in m_collections)
            {
                var collection = kvp.Value;
                if (m_allItems.TryGetValue(kvp.Key, out var items))
                {
                    collection.TotalCount = items.Count;
                    collection.Progress = collection.UnlockedItems.Count;
                    collection.IsCompleted = collection.Progress >= collection.TotalCount;
                }
            }
        }
    }

    //===========================================================
    // 数据包装类
    //===========================================================

    [Serializable]
    public class CollectionDataWrapper
    {
        public Dictionary<CollectionType, CollectionData> Collections;
        public Dictionary<string, bool> Items;
    }
}
