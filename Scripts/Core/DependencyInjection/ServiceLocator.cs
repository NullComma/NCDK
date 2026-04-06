using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace NullCore
{
    /// <summary>
    /// A static Service Locator for registering and resolving services basicly without reflection overhead.
    /// </summary>
    public static class ServiceLocator

    {
        static readonly ConcurrentDictionary<Type, object> _instances = new();
        static readonly ConcurrentDictionary<Type, Func<object>> _lazyFactories = new();

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
            _lazyFactories.Clear();
        }

        /// <summary>
        /// Registers an existing instance as a singleton for type T.
        /// </summary>
        public static void Register<T>(T instance)
        {
            var typeOf = typeof(T);

            // Safety check to ensure we aren't registering generic Object types unintentionally
            if (IsGenericObjectType(typeOf))
            {
                typeOf = instance.GetType();
                if (IsGenericObjectType(typeOf))
                {
                    throw new Exception("Cannot register instance of type Object, MonoBehaviour or object. Use a more specific type.");
                }
            }


            _instances[typeOf] = instance;
        }

        public static void Register<T>()
        {
            _instances[typeof(T)] = Activator.CreateInstance<T>();
        }


        /// <summary>
        /// Registers a factory delegate to create the instance only when first invoked.
        /// </summary>
        public static void RegisterLazy<T>(Func<T> factory)
        {
            var typeOf = typeof(T);

            if (IsGenericObjectType(typeOf))
            {
                throw new Exception("Cannot register instance of type Object, MonoBehaviour or object. Use a more specific type.");
            }


            _lazyFactories[typeOf] = () => factory();
        }

        public static void RegisterLazy<T>()
        {
            _lazyFactories[typeof(T)] = () => Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Resolves and returns the registered instance of type T.
        /// </summary>
        public static T Resolve<T>()
        {
            if (EApplication.IsQuitting)
            {
                Debug.LogError($"Tried to Resolve a '{typeof(T).Name}' dependency while quitting application! Will not be resolved.");
                return default;
            }
            if (_instances.TryGetValue(typeof(T), out var instance))
                return (T)instance;


            if (_lazyFactories.TryGetValue(typeof(T), out var factory))
            {
                var newInstance = (T)factory();
                _instances[typeof(T)] = newInstance;

                _lazyFactories.TryRemove(typeof(T), out _);
                return newInstance;
            }


            throw new Exception($"Instance of type {typeof(T)} not registered. Make sure it's registered through an Installer or [IPreLoad].");
        }

        /// <summary>
        /// Resolves type T and outputs the instance.
        /// </summary>
        public static void Resolve<T>(out T instance)
        {
            instance = Resolve<T>();
        }

        /// <summary>
        /// Resolves and returns the registered instance of the specified type.
        /// </summary>
        public static object Resolve(Type serviceType)
        {
            if (_instances.TryGetValue(serviceType, out var instance))
                return instance;


            if (_lazyFactories.TryGetValue(serviceType, out var factory))
            {
                var newInstance = factory();
                _instances[serviceType] = newInstance;
                _lazyFactories.TryRemove(serviceType, out _);
                return newInstance;
            }


            throw new Exception($"Instance of type {serviceType} not registered. Make sure it's registered through an Installer or [IPreLoad].");
        }

        static bool IsGenericObjectType(Type t)
        {
            return t == typeof(UnityEngine.Object) || t == typeof(MonoBehaviour) || t == typeof(System.Object) || t == typeof(object);
        }

        public static T Get<T>()
        {
            return Resolve<T>();
        }
    }
}
