using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using EnigmaCore.SceneVariables;

namespace EnigmaCore.Editor.SceneVariables
{
    
    public class VariableComponentBaseEditor : UnityEditor.Editor
    {
        protected SerializedProperty variableName;
        protected SerializedProperty variableType;
        protected SceneVariableManager manager;

        protected virtual void OnEnableBase()
        {
            manager = serializedObject.FindProperty("Manager").objectReferenceValue as SceneVariableManager;
            variableName = serializedObject.FindProperty("variableName");
            variableType = serializedObject.FindProperty("variableType");
        }

        protected void DrawVariableManagement()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            // Botão para ir para o Manager
            if (GUILayout.Button("Go to SceneVariableManager"))
            {
                if (manager) Selection.activeObject = manager;
                else Debug.LogWarning("No SceneVariableManager found in scene!");
            }

            // Lista de variáveis existentes
            var variables = manager?.GetAllVariables() ?? new List<SceneVariableManager.VariableData>();
            List<string> variableNames = new List<string>();
            foreach (var v in variables) variableNames.Add(v.name);

            int selectedIndex = variableNames.IndexOf(variableName.stringValue);
            int newIndex = EditorGUILayout.Popup("Registered Variables",selectedIndex,variableNames.ToArray());

            if (newIndex != selectedIndex && newIndex >= 0)
            {
                variableName.stringValue = variables[newIndex].name;
                variableType.enumValueIndex = (int)variables[newIndex].type;
            }

            // Criação de nova variável
            if (selectedIndex == -1)
            {
                EditorGUILayout.HelpBox("Variable not registered!", MessageType.Error);
            }

            EditorGUILayout.EndVertical();
        }
    }
    
}