using System.Collections.Generic;
using UnityEngine;

namespace EnigmaCore
{
    /// <summary>
    /// Static class to manage AssetBundle memory usage with reference counting.
    /// Prevents the "AssetBundle already loaded" error when multiple objects use the same file.
    /// </summary>
    public static class AssetBundleReferenceCache
    {
        // Map: File Path -> Loaded Bundle
        private static readonly Dictionary<string, AssetBundle> LoadedBundles = new();
        
        // Map: File Path -> Usage Count
        private static readonly Dictionary<string, int> ReferenceCount = new();

        /// <summary>
        /// Tries to get a loaded bundle from the cache.
        /// If found, increments the reference count and returns it.
        /// Returns null if not loaded yet.
        /// </summary>
        public static AssetBundle Acquire(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            if (LoadedBundles.TryGetValue(path, out var bundle))
            {
                // Safety check: if Unity unloaded it externally (should not happen if managed correctly)
                if (bundle == null) 
                {
                    LoadedBundles.Remove(path);
                    ReferenceCount.Remove(path);
                    return null;
                }

                if (ReferenceCount.ContainsKey(path))
                {
                    ReferenceCount[path]++;
                }
                else
                {
                    ReferenceCount[path] = 1;
                }

                // Debug.Log($"[AssetBundleCache] Acquired: {path} (Refs: {ReferenceCount[path]})");
                return bundle;
            }
            return null;
        }

        /// <summary>
        /// Registers a newly loaded bundle into the cache with 1 reference.
        /// </summary>
        public static void Register(string path, AssetBundle bundle)
        {
            if (string.IsNullOrEmpty(path) || bundle == null) return;

            if (LoadedBundles.ContainsKey(path))
            {
                // Edge case: If it was loaded in parallel, we use the existing one and unload the duplicate
                bundle.Unload(true);
                Acquire(path); 
                return;
            }

            LoadedBundles[path] = bundle;
            ReferenceCount[path] = 1;
            // Debug.Log($"[AssetBundleCache] Registered new: {path}");
        }

        /// <summary>
        /// Decrements reference count. If it reaches zero, unloads the bundle from memory.
        /// </summary>
        public static void Release(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            if (ReferenceCount.ContainsKey(path))
            {
                ReferenceCount[path]--;

                // Debug.Log($"[AssetBundleCache] Released: {path} (Refs remaining: {ReferenceCount[path]})");

                if (ReferenceCount[path] <= 0)
                {
                    if (LoadedBundles.TryGetValue(path, out var bundle))
                    {
                        if (bundle != null) bundle.Unload(true);
                    }
                    
                    LoadedBundles.Remove(path);
                    ReferenceCount.Remove(path);
                    // Debug.Log($"[AssetBundleCache] Unloaded from memory: {path}");
                }
            }
        }
    }
}