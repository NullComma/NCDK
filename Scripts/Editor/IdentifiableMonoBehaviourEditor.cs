#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullCore.Editor
{
    [CustomEditor(typeof(IdentifiableMonoBehaviour))]
    public class IdentifiableMonoBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            IdentifiableMonoBehaviour targetObj = (IdentifiableMonoBehaviour)target;
            if (targetObj.ID == SerializableGuid.Empty) return;

            if (GuidReferenceTracker.IsDirty)
            {
                GUILayout.Space(15);
                EditorGUILayout.HelpBox("Scene hierarchy changed. Reference cache might be outdated.", MessageType.Warning);
                if (GUILayout.Button("Update Reference Cache", GUILayout.Height(30)))
                {
                    GuidReferenceTracker.ForceRebuild();
                }
                GUILayout.Space(5);
            }

            List<Object> referencers = GuidReferenceTracker.GetReferencers(targetObj.ID);

            if (referencers != null && referencers.Count != 0)
            {
                GUILayout.Label("Referenced By:", EditorStyles.boldLabel);

                EditorGUI.BeginDisabledGroup(true);
                foreach (Object refObj in referencers)
                {
                    EditorGUILayout.ObjectField(refObj, typeof(Object), true);
                }
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
#endif