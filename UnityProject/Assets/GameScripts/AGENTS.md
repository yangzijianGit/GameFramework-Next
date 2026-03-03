# GameScripts - Game Logic

**Directory:** `Assets/GameScripts/`

## OVERVIEW
Main game logic code with hot-reload support (HybridCLR) and runtime/hotfix separation.

## STRUCTURE
```
GameScripts/
├── HotFix/          # Runtime-updatable code (DLL)
│   ├── GameBase/    # Base systems (BaseLogicSys<T>)
│   ├── GameLogic/   # Game implementation
│   │   ├── System/ # UISystem, Pool, CodeTypes
│   │   └── MatchCore/ # Game core (Battle, Manager, Utils)
│   └── GameProto/  # Config (Luban) + generated code
├── Runtime/         # Non-hotfix code (compiled into game)
│   ├── Procedure/   # Game flow (ProcedureBase)
│   ├── Launcher/   # Startup UI
│   ├── Helper/     # Utilities
│   └── Constant/   # Constants
└── Editor/         # Editor tools
```

## WHERE TO LOOK
| Task | Location |
|------|----------|
| Game entry | `HotFix/GameLogic/GameApp.cs` |
| Game flow | `Runtime/Procedure/Procedure*.cs` |
| UI system | `HotFix/GameLogic/System/UISystem/` |
| Config | `HotFix/GameProto/GameConfig/` |
| Editor tools | `Editor/EditorTools/*.cs` |

## CONVENTIONS
- Follows parent AGENTS.md patterns
- HotFix code: use UniTask, avoid UnityEngine.Object references
- Event system via `[EventInterfaceImp]`
- UI: inherit `UIWindow`, use `[WindowAttribute]`

## ANTI-PATTERNS
- ENateDispose.cs has incomplete Dispose pattern (2 TODOs)
- No unit tests yet (Assets/Tests/ doesn't exist)
