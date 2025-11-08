#if UNITY_ADDRESSABLES_EXIST
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CDK {
    public static class CAddressablesExtensions {

        public static void ClearAddressables() {
#if !UNITY_WEBGL
            Caching.ClearCache();
#endif
            Addressables.UpdateCatalogs();
            Addressables.CleanBundleCache();
        }

    }
}
#endif