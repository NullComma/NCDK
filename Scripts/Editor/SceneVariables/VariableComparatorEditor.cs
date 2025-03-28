using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using EnigmaCore.SceneVariables;

namespace EnigmaCore.Editor.SceneVariables
{
    [CustomEditor(typeof(VariableComparator))]
    public class VariableComparatorEditor : VariableComponentBaseEditor
    {
        private SerializedProperty comparison;
        private SerializedProperty compareFloat;
        private SerializedProperty compareBool;

        void OnEnable()
        {
            OnEnableBase();
            comparison = serializedObject.FindProperty("comparison");
            compareFloat = serializedObject.FindProperty("compareFloat");
            compareBool = serializedObject.FindProperty("compareBool");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            DrawVariableManagement();
            EditorGUILayout.Space();
        
            // Mostrar campos específicos
            EditorGUILayout.PropertyField(comparison);
        
            if ((SceneVariableManager.VariableType)variableType.enumValueIndex == SceneVariableManager.VariableType.Float)
            {
                EditorGUILayout.PropertyField(compareFloat);
            }
            else
            {
                EditorGUILayout.PropertyField(compareBool);
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onConditionMet"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onConditionNotMet"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}