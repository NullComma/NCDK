using System;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace NCDK
{
    [Serializable]
    public class MethodParameter
    {
        public enum ParamType { String, Float, Int, Bool, Vector2, Vector3, Color, GuidReference, Enum }
        public ParamType type;
        public string stringValue;
        public float floatValue;
        public int intValue;
        public bool boolValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public Color colorValue;
        public SerializableGuidReference guidReference;
        public int enumValue; // Storing as int
    }

    [Serializable]
    public class ObjectMethodCall
    {
        public SerializableGuidReference targetId;
        public string componentTypeName;
        public string methodName;
        public List<MethodParameter> parameters = new List<MethodParameter>();

        public void InvokeCall(string callerName = "")
        {
            if (IdentifiableMonoBehaviour.Registry.TryGetValue(targetId.Value, out IdentifiableMonoBehaviour obj))
            {
                if (!string.IsNullOrEmpty(componentTypeName) && !string.IsNullOrEmpty(methodName))
                {
                    Component targetComponent = obj.GetComponent(componentTypeName);
                    if (targetComponent != null)
                    {
                        System.Reflection.MethodInfo method = targetComponent.GetType()
                            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                            .FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == parameters.Count);
                        if (method != null)
                        {
                            System.Reflection.ParameterInfo[] paramInfos = method.GetParameters();
                            object[] args = new object[parameters.Count];
                            for (int i = 0; i < parameters.Count; i++)
                            {
                                switch (parameters[i].type)
                                {
                                    case MethodParameter.ParamType.String: args[i] = parameters[i].stringValue; break;
                                    case MethodParameter.ParamType.Float: args[i] = parameters[i].floatValue; break;
                                    case MethodParameter.ParamType.Int: args[i] = parameters[i].intValue; break;
                                    case MethodParameter.ParamType.Bool: args[i] = parameters[i].boolValue; break;
                                    case MethodParameter.ParamType.Vector2: args[i] = parameters[i].vector2Value; break;
                                    case MethodParameter.ParamType.Vector3: args[i] = parameters[i].vector3Value; break;
                                    case MethodParameter.ParamType.Color: args[i] = parameters[i].colorValue; break;
                                    case MethodParameter.ParamType.Enum:
                                        if (i < paramInfos.Length && paramInfos[i].ParameterType.IsEnum)
                                        {
                                            args[i] = Enum.ToObject(paramInfos[i].ParameterType, parameters[i].enumValue);
                                        }
                                        else args[i] = parameters[i].enumValue;
                                        break;
                                    case MethodParameter.ParamType.GuidReference:
                                        if (IdentifiableMonoBehaviour.Registry.TryGetValue(parameters[i].guidReference.Value, out IdentifiableMonoBehaviour paramObj))
                                        {
                                            if (i < paramInfos.Length)
                                            {
                                                Type pType = paramInfos[i].ParameterType;
                                                if (pType == typeof(GameObject)) args[i] = paramObj.gameObject;
                                                else if (pType.IsSubclassOf(typeof(Component))) args[i] = paramObj.GetComponent(pType);
                                                else args[i] = paramObj; // Fallback
                                            }
                                        }
                                        break;
                                }
                            }
                            method.Invoke(targetComponent, args);
                        }
                        else
                        {
                            Debug.LogError($"<color=orange>Caller '{callerName}' could not find public method '{methodName}' on component '{componentTypeName}' on object with ID: {targetId.Value}</color>");
                        }
                    }
                    else
                    {
                        Debug.LogError($"<color=orange>Caller '{callerName}' could not find component '{componentTypeName}' on object with ID: {targetId.Value}</color>");
                    }
                }
            }
            else
            {
                Debug.LogError($"<color=orange>Caller '{callerName}' could not find object for method call with ID: {targetId.Value}</color>");
            }
        }
    }
}
