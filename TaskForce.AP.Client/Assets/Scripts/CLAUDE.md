# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Building and Running

- **Build:** Use the Unity Editor build pipeline (File > Build Settings).
- **Run:** Open the entry scene containing `Starter.cs` in the Unity Editor and press **Play**.
- **Testing:** The `Core` assembly has no Unity engine dependencies (`noEngineReferences: true` in `Core.asmdef`), so its logic can be tested with standard C# test frameworks (NUnit/XUnit) without a Unity Editor.

## Architecture Overview

The project enforces a strict separation between game logic and the Unity engine across two namespaces:

### `TaskForce.AP.Client.Core` (Pure C#, no UnityEngine references)

- **`Entity/`** — Data models for `Unit`, `AttributeStore`, `Skill`. A unit's stats are stored as string-keyed `Attribute` values in `AttributeStore`. Skills are dynamically added to units and can apply `IModifyAttributeEffect`.
- **`BattleFieldScene/`** — Gameplay logic for the battlefield. `UnitLogic` is the abstract base for per-unit AI/control behaviors. Concrete types: `PlayerUnitLogic`, `NonPlayerUnitLogic`, `MonkLogic`. The `UnitLogicFactory` maps string IDs (`"PLAYER"`, `"NON_PLAYER"`, `"MONK"`) to logic constructors.
- **`BattleFieldScene/Skills/`** — Active skill implementations (e.g., `SheepMissileSkill`, `DynamiteSkill`, `PowderKegSkill`, `HealSkill`, `MeleeAttackSkill`). Skills interact with the battlefield via `IUnit` and `Core.View` interfaces only.
- **`GameData/`** — Plain data structures populated from CSV: `Stage`, `StageEnemyUnit`, `Skill`, `Unit`, etc. Accessed globally through `GameDataStore`.
- **`View/BattleFieldScene/`** — Interfaces describing what the logic needs from the visual layer: `IUnit`, `IWorld`, `IMissile`, `IExplosion`, `IPowderKeg`, `ISoul`, `IFollowCamera`, `IWindowStack`, etc.

### `TaskForce.AP.Client.UnityWorld` (Unity MonoBehaviours)

- **`Starter.cs`** — Entry point `MonoBehaviour`. Loads CSV data into `GameDataStore`, `TextStore`, and `AssetPathStore` asynchronously, then hands off to `BattleFieldSceneLoader`.
- **`BattleFieldSceneLoader.cs`** — The composition root for the battlefield. Creates all factories, wires all events, and calls `BattleFieldSceneController.Start()`. This is where skill creators are registered on `SkillFactory` and where `BattleLog`/`BattleLogRecorder` are set up.
- **`View/BattleFieldScene/`** — Concrete Unity implementations of `Core.View` interfaces (e.g., `Unit.cs`, `Explosion.cs`, `PowderKeg.cs`, `Sheep.cs`).
- **`ObjectFactory`** — Creates and pools Unity GameObjects by `ObjectID` string keys. Prepare handlers can be registered per object type to configure freshly created instances.

### Key Design Patterns

- **Dependency Injection via Factories:** No singletons. Dependencies flow in through constructors; `BattleFieldSceneLoader` is the manual DI root.
- **Event-Driven Updates:** C# events connect layers. Entity events propagate upward (e.g., `DiedEvent`, `LevelUpEvent`, `ExpChangedEvent`). Always unsubscribe in `Destroy()` to prevent leaks.
- **`IUpdatable` / `ILoop`:** Objects that need per-frame updates implement `IUpdatable` and register themselves with `ILoop`. `UnitLogic.StartControl()` registers to the loop; `Destroy()` removes from it.
- **`ITime` / `Timer`:** All time-based logic uses `ITime` (implemented by `UnityWorld.Time`). `Timer` and `RepeatTimer` wrap `ITime` + `ILoop` for countdowns and intervals.
- **Data-Driven:** Game balance comes from CSV files loaded at startup. Formula evaluation is handled by `FormulaCalculator` with named formulas registered in `BattleFieldSceneLoader`.

### Adding a New Skill

1. Add a skill ID to `Core.Entity.SkillID`.
2. Implement `Core.BattleFieldScene.Skills.ISkill` for the battlefield behavior.
3. Register a creator in `BattleFieldSceneLoader` via `skillFactory.AddCreator(SkillID.X, ...)`.
4. Add the skill's data rows to the relevant CSV game data files.

## Pre-Implementation Process

Before implementing any task:
1. Analyze the relevant interfaces and existing code, then ask about any ambiguities.
2. Based on the answers, write Given/When/Then scenarios and get confirmation.
3. Describe the implementation plan: which files to create or modify, what each will contain, and how they relate to each other. Get confirmation.
4. Only begin implementation after confirmation.
5. After implementation, self-review the code against the confirmed scenarios and existing code patterns, then report the result.

## Conventions

- **Never reference `UnityEngine` types inside `Core/`.** Visual operations must go through interfaces in `Core.View`.
- **`BattleLog` (branch `battle-log`)** — `BattleLog` accumulates battle-time and kill count. `BattleLogRecorder` implements `IUpdatable` and subscribes to `DiedEvent` per enemy unit created. Both are wired in `BattleFieldSceneLoader`.
- **Localization** is handled through `TextStore`, loaded from CSV at startup in the language specified in `Starter.cs` (currently `"ko"`).
- **`MockSoundPlayer`** is a placeholder; a real `ISoundPlayer` implementation is a known TODO (see comment in `BattleFieldSceneLoader.cs:167`).
