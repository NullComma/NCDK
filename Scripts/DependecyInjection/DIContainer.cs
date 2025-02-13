using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnigmaCore.DependecyInjection
{
    public static class DIContainer {
        static Dictionary<Type, object> _instances = new ();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init() {
            CApplication.QuittingEvent += Dispose;
        }

        static void Dispose()
        {
            foreach (var instance in _instances.Values)
            {
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _instances.Clear();
            CApplication.QuittingEvent -= Dispose;
        }

        public static void Register<T>(T instance) {
            _instances[typeof(T)] = instance;
        }

        public static void Register(Type serviceType)
        {
            var constructor = serviceType.GetConstructors().FirstOrDefault();

            if (constructor == null)
            {
                throw new Exception($"No constructor found for {serviceType}");
            }

            var parameters = constructor.GetParameters();
            var parametersInstances = parameters.Select(param =>
            {
                var resolved = Resolve(param.ParameterType);
                if (resolved == null)
                {
                    throw new Exception($"Dependency {param.ParameterType} not registered for {serviceType}");
                }
                return resolved;
            }).ToArray();

            var instance = constructor.Invoke(parametersInstances);

            _instances[serviceType] = instance;
        }

        public static T Resolve<T>() {
            if (CApplication.IsQuitting)
            {
                Debug.LogError($"Tried to Resolve a '{typeof(T).Name}' dependency while quitting application! Will not be resolved.");
                return default;
            }
            if (_instances.TryGetValue(typeof(T), out var instance)) return (T)instance;
            throw new Exception($"Instance of type {typeof(T)} not registered.");
        }

        public static void Resolve<T>(out T instance)
        {
            instance = Resolve<T>();
        }

        public static object Resolve(Type serviceType)
        {
            if (_instances.TryGetValue(serviceType, out var instance))
            {
                return instance;
            }
            throw new Exception($"Instance of type {serviceType} not registered.");
        }
    }

}