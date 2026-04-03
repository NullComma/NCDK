using System;
using NullCore.DependencyInjection;
using NullCore.UI;
using UnityEngine;

namespace NullCore.Interaction {
	public class Interactable : MonoBehaviour, IInteractable {

		#region <<---------- Properties and Fields ---------->>
		
		[NonSerialized,Inject] BlockingEventsManager _blockingEventsManager;
        [SerializeField] protected bool _debug;
		[SerializeField] bool onlyWorkOneTimePerSceneLoad;
        [SerializeField] Transform _interactionPromptPoint;
		[SerializeField] GameObject[] _setActiveStateOnBecameInteractionTarget;
		[SerializeField] CUnityEventTransform InteractEvent = new();
		[SerializeField] StateUnityEvents BecameTargetEvent = new();
		#endregion <<---------- Properties and Fields ---------->>


		
		
		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake()
		{
			this.Inject();
		}

		protected virtual void OnEnable() {
			// show enable checkbox
		}
        
		#endregion <<---------- MonoBehaviour ---------->>


		
		
		#region <<---------- CIInteractable ---------->>

        public virtual bool CanBeInteractedWith() {
            return this != null && enabled;
        }

        /// <summary>
        /// Returns TRUE if interacted sucesfull.
        /// </summary>
		public virtual bool OnInteract(Transform interactingTransform) {
			if (!enabled || gameObject == null || !gameObject.activeInHierarchy || _blockingEventsManager.InMenuOrPlayingCutscene) return false;
			InteractEvent?.Invoke(interactingTransform);
			if (onlyWorkOneTimePerSceneLoad) {
				Destroy(this);
			}
            return true;
        }

		public virtual void OnBecameInteractionTarget(Transform lookingTransform)
		{
			_setActiveStateOnBecameInteractionTarget.CDoForEachNotNull(g=>g.SetActive(true));
			BecameTargetEvent.Trigger(true);
		}

		public virtual void OnStoppedBeingInteractionTarget(Transform lookingTransform)
		{
			_setActiveStateOnBecameInteractionTarget.CDoForEachNotNull(g=>g.SetActive(false));
			BecameTargetEvent.Trigger(false);
		}

        public Vector3 GetInteractionPromptPoint() {
            if(this == null)return Vector3.zero;
            return _interactionPromptPoint != null ? _interactionPromptPoint.position : transform.position;
        }

        #endregion <<---------- CIInteractable ---------->>

    }
}