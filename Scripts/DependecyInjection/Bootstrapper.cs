using System;
using System.Linq;
using UnityEngine;

namespace EnigmaCore.DependencyInjection
{
    /// <summary>
    /// Automatically finds and executes all IInstaller implementations in the AppDomain.
    /// </summary>
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitializeSubsystemRegistration()
        {
            var installers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => typeof(IInstaller).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => 
                {
                    try 
                    {
                        return Activator.CreateInstance(t, true) as IInstaller;
                    }
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
                try 
                {
                    installer.Install();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Bootstrapper] Failed to install {installer.GetType().Name}: {e}");
                }
            }
        }
    }
}