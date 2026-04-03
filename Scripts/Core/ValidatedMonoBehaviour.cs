using UnityEngine;

namespace NullCore
{
	/// <summary>
	/// Base class that automatically validates AutoRef attributes in the Editor.
	/// </summary>
	public abstract class ValidatedMonoBehaviour : MonoBehaviour
	{

#if UNITY_EDITOR

		protected virtual void OnValidate()
		{
			this.ValidateRefs();
		}
#endif
	}
}