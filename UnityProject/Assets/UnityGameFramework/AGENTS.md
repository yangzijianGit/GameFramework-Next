# UnityGameFramework - Core Framework

**Directory:** `Assets/UnityGameFramework/Scripts/Runtime/`

## OVERVIEW
Core game framework based on GameFramework (unitygameframework.com). Provides UI, Sound, Resource, Network, Scene, Entity, Setting, Download, WebRequest components.

## STRUCTURE
```
UnityGameFramework/Scripts/Runtime/
├── UI/            # UIComponent, UIForm, UIGroup
├── Sound/         # SoundComponent, play/stop music/sound
├── Resource/      # ResourceManager, YooAsset integration
├── Network/       # Socket, WebSocket support
├── Scene/         # Scene loading/unloading
├── Entity/        # Object pooling, entity component
├── Setting/       # PlayerPrefs abstraction
├── Download/      # Background download
├── WebRequest/    # HTTP requests
├── Variable/      # Type-safe variable wrappers (VarInt, VarString, etc.)
├── Utility/       # Extensions, helpers (Log, StringExtension)
└── Debugger/      # In-game debugger
```

## WHERE TO LOOK
| Task | Location |
|------|----------|
| UI management | `UI/UIComponent.cs` |
| Audio | `Sound/SoundComponent.cs` |
| Resources | `Resource/ResourceManager.cs` |
| Logging | `Utility/Log.cs` |
| FSM | `Procedure/ProcedureComponent.cs` (in framework) |

## CONVENTIONS
- Component-based: each feature has a Component class
- Access via `GameModule.GetComponent<XxxComponent>()`
- Use Variable classes for type-safe data passing
- Follow Unity GameFramework documentation patterns

## ANTI-PATTERNS
- Large number of similar Var* classes (could use generics)
- Some legacy code patterns in older subdirectories
