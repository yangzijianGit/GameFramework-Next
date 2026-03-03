# 💰 IAA 广告变现模块设计方案

> 对比市面主流IAA游戏，重新设计广告变现模块

## 1. 现有文档分析

### 当前游戏系统 (已有)
| 模块 | 状态 | 说明 |
|------|------|------|
| 消除系统 | ✅ 完整 | Match-3核心玩法 |
| 技能系统 | ✅ 完整 | 消除/生成/回合结束技能 |
| 货币系统 | ✅ 完整 | Diamond/Dust/Energy/Move/Star |
| 道具系统 | ✅ 完整 | 背包、兑换 |
| 服装系统 | ✅ 完整 | 服装收集、穿戴、技能 |
| 养成系统 | ✅ 完整 | 玩家等级、乐园等级 |
| 关卡系统 | ✅ 完整 | 普通/精英/Boss关卡 |

### 缺失的IAA模块
- 广告系统
- 广告奖励系统
- 用户分群
- 广告数据追踪

---

## 2. 市面IAA游戏模块对比

### 主流广告形式
| 广告形式 | 展示位置 | 用户体验 | 变现效率 |
|----------|----------|----------|----------|
| **Rewarded Video** | 全屏视频 | 最好(主动观看) | 最高 |
| **Interstitial** | 关卡切换/暂停 | 中等 | 高 |
| **Banner** | 底部常驻 | 较差 | 低 |
| **Native Ad** | 信息流样式 | 最好 | 中高 |
| **Offerwall** | 墙壁式列表 | 中等 | 高 |

### Match-3头部游戏变现策略
| 游戏 | IAP策略 | IAA策略 |
|------|---------|---------|
| Candy Crush | 钻石内购 | Banner+Interstitial+Rv |
| Royal Match | 道具/体力 | Rewarded Video(免费体力) |
| Toon Blast | 生命/道具 | 激励视频(免费步数) |

---

## 3. 广告模块设计

### 3.1 广告系统架构
```
AdSystem (广告系统)
├── AdConfig        # 广告配置
├── AdNetworkMgr    # 广告网络管理
│   ├── AdMob       # Google AdMob
│   ├── UnityAds    # Unity Ads
│   ├── IronSource  # 聚合SDK
│   └── Custom      # 其他广告商
├── AdPlacementMgr  # 广告位管理
├── AdRewardMgr     # 奖励发放
└── AdAnalytics    # 数据追踪
```

### 3.2 广告位设计

#### 激励视频位 (Rewarded Video)
| 广告位ID | 触发时机 | 奖励 |
|----------|----------|------|
| `rv_extra_move` | 步数耗尽 | +5步 |
| `rv_extra_time` | 时间耗尽 | +30秒 |
| `rv_extra_energy` | 体力不足 | 满体力 |
| `rv_daily_reward` | 每日奖励 | 随机货币 |
| `rv_chest_free` | 宝箱免费领取 | 宝箱奖励 |
| `rv_special_skill` | 技能冷却 | 立即可用 |

#### 插屏广告位 (Interstitial)
| 广告位ID | 触发时机 | 频率 |
|----------|----------|------|
| `inter_level_complete` | 关卡完成 | 每3关 |
| `inter_level_fail` | 关卡失败 | 每2关 |
| `inter_pause` | 暂停菜单 | 每次暂停 |
| `inter_home_enter` | 进入主界面 | 每5次 |
| `inter_shop_enter` | 进入商店 | 每次 |

#### Banner广告位 (Banner)
| 广告位ID | 位置 | 场景 |
|----------|------|------|
| `banner_main` | 底部 | 主界面 |
| `banner_level` | 底部 | 关卡选择 |
| `banner_store` | 底部 | 商店 |

### 3.3 广告配置表结构
```json
{
  "ad_placement": [
    {
      "id": "rv_extra_move",
      "type": "rewarded_video",
      "reward": {
        "type": "move",
        "value": 5
      },
      "cooldown": 0,
      "daily_limit": 3,
      "show_chance": 100
    }
  ],
  "interstitial": [
    {
      "id": "inter_level_complete",
      "cooldown_level": 3,
      "min_level": 5
    }
  ]
}
```

---

## 4. 奖励系统设计

### 4.1 奖励类型
```csharp
public enum AdRewardType
{
    Currency,     // 货币
    Item,        // 道具
    ExtraMove,   // 额外步数
    ExtraTime,   // 额外时间
    Energy,      // 体力
    Chest,       // 宝箱
    Skin,        // 皮肤
    Boost        // 加成
}
```

### 4.2 奖励配置
```csharp
[System.Serializable]
public class AdRewardConfig
{
    public string rewardId;
    public AdRewardType type;
    public int baseValue;           // 基础值
    public float multiplier;        // 倍率
    public bool isVipBoost;         // VIP是否加成
    public int dailyLimit;          // 每日上限
}
```

### 4.3 奖励发放流程
```
1. 广告观看完成回调
       ↓
2. 验证广告有效性
       ↓
3. 检查每日限制
       ↓
4. 计算奖励金额
       ↓
5. 发放奖励
       ↓
6. 记录日志/上报
       ↓
7. 播放领取动画
```

---

## 5. 用户分群策略

### 5.1 基础分群
| 用户类型 | 定义 | 广告策略 |
|----------|------|----------|
| **Non-paying** | 从未付费 | 全量广告 |
| **Whale** | 高额付费 | 极少广告/无广告 |
| **Minnow** | 小额付费 | 减少插屏 |
| **Ad-lover** | 观看广告多 | 增加广告 |

### 5.2 IAA用户分层
```csharp
public enum AdUserSegment
{
    FreeUser,      // 免费用户 - 全量广告
    LightPayer,   // 轻度付费 - 减少插屏
    HeavyPayer,   // 重度付费 - 仅激励视频
    NewUser,      // 新用户 - 前3天无广告
}
```

### 5.3 分群触发规则
- **首次付费后**: 从 FreeUser → LightPayer
- **累计消费>100元**: LightPayer → HeavyPayer
- **新用户注册**: 自动标记为 NewUser，3天后 → FreeUser

---

## 6. 广告网络管理

### 6.1 聚合SDK选择
| SDK | 特点 | 适用 |
|-----|------|------|
| **IronSource** | 聚合能力强 | 成熟项目 |
| **Unity Ads** | Unity原生 | 起步阶段 |
| **AdMob** | Google生态 | 全球市场 |
| **AppLovin** | 填充率高 | 休闲游戏 |

### 6.2 广告加载策略
```
1. 预加载激励视频
   - 进入关卡前预加载下一个
   - 保持2个广告实例可用

2. 插屏按需加载
   - 关卡结束时动态请求
   - 加载失败不影响游戏

3. 分层降级
   - 激励视频失败 → 插屏
   - 插屏失败 → Banner
   - Banner失败 → 不展示
```

---

## 7. 数据追踪

### 7.1 广告事件
```csharp
public enum AdEvent
{
    AdRequested,      // 广告请求
    AdLoaded,         // 广告加载成功
    AdShown,          // 广告展示
    AdClicked,        // 广告点击
    AdCompleted,       // 视频观看完成
    AdSkipped,        // 视频跳过
    AdFailed,         // 广告失败
    RewardGranted     // 奖励发放
}
```

### 7.2 关键指标
| 指标 | 说明 | 优化目标 |
|------|------|----------|
| eCPM | 千次展示收入 | >$15 |
| Fill Rate | 广告填充率 | >95% |
| IR | 激励视频展示率 | >40% |
| CTR | 广告点击率 | >5% |
| RV Completion | 视频完成率 | >85% |

### 7.3 数据上报
```csharp
public void TrackAdEvent(string placementId, AdEvent evt, Dictionary<string, object> data)
{
    var log = new {
        event = evt.ToString(),
        placement = placementId,
        timestamp = DateTime.UtcNow,
        userId = PlayerData.Instance.UserId,
        level = PlayerData.Instance.CurrentLevel,
        segment = GetUserSegment(),
        extra = data
    };
    Analytics.Track("ad_event", log);
}
```

---

## 8. 商业化平衡

### 8.1 广告频率上限
| 用户类型 | 激励视频/天 | 插屏/关卡 | Banner |
|----------|-------------|----------|--------|
| FreeUser | 10 | 1 | 常驻 |
| LightPayer | 15 | 1 | 常驻 |
| HeavyPayer | 无限制 | 0 | 关闭 |
| NewUser | 5 | 0 | 关闭 |

### 8.2 广告收益模型
```
日收入 = DAU × ARPU × (IAP占比 + IAA占比)
      = DAU × (付费率 × ARPPU + 人均广告展示 × eCPM/1000)
```

### 8.3 IAA+IAP混合策略
- **免费用户**: 通过广告获得原本需要付费的资源
- **付费用户**: 减少广告干扰，提升付费意愿
- **数据驱动**: A/B测试优化展示频率

---

## 9. 实现优先级

### Phase 1: 基础功能
- [ ] 广告SDK集成 (Unity Ads / IronSource)
- [ ] 激励视频广告位 (3-5个)
- [ ] 奖励发放系统
- [ ] 基础数据追踪

### Phase 2: 优化体验
- [ ] 用户分群系统
- [ ] 插屏广告位
- [ ] 频率控制
- [ ] 数据看板

### Phase 3: 商业化扩展
- [ ] Banner广告
- [ ] A/B测试框架
- [ ] 多广告SDK聚合
- [ ] 高级分析

---

## 10. 文件结构建议
```
Assets/GameScripts/HotFix/GameLogic/
├── System/AdsSystem/
│   ├── AdSystem.cs           # 主系统
│   ├── AdConfig.cs           # 配置
│   ├── AdNetworkMgr.cs       # 网络管理
│   ├── AdPlacementMgr.cs     # 广告位管理
│   ├── AdRewardMgr.cs        # 奖励管理
│   ├── AdAnalytics.cs        # 数据分析
│   └── AdUserSegment.cs      # 用户分群
```

---

## 总结

| 模块 | 当前状态 | 建议 |
|------|----------|------|
| 消除/技能/关卡 | ✅ 完整 | 保持 |
| 货币/道具/服装 | ✅ 完整 | 保持 |
| 广告系统 | ❌ 缺失 | **新增** |
| 激励视频 | ❌ 缺失 | **新增** |
| 用户分群 | ❌ 缺失 | **新增** |
| 数据追踪 | ❌ 缺失 | **新增** |

**核心建议**: 采用 "IAA + IAP" 混合变现模式，激励视频为核心(用户接受度最高)，插屏为辅，Banner仅在特定场景使用。
