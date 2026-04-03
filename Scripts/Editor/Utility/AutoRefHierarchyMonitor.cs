using UnityEditor;
using UnityEngine;

namespace NullCore.Editor
{
	[InitializeOnLoad]
	public static class AutoRefHierarchyMonitor
	{
		static AutoRefHierarchyMonitor()
		{
			EditorApplication.hierarchyChanged += OnHierarchyChanged;
		}

		static void OnHierarchyChanged()
		{
			if (Application.isPlaying) return;

			GameObject[] selectedObjects = Selection.gameObjects;
			if (selectedObjects == null || selectedObjects.Length == 0) return;

			foreach (GameObject obj in selectedObjects)
			{
				Component[] components = obj.GetComponentsInChildren<Component>(true);
				foreach (Component comp in components)
				{
					if (comp == null) continue;

					// False to avoid spamming the console
					AutoRefValidator.Validate(comp, false);
				}
			}
		}
	}
}
