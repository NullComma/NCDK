using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace NCDK {
	
	/// <summary>
	/// From https://forum.unity.com/threads/how-can-i-get-bundle-version-and-bundle-version-code-through-script.68331/#post-6497023
	/// </summary>
	public class CVersionWriter {
		private const string fileName = "GameBundleVersion.txt";

		[DidReloadScripts]
		public static void CheckVersion() {
			string finalPath = Path.Combine(Application.dataPath, "Resources", fileName);

			string newText = PlayerSettings.bundleVersion;

			using (var streamWriter = File.CreateText(finalPath)) {
				streamWriter.Write(newText);
			}
		}
		
	}
}
