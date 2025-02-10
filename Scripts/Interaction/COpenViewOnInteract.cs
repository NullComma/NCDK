using EnigmaCore.Interaction;
using EnigmaCore.UI;
using UnityEngine;

namespace EnigmaCore {
    public class COpenViewOnInteract : CInteractable {

        [SerializeField] View _viewToOpen;
        
        
        
        public override bool OnInteract(Transform interactingTransform) {
            if (!base.OnInteract(interactingTransform)) return false;
            View.InstantiateAndOpen(_viewToOpen, null, null);
            return true;
        }
        
    }
}