using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnigmaCore.DependencyInjection
{
    /// <summary>
    /// Handles both Global DI Installation and Per-Scene PreLoad logic.
    /// </summary>
    public static class Bootstrapper
    {
        // ---------------------------------------------------------
        // PART 1: DI Installation
        // ---------------------------------------------------------
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitializeServices()
        {
            string[] ignoredPrefixes = new[] 
            { 
                "UnityEngine", 
                "UnityEditor", 
                "mscorlib", 
                "System", 
                "Mono", 
                "Microsoft", 
                "nunit",
                "Bee",
                "Gradle"
            };

            var installers = AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => 
                {
                    string name = asm.GetName().Name;
                    return !ignoredPrefixes.Any(prefix => name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
                })
                .SelectMany(asm => 
                {
                    try { return asm.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .Where(t => typeof(IInstaller).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => 
                {
                    try { return Activator.CreateInstance(t, true) as IInstaller; }
                    catch (Exception e)
                    {
                        Debug.LogError($"[Bootstrapper] Failed to instantiate installer '{t.Name}': {e.Message}");
                        return null;
                    }
                })
                .Where(i => i != null)
                .OrderBy(i => i.Priority)
                .ToList();

            foreach (var installer in installers)
            {
                try { installer.Install(); }
                catch (Exception e)
                {
                    Debug.LogError($"[Bootstrapper] Failed to install {installer.GetType().Name}: {e}");
                }
            }
        }
        
        // ---------------------------------------------------------
        // PART 2: Scene Lifecycle Hooks
        // ---------------------------------------------------------
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeSceneWatcher()
        {
            // Prevents duplicate registration in case of Domain Reload disabled
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!scene.IsValid()) return;

            // Using standard for-loops to avoid extra garbage collection during scene load
            var rootObjects = scene.GetRootGameObjects();
            
            for (int i = 0; i < rootObjects.Length; i++)
            {
                var preLoadables = rootObjects[i].GetComponentsInChildren<IPreLoad>(true);
                
                for (int j = 0; j < preLoadables.Length; j++)
                {
                    try 
                    {
                        preLoadables[j].OnPreLoad();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[Bootstrapper] Error executing OnPreLoad for {preLoadables[j].GetType().Name}: {e}");
                    }
                }
            }
        }
    }
}