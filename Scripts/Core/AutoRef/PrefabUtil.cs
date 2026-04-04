using UnityEngine;

namespace NullCore.Refs
{
    // Subset taken from NullCore.Utils.PrefabUtil for open sourcing
    internal class PrefabUtil
    {
        internal static bool IsUninstantiatedPrefab(GameObject obj)
            => obj.scene.rootCount == 0;
    }
}