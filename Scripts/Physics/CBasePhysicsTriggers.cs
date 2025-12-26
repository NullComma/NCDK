using System;
using UnityEngine;

namespace EnigmaCore {
	public abstract class CBasePhysicsTriggers : MonoBehaviour {
	
		[SerializeField] [CTagSelector] protected string _tag = "Player";

        protected bool TriggerOnce => this._triggerOnce;
        [SerializeField] private bool _triggerOnce;
        protected bool _triggered;
        protected bool CannotTrigger => this.TriggerOnce && this._triggered;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Transform Events")]
        #endif
		[SerializeField] protected CUnityEventTransform Enter;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Transform Events")]
        #endif
        [SerializeField] protected CUnityEventTransform Exit;
		[Space]
        #if ODIN_INSPECTOR
        [FoldoutGroup("Transform Booleans")]
        #endif
		[SerializeField] protected CUnityEventBool Entered;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Transform Booleans")]
        #endif
		[SerializeField] protected CUnityEventBool Exited;
        protected Collider _collider;



        protected virtual void Awake() {
	        TryGetComponent(out _collider);
        }

		protected virtual void Reset()
		{
			this.gameObject.layer = 2; // Ignore Raycast
		}

        protected virtual bool WillIgnoreTrigger(Component col) {
			return !this._tag.CIsNullOrEmpty() && !col.CompareTag(this._tag);
		}
        
    }
}