using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EnigmaCore
{
    public static class ReflectionHelpers
    {
        /// Code by @Bunny83 from this Unity Answer: https://discussions.unity.com/t/c-using-reflection-to-automate-finding-classes/140638
        public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if(type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }

            return result.ToArray();
        }

        public static T GetPrivateField<T>(this object obj, string fieldName)
        {
            var type = obj.GetType();
            FieldInfo fieldInfo = null;
            while (type != null)
            {
                fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (fieldInfo != null) break;
                type = type.BaseType;
            }

            if(fieldInfo == null)
            {
                throw new ArgumentException($"Field {fieldName} not found in type {obj.GetType().FullName} or its base types.");
            }

            return (T)fieldInfo.GetValue(obj);
        }

        public static void SetPrivateField<T>(this object obj, string fieldName, T value)
        {
            var type = obj.GetType();
            FieldInfo fieldInfo = null;
            while (type != null)
            {
                fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (fieldInfo != null) break;
                type = type.BaseType;
            }

            if(fieldInfo == null)
            {
                throw new ArgumentException($"Field {fieldName} not found in type {obj.GetType().FullName} or its base types.");
            }

            fieldInfo.SetValue(obj, value);
        }

        public static T GetPrivateProperty<T>(this object obj, string propertyName)
        {
            var type = obj.GetType();
            PropertyInfo propertyInfo = null;
            while (type != null)
            {
                propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (propertyInfo != null) break;
                type = type.BaseType;
            }

            if(propertyInfo == null)
            {
                throw new ArgumentException($"Property {propertyName} not found in type {obj.GetType().FullName} or its base types.");
            }

            return (T)propertyInfo.GetValue(obj);
        }

        public static void SetPrivateProperty<T>(this object obj, string propertyName, T value)
        {
            var type = obj.GetType();
            PropertyInfo propertyInfo = null;
            while (type != null)
            {
                propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (propertyInfo != null) break;
                type = type.BaseType;
            }

            if(propertyInfo == null)
            {
                throw new ArgumentException($"Property {propertyName} not found in type {obj.GetType().FullName} or its base types.");
            }

            propertyInfo.SetValue(obj, value);
        }

        /// <summary>
        /// Invokes a private instance method that does not return a value (void).
        /// </summary>
        public static void InvokePrivateMethod(this object obj, string methodName, params object[] parameters)
        {
            var methodInfo = GetMethodInfo(obj.GetType(), methodName, parameters);
            if(methodInfo == null)
            {
                Debug.LogError(
                    $"Method '{methodName}' with specified parameters not found in type {obj.GetType().FullName}");
                return;
            }

            methodInfo.Invoke(obj, parameters);
        }

        /// <summary>
        /// Invokes a private instance method that returns a value.
        /// </summary>
        public static T InvokePrivateMethod<T>(this object obj, string methodName, params object[] parameters)
        {
            var methodInfo = GetMethodInfo(obj.GetType(), methodName, parameters);
            if(methodInfo == null)
            {
                Debug.LogError(
                    $"Method '{methodName}' with specified parameters not found in type {obj.GetType().FullName}");
                return default;
            }

            return (T)methodInfo.Invoke(obj, parameters);
        }

        /// <summary>
        /// Finds a MethodInfo in a given type that best matches the provided method name and parameter values.
        /// This robust version correctly handles null parameters, inheritance, and polymorphism.
        /// </summary>
        /// <exception cref="AmbiguousMatchException">Thrown if more than one method matches the arguments.</exception>
        private static MethodInfo GetMethodInfo(Type type, string methodName, object[] parameters)
        {
            var searchFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                              BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            var parameterCount = parameters?.Length ?? 0;

            // 1. Get all methods from the type that match the name and parameter count.
            var candidates = type.GetMethods(searchFlags)
                .Where(m => m.Name == methodName && m.GetParameters().Length == parameterCount)
                .ToList();

            if(candidates.Count == 0)
            {
                return null; // No method found with that name and parameter count.
            }

            if(candidates.Count == 1)
            {
                return candidates.First(); // If there's only one, it's our match.
            }

            // 2. If there are multiple candidates, we need to find the best match based on parameter types.
            var validCandidates = new List<MethodInfo>();

            foreach (var candidate in candidates)
            {
                bool isMatch = true;
                var methodParameters = candidate.GetParameters();
                for (int i = 0; i < parameterCount; i++)
                {
                    var methodParamType = methodParameters[i].ParameterType;
                    var passedValue = parameters[i];

                    if(passedValue == null)
                    {
                        // A null value is valid only if the target parameter is a reference type or a Nullable value type.
                        if(!methodParamType.IsValueType || Nullable.GetUnderlyingType(methodParamType) != null) continue;
                        isMatch = false;
                        break;
                    }

                    // A non-null value is valid if its type can be assigned to the parameter's type.
                    // This correctly handles base classes and interfaces.
                    if(methodParamType.IsAssignableFrom(passedValue.GetType())) continue;
                    isMatch = false;
                    break;
                }

                if(isMatch)
                {
                    validCandidates.Add(candidate);
                }
            }

            // 3. Final validation.
            if(validCandidates.Count > 1)
            {
                // If we still have more than one valid candidate, the call is ambiguous.
                throw new AmbiguousMatchException(
                    $"Ambiguous method call. More than one method named '{methodName}' matches the provided arguments."
                );
            }

            return validCandidates.FirstOrDefault();
        }
    }
}