using System;
using UnityEngine;

namespace NCDK
{
	/// <summary>
	/// Defines the search scope for the auto-assigned reference.
	/// </summary>
	public enum RefLocation
	{
		Self,
		Child,
		Parent,
		Scene
	}

	/// <summary>
	/// Automatically fetches and assigns the required component reference in the Editor.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class AutoRefAttribute : PropertyAttribute
	{
		public RefLocation Location { get; }

		public AutoRefAttribute(RefLocation location = RefLocation.Self)
		{
			Location = location;
		}
	}
}