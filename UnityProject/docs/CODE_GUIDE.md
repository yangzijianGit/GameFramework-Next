# 📖 Unity Game Framework 代码说明文档

本指南详细解释项目的核心代码结构、关键类实现和使用方法。基于Unity Game Framework的模块化设计。

## 1. 逻辑系统核心代码

### BaseLogicSys.cs
**文件路径**: `Assets/GameScripts/HotFix/GameBase/BaseLogicSys.cs`

```csharp
public abstract class BaseLogicSys<T> : ILogicSys where T : new()
{
    private static T _instance;
    public static T Instance => _instance ?? (_instance = new T());
    
    public virtual bool OnInit() => true;
    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    // ... 其他生命周期方法
}
```

**说明**:
- 单例模式实现，支持热更新域。
- 生命周期方法由GameApp统一调用，避免多余MonoBehaviour。
- `OnInit()` 用于初始化，`OnUpdate()` 为主要更新循环。

**使用示例**:
```csharp
public class UISystem : BaseLogicSys<UISystem> { }
UISystem.Instance.OnInit(); // 初始化
```

### GameApp.cs
**文件路径**: `Assets/GameScripts/HotFix/GameLogic/GameApp.cs`

```csharp
public partial class GameApp : Singleton<GameApp>
{
    public static void Entrance(object[] objects)
    {
        s_HotfixAssembly = (List<Assembly>)objects[0];
        Instance.InitSystem();
        Instance.Start();
        // 绑定Unity事件
        Utility.Unity.AddUpdateListener(Instance.Update);
    }
    
    private void Update()
    {
        foreach (var logic in m_ListLogicMgr)
        {
            logic.OnUpdate();
        }
    }
}
```

**说明**:
- 热更新入口点，接收Assembly列表。
- 驱动所有逻辑系统的更新循环，使用TProfiler进行性能分析。
- `Shutdown()` 处理游戏关闭，支持重启模式。

## 2. 流程系统代码

### ProcedureBase.cs
**文件路径**: `Assets/GameScripts/Runtime/Procedure/ProcedureBase.cs`

```csharp
public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
{
    public abstract bool UseNativeDialog { get; }
}
```

**说明**:
- 继承GameFramework的状态机基类。
- `UseNativeDialog` 控制是否使用原生对话框。

### ProcedureLaunch.cs & ProcedureSplash.cs
**示例状态流程**:
```csharp
public class ProcedureLaunch : ProcedureBase
{
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        InitLanguageSettings();
        InitSoundSettings();
    }
    
    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        ChangeState<ProcedureSplash>(procedureOwner); // 切换到下一个状态
    }
}
```

**说明**:
- 启动流程：语言和声音初始化。
- Splash流程：加载文本、UI初始化和资源包准备。
- 通过`ChangeState<T>()` 管理状态转换。

## 3. 事件系统代码

### EventInterfaceHelper.cs
**文件路径**: `Assets/GameScripts/HotFix/GameLogic/Event/EventInterfaceHelper.cs`

```csharp
public class EventInterfaceHelper
{
    public static void Init()
    {
        RegisterEventInterface_Logic.Register(GameEvent.EventMgr);
        RegisterEventInterface_UI.Register(GameEvent.EventMgr);
    }
}
```

**说明**:
- 初始化事件接口注册，支持Logic和UI分组。

### RegisterEventInterface_Logic.cs
```csharp
class RegisterEventInterface_Logic
{
    public static void Register(EventMgr mgr)
    {
        var types = CodeTypes.Instance.GetTypes(typeof(EventInterfaceImpAttribute));
        foreach (Type type in types)
        {
            var attr = type.GetCustomAttribute<EventInterfaceImpAttribute>();
            if (attr?.EventGroup == EEventGroup.GroupLogic)
            {
                var obj = Activator.CreateInstance(type, mgr.Dispatcher);
                mgr.RegWrapInterface(obj.GetType().GetInterfaces()[0]?.FullName, obj);
            }
        }
    }
}
```

**说明**:
- 使用反射扫描带有`EventInterfaceImpAttribute`的类型。
- 自动创建实例并注册到EventMgr，支持分组过滤。

**使用示例**:
```csharp
[EventInterfaceImp(EEventGroup.GroupLogic)]
public class MyEventHandler : IEventInterface { }
```

## 4. UI系统代码

### UISystem.cs
**文件路径**: `Assets/GameScripts/HotFix/GameLogic/System/UISystem/UISystem.cs`

```csharp
public sealed partial class UISystem : BaseLogicSys<UISystem>
{
    private readonly List<UIWindow> _stack = new List<UIWindow>(128);
    
    public void ShowUIAsync<T>(params object[] userDatas) where T : UIWindow
    {
        ShowUIImp(typeof(T), true, userDatas);
    }
    
    private void ShowUIImp(Type type, bool isAsync, params object[] userDatas)
    {
        string windowName = type.FullName;
        if (IsContains(windowName))
        {
            var window = GetWindow(windowName);
            Pop(window);
            Push(window);
            window.TryInvoke(OnWindowPrepare, userDatas);
        }
        else
        {
            var window = CreateInstance(type);
            Push(window);
            window.InternalLoad(window.AssetName, OnWindowPrepare, isAsync, userDatas).Forget();
        }
    }
    
    public void CloseUI<T>() where T : UIWindow
    {
        var window = GetWindow(typeof(T).FullName);
        if (window != null)
        {
            window.InternalDestroy();
            Pop(window);
            OnSortWindowDepth(window.WindowLayer);
        }
    }
}
```

**说明**:
- 窗口堆栈管理：Push/Pop控制显示顺序。
- 深度排序：`OnSortWindowDepth()` 按层级计算Canvas深度。
- 可见性控制：全屏窗口自动隐藏下层。
- 异步加载：使用UniTask支持非阻塞UI加载。

### UIWindow.cs (关键属性)
- `WindowLayer`: UI层级 (UILayer枚举)
- `FullScreen`: 是否全屏
- `HideTimeToClose`: 隐藏后自动关闭时间
- `AssetName`: 对应Prefab路径

**使用示例**:
```csharp
[WindowAttribute(WindowLayer.UI, true, "UILoginForm.prefab")]
public class UILoginForm : UIWindow
{
    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        // 初始化UI逻辑
    }
}
```

## 5. 资源配置代码

### ConfigSystem.cs
**文件路径**: `Assets/GameScripts/HotFix/GameProto/ConfigSystem.cs`

```csharp
public class ConfigSystem : BaseLogicSys<ConfigSystem>
{
    // Luban生成的配置管理
    public static Tables Tables { get; private set; }
    
    public override void OnInit()
    {
        Tables = new Tables();
        LoadAllConfigs();
    }
    
    private void LoadAllConfigs()
    {
        // 加载Luban生成的配置表
        Tables.LoadItemExchange();
        Tables.LoadTbItem();
    }
}
```

**说明**:
- 通过Luban生成的Bean类管理静态配置。
- `Tables.cs` 作为配置入口，托管所有表数据。

### BeanBase.cs (Luban基础类)
- 支持序列化和反序列化
- 自动属性绑定和验证

## 6. 框架组件说明

### Resource Component
- 扩展GameFramework的资源加载，支持YooAsset集成。
- 自动处理资源依赖和卸载。

### Sound Component
- 音频组管理：Music, Sound, UISound
- 持久化设置：静音和音量通过Setting保存。

### Localization
- 支持多语言切换：English, ChineseSimplified等
- 动态加载本地化文本表。

## 7. 代码最佳实践

- **单例使用**: 仅在系统级使用，避免滥用。
- **异步编程**: UI加载和资源操作使用UniTask避免阻塞。
- **事件驱动**: 跨模块通信使用事件系统，避免直接引用。
- **资源管理**: 始终通过Resource.Load/Unload操作资源。
- **热更新**: 游戏逻辑代码放置在HotFix目录，遵循HybridCLR规范。

## 8. 常见问题与解决方案

- **UI深度重叠**: 检查WindowLayer和FullScreen属性。
- **事件未触发**: 验证EventInterfaceImpAttribute标记和注册调用。
- **资源加载失败**: 检查YooAsset配置和依赖关系。
- **热更新异常**: 确保DLL兼容性和Assembly引用正确。

此文档将随着项目发展而更新，建议结合ARCHITECTURE.md和TODO.md使用。
