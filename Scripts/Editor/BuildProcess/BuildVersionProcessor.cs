using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace EnigmaCore.Editor {
    
    public class BuildVersionProcessor : IPreprocessBuildWithReport {
        
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) {
            string resourcesPath = Path.Combine(Application.dataPath, "Resources");
            string fullPath = Path.Combine(resourcesPath, StaticStrings.GameBundleVersion + ".txt");

            if (!Directory.Exists(resourcesPath)) {
                Directory.CreateDirectory(resourcesPath);
            }

            // Strictly the raw version number (e.g., "1.0.0")
            string rawVersion = PlayerSettings.bundleVersion;

            File.WriteAllText(fullPath, rawVersion);
            AssetDatabase.ImportAsset(fullPath, ImportAssetOptions.ForceUpdate);
            
            Debug.Log($"[{nameof(BuildVersionProcessor)}] Saved raw build version: {rawVersion}");
        }
    }
}