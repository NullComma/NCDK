using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;

namespace NCDK {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VersionNumberTrigger : MonoBehaviour {

        [SerializeField] private string prefix = "v";
        [SerializeField] private string suffix = "";
        [Tooltip("Appends the short build id from the watermark resource, e.g. v1.0.0 (e2acb6). Builds only.")]
        [SerializeField] private bool appendBuildId = false;
        [Tooltip("Resources text asset whose last dash-separated segment is the build id.")]
        [SerializeField] private string buildIdResource = "BuildWatermark";
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
                this.textMeshProUGUI.text = $"{prefix}{parsedVersion.ToString()}{suffix}{BuildIdSuffix()}";
            } else {
                // Fallback in case someone typed "1.0.0a" in PlayerSettings, 
                // which breaks System.Version strict parsing.
                Debug.LogWarning($"[VersionNumberGetter] '{rawVersionText}' is not a strict System.Version format. Falling back to raw string.");
                this.textMeshProUGUI.text = $"{prefix}{rawVersionText}{suffix}{BuildIdSuffix()}";
            }
        }

        /// <summary>
        /// Reads the tail of the build watermark (WTDR-1.0.0-20261020-2213-e2acb6 -> e2acb6) so the UI can
        /// show which build is running. The version file itself stays a strict System.Version string;
        /// nothing parses this display text back.
        /// </summary>
        private string BuildIdSuffix() {
#if UNITY_EDITOR
            return string.Empty;
#else
            if (!appendBuildId || string.IsNullOrEmpty(buildIdResource)) return string.Empty;

            try {
                var textAsset = Resources.Load<TextAsset>(buildIdResource);
                if (textAsset == null) return string.Empty;

                string token = textAsset.text.Trim();
                Resources.UnloadAsset(textAsset);

                int lastDash = token.LastIndexOf('-');
                if (lastDash < 0 || lastDash >= token.Length - 1) return string.Empty;

                return $" ({token.Substring(lastDash + 1)})";
            } catch (Exception e) {
                Debug.LogError($"[VersionNumberGetter] Error loading build id: {e.Message}");
                return string.Empty;
            }
#endif
        }
    }
}