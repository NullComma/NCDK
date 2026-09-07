# Time & Pause (`NCDK`)

## `ETime` (`Assets/NCDK/Scripts/System/ETime.cs`)

Static wrapper over `Time.timeScale`.

```csharp
ETime.TimeScale = 0.5f;          // fires OnTimeScaleChanged(old, new)
bool paused = ETime.IsPaused;    // ⇔ Mathf.Approximately(Time.timeScale, 0)
ETime.IsPaused = true;           // timeScale = 0; fires OnTimePaused(true)
```

Events: `OnTimeScaleChanged(float old, float new)`, `OnTimePaused(bool)`. Additive
subscription internally (safe to subscribe multiple times). Note: `DeltaTimeScaled` is
obsolete — use `Time.deltaTime` directly.

## `Retainable` (`Assets/NCDK/Scripts/Tools/Misc/Retainable.cs`)

Reference-counted boolean used by every "blocking/paused" concept.

```csharp
Retainable r = new();
r.Retain(a);    // → IsRetained true, returns IReleaseHandle
r.Retain(a);    // ignored (dedupe by source)
r.Retain(b);    // still true
r.Release(a);   // still true (b holds it)
r.Release(b);   // → IsRetained false, StateEvent(false)
```

- `StateEvent(bool)` fires only when retained state flips.
- Returns an `IDisposable` from `Retain` that releases on `Dispose()` — ideal for
  `using` blocks and `OnDisable` symmetry.
- Dead `UnityEngine.Object` sources are auto-removed on read (`UpdateRetainedState`).
- Editor: `DebugRetainedObjectsCollection` for debugging who holds what.

## `TimePauseManager` (`Assets/NCDK/Scripts/System/TimePauseManager.cs`)

Registered lazily by `CoreInstaller`. Delegates to an internal `Retainable`; state changes
drive `ETime.IsPaused`.

```csharp
var tpm = ServiceLocator.Resolve<TimePauseManager>();
tpm.Retain(this);   // pause (scale 0) — composable with other owners
tpm.Release(this);  // resume only if no one else retains
```

## `BlockingEventsManager` (`Assets/NCDK/Scripts/Gameplay/BlockingEventsManager.cs`)

Two retainables expose blocking state to gameplay:
- `MenuRetainable`      → `IsInMenu`
- `PlayingCutsceneRetainable` → `IsPlayingCutscene`
- `InMenuOrPlayingCutscene` = `IsInMenu || IsPlayingCutscene` (broadcast on change).

Gameplay code (movement, AI, interaction) checks `InMenuOrPlayingCutscene`; UI `View`s
retain `MenuRetainable` on open.

## Consumers of `ETime.OnTimePaused`

Where `IsPaused`/`OnTimePaused` is observed so "paused" isn't just timeScale:
- `SoundManager` — pauses FMOD events with `autoPauseManagment`.
- `EventEmitterPauser` — per-emitter FMOD pause.
- `FootstepsSource` — skips feet when `! _playWhenGamePaused && ETime.IsPaused`.

## Related

- `GamePauseController` (game code) — global pause owner: Esc / focus regain / **gamepad
  disconnect** (Steam requirement, opens its pause menu) via `ServiceLocator`-resolved
  `MenuRetainable` + `TimePauseManager`.
- `FMODTimelineEventPauser` (game code) — pauses FMOD **Timeline-track** events on
  `ETime.OnTimePaused`, via hooks patched into the FMOD Unity integration.
- Chronos (third-party, `Assets/Plugins/Ludiq/Ludiq.Chronos`) provides per-object timelines;
  NCDK does not wrap it.