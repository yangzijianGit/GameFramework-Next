# 🏗️ Unity Game Framework 架构文档

## 📋 项目概述
这是一个基于Unity Game Framework的模块化游戏框架，支持热更新（HybridCLR）、资源管理（YooAsset）和异步编程（UniTask）。采用清晰的分层架构设计。

## 🎯 核心架构组件

### 1. 逻辑系统层 (Logic Systems)
- **BaseLogicSys<T>**: 所有逻辑系统的抽象基类，采用单例模式
- **GameApp**: 热更新域入口，统一驱动所有逻辑系统生命周期
- **生命周期管理**: OnInit, OnStart, OnUpdate, OnLateUpdate, OnDestroy等

### 2. 流程管理系统 (Procedure System)
- **ProcedureBase**: 流程基类，继承自GameFramework状态机
- **流程状态链**: Launch → Splash → InitPackage → InitResources → LoadAssembly → StartGame
- 每个流程负责特定初始化任务，确保有序的资源加载和状态转换

### 3. 事件系统 (Event System)
- **基于属性的自动注册**: EventInterfaceImpAttribute标记事件处理器
- **分组管理**: GroupLogic, GroupUI等事件组
- **反射发现**: 自动扫描和注册事件接口实现

### 4. UI管理系统 (UI System)
- **UISystem**: 完整的窗口堆栈管理
- **窗口生命周期**: 加载 → 创建 → 刷新 → 销毁
- **层级管理**: 支持多层UI，自动深度排序和可见性控制
- **资源管理**: 支持同步/异步加载，自动资源释放

### 5. 资源配置系统
- **Luban配置**: 数据表配置管理
- **YooAsset**: 资源打包、加载和热更新
- **HybridCLR**: 热更新技术支持

### 6. 框架组件结构
- **Base组件**: 基础功能模块
- **Debugger**: 调试和性能分析
- **Resource**: 资源加载管理
- **Localization**: 多语言支持
- **Sound**: 音频管理

## 📚 架构设计原则

### 模块化设计
- 每个系统都是独立的模块，通过接口进行通信
- 逻辑系统通过BaseLogicSys基类统一管理
- 事件系统提供松耦合的通信机制

### 生命周期管理
- 游戏对象具有完整的生命周期方法
- 由GameApp统一驱动更新循环
- 资源自动释放和内存管理

### 热更新支持
- HybridCLR实现代码热更新
- YooAsset管理资源热更新
- 配置数据可通过Luban动态更新

### 扩展性
- 易于添加新的逻辑系统和功能模块
- 事件系统支持动态注册和分发
- UI系统支持自定义窗口和控件

## 🎯 核心代码结构

```
Assets/
├── GameScripts/
│   ├── HotFix/          # 热更新代码
│   │   ├── GameBase/    # 基础逻辑系统
│   │   ├── GameLogic/   # 游戏逻辑
│   │   └── GameProto/   # 配置协议
│   └── Runtime/         # 运行时代码
│       ├── Procedure/   # 流程系统
│       ├── Launcher/    # 启动器
│       └── Helper/      # 工具类
├── UnityGameFramework/  # 游戏框架核心
│   ├── Scripts/
│   │   ├── Runtime/     # 运行时组件
│   │   └── Editor/      # 编辑器工具
│   └── ResRaw/         # 资源配置
└── Packages/
    ├── YooAsset/       # 资源管理
    ├── UniTask/        # 异步编程
    └── HybridCLR/     # 热更新支持
```

## 🔧 开发指南

### 添加新逻辑系统
1. 继承BaseLogicSys<T>基类
2. 实现必要的生命周期方法
3. 在GameApp中注册系统

### 创建新流程
1. 继承ProcedureBase基类
2. 实现状态进入/退出逻辑
3. 在流程管理中配置状态转换

### 实现事件处理
1. 定义事件接口
2. 使用EventInterfaceImpAttribute标记处理类
3. 实现事件处理方法

### 开发UI窗口
1. 继承UIWindow基类
2. 使用WindowAttribute配置窗口属性
3. 实现窗口生命周期方法

## 🚀 性能优化建议

- 使用对象池管理频繁创建销毁的对象
- 合理设置资源加载优先级和依赖关系
- 利用异步编程避免阻塞主线程
- 使用调试器监控性能指标

## 📝 注意事项

- 热更新代码需要遵循HybridCLR规范
- 资源引用需要正确管理以避免内存泄漏
- 事件订阅需要及时取消订阅
- UI窗口需要正确处理可见性和深度
