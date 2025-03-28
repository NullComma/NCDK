using UnityEngine;
using UnityEngine.Events;

namespace EnigmaCore.SceneVariables
{
    public class VariableComparator : BaseVariable
    {
        public enum ComparisonType
        {
            Equals,
            NotEquals,
            GreaterThan,
            LessThan
        }

        [SerializeField] string variableName;
        [SerializeField] SceneVariableManager.VariableType variableType;
        [SerializeField] ComparisonType comparison;
        [SerializeField] float compareFloat;
        [SerializeField] bool compareBool;

        public UnityEvent onConditionMet;
        public UnityEvent onConditionNotMet;

        void OnEnable()
        {
            Manager.OnVariableModified += HandleVariableModified;
        }

        void OnDisable()
        {
            if (Manager != null) Manager.OnVariableModified -= HandleVariableModified;
        }

        void HandleVariableModified(string modifiedVariableName)
        {
            if (modifiedVariableName == variableName)
            {
                CheckCondition();
            }
        }

        public void CheckCondition()
        {
            if (!ValidateVariable()) return;

            var variable = Manager.GetOrCreateVariable(variableName, variableType);
            bool result = false;

            switch (variable.type)
            {
                case SceneVariableManager.VariableType.Float:
                    result = CompareFloat(variable.floatValue);
                    break;
                case SceneVariableManager.VariableType.Bool:
                    result = CompareBool(variable.boolValue);
                    break;
            }

            (result ? onConditionMet : onConditionNotMet).Invoke();
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

        bool CompareFloat(float value)
        {
            switch (comparison)
            {
                case ComparisonType.Equals: return Mathf.Approximately(value,compareFloat);
                case ComparisonType.NotEquals: return !Mathf.Approximately(value,compareFloat);
                case ComparisonType.GreaterThan: return value > compareFloat;
                case ComparisonType.LessThan: return value < compareFloat;
            }
            return false;
        }

        bool CompareBool(bool value)
        {
            switch (comparison)
            {
                case ComparisonType.Equals: return value == compareBool;
                case ComparisonType.NotEquals: return value != compareBool;
                default: return false;
            }
        }
    }
}