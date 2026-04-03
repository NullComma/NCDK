using UnityEngine;

namespace NullCore.Interaction {
    public interface IInteractable {
        bool CanBeInteractedWith();
        void OnBecameInteractionTarget(Transform lookingTransform);
        bool OnInteract(Transform interactingTransform);
        void OnStoppedBeingInteractionTarget(Transform lookingTransform);
        Vector3 GetInteractionPromptPoint();
    }
}