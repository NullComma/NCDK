using UnityEngine;
using NCDK.Refs;

namespace NCDK
{
	/// <summary>
	/// Projects a simple quad shadow onto the ground using a single raycast.
	/// Resizes dynamically based on the target renderer's bounds and tilt.
	/// Operates with zero garbage allocation.
	/// </summary>
	[RequireComponent(typeof(Renderer))]
	public class QuadShadowFollower : ValidatedMonoBehaviour
	{
		[SerializeField, Scene] Renderer cachedRenderer;
		public Renderer coinRenderer;
		public LayerMask groundMask = 1;
		public float raycastDistance = 10f;
		public float groundOffset = 0.02f;
		public float minimumThickness = 0.02f;
		public float overallMultiplier = 1.0f;
		public Vector3 globalPositionOffset = Vector3.zero;
		public Vector3 rotationOffset = new Vector3(90f, 0f, 0f);

		[System.NonSerialized] Vector3 initialLocalBoundsSize;
		[System.NonSerialized] bool hasInitialized;

		void Start()
		{
			Initialize();
		}

		void Initialize()
		{
			if (hasInitialized) return;

			if (coinRenderer != null)
			{
				initialLocalBoundsSize = coinRenderer.localBounds.size;
				hasInitialized = true;
			}
		}

		void LateUpdate()
		{
			if (coinRenderer == null) return;
			if (!hasInitialized) Initialize();

			Transform coinTransform = coinRenderer.transform;
			Vector3 rayStartPos = coinTransform.position + globalPositionOffset;

			if (Physics.Raycast(rayStartPos, Vector3.down, out RaycastHit hit, raycastDistance, groundMask))
			{
				if (!cachedRenderer.enabled)
					cachedRenderer.enabled = true;

				transform.position = hit.point + (hit.normal * groundOffset);

				Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				transform.rotation = surfaceRotation * Quaternion.Euler(rotationOffset);

				float tiltFactor = Mathf.Abs(Vector3.Dot(coinTransform.forward, Vector3.up));

				Vector3 worldScale = coinTransform.lossyScale;
				float scaledDiameterX = initialLocalBoundsSize.x * worldScale.x;
				float scaledDiameterY = initialLocalBoundsSize.y * worldScale.y;
				float maxDiameter = Mathf.Max(scaledDiameterX, scaledDiameterY);

				float effectiveHeight = Mathf.Max(maxDiameter * tiltFactor, minimumThickness * worldScale.z);

				transform.localScale = new Vector3(maxDiameter * overallMultiplier, effectiveHeight * overallMultiplier, 1f);
			}
			else
			{
				if (cachedRenderer.enabled)
					cachedRenderer.enabled = false;
			}
		}
	}
}
