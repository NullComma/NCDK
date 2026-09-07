# ScriptableObject Database

Generic Resources-backed database. The base class
`ScriptableObjectDatabase<T>` lives in the **game project** at
`Assets/_game/Scripts/ScriptableObjects/ScriptableObjectDatabase.cs` (namespace `wtdr`) —
concrete databases subclass it with a Resources path. It is documented together with NCDK
because it is the shared data pattern used by NCDK/ScriptableObjects and game code.

```csharp
public abstract class ScriptableObjectDatabase<T> where T : IdentifiableScriptableObject
{
    protected ScriptableObjectDatabase(string resourcesPath); // e.g. base("Anomalies")
    public void Reload();          // clear + Load
    public void Load();            // Resources.LoadAll<T>(path); idempotent
    public UniTask LoadAsync();    // wrapper (Load is synchronous)
    public bool TryGet(SerializableGuid id, out T asset);
    public T    Get(SerializableGuid id);          // logs error + null if missing
    public IEnumerable<T> Values;                   // registry values (ensures load)
}
```

## Behavior & gotchas

- **Loading:** `Resources.LoadAll<T>(_resourcesPath)` — the path is relative to any
  Resources folder. `LoadAll` on a folder is **recursive**: subfolders are searched.
  Empty path `""` loads the whole Resources tree.
- **Keying:** dictionary keyed by `IdentifiableScriptableObject.ID` (`SerializableGuid`).
  Duplicate IDs are logged and skipped on populate.
- **Lazy:** `TryGet`/`Get`/`Values` call `EnsureLoaded` automatically; `Load` is idempotent.
- **Consequences of Resources:** every asset under a Resources folder is packed into all
  builds. Keep only data that must load at runtime there; avoid shipping heavy exclusive
  assets (models/textures) under Resources unless always needed.
- For editor-time, arbitrary loading prefer `AssetDatabase` (e.g. `AnomalyDatabase.Reload()`
  still works in-editor because Resources is project-relative there).

## Concrete databases

| Database | Resources path | Asset folder |
|----------|----------------|--------------|
| `AnomalyDatabase` (`Scripts/Anomalies/AnomalyDatabase.cs`, `wtdr`) | `"Anomalies"` | `Assets/Resources/Anomalies/` |
| `SurfaceDatabase` (`Scripts/Gameplay/Footsteps/SurfaceDatabase.cs`, `wtdr`) | `"Surfaces"` | `Assets/Resources/Surfaces/` |

## Related

- `IdentifiableScriptableObject` — SO base providing `SerializableGuid ID`.
- `SerializableGuid` — GUID that can be used as a dictionary key (works with Unity
  serialization + `[SerializeField]`).