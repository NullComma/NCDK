using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnigmaCore {
	
	[ExecuteInEditMode]
	public class CVersionNumberGetter : MonoBehaviour {

		[SerializeField] private CUnityEventString versionStringEvent;
		[SerializeField] private bool showDate = true;
		[NonSerialized] private string _resourceFileName = "GameBundleVersion";

		private void Awake() {
			this.LoadAndInvokeVersion();
		}

#if UNITY_EDITOR
		private void OnValidate() {
			this.UpdateVersionFileIfNeeded();
			this.LoadAndInvokeVersion();
		}

		[Button] // User added this, keeping it
		private void UpdateVersionFileIfNeeded() {
			string resourcesPath = Path.Combine(Application.dataPath, "Resources");
			string fullPath = Path.Combine(resourcesPath, _resourceFileName + ".txt");
			string currentVersion = PlayerSettings.bundleVersion;

			// Ensure Resources directory exists
			if (!Directory.Exists(resourcesPath)) {
				Directory.CreateDirectory(resourcesPath);
			}

			string fileContent = "";
			string savedVersion = "";
			
			if (File.Exists(fullPath)) {
				fileContent = File.ReadAllText(fullPath);
				// Assuming format: VERSION|DATE
				string[] parts = fileContent.Split('|');
				if (parts.Length > 0) savedVersion = parts[0];
				
				// Force update if old format (no date)
				if (parts.Length < 2) savedVersion = ""; 
			}

			// Only update if version changed or file missing/old format
			if (savedVersion != currentVersion) {
				// Save in strict invariant format (UTC) for parsing reliability
				string dateStr = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture); 
				string newContent = $"{currentVersion}|{dateStr}";
				File.WriteAllText(fullPath, newContent);
				AssetDatabase.Refresh(); // Ensure Unity sees the changes
			}
		}
#endif

		private void LoadAndInvokeVersion() {
			string versionDisplay = "0.0.0";
			
			try {
				var textAsset = Resources.Load<TextAsset>(this._resourceFileName);
				
				#if UNITY_EDITOR
				// Fallback if load fails (e.g. first run before refresh) but we have settings
				if (textAsset == null) {
					versionDisplay = PlayerSettings.bundleVersion + " (Dev)";
				}
				#endif

				if (textAsset != null && !string.IsNullOrEmpty(textAsset.text)) {
					string[] parts = textAsset.text.Split('|');
					string version = parts[0];
					
					if (this.showDate && parts.Length > 1) {
						// Parse the stored UTC date
						if (DateTime.TryParseExact(parts[1], "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime date)) {
							// Display in user's local format (e.g. 14/01/2026 in BR, 1/14/2026 in US)
							versionDisplay = $"v{version} ({date.ToLocalTime():d})";
						} else {
							// Fallback if parsing fails
							versionDisplay = $"v{version} ({parts[1]})";
						}
					} else {
						versionDisplay = $"v{version}";
					}
				}
			}
			catch (Exception e) {
				Debug.LogError($"[CVersionNumberGetter] Error loading version: {e.Message}");
			}

			this.versionStringEvent?.Invoke(versionDisplay);
		}
	}
}