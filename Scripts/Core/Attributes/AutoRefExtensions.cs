using UnityEngine;

namespace NCDK
{
	/// <summary>
	/// Extension methods for AutoRef validation.
	/// </summary>
	public static class AutoRefExtensions
	{
		public static void ValidateRefs(this Component component)
		{
#if UNITY_EDITOR
			AutoRefValidator.Validate(component);
#endif
		}
	}
}