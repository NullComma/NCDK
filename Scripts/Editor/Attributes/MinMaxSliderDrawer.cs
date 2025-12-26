#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EnigmaCore
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (MinMaxSliderAttribute)attribute;
            EnigmaEditorUtils.DrawMinMaxSlider(position, property, attr);
        }
    }
}
#endif