using EnigmaCore.Interaction;
using EnigmaCore.UI;
using UnityEngine;

namespace EnigmaCore {
    public class OpenViewOnInteract : Interactable {

        [SerializeField] View _viewToOpen;
        
        
        
        public override bool OnInteract(Transform interactingTransform) {
            if (!base.OnInteract(interactingTransform)) return false;
            Instantiate(_viewToOpen).gameObject.SetActive(true);
            return true;
        }
        
    }
}