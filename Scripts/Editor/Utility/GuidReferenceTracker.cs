#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NCDK.Editor
{
    /// <summary>
    /// Global cache to track which objects in the scene reference a specific SerializableGuid.
    /// Rebuilds only when explicitly requested or when the cache is empty.
    /// </summary>
    [InitializeOnLoad]
    public static class GuidReferenceTracker
    {
        [System.NonSerialized]
        static Dictionary<SerializableGuid, List<Object>> _referencersCache;

        [System.NonSerialized]
        static bool _isDirty = true;

        public static bool IsDirty => _isDirty;

        static GuidReferenceTracker()
        {
            // Mark cache as dirty when scene changes, but do not auto-rebuild to prevent Editor lag loops
            EditorApplication.hierarchyChanged += () => _isDirty = true;
            Undo.undoRedoPerformed += () => _isDirty = true;
        }

        public static void MarkDirty() => _isDirty = true;

        public static void ForceRebuild()
        {
            _referencersCache = new Dictionary<SerializableGuid, List<Object>>();
            var allBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (MonoBehaviour mb in allBehaviours)
            {
                if (mb == null) continue;

                SerializedObject so = new SerializedObject(mb);
                SerializedProperty sp = so.GetIterator();
                bool enterChildren = true;

                while (sp.NextVisible(enterChildren))
                {
                    enterChildren = true;

                    if (sp.type == "SerializableGuidReference")
                    {
                        SerializedProperty valueProp = sp.FindPropertyRelative("Value");
                        if (valueProp != null)
                        {
                            SerializedProperty p1 = valueProp.FindPropertyRelative("Part1");
                            SerializedProperty p2 = valueProp.FindPropertyRelative("Part2");
                            SerializedProperty p3 = valueProp.FindPropertyRelative("Part3");
                            SerializedProperty p4 = valueProp.FindPropertyRelative("Part4");

                            if (p1 != null && p2 != null && p3 != null && p4 != null)
                            {
                                SerializableGuid guid = new SerializableGuid(
                                    (uint)p1.longValue,
                                    (uint)p2.longValue,
                                    (uint)p3.longValue,
                                    (uint)p4.longValue);

                                if (guid != SerializableGuid.Empty)
                                {
                                    if (!_referencersCache.ContainsKey(guid))
                                    {
                                        _referencersCache[guid] = new List<Object>();
                                    }
                                    _referencersCache[guid].Add(mb.gameObject);
                                }
                            }
                        }
                    }
                }
            }

            _isDirty = false;
        }

        public static List<Object> GetReferencers(SerializableGuid targetId)
        {
            if (_referencersCache == null)
            {
                ForceRebuild();
            }

            if (_referencersCache.TryGetValue(targetId, out List<Object> list))
            {
                return list;
            }

            return null;
        }
    }
}
#endif