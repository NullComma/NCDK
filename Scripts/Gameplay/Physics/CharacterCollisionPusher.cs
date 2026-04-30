using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NCDK {
    [AddComponentMenu(StaticStrings.PrefixScripts + "Physics/CCharacterCollisionPusher")]
    [RequireComponent(typeof(CharacterController))]
	[DisallowMultipleComponent]
    public class CharacterCollisionPusher : MonoBehaviour {
        
        #region <<---------- Properties and Fields ---------->>
        [SerializeField] private float _pushPower = 2.0f;
        
        [Tooltip("If true, only pushes objects on specific layers.")]
        [SerializeField] private bool _pushLayersOnly = true;
        [SerializeField] private LayerMask _pushLayers = 1;

        #endregion <<---------- Properties and Fields ---------->>

        #region <<---------- MonoBehaviour ---------->>
        private void OnControllerColliderHit(ControllerColliderHit hit) {
            Rigidbody body = hit.collider.attachedRigidbody;

            // no rigidbody or kinematic
            if (body == null || body.isKinematic) {
                return;
            }

            // We dont want to push objects below us
            if (hit.moveDirection.y < -0.3) {
                return;
            }
            
            // Check layer mask if needed
             if (_pushLayersOnly && (_pushLayers.value & (1 << body.gameObject.layer)) == 0) {
                return;
            }

            // Apply push
            // We use velocity here because AddForce with CharacterController often leads to runaway acceleration due to continuous collision
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            Vector3 newVelocity = pushDir * _pushPower;
#if UNITY_6000_0_OR_NEWER	
            newVelocity.y = body.linearVelocity.y; // Preserve gravity
            body.linearVelocity = newVelocity;
#else
            newVelocity.y = body.velocity.y; // Preserve gravity
            body.velocity = newVelocity;
#endif
        }
        #endregion <<---------- MonoBehaviour ---------->>

        #region <<---------- Editor ---------->>
        #if UNITY_EDITOR
        
        [MenuItem("CONTEXT/CharacterController/Add Character Collision Pusher")]
        static void AddCollisionPusher(MenuCommand command) {
            CharacterController cc = (CharacterController)command.context;
            
            // Start Undo Group
            Undo.SetCurrentGroupName("Add Character Collision Pusher");
            int group = Undo.GetCurrentGroup();

            if(cc.GetComponent<CharacterCollisionPusher>() == null) {
                Undo.AddComponent<CharacterCollisionPusher>(cc.gameObject);
                Debug.Log($"[CCharacterCollisionPusher] Added to {cc.name}", cc);
            }
            else {
                 Debug.LogWarning($"[CCharacterCollisionPusher] Already added to {cc.name}", cc);
            }
            
            Undo.CollapseUndoOperations(group);
        }
        
        #endif
        #endregion <<---------- Editor ---------->>
    }
}
