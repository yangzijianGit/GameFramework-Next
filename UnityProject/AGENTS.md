# AGENTS.md - Unity Game Framework Development Guide

This file provides essential information for agentic coding agents working with this Unity Game Framework project.

## Project Overview

This is a Unity 6000.3.10f1 project using a custom Game Framework with hot-reload support via HybridCLR, resource management via YooAsset, and async programming via UniTask. The project follows a modular architecture with clear separation between runtime code, hot-fix code, and framework components.

## Build and Test Commands

### Unity Commands
- **Build Player**: Use Unity Editor `File > Build Settings` or Unity CLI
- **Run Tests**: Unity Test Runner via `Window > General > Test Runner`
- **Enter Play Mode**: Unity Editor Play button or via Unity CLI automation

### Development Commands
- **Compile Assemblies**: Unity automatically compiles when scripts change
- **Refresh Assets**: `Assets > Refresh` or Ctrl+R in Unity Editor
- **Clear Console**: Unity Console window clear button
- **Run Specific Scene**: Open scene and use Play Mode

### Hot Reload and Resource Updates
- **Generate Hot-fix DLL**: Via Unity Editor menu or build scripts
- **Update YooAsset Bundles**: Through YooAsset editor tools
- **Luban Config Generation**: Via custom editor tools in `Assets/GameScripts/Editor/EditorTools/LubanTools.cs`

### Single Test Execution
Since this project doesn't appear to have extensive unit tests set up:
- Create test files in `Assets/Tests/` directory if needed
- Use Unity Test Runner to run individual tests
- For manual testing, create specific test scenes

## Code Style Guidelines

### Naming Conventions
- **Classes**: PascalCase (e.g., `ProcedureSplash`, `UISystem`, `BaseLogicSys<T>`)
- **Methods**: PascalCase (e.g., `OnInit()`, `ShowUIAsync()`, `ChangeState<T>()`)
- **Variables**: camelCase for locals, `_camelCase` for private fields
- **Constants**: PascalCase (e.g., `AssetPriority`, `FadeVolumeDuration`)
- **Interfaces**: Prefix with 'I' (e.g., `ILogicSys`, `IEventInterface`)
- **Namespaces**: PascalCase (e.g., `GameMain`, `GameBase`, `GameLogic.System.UISystem`)

### File Organization
```
Assets/
├── GameScripts/
│   ├── HotFix/              # Hot-reloadable code
│   │   ├── GameBase/       # Base logic systems
│   │   ├── GameLogic/      # Game logic implementation
│   │   └── GameProto/      # Protocol and config
│   ├── Runtime/             # Non-hot-fix runtime code
│   │   ├── Procedure/      # Game flow states
│   │   ├── Launcher/        # Launcher UI
│   │   └── Helper/         # Utility classes
│   └── Editor/              # Editor tools
├── UnityGameFramework/      # Core framework
└── Packages/                # External packages
```

### Import Organization
```csharp
// System namespaces first
using System;
using System.Collections.Generic;

// Unity namespaces
using UnityEngine;
using UnityEngine.UI;

// Third-party namespaces
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Fsm;

// Project namespaces
using GameMain;
using GameBase;
```

### Type and Style Guidelines

#### Generic Types
- Use `<T>` for generic type parameters
- Prefer `where T : new()` for creatable types
- Use descriptive generic names like `TWindow`, `TSystem`

#### Async Programming
- Use `UniTask` instead of `Task` for Unity async operations
- Use `async UniTaskVoid` for fire-and-forget methods
- Always call `.Forget()` on fire-and-forget UniTasks
- Use `await UniTask.Delay()` for delays instead of `Coroutine`

```csharp
private async UniTaskVoid StartGame()
{
    await UniTask.Delay(TimeSpan.FromSeconds(1f));
    UILoadMgr.HideAll();
}
```

#### Event System Patterns
- Use `[EventInterfaceImp(EEventGroup.GroupLogic)]` attributes
- Implement event interfaces rather than direct method calls
- Register events through `EventInterfaceHelper`

#### UI Development
- Inherit from `UIWindow` for UI forms
- Use `[WindowAttribute(WindowLayer.UI, true, "PrefabPath.prefab")]`
- Implement lifecycle methods: `OnOpen()`, `OnClose()`, `OnUpdate()`
- Use `ShowUIAsync<T>()` and `CloseUI<T>()` for window management

#### Logic Systems
- Inherit from `BaseLogicSys<T>` for system classes
- Implement lifecycle: `OnInit()`, `OnStart()`, `OnUpdate()`, `OnDestroy()`
- Access via singleton pattern: `MySystem.Instance.SomeMethod()`

#### Error Handling
- Use `Log.Warning()` and `Log.Error()` from GameFramework
- Validate parameters and return early
- Use try-catch for external dependencies
- Log errors with descriptive messages including context

```csharp
if (string.IsNullOrEmpty(assetName))
{
    Log.Warning("Can not load sound '{0}' from data table.", assetName.ToString());
    return null;
}
```

### Constants and Configuration
- Define constants in `Constant.AssetPriority` for resource priorities
- Use asset priority system: ConfigAsset (100), UIFormAsset (50), MusicAsset (20), etc.
- Store settings through `GameModule.Setting` with formatted keys

### Memory and Resource Management
- Use YooAsset for all resource loading/unloading
- Implement proper cleanup in `OnDestroy()` methods
- Use object pools for frequently created objects
- Avoid memory leaks by unsubscribing from events

### Framework-Specific Patterns

#### Procedure System
```csharp
public class ProcedureSplash : ProcedureBase
{
    public override bool UseNativeDialog => true;
    
    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        ChangeState<ProcedureInitPackage>(procedureOwner);
    }
}
```

#### Sound System
```csharp
// Extension methods for sound component
soundComponent.PlayMusic("BackgroundMusic");
soundComponent.PlayUISound("ButtonClick");
soundComponent.SetVolume("Music", 0.8f);
```

#### UI Script Generation
- Use Unity Editor menu: `GameObject > ScriptGenerator > UIProperty`
- Follow naming conventions: `m_btn` for buttons, `m_toggle` for toggles, etc.
- Generated code includes proper event binding with UniTask support

### Important Notes for Agents

1. **Hot-Reload Awareness**: Code in `HotFix/` directories can be updated at runtime
2. **Assembly Dependencies**: Respect assembly definition references and boundaries
3. **Resource Loading**: Always use YooAsset APIs, never direct Resources.Load
4. **Async Operations**: Use UniTask for all async operations to maintain compatibility
5. **Event-Driven Architecture**: Prefer events over direct method calls for cross-system communication
6. **Unity Lifecycle**: Follow Unity lifecycle methods and GameFramework patterns
7. **Localization**: Use the built-in localization system for all user-facing text
8. **Performance**: Consider the mobile target and optimize accordingly

### Common Patterns to Follow

- **Singleton Systems**: Most systems use `BaseLogicSys<T>` singleton pattern
- **State Machines**: Game flow uses Procedure system for state management
- **UI Stack**: UI windows are managed in a stack with depth sorting
- **Asset Priorities**: Use defined priority constants for consistent loading order
- **Extension Methods**: Use extension methods for framework component enhancement
- **Attribute-Based Registration**: Use attributes for automatic system registration

### Files to Understand First

1. `Assets/GameScripts/HotFix/GameBase/BaseLogicSys.cs` - System base class
2. `Assets/GameScripts/HotFix/GameLogic/GameApp.cs` - Application entry point
3. `Assets/GameScripts/Runtime/Procedure/ProcedureBase.cs` - State machine base
4. `Assets/GameScripts/HotFix/GameLogic/System/UISystem/UISystem.cs` - UI management
5. `Assets/UnityGameFramework/Scripts/Editor/UI/ScriptGenerator.cs` - Code generation tools

This framework emphasizes modularity, hot-reload capability, and clear architectural boundaries. Follow these patterns to maintain consistency with the existing codebase.