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

            // Defer the heavy check to avoid asset loading during OnValidate
            EditorApplication.delayCall -= DeferredValidate;
            EditorApplication.delayCall += DeferredValidate;
#endif
        }

#if UNITY_EDITOR
        private void DeferredValidate()
        {
            EditorApplication.delayCall -= DeferredValidate;
            if (this == null) return;

            if (!IsUnique(this, logError: false))
            {
                Debug.Log($"[IdentifiableScriptableObject] Resetting ID because IsUnique returned false. Object: {name}", this);
                ResetId();
            }
            else
            {
                _lastCheckId = _id;
            }
        }
#endif

        public static bool IsUnique(IdentifiableScriptableObject original, bool logError = true)
        {
            if (original == null)
            {
                Debug.LogError("Original IdentifiableScriptableObject is null.");
                return false;
            }

#if UNITY_EDITOR
            if (IdentifiableEditorHooks.GetAllObjects == null) return true;

            var allWithSameId = IdentifiableEditorHooks.GetAllObjects(original.ID);
            
            // Filter to check if there's any OTHER object with the same ID
            bool hasDuplicate = false;
            foreach (var obj in allWithSameId)
            {
                if (obj == original) continue;
                
                // If it's a persistent asset, we must check if it's the same asset via AssetDatabase
                if (UnityEditor.EditorUtility.IsPersistent(original) && UnityEditor.EditorUtility.IsPersistent(obj))
                {
                    string originalPath = UnityEditor.AssetDatabase.GetAssetPath(original);
                    string otherPath = UnityEditor.AssetDatabase.GetAssetPath(obj);
                    if (originalPath == otherPath) continue;
                }

                hasDuplicate = true;
                if (logError)
                {
                    string otherPath = IdentifiableEditorHooks.GetObjectPath != null 
                        ? IdentifiableEditorHooks.GetObjectPath(obj) 
                        : obj.name;
                    Debug.LogError($"Duplicate ID {original.ID} detected on {original.name}. Existing: {otherPath}", original);
                }
                break;
            }
            
            return !hasDuplicate;
#else
            return true;
#endif
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