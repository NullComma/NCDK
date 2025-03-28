using UnityEditor;
using EnigmaCore.SceneVariables;

namespace EnigmaCore.Editor.SceneVariables
{
    [CustomEditor(typeof(VariableModifier))]

    public class VariableModifierEditor : VariableComponentBaseEditor
    {
        private SerializedProperty operation;
        private SerializedProperty floatValue;
        private SerializedProperty boolValue;

        void OnEnable()
        {
            OnEnableBase();
            operation = serializedObject.FindProperty("operation");
            floatValue = serializedObject.FindProperty("floatValue");
            boolValue = serializedObject.FindProperty("boolValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawVariableManagement();
            EditorGUILayout.Space();

            // Mostrar campos específicos
            EditorGUILayout.PropertyField(operation);

            if ((SceneVariableManager.VariableType)variableType.enumValueIndex == SceneVariableManager.VariableType.Float)
            {
                EditorGUILayout.PropertyField(floatValue);
            }
            else
            {
                EditorGUILayout.PropertyField(boolValue);
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onModified"));

            serializedObject.ApplyModifiedProperties();
        }
    }

}