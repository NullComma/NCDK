#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;

namespace NCDK
{
    /// <summary>
    /// Core validation logic for AutoRef attributes.
    /// </summary>
    public static class AutoRefValidator
    {
        [System.NonSerialized]
        static readonly Dictionary<Type, FieldInfo[]> FieldCache = new Dictionary<Type, FieldInfo[]>();

        const long ComponentLogThresholdMs = 2; // Threshold in milliseconds for a single component

        public static bool Validate(Component target, bool logPerformance = false)
        {
            if (target == null) return false;

            Stopwatch stopwatch = null;
            if (logPerformance) stopwatch = Stopwatch.StartNew();

            Type type = target.GetType();
            if (!FieldCache.TryGetValue(type, out FieldInfo[] fields))
            {
                fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                FieldCache[type] = fields;
            }

            bool isModified = false;

            foreach (FieldInfo field in fields)
            {
                AutoRefAttribute attr = field.GetCustomAttribute<AutoRefAttribute>();
                if (attr == null) continue;

                object currentValue = field.GetValue(target);
                Type fieldType = field.FieldType;
                bool isArray = fieldType.IsArray;
                bool isList = fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>);
                
                if (!isArray && !isList)
                {
                    // Single reference
                    if (currentValue != null && !currentValue.Equals(null)) continue;
                }
                else
                {
                    // Collection - only skip if it has elements
                    if (isArray && currentValue != null && ((Array)currentValue).Length > 0) continue;
                    if (isList && currentValue != null && ((IList)currentValue).Count > 0) continue;
                }

                Type elementType = isArray ? fieldType.GetElementType() : (isList ? fieldType.GetGenericArguments()[0] : fieldType);

                object foundRef = FindReference(target, attr.Location, elementType, isArray || isList);

                if (foundRef != null && (!isArray && !isList || (isArray && ((Array)foundRef).Length > 0) || (isList && ((IList)foundRef).Count > 0)))
                {
                    if (isArray)
                    {
                        Array sourceArray = (Array)foundRef;
                        Array typedArray = Array.CreateInstance(elementType, sourceArray.Length);
                        Array.Copy(sourceArray, typedArray, sourceArray.Length);
                        field.SetValue(target, typedArray);
                    }
                    else if (isList)
                    {
                        IList sourceList = (IList)foundRef;
                        IList typedList = (IList)Activator.CreateInstance(fieldType);
                        foreach (var item in sourceList)
                        {
                            typedList.Add(item);
                        }
                        field.SetValue(target, typedList);
                    }
                    else
                    {
                        field.SetValue(target, foundRef);
                    }
                    isModified = true;
                }
                else
                {
                    Debug.LogError($"[AutoRef] Failed to resolve '{field.Name}' on '{target.gameObject.name}'. Location: {attr.Location}");
                }
            }

            if (isModified && !Application.isPlaying)
            {
                EditorUtility.SetDirty(target);
            }

            if (logPerformance)
            {
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds >= ComponentLogThresholdMs)
                {
                    Debug.LogError($"[AutoRef] Validation on '{target.gameObject.name}' took {stopwatch.ElapsedMilliseconds}ms. Consider reducing component complexity.");
                }
            }

            return isModified;
        }

        static object FindReference(Component target, RefLocation location, Type type, bool isMultiple)
        {
            switch (location)
            {
                case RefLocation.Self:
                    return isMultiple ? (object)target.GetComponents(type) : target.GetComponent(type);
                case RefLocation.Child:
                    return isMultiple ? (object)target.GetComponentsInChildren(type, true) : target.GetComponentInChildren(type, true);
                case RefLocation.Parent:
                    return isMultiple ? (object)target.GetComponentsInParent(type, true) : target.GetComponentInParent(type, true);
                case RefLocation.Scene:
                    return isMultiple 
                        ? (object)Object.FindObjectsByType(type, FindObjectsInactive.Include, FindObjectsSortMode.None) 
                        : Object.FindFirstObjectByType(type, FindObjectsInactive.Include);
                default:
                    return null;
            }
        }
    }
}
#endif