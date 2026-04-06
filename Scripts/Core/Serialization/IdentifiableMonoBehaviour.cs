using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace NullCore
{
    [DisallowMultipleComponent]
    public class IdentifiableMonoBehaviour : MonoBehaviour, IIdentifiableObject
    {

        public SerializableGuid ID => _id;
        [SerializeField] SerializableGuid _id = SerializableGuid.NewGuid();

        public static readonly Dictionary<SerializableGuid, IdentifiableMonoBehaviour> Registry = new();

        [ContextMenu("Force reset ID")]
        void ResetId()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Reset ID");
            _id = SerializableGuid.NewGuid();
            EditorUtility.SetDirty(this);
            if (!Application.isPlaying && gameObject.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
            Debug.Log($"Generated new ID for '{name}': {_id}", this);
#endif
        }

        void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) ValidateEditor();
#endif
        }

        void OnEnable()
        {
            if (_id != SerializableGuid.Empty)
            {
                Registry[_id] = this;
            }
        }

        void OnDisable()
        {
            if (_id != SerializableGuid.Empty)
            {
                Registry.Remove(_id);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            ValidateEditor();
        }

        private void ValidateEditor()
        {
            if (_id == default)
            {
                ResetId();
                return;
            }

            // 1. Check if we are a newly instantiated prefab (Instance ID matches Source ID)
            if (PrefabUtility.IsPartOfPrefabInstance(this))
            {
                var source = PrefabUtility.GetCorrespondingObjectFromSource(this);
                if (source != null && source._id == this._id)
                {
                    // Values are identical to prefab source, meaning it's a fresh spawn or hasn't been unique-ified yet.
                    ResetId();
                    return;
                }
            }

            // 2. Check for duplicates in Scene
            // finding all objects is heavy, but OnValidate in Editor is acceptable for this safety.
            var allIdentifiables = FindObjectsByType<IdentifiableMonoBehaviour>(FindObjectsInactive.Include);
            foreach (var other in allIdentifiables)
            {
                if (other == this) continue;
                if (other._id == this._id)
                {
                    // Collision detected!
                    // If we are the one being validated (likely the new one), we change.
                    ResetId();
                    return;
                }
            }
        }
#endif
    }
}