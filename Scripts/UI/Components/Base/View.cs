using System;
using EnigmaCore.DependecyInjection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace EnigmaCore.UI
{
    public abstract class View : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] protected EventSystem _eventSystem;
        [SerializeField] protected CUIButton _buttonReturn;
        
        [Header("Behavior")]
        public bool ShouldPauseTheGame = true;
        public bool CanCloseWithCancel = true;

        [Inject] [NonSerialized] protected ViewManager _viewManager;

        GameObject _lastSelectedObject;

        public event Action CloseEvent = delegate { };

        protected virtual void Awake()
        {
            this.Inject();
            if (_eventSystem == null) _eventSystem = GetComponentInChildren<EventSystem>(true);
            _viewManager.NotifyViewOpened(this);
        }

		protected virtual void OnEnable()
        {
            // Store which object was selected right before this view opened.
            if (EventSystem.current != null)
                _lastSelectedObject = EventSystem.current.currentSelectedGameObject;

            // --- The Core of the New System ---
            // Notify the manager that this view is now the active one.
            _viewManager.NotifyViewShown(this);

            if (_eventSystem != null && _eventSystem.TryGetComponent(out EventSystemHandlers handlers))
                handlers.CancelEvent += OnCancelEvent;

            if (_buttonReturn) _buttonReturn.ClickEvent += Close;
        }

        void LateUpdate()
        {
            // This logic should only run for the view that is currently on top.
            if (_viewManager == null || _viewManager.CurrentTopView != this)
            {
                return;
            }

            // This is the failsafe for gamepad navigation.
            // If nothing is selected and a gamepad is connected, it means a mouse user
            // might have deselected everything, breaking controller navigation.
            if (_eventSystem.currentSelectedGameObject == null && Gamepad.current != null)
            {
                // Find the first available selectable object and select it to restore navigation.
                var objectToSelect = FirstSelectedObject();
                if (objectToSelect != null)
                {
                    _eventSystem.SetSelectedGameObject(objectToSelect);
                }
            }
        }
        
        protected virtual void OnDisable()
        {
            // --- The Core of the New System ---
            // Notify the manager that this view has been closed.
            
            if (_eventSystem != null && _eventSystem.TryGetComponent(out EventSystemHandlers handlers))
                handlers.CancelEvent -= OnCancelEvent;
            
            if(_buttonReturn) _buttonReturn.ClickEvent -= Close;
            _viewManager.NotifyViewHidden(this);
        }

		void OnDestroy()
		{
            if(_viewManager != null) _viewManager.NotifyViewClosed(this);
            CloseEvent.Invoke();
		}

		public virtual void Show()
        {
            gameObject.SetActive(true);

            // Attempt to re-select the object that was selected before the next view was opened.
            var objectToSelect = _lastSelectedObject != null ? _lastSelectedObject : FirstSelectedObject();
            if (objectToSelect != null)
            {
                _eventSystem.SetSelectedGameObject(objectToSelect);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Close()
        {
            gameObject.CDestroy();
        }

        private void OnCancelEvent(BaseEventData obj)
        {
            if (CanCloseWithCancel) Close();
        }
        
        private GameObject FirstSelectedObject()
        {
            if (_eventSystem.firstSelectedGameObject != null) return _eventSystem.firstSelectedGameObject;
            var firstInteractable = GetComponentInChildren<CUIInteractable>();
            return firstInteractable != null ? firstInteractable.gameObject : null;
        }
    }
}