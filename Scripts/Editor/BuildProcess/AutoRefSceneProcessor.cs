#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace NCDK.Editor
{
	/// <summary>
	/// Automatically scans and validates AutoRef attributes before a build and when entering Play Mode.
	/// </summary>
	public class AutoRefSceneProcessor : IProcessSceneWithReport
	{
		public int callbackOrder => 0;

		const long SceneLogThresholdMs = 20; // Threshold in milliseconds for the entire scene scan

		public void OnProcessScene(Scene scene, BuildReport report)
		{
			if (!scene.isLoaded) return;

			Stopwatch stopwatch = Stopwatch.StartNew();
			int modifiedCount = 0;

			GameObject[] rootObjects = scene.GetRootGameObjects();
			foreach (GameObject root in rootObjects)
			{
				Component[] components = root.GetComponentsInChildren<Component>(true);
				foreach (Component comp in components)
				{
					// We pass false here to avoid spamming the console for individual components during a bulk scan.
					// The total time of the scene is what matters here.
					if (AutoRefValidator.Validate(comp, false)) 
					{
						modifiedCount++;
					}
				}
			}

			stopwatch.Stop();
			
			if (stopwatch.ElapsedMilliseconds > SceneLogThresholdMs)
			{
				Debug.LogError($"[AutoRef] Scene '{scene.name}' scan took {stopwatch.ElapsedMilliseconds}ms. Validated {modifiedCount} components.");
			}
			else if (modifiedCount > 0)
			{
				Debug.Log($"<color=#42f5e3>[AutoRef]</color> Scene '{scene.name}' successfully validated in {stopwatch.ElapsedMilliseconds}ms. Auto-assigned {modifiedCount} references.");
			}
		}
	}
}
#endif