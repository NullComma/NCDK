using System;
using System.Collections.Generic;
using System.Reflection;

namespace EnigmaCore {
	public static class ReflectionHelpers {
		
		// Code by @Bunny83 from this Unity Answer: https://answers.unity.com/questions/983125/c-using-reflection-to-automate-finding-classes.html
		public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
		{
			var result = new List<System.Type>();
			var assemblies = aAppDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					if (type.IsSubclassOf(aType))
						result.Add(type);
				}
			}
			return result.ToArray();
		}
		
		public static T GetPrivateField<T>(this object obj, string fieldName)
		{
			var fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (fieldInfo == null)
			{
				throw new ArgumentException($"Field {fieldName} not found in type {obj.GetType().FullName}");
			}
			return (T)fieldInfo.GetValue(obj);
		}

		public static void SetPrivateField<T>(this object obj, string fieldName, T value)
		{
			var fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (fieldInfo == null)
			{
				throw new ArgumentException($"Field {fieldName} not found in type {obj.GetType().FullName}");
			}
			fieldInfo.SetValue(obj, value);
		}
		
		public static T GetPrivateProperty<T>(this object obj, string propertyName)
		{
			var propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (propertyInfo == null)
			{
				throw new ArgumentException($"Property {propertyName} not found in type {obj.GetType().FullName}");
			}
			return (T)propertyInfo.GetValue(obj);
		}
		
		public static void SetPrivateProperty<T>(this object obj, string propertyName, T value)
		{
			var propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (propertyInfo == null)
			{
				throw new ArgumentException($"Property {propertyName} not found in type {obj.GetType().FullName}");
			}
			propertyInfo.SetValue(obj, value);
		}
		
		public static T GetPrivateMethod<T>(this object obj, string methodName, params Type[] parameterTypes)
		{
			var methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, null, parameterTypes, null);
			if (methodInfo == null)
			{
				throw new ArgumentException($"Method {methodName} not found in type {obj.GetType().FullName}");
			}
			return (T)(object)methodInfo.Invoke(obj, null);
		}
		
		public static void SetPrivateMethod(this object obj, string methodName, params object[] parameters)
		{
			var methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (methodInfo == null)
			{
				throw new ArgumentException($"Method {methodName} not found in type {obj.GetType().FullName}");
			}
			methodInfo.Invoke(obj, parameters);
		}
		
	}
}
