using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace NCDK.UI
{
    /// <summary>
    /// Controls the visibility of Input Prompts located in a View's footer.
    /// It automatically hides/shows "Submit" based on UI selection interactability,
    /// and "Cancel" based on if the current View allows closing via cancel.
    /// </summary>
    public class ViewInputFooterController : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The View that this footer belongs to. If null, it will try to find one on Awake.")]
        [SerializeField] private View _parentView;

        [Header("Bindings")]
        public InputActionReference SubmitAction;
        public GameObject SubmitPrompt;
        public InputActionReference CancelAction;
        public GameObject CancelPrompt;

        // Cache the event system to avoid repeated calls
        [System.NonSerialized] private EventSystem _eventSystem;

        private void OnValidate()
        {
            if (_parentView == null)
            {
                _parentView = GetComponentInParent<View>();
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        private void Awake()
        {
            if (_parentView == null)
            {
                _parentView = GetComponentInParent<View>();
            }

            _eventSystem = EventSystem.current;
        }

        private void Update()
        {
            if (_parentView == null || _eventSystem == null)
                return;

            GameObject currentSelected = _eventSystem.currentSelectedGameObject;
            bool isSelectionInteractable = false;

            // Check if the currently selected object is interactable
            if (currentSelected != null && currentSelected.activeInHierarchy)
            {
                Selectable selectable = currentSelected.GetComponent<Selectable>();
                if (selectable != null && selectable.interactable)
                {
                    isSelectionInteractable = true;
                }
                else
                {
                    // Fallback for custom interactables that might not use standard UI Selectable
                    UIInteractable customInteractable = currentSelected.GetComponent<UIInteractable>();
                    if (customInteractable != null)
                    {
                        isSelectionInteractable = true;
                    }
                }
            }

            // Handle Submit Prompt
            if (SubmitAction != null && SubmitPrompt != null)
            {
                bool shouldShowSubmit = isSelectionInteractable && EInput.CurrentDevice == InputDeviceType.Gamepad;
                if (SubmitPrompt.activeSelf != shouldShowSubmit)
                {
                    SubmitPrompt.SetActive(shouldShowSubmit);
                }
            }

            // Handle Cancel Prompt
            if (CancelAction != null && CancelPrompt != null)
            {
                if (CancelPrompt.activeSelf != _parentView.CanCloseWithCancel)
                {
                    CancelPrompt.SetActive(_parentView.CanCloseWithCancel);
                }
            }
        }
    }
}
