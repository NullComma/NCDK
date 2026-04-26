#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace NullCore.Editor
{
    /// <summary>
    /// Performs a final check for duplicate SerializableGuids before building the project.
    /// </summary>
    public class IdentifiableBuildValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("[IdentifiableBuildValidator] Checking for duplicate SerializableGuids before build...");
            
            bool hasDuplicates = IdentifiableRegistry.CheckForDuplicates(logErrors: true);
            
            if (hasDuplicates)
            {
                // As per user request, we only log errors and continue the build.
                Debug.LogWarning("[IdentifiableBuildValidator] Duplicate GUIDs were found. Please review the errors above. Proceeding with build as requested.");
            }
            else
            {
                Debug.Log("[IdentifiableBuildValidator] No duplicate GUIDs found. Build safe.");
            }
        }
    }
}
#endif
