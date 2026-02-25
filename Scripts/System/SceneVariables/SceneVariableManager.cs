using System;
using UnityEngine;
using System.Collections.Generic;

namespace EnigmaCore.SceneVariables
{
    public class SceneVariableManager : MonoBehaviour
    {
        public event System.Action<string> OnVariableModified;
        public void NotifyVariableModified(string variableName) => OnVariableModified?.Invoke(variableName);

        [Serializable]
        public class VariableData
        {
            public string name;
            public VariableType type;
            public float floatValue;
            public bool boolValue;
        }

        public enum VariableType
        {
            Float,
            Bool
        }

        [SerializeField] List<VariableData> variables = new ();
        
        public VariableData GetOrCreateVariable(string name, VariableType type)
        {
            var variable = variables.Find(v => v.name == name);
            if (variable != null) return variable;
            variable = new VariableData
            {
                name = name,
                type = type,
                floatValue = 0f,
                boolValue = false
            };
            variables.Add(variable);
            return variable;
        }

        public List<VariableData> GetAllVariables() => variables;
    }
}