#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NCDK.Editor
{
    [CustomPropertyDrawer(typeof(ObjectMethodCall))]
    public class ObjectMethodCallDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight; // Target ID
            
            SerializedProperty targetIdProp = property.FindPropertyRelative("targetId");
            SerializableGuid currentGuid = GetGuid(targetIdProp);
            
            if (currentGuid != SerializableGuid.Empty)
            {
                height += EditorGUIUtility.singleLineHeight * 2 + 4; // Comp and Method dropdowns
                
                SerializedProperty parametersProp = property.FindPropertyRelative("parameters");
                if (parametersProp.isArray)
                {
                    height += (EditorGUIUtility.singleLineHeight + 2) * parametersProp.arraySize;
                }
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // Draw Target ID
            Rect targetRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            SerializedProperty targetIdProp = property.FindPropertyRelative("targetId");
            EditorGUI.PropertyField(targetRect, targetIdProp, new GUIContent("Target Object"));
            
            SerializableGuid currentGuid = GetGuid(targetIdProp);
            if (currentGuid != SerializableGuid.Empty)
            {
                GameObject targetObj = FindObjectInScene(currentGuid);
                if (targetObj != null)
                {
                    SerializedProperty compProp = property.FindPropertyRelative("componentTypeName");
                    SerializedProperty methodProp = property.FindPropertyRelative("methodName");
                    SerializedProperty parametersProp = property.FindPropertyRelative("parameters");

                    Component[] allComponents = targetObj.GetComponents<Component>();
                    List<Component> validComponents = new List<Component>();
                    List<string> compNames = new List<string>();

                    foreach (var c in allComponents)
                    {
                        if (c == null) continue;
                        var methods = c.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                            .Where(m => IsSupportedMethod(m) && IsCustomMethod(m)).ToList();
                        
                        if (methods.Count > 0)
                        {
                            validComponents.Add(c);
                            compNames.Add(c.GetType().Name);
                        }
                    }
                    
                    compNames = compNames.Distinct().ToList();

                    // Component Selection
                    Rect compRect = new Rect(position.x, targetRect.yMax + 2, position.width, EditorGUIUtility.singleLineHeight);
                    int compIndex = Mathf.Max(0, compNames.IndexOf(compProp.stringValue));
                    if (compNames.Count > 0)
                    {
                        compIndex = EditorGUI.Popup(compRect, "Component", compIndex, compNames.ToArray());
                        compProp.stringValue = compNames[compIndex];
                    }

                    // Method Selection
                    Rect methodRect = new Rect(position.x, compRect.yMax + 2, position.width, EditorGUIUtility.singleLineHeight);
                    if (compNames.Count > 0 && !string.IsNullOrEmpty(compProp.stringValue))
                    {
                        Component selectedComp = allComponents.FirstOrDefault(c => c != null && c.GetType().Name == compProp.stringValue);
                        if (selectedComp != null)
                        {
                            var methods = selectedComp.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                .Where(m => IsSupportedMethod(m) && IsCustomMethod(m)).ToList();
                                
                            List<string> methodNames = methods.Select(m => m.Name).ToList();
                            methodNames.Insert(0, "None");
                            
                            int methodIndex = Mathf.Max(0, methodNames.IndexOf(methodProp.stringValue));
                            int newMethodIndex = EditorGUI.Popup(methodRect, "Method", methodIndex, methodNames.ToArray());
                            
                            if (newMethodIndex != methodIndex)
                            {
                                methodProp.stringValue = newMethodIndex == 0 ? "" : methodNames[newMethodIndex];
                                UpdateParameters(parametersProp, newMethodIndex == 0 ? null : methods[newMethodIndex - 1]);
                            }
                            
                            // Draw Parameters
                            if (newMethodIndex > 0)
                            {
                                float yOffset = methodRect.yMax + 2;
                                for (int i = 0; i < parametersProp.arraySize; i++)
                                {
                                    Rect paramRect = new Rect(position.x + 15, yOffset, position.width - 15, EditorGUIUtility.singleLineHeight);
                                    SerializedProperty element = parametersProp.GetArrayElementAtIndex(i);
                                    DrawParameter(paramRect, element, methods[newMethodIndex - 1].GetParameters()[i]);
                                    yOffset += EditorGUIUtility.singleLineHeight + 2;
                                }
                            }
                        }
                    }
                }
                else
                {
                    GUI.Label(new Rect(position.x, targetRect.yMax + 2, position.width, EditorGUIUtility.singleLineHeight), 
                        "Object not loaded in current scene(s)", EditorStyles.helpBox);
                }
            }

            EditorGUI.EndProperty();
        }

        private bool IsCustomMethod(MethodInfo method)
        {
            Type decType = method.DeclaringType;
            return decType != typeof(MonoBehaviour) && 
                   decType != typeof(Behaviour) && 
                   decType != typeof(Component) && 
                   decType != typeof(UnityEngine.Object) && 
                   decType != typeof(object);
        }

        private bool IsSupportedMethod(MethodInfo method)
        {
            if (method.IsSpecialName) return false; // Ignore getters/setters
            
            var parameters = method.GetParameters();
            foreach (var p in parameters)
            {
                if (p.ParameterType != typeof(string) && 
                    p.ParameterType != typeof(float) && 
                    p.ParameterType != typeof(int) && 
                    p.ParameterType != typeof(bool) &&
                    p.ParameterType != typeof(Vector2) &&
                    p.ParameterType != typeof(Vector3) &&
                    p.ParameterType != typeof(Color) &&
                    !p.ParameterType.IsEnum &&
                    p.ParameterType != typeof(GameObject) &&
                    !p.ParameterType.IsSubclassOf(typeof(Component)))
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateParameters(SerializedProperty parametersProp, MethodInfo method)
        {
            if (method == null)
            {
                parametersProp.ClearArray();
                return;
            }

            var parameters = method.GetParameters();
            parametersProp.ClearArray();
            parametersProp.arraySize = parameters.Length;

            for (int i = 0; i < parameters.Length; i++)
            {
                SerializedProperty element = parametersProp.GetArrayElementAtIndex(i);
                SerializedProperty typeProp = element.FindPropertyRelative("type");

                if (parameters[i].ParameterType == typeof(string)) typeProp.enumValueIndex = (int)MethodParameter.ParamType.String;
                else if (parameters[i].ParameterType == typeof(float)) typeProp.enumValueIndex = (int)MethodParameter.ParamType.Float;
                else if (parameters[i].ParameterType == typeof(int)) typeProp.enumValueIndex = (int)MethodParameter.ParamType.Int;
                else if (parameters[i].ParameterType == typeof(bool)) typeProp.enumValueIndex = (int)MethodParameter.ParamType.Bool;
                else if (parameters[i].ParameterType == typeof(Vector2)) typeProp.enumValueIndex = (int)MethodParameter.ParamType.Vector2;
                else if (parameters[i].ParameterType == typeof(Vector3)) typeProp.enumValueIndex = (int)MethodParameter.ParamType.Vector3;
                else if (parameters[i].ParameterType == typeof(Color)) typeProp.enumValueIndex = (int)MethodParameter.ParamType.Color;
                else if (parameters[i].ParameterType.IsEnum) typeProp.enumValueIndex = (int)MethodParameter.ParamType.Enum;
                else if (parameters[i].ParameterType == typeof(GameObject) || parameters[i].ParameterType.IsSubclassOf(typeof(Component))) typeProp.enumValueIndex = (int)MethodParameter.ParamType.GuidReference;
            }
        }

        private void DrawParameter(Rect rect, SerializedProperty paramProp, ParameterInfo paramInfo)
        {
            MethodParameter.ParamType pType = (MethodParameter.ParamType)paramProp.FindPropertyRelative("type").enumValueIndex;
            string paramName = paramInfo.Name;
            
            switch (pType)
            {
                case MethodParameter.ParamType.String:
                    EditorGUI.PropertyField(rect, paramProp.FindPropertyRelative("stringValue"), new GUIContent(paramName));
                    break;
                case MethodParameter.ParamType.Float:
                    EditorGUI.PropertyField(rect, paramProp.FindPropertyRelative("floatValue"), new GUIContent(paramName));
                    break;
                case MethodParameter.ParamType.Int:
                    EditorGUI.PropertyField(rect, paramProp.FindPropertyRelative("intValue"), new GUIContent(paramName));
                    break;
                case MethodParameter.ParamType.Bool:
                    EditorGUI.PropertyField(rect, paramProp.FindPropertyRelative("boolValue"), new GUIContent(paramName));
                    break;
                case MethodParameter.ParamType.Vector2:
                    EditorGUI.PropertyField(rect, paramProp.FindPropertyRelative("vector2Value"), new GUIContent(paramName));
                    break;
                case MethodParameter.ParamType.Vector3:
                    EditorGUI.PropertyField(rect, paramProp.FindPropertyRelative("vector3Value"), new GUIContent(paramName));
                    break;
                case MethodParameter.ParamType.Color:
                    EditorGUI.PropertyField(rect, paramProp.FindPropertyRelative("colorValue"), new GUIContent(paramName));
                    break;
                case MethodParameter.ParamType.Enum:
                    SerializedProperty enumProp = paramProp.FindPropertyRelative("enumValue");
                    string[] enumNames = Enum.GetNames(paramInfo.ParameterType);
                    enumProp.intValue = EditorGUI.Popup(rect, paramName, enumProp.intValue, enumNames);
                    break;
                case MethodParameter.ParamType.GuidReference:
                    EditorGUI.PropertyField(rect, paramProp.FindPropertyRelative("guidReference"), new GUIContent(paramName));
                    break;
            }
        }

        private SerializableGuid GetGuid(SerializedProperty targetIdProp)
        {
            SerializedProperty valueProp = targetIdProp.FindPropertyRelative("Value");
            if (valueProp == null) return SerializableGuid.Empty;
            
            return new SerializableGuid(
                (uint)valueProp.FindPropertyRelative("Part1").longValue,
                (uint)valueProp.FindPropertyRelative("Part2").longValue,
                (uint)valueProp.FindPropertyRelative("Part3").longValue,
                (uint)valueProp.FindPropertyRelative("Part4").longValue
            );
        }

        private GameObject FindObjectInScene(SerializableGuid guid)
        {
            var objs = UnityEngine.Object.FindObjectsByType<IdentifiableMonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var obj in objs)
            {
                if (obj.ID == guid) return obj.gameObject;
            }
            return null;
        }
    }
}
#endif
