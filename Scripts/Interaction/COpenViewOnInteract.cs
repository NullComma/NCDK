using EnigmaCore.Interaction;
using EnigmaCore.UI;
using UnityEngine;

namespace EnigmaCore {
    public class COpenViewOnInteract : CInteractable {

        [SerializeField] View _viewToOpen;
        
        
        
        public override bool OnInteract(Transform interactingTransform) {
            if (!base.OnInteract(interactingTransform)) return false;
            Instantiate(_viewToOpen).gameObject.SetActive(true);
            return true;
        }
        
    }
}