#if UNITY_EDITOR && !UNITY_6000_5_OR_NEWER
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AutoStaticsCleanup
{
    [InitializeOnLoad]
    internal static class AutoStaticsCleanupRegistrar
    {
        static AutoStaticsCleanupRegistrar()
        {
            EditorApplication.playModeStateChanged -= OnChange;
            EditorApplication.playModeStateChanged += OnChange;
        }

        private static void OnChange(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingEditMode
                && change != PlayModeStateChange.EnteredEditMode) return;

            var snapshot = DelegateAutoCleanup.RegisteredInstances.ToArray();
            foreach (var c in snapshot)
            {
                try
                {
                    c.Cleanup();
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to cleanup " + c + ": " + e.Message);
                }
            }
        }
    }
}
#endif
