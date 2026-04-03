using System;
using UnityEngine;

namespace NullCore
{
    [RequireComponent(typeof(Animator))]
    public class RootMotionReceiver : MonoBehaviour
    {
        public Vector3 DeltaPosition { get; private set; }
        public Quaternion DeltaRotation { get; private set; }
        [NonSerialized] Animator _animator;


        void Awake()
        {
            _animator = this.CGetComponentInChildrenOrInParent<Animator>();
        }

        void OnAnimatorMove()
        {
            DeltaPosition = _animator.deltaPosition;
            DeltaRotation = _animator.deltaRotation;
        }

        public void SetAnimationRootMotionEnabledState(bool state)
        {
            if(_animator) _animator.applyRootMotion = state;
        }
    }
}