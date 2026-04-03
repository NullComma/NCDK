using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NullCore {
	public class TwoColorsRandomizer : MonoBehaviour {

		[SerializeField] private Color _colorOne = Color.magenta;
		[SerializeField] private Color _colorTwo = Color.blue;

		[SerializeField] private CUnityEventColor _colorEvent;

		public void RandomizeColorsAndTriggerEvent() {
			this._colorEvent?.Invoke(new Color(
				Random.Range(this._colorOne.r, this._colorTwo.r), 
				Random.Range(this._colorOne.g, this._colorTwo.g), 
				Random.Range(this._colorOne.b, this._colorTwo.b)
			));
		}
	}
	
	#if UNITY_EDITOR
	[CustomEditor(typeof(TwoColorsRandomizer))]
	public class CTwoColorsRandomizerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			this.DrawDefaultInspector();
        
			var myScript = (TwoColorsRandomizer)this.target;
			if(GUILayout.Button(nameof(TwoColorsRandomizer.RandomizeColorsAndTriggerEvent)))
			{
				myScript.RandomizeColorsAndTriggerEvent();
			}
		}
	}
	#endif
}