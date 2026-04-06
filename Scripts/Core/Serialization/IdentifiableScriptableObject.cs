using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NullCore
{
    public class IdentifiableScriptableObject : ScriptableObject, IIdentifiableObject
    {
        public SerializableGuid ID => _id;
        [SerializeField] SerializableGuid _id = SerializableGuid.NewGuid();

        [NonSerialized] private SerializableGuid _lastCheckId;

        protected virtual void Reset()
        {
            OnValidate();
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;

            if (ID == default)
            {
                Debug.Log($"[IdentifiableScriptableObject] Resetting ID because it is default (0). Object: {name}", this);
                ResetId();
                return;
            }

            // Optimization: If the ID hasn't changed since the last valid check, skip the heavy search.
            // When an object is duplicated, _id is copied but _lastCheckId resets, forcing a re-validation.
            if (_id == _lastCheckId) return;

            if (!IsUnique(this, logError: false))
            {
                Debug.Log($"[IdentifiableScriptableObject] Resetting ID because IsUnique returned false. Object: {name}", this);
                ResetId();
            }
            else
            {
                _lastCheckId = _id;
            }
#endif
        }

        public static bool IsUnique(IdentifiableScriptableObject original, bool logError = true)
        {
            if (original == null)
            {
                Debug.LogError("Original IdentifiableScriptableObject is null.");
                return false;
            }

#if UNITY_EDITOR
            string originalGuid = null;
            long originalLocalId = 0;
            bool originalIsPersistent = EditorUtility.IsPersistent(original);
            if (originalIsPersistent)
            {
                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(original, out originalGuid, out originalLocalId))
                {
                    // If we can't identify the original object (e.g. during rename or fresh import), 
                    // we assume it is unique to prevent accidental ID resets.
                    return true;
                }
            }
#endif

            foreach (var mb in Resources.FindObjectsOfTypeAll<IdentifiableScriptableObject>())
            {
                if (mb == original) continue;

#if UNITY_EDITOR
                // 1. Ghost Check: Persistent vs Non-Persistent
                // If original is persistent (saved asset), ignore transient in-memory copies or un-persisted ghosts.
                if (originalIsPersistent && !EditorUtility.IsPersistent(mb)) continue;

                // 2. Identity Check: Same underlying asset?
                if (originalIsPersistent && EditorUtility.IsPersistent(mb))
                {
                    // Check if 'mb' is a stale handle (persistent but no valid GUID)
                    if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mb, out string mbGuid, out long mbLocalId))
                    {
                        continue;
                    }

                    // Both have valid GUIDs. Are they the same?
                    if (originalGuid != null && originalGuid == mbGuid && originalLocalId == mbLocalId) continue;
                }
#endif

                if (mb.ID == original.ID)
                {
                    if (logError)
                    {
                        Debug.LogError($"Duplicate ID {original.ID} detected on {original.name}. Existing: {mb.name}", original);
                    }
                    return false;
                }
            }
            return true;
        }

        void ResetId()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, name + " ID reset " + GetInstanceID());
            _id = Guid.NewGuid().ToSerializableGuid();
            _lastCheckId = _id;

            EditorUtility.SetDirty(this);
#endif
            Debug.Log($"Setting new ID to object '{name}': {_id.ToString()}", this);
        }
    }
}