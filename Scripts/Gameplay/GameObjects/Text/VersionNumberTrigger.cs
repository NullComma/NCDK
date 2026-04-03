using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;

namespace NullCore {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VersionNumberTrigger : MonoBehaviour {

        [SerializeField] private string prefix = "v";
        [SerializeField] private string suffix = "";
        [NonSerialized] private TextMeshProUGUI textMeshProUGUI;
        
        private void Awake() {
            TryGetComponent(out this.textMeshProUGUI);
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
                this.textMeshProUGUI.text = finalDisplay;
            } else {
                // Fallback in case someone typed "1.0.0a" in PlayerSettings, 
                // which breaks System.Version strict parsing.
                Debug.LogWarning($"[VersionNumberGetter] '{rawVersionText}' is not a strict System.Version format. Falling back to raw string.");
                string finalDisplay = $"{prefix}{rawVersionText}{suffix}";
                this.textMeshProUGUI.text = finalDisplay;
            }
        }
    }
}