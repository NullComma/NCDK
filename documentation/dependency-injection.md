# Dependency Injection (`NCDK` / `Assets/NCDK/Scripts/Core/DependencyInjection`)

Installers run **once per domain load**, before the first scene. Everything else resolves
lazily through the static `ServiceLocator`.

## Bootstrapper

`Bootstrapper.cs`:
- `[RuntimeInitializeOnLoadMethod(SubsystemRegistration)]` scans all assemblies (skipping
  `UnityEngine`, `UnityEditor`, `System`, `Microsoft`, `Mono`, `nunit`, `Bee`, `Gradle`) for
  concrete `IInstaller` implementations, orders by `IInstaller.Priority`, runs `Install()`.
- `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` subscribes to `SceneManager.sceneLoaded`;
  on every loaded scene it calls `OnPreLoad()` on all `IPreLoad` components found in root
  objects (used to register scene-scoped services such as the `Camera`).

## IInstaller

```csharp
public interface IInstaller { int Priority { get; } void Install(); }
```
Priorities used in practice: `CoreInstaller = int.MinValue`, game general installers around
`-1000`, game-specific installers at `0`.

`CoreInstaller` registers the reusable core:
- `BlockingEventsManager`, `CursorManager(blockingEventsManager)`
- lazy `TimePauseManager`, lazy `UISoundsBankSO` (Resources), lazy `Fader` component.

## ServiceLocator

`ServiceLocator.cs` — `ConcurrentDictionary<Type, object>` + lazy factories.

- `Register<T>(instance)` — eager; refuses `UnityEngine.Object`/`MonoBehaviour`/`object`.
- `Register<T>()` / `RegisterLazy<T>()` / `RegisterLazy<T>(factory)`.
- `Resolve<T>()` — **throws** if missing (suggests registering via installer or `IPreLoad`).
  `Resolve(Type)`, `Get<T>()` aliases.
- `TryResolve<T>()` / `TryResolve<T>(out T)` — return default/null, no throw.
- `IDisposable` instances are disposed on `Application.quitting`.
- Annotated `[AutoStaticsCleanup]` so static state is reset on domain reload.

**Lazy + scene persistence trap:** a lazily registered `MonoBehaviour` must come from a
`DontDestroyOnLoad` owner (e.g. a `GameObjectCreate` component that persists) or it is
destroyed on scene load while the factory entry is consumed.

## Object creation & lifetime helpers

- `GameObjectCreate.WithComponent<T>(name, hideFlags)` — bare GameObject + component.
- `gameObject.DontDestroyOnLoad()` / `mb.DontDestroyOnLoad()` extensions
  (`Tools/Extensions/GameObjectExtensions.cs`, `MonoBehaviourExtensions.cs`).
- `SingletonHelper` — `new GameObject(name).DontDestroyOnLoad().AddComponent<T>()`.
- `DontDestroyOnLoadTrigger` — auto-trigger component that persists its GO.
- `AutoStaticsCleanup` / `StaticRoutinesRunner` — static-state reset utilities.

## IPreLoad

Interface with `void OnPreLoad()`. Returned by scene roots; executed by `Bootstrapper` at
scene load for the whole hierarchy. Use to register scene-scoped services before any
`Start()` runs.