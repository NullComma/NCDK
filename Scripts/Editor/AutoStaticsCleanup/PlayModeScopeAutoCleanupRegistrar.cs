#if UNITY_EDITOR && !UNITY_6000_5_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NCDK
{
    [InitializeOnLoad]
    internal static class PlayModeScopeAutoCleanupRegistrar
    {
        private static readonly Dictionary<Type, PlayModeScopeAutoCleanup> SByType = new();

        static PlayModeScopeAutoCleanupRegistrar()
        {
            foreach (var t in TypeCache.GetTypesDerivedFrom<PlayModeScopeAutoCleanup>())
            {
                if (t.IsAbstract || t.ContainsGenericParameters) continue;
                var instance = (PlayModeScopeAutoCleanup) Activator.CreateInstance(t);
                var instanceType = instance.GetType();
                SByType[instanceType] = instance;
            }

            EditorApplication.playModeStateChanged -= OnChange;
            EditorApplication.playModeStateChanged += OnChange;
        }

        private static void OnChange(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.ExitingEditMode
                && change != PlayModeStateChange.ExitingPlayMode) return;
            foreach (var c in SByType.Values)
                c.Cleanup();
        }
    }
}
#endif
