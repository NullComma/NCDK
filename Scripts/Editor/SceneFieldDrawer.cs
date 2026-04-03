using UnityEngine;
using UnityEditor;

namespace NullCore {
	[CustomPropertyDrawer(typeof(SceneField))]
	public class CSceneFieldPropertyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
 
			EditorGUI.BeginProperty(position, label, sceneAssetProp);
			EditorGUI.PropertyField(position, sceneAssetProp, label);
			EditorGUI.EndProperty();
		}
	}

}
