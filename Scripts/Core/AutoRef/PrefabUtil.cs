using UnityEngine;

namespace NCDK.Refs
{
    // Subset taken from NCDK.Utils.PrefabUtil for open sourcing
    internal class PrefabUtil
    {
        internal static bool IsUninstantiatedPrefab(GameObject obj)
            => obj.scene.rootCount == 0;
    }
}