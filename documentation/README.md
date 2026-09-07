# NCDK — Systems Documentation

This is the living reference for the **NCDK** library (Unity toolkit shared across
NullComma projects, submodule at `Assets/NCDK/`). It exists so AI assistants (and
developers) can answer "how does NCDK do X" without reverse-engineering the source each
time. It complements the public repo README (`.github/README.md`).

> **Keep it fresh:** when you touch an NCDK system in this repo, update the matching page
> here in the same pass. Code is the source of truth; this is the index of wiring and
> intended usage.

## Namespace layout

| Namespace | Area |
|-----------|------|
| `NCDK` | Root: `ServiceLocator`, `ETime`, `Retainable`, `GameObjectCreate`, installers, `BlockingEventsManager`, extensions (`Scripts/Tools/Extensions`, `Scripts/Tools/Utils`, `Scripts/Tools/Static`) |
| `NCDK.Refs` | Scene-reference attributes (`[Self]`, `[Child]`, `[Parent]`, `[Anywhere]`, `[Scene]`) — assembly `NCDK.Refs` |
| `NCDK.UI` | `View`, `UIButton`, `UIInteractable`, event system handlers, footer/prompts, localization trigger |
| `NCDK` (Gameplay/*) | Triggers, timelines, characters/IK, damage, interaction, physics, transform/rotators, movement, object lifetime |
| `NCDK` (System/*) | Input, save, scenes, scriptable-object database, splash, assets |
| `NCDK` (Animation/*) | Fader, behaviours, avatar masks |
| `NCDK.Editor` | Property/attribute drawers, scene-variable editors, build/process tools, timeline editor |

## System pages

| Page | Covers |
|------|--------|
| [Dependency Injection](dependency-injection.md) | `Bootstrapper`, `IInstaller`, `ServiceLocator`, `IPreLoad`, `GameObjectCreate` |
| [Scene Refs (`NCDK.Refs`)](refs.md) | `[Self]`/`[Child]`/`[Parent]`/`[Anywhere]`/`[Scene]`, `Flag`, validation |
| [Time & Pause](time-and-pause.md) | `ETime`, `TimePauseManager`, `Retainable`, `OnTimePaused` consumers |
| [Views & UI](views-and-ui.md) | `View` navigation stack, blocking, `UIButton`, event-system wiring |
| [Database](database.md) | `ScriptableObjectDatabase<T>` (Resources-based), `IdentifiableScriptableObject` |

## Cross-project conventions

- **No manual singletons.** Register/consume through `ServiceLocator` (then use
  `TryResolve<T>()` for optional deps).
- **Identity:** `SerializableGuid` everywhere (`IdentifiableScriptableObject` for SOs,
  `IdentifiableMonoBehaviour` registry for scene objects).
- **Blocking flags:** use `Retainable` (never bare booleans) so multiple systems compose.
- **Refs:** prefer `NCDK.Refs` attributes + `OnValidate() => this.ValidateRefs()` over
  raw `GetComponent`; `TryGetComponent` for manual lookups.