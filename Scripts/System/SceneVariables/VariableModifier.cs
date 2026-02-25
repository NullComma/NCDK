using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore.SceneVariables
{
    public class VariableModifier : BaseVariable
    {
        public enum OperationType
        {
            Add,
            Subtract,
            Set
        }

        [SerializeField] string variableName;
        [SerializeField] SceneVariableManager.VariableType variableType;
        [SerializeField] OperationType operation;
        [SerializeField] float floatValue;
        [SerializeField] bool boolValue;

        public UnityEvent onModified;


        public void Execute()
        {
            if (!ValidateVariable()) return;

            var variable = Manager.GetOrCreateVariable(variableName, variableType);

            switch (variable.type)
            {
                case SceneVariableManager.VariableType.Float:
                    ModifyFloat(variable);
                    break;
                case SceneVariableManager.VariableType.Bool:
                    variable.boolValue = boolValue;
                    break;
            }

            onModified.Invoke();
            Manager.NotifyVariableModified(variableName);
        }

        bool ValidateVariable()
        {
            if (Manager.GetOrCreateVariable(variableName, variableType) == null)
            {
                Debug.LogError($"Variable '{variableName}' not found in SceneVariableManager!");
                return false;
            }
            return true;
        }

        void ModifyFloat(SceneVariableManager.VariableData variable)
        {
            switch (operation)
            {
                case OperationType.Add:
                    variable.floatValue += floatValue;
                    break;
                case OperationType.Subtract:
                    variable.floatValue -= floatValue;
                    break;
                case OperationType.Set:
                    variable.floatValue = floatValue;
                    break;
            }
        }
    }
}