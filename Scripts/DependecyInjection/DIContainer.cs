using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EnigmaCore.DependecyInjection
{
    public static class DIContainer {
        static readonly ConcurrentDictionary<Type, object> _instances = new ();
        /// Cache for reflection results. Maps a Type to a list of its injectable fields.
        static readonly Dictionary<Type, List<FieldInfo>> _injectionCache = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            Application.quitting += Dispose;
        }

        static void Dispose()
        {
            Application.quitting -= Dispose;
            foreach (var instance in _instances.Values)
            {
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _instances.Clear();
        }

        public static void Register<T>(T instance)
        {
            var typeOf = typeof(T);
            if(typeOf == typeof(UnityEngine.Object) || typeOf == typeof(MonoBehaviour) || typeOf == typeof(System.Object) || typeOf == typeof(object))
            {
                typeOf = instance.GetType();
                if(typeOf == typeof(UnityEngine.Object) || typeOf == typeof(MonoBehaviour) || typeOf == typeof(System.Object) || typeOf == typeof(object))
                {
                    throw new Exception("Cannot register instance of type Object, MonoBehaviour or object. Use a more specific type.");
                }
            }
            _instances[typeOf] = instance;
            InjectAttributes(instance);
        }

        public static void Register(Type serviceType)
        {
            var constructors = serviceType.GetConstructors();
            if (constructors.Length != 1)
                throw new Exception($"Type {serviceType} must have exactly one public constructor to be registered by type.");

            var constructor = constructors[0];
            var parameters = constructor.GetParameters();
            var parametersInstances = parameters.Select(param =>
            {
                var resolved = Resolve(param.ParameterType);
                if (resolved == null)
                    throw new Exception($"Dependency {param.ParameterType} not registered for {serviceType}");
                return resolved;
            }).ToArray();

            var instance = constructor.Invoke(parametersInstances);
            Register(instance);
        }

        public static T Resolve<T>() {
            if (EApplication.IsQuitting)
            {
                Debug.LogError($"Tried to Resolve a '{typeof(T).Name}' dependency while quitting application! Will not be resolved.");
                return default;
            }
            if (_instances.TryGetValue(typeof(T), out var instance))
                return (T)instance;
            throw new Exception($"Instance of type {typeof(T)} not registered.");
        }

        public static void Resolve<T>(out T instance)
        {
            instance = Resolve<T>();
        }

        public static object Resolve(Type serviceType)
        {
            if (_instances.TryGetValue(serviceType, out var instance))
                return instance;
            throw new Exception($"Instance of type {serviceType} not registered.");
        }

        public static void InjectDependencies(object target) {
            InjectAttributes(target);
        }
        
        static void InjectAttributes(object target) {
            if (!Application.isPlaying)
            {
                Debug.LogError("Tried to inject a dependency on Editor, this is not supported. Did you called Inject() inside an OnValidate method?");
                return;
            }
            if (EApplication.IsQuitting)
            {
                Debug.LogError($"Tried to inject dependencies on a target while quitting application! Will not be injected.");
                return;
            }
            if (target == null)
            {
                Debug.LogError("Tried to inject dependencies on a null target!");
                return;
            }
            
            var type = target.GetType();
            
            // 1. Check if the type is already in our cache
            if (!_injectionCache.TryGetValue(type, out var injectableFields))
            {
                // 2. If not, perform reflection once and build the list of fields
                injectableFields = new List<FieldInfo>();
                var currentType = type;
                while (currentType != null)
                {
                    var fields = currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (var field in fields)
                    {
                        if (Attribute.IsDefined(field, typeof(InjectAttribute)))
                        {
                            injectableFields.Add(field);
                        }
                    }
                    currentType = currentType.BaseType;
                }
                // 3. Add the results to the cache for future use
                _injectionCache[type] = injectableFields;
            }

            // 4. Inject dependencies using the cached field info
            foreach (var field in injectableFields)
            {
                try
                {
                    var dependency = Resolve(field.FieldType);
                    field.SetValue(target, dependency);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error injecting dependency for {field.FieldType} in {target.GetType()}: {ex.Message}");
                }
            }
        }

    }
}