# Views & UI (`NCDK.UI`)

`View` is the base for every UI panel/menu in the game. Opening a view optionally pauses
the game and registers "in menu" blocking; a navigation stack manages focus.

## `View` (`Assets/NCDK/Scripts/UI/Components/Base/View.cs`)

```csharp
public abstract class View : MonoBehaviour
```

Lifecycle:
- `Awake()` resolves `BlockingEventsManager` + `TimePauseManager` from `ServiceLocator`,
  falls back to a child `EventSystem`.
- `OnEnable()`:
  1. remembers `EventSystem.current.currentSelectedGameObject`,
  2. `_blockingEventsManager.MenuRetainable.Retain(this)`,
  3. if `ShouldPauseTheGame` → `timePauseManager.Retain(this)` (timeScale 0),
  4. hooks cancel/`_buttonReturn` events,
  5. **navigation push:** if `ActiveView != this`, hide previous view, set `ActiveView = this`,
  6. re-selects the remembered object (or `firstSelectedGameObject`).
- `OnDisable()` releases menu retain + pause retain, unhooks events.
- `OnDestroy()` pops the stack (restores `_returnToView`) and invokes `CloseEvent`.

Public surface:
- `static View ActiveView` — top of the stack.
- `ShouldPauseTheGame` (default `true`), `CanCloseWithCancel` (default `true`).
- `Show()` / `Hide()` / `Close()` / `CloseAllViews()` / `CloseByCancelled()`.
- `event Action CloseEvent`.

## Gamepad/mouse navigation

- `EventSystemHandlers` (NCDK) exposes `CancelEvent`; `OnCancelEvent` → `CloseByCancelled`.
- `LateUpdate` failsafe re-selects `firstSelectedGameObject` (or the first `UIInteractable`)
  if nothing is selected and a gamepad is connected.
- `ViewInputFooterController` shows input prompts/footer for the active view.

## Buttons & interactables

- `UIButton` — click event (`ClickEvent`) wired to view actions (e.g. `_buttonReturn`).
- `UIInteractable` — base for UI elements participating in gamepad selection.
- Serialized `_eventSystem` auto-resolved in `OnValidate` for the choosing hierarchy.

## Multi-scene views

- `View.CloseAllViews()` finds all `View` instances (including inactive) and closes them —
  used by e.g. Wicked Angel to drop the pause-menu stack before a death sequence.

## Common game views (in `Assets/_game/Scripts/Views/`)

`PauseMenuView`, `CameraSpeedView` (`ShouldPauseTheGame = false`), `GammaConfigView`,
`GameOverView`, `VictoryView`, plus prefabs `Prefabs/View - *.prefab` (Pause Menu, Settings,
Anomaly Report, Adjust Gamma, Audio Settings, Graphics Settings, Camera Speed, Seed Select,
Select Game Mode, Confirm, I2Loc Select Language).