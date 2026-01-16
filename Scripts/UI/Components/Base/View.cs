using System;
using EnigmaCore.DependencyInjection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace EnigmaCore.UI
{
    public abstract class View : MonoBehaviour
    {
        #region Properties
        
        [Header("Setup")]
        [SerializeField] protected EventSystem _eventSystem;
        [SerializeField] protected CUIButton _buttonReturn;
        
        [Header("Behavior")]
        public bool ShouldPauseTheGame = true;
        public bool CanCloseWithCancel = true;

        [Inject] [NonSerialized] protected CBlockingEventsManager _blockingEventsManager;
        [Inject] [NonSerialized] protected TimePauseManager timePauseManager;
        
        public static View ActiveView { get; private set; }
        protected View _returnToView;

        GameObject _lastSelectedObject;

        public event Action CloseEvent = delegate { };
        
        #endregion

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if(_eventSystem == null)
            {
                _eventSystem = GetComponentInChildren<EventSystem>();
                if(_eventSystem != null)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
#endif
        }

        protected virtual void Awake()
        {
            this.Inject();
            Debug.Log($"[View] Awake: {gameObject.name} | Frame: {Time.frameCount}");
            if (_eventSystem == null) _eventSystem = GetComponentInChildren<EventSystem>(true);
        }

		protected virtual void OnEnable()
        {
            // Store which object was selected right before this view opened.
            if (EventSystem.current != null)
                _lastSelectedObject = EventSystem.current.currentSelectedGameObject;

            _blockingEventsManager.MenuRetainable.Retain(this);
            
            if (ShouldPauseTheGame)
                timePauseManager.Retain(this);

            if (_eventSystem != null && _eventSystem.TryGetComponent(out EventSystemHandlers handlers))
                handlers.CancelEvent += OnCancelEvent;

            if (_buttonReturn) _buttonReturn.ClickEvent += CloseByCancelled;
            
            // Navigation Logic: "Push" to stack
            // Hide previous view (Release functionality)
            if (ActiveView != this)
            {
                if (ActiveView != null)
                {
                    _returnToView = ActiveView;
                    ActiveView.Hide();
                }
                ActiveView = this;
            }
            
            // Attempt to re-select the object that was selected before the next view was opened.
            var objectToSelect = _lastSelectedObject != null ? _lastSelectedObject : FirstSelectedObject();
            if (objectToSelect != null)
            {
                _eventSystem.SetSelectedGameObject(objectToSelect);
            }
            
        }

        void LateUpdate()
        {
            // This is the failsafe for gamepad navigation.
            // If nothing is selected and a gamepad is connected, it means a mouse user
            // might have deselected everything, breaking controller navigation.
            if (_eventSystem.currentSelectedGameObject == null || !_eventSystem.currentSelectedGameObject.activeInHierarchy)
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
            if (_eventSystem != null && _eventSystem.TryGetComponent(out EventSystemHandlers handlers))
                handlers.CancelEvent -= OnCancelEvent;
            
            if(_buttonReturn) _buttonReturn.ClickEvent -= CloseByCancelled;

            _blockingEventsManager.MenuRetainable.Release(this);

            if (ShouldPauseTheGame)
                timePauseManager.Release(this);
        }

		void OnDestroy()
		{
            Debug.Log($"[View] OnDestroy: {gameObject.name} | Frame: {Time.frameCount}");
            
            // Navigation Logic: "Pop" from stack
            if (ActiveView == this)
            {
                // We are the active view and we are dying. Pass control back.
                ActiveView = null; // Clear first so Show() doesn't capture us as parent
                if(_returnToView) _returnToView.gameObject.SetActive(true);
            }
            CloseEvent.Invoke();
		}

        public virtual void Show()
        {
            if(EApplication.IsQuitting)
            {
                Debug.Log($"[View] Show() called on '{gameObject.name}' during application quitting. Ignoring.");
                return;
            }
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Immediately closes and destroys this view.
        /// </summary>
        public void Close() => gameObject.CDestroy();

        /// <summary>
        /// Closes the view if CanCloseWithCancel is true.
        /// </summary>
        public void CloseByCancelled()
        {
            if(!CanCloseWithCancel) return;
            Close();
        }

        private void OnCancelEvent(BaseEventData obj)
        {
            CloseByCancelled();
        }
        
        private GameObject FirstSelectedObject()
        {
            if (_eventSystem.firstSelectedGameObject != null) return _eventSystem.firstSelectedGameObject;
            var firstInteractable = GetComponentInChildren<CUIInteractable>();
            return firstInteractable != null ? firstInteractable.gameObject : null;
        }
    }
}