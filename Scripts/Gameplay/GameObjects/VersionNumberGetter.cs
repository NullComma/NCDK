using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnigmaCore {
    
    public class VersionNumberGetter : MonoBehaviour {

        [SerializeField] private CUnityEventString versionStringEvent;
        [SerializeField] private string prefix = "v";
        [SerializeField] private string suffix = "";
        
        private void Awake() {
            this.LoadAndInvokeVersion();
        }

        private void LoadAndInvokeVersion() {
            string rawVersionText = "0.0.0";

#if UNITY_EDITOR
            // Editor: Grab directly from settings
            rawVersionText = PlayerSettings.bundleVersion;
#else
            // Build: Load the strictly formatted text file
            try {
                var textAsset = Resources.Load<TextAsset>(StaticStrings.GameBundleVersion);

                if (textAsset != null && !string.IsNullOrEmpty(textAsset.text)) {
                    // Trim removes any accidental whitespace or hidden newline characters
                    rawVersionText = textAsset.text.Trim(); 
                    Resources.UnloadAsset(textAsset);
                }
            }
            catch (Exception e) {
                Debug.LogError($"[VersionNumberGetter] Error loading version file: {e.Message}");
            }
#endif

            // Safely cast to a C# Version object to validate the data
            if (Version.TryParse(rawVersionText, out Version parsedVersion)) {
                // Apply the extra formatting exclusively for the UI event
                string finalDisplay = $"{prefix}{parsedVersion.ToString()}{suffix}";
                this.versionStringEvent?.Invoke(finalDisplay);
            } else {
                // Fallback in case someone typed "1.0.0a" in PlayerSettings, 
                // which breaks System.Version strict parsing.
                Debug.LogWarning($"[VersionNumberGetter] '{rawVersionText}' is not a strict System.Version format. Falling back to raw string.");
                string finalDisplay = $"{prefix}{rawVersionText}{suffix}";
                this.versionStringEvent?.Invoke(finalDisplay);
            }
        }
    }
}