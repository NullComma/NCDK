# Scene Refs (`NCDK.Refs`)

Modern component-reference system. **Do not use the legacy `[AutoRef]`/`AutoRefExtensions` —
it was removed.** This is the attribute family from `Assets/NCDK/Scripts/Core/AutoRef/`
(namespace `NCDK.Refs`, assembly `NCDK.Refs`); the *folder* is still named `AutoRef` for
historical reasons.

## Usage

```csharp
using NCDK.Refs;

[SerializeField, Self] CharacterController characterController;   // same GO
[SerializeField, Child(Flag.Optional)] Rigidbody childRigidbody;  // child hierarchy, optional
[SerializeField, Parent] Transform platform;                      // parent hierarchy
[SerializeField, Scene] Light mainLight;                          // anywhere in scene
[SerializeField, Anywhere] Interactable door;                     // manual assign, null-check only

protected void OnValidate() => this.ValidateRefs();               // re-resolve + log broken refs
```

## Attributes (all extend `SceneRefAttribute`)

| Attribute | Resolution |
|-----------|------------|
| `[Self]` | `GetComponent<T>()` on the same GameObject |
| `[Child]` | `GetComponentInChildren<T>()` |
| `[Parent]` | `GetComponentInParent<T>()` |
| `[Scene]` | `FindAnyObjectByType<T>()` / `FindObjectsOfType<T>()` |
| `[Anywhere]` | no auto-assignment; validates non-null only — the **only** one the user assigns manually (stays editable in the Inspector) |

`[Self]`/`[Child]`/`[Parent]`/`[Scene]` fields are read-only in the Inspector (auto-resolved).

## `Flag` options

| Flag | Meaning |
|------|---------|
| `Optional` | allow null/empty result |
| `IncludeInactive` | include inactive components (Child/Parent only) |
| `Editable` | let the user override the auto-picked value (location still validated) |
| `ExcludeSelf` | skip current GO when searching children/parents |
| `EditableAnywhere` | `Editable` + can be set anywhere without location validation |

Attributes accept an optional `Type filter` (`SceneRefFilter` subclass) to constrain the
resolved object's type beyond the field type.

## Validation

- `SceneRefValidatorOnSave` (`Editor`) — validates scene/prefab refs on save.
- `this.ValidateRefs()` in `OnValidate()` re-resolves and logs broken refs to the Console.
- Manual `TryGetComponent<T>(out var x)` + `Debug.LogError` is the fallback pattern when an
  attribute doesn't fit (e.g. runtime `Interactor` raycasts).

## Related

- `ValidatedMonoBehaviour` — NCDK base class scripts can use for validation hooks.
- `InterfaceRef<T>` — serializable interface references with a property drawer
  (`InterfaceRefPropertyDrawer`) for inspector-friendly interface fields.