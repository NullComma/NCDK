using System;
using System.Linq;
using EnigmaCore.DependecyInjection;
using EnigmaCore.EnigmaCore.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace EnigmaCore.UI {
	public abstract class View : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		public static View LastOpenedView { get; private set; }

		public GameObject FirstSelectedObject => _eventSystem.firstSelectedGameObject;

		[Header("Setup")]
		[SerializeField] protected EventSystem _eventSystem;
        [Space]
        [SerializeField] CUIButton _buttonReturn;
        protected CUIButton ButtonReturn => _buttonReturn;

        public bool ShouldPauseTheGame = true;

		[Obsolete("Trigger audio using OnOpen/OnClose unity events")]
		[NonSerialized] protected bool _shouldPlayOpenAndCloseMenuSound;

		public bool CanCloseByReturnButton => _canCloseByReturnButton;
		[NonSerialized] protected bool _canCloseByReturnButton = true;

		[NonSerialized] View _previous;
		[NonSerialized] CUIInteractable _previousButton;

		public event Action<View> OpenEvent;
		public event Action<View> CloseEvent;

		[NonSerialized,Inject] CBlockingEventsManager _blockingEventsManager;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake()
		{
			this.Inject();
			_eventSystem = GetComponentInChildren<EventSystem>();
		}
	
		protected virtual void OnEnable() {
			UpdateEventSystemAndCheckForObjectSelection(_eventSystem.firstSelectedGameObject);
            if(_buttonReturn) _buttonReturn.ClickEvent += CloseView;
            _blockingEventsManager.MenuRetainable.Retain(this);
            LastOpenedView = this;
            _eventSystem.CGetOrAddComponent<EventSystemHandlers>().CancelEvent += OnCancelEvent;
		}

		void LateUpdate()
		{
			if (_eventSystem == null || (_eventSystem.currentSelectedGameObject != null && _eventSystem.currentSelectedGameObject.GetComponent<CUIInteractable>() != null)) return;
			var childrens = GetComponentsInChildren<CUIInteractable>(true);
			if (childrens.Length <= 0) {
				Debug.LogError($"Could not find object to select with a '{nameof(CUIInteractable)}' in '{name}', this will lead to non functional UI in Controllers.", this);
				return;
			}
			var toSelect = childrens.FirstOrDefault(i => i.isActiveAndEnabled);
			if (toSelect == null)
			{
				toSelect = childrens[0]; // select first that is disabled anyway
				Debug.LogWarning($"Could not find any active object to select with a '{nameof(CUIInteractable)}' in '{name}', selecting first one that is disabled.", this);
			}
			Debug.Log($"Auto selecting item '{toSelect.name}' on menu '{name}'", toSelect);
			_eventSystem.SetSelectedGameObject(toSelect.gameObject);
		}

		protected virtual void OnDisable() {
			_blockingEventsManager?.MenuRetainable.Release(this);
            if(_buttonReturn) _buttonReturn.ClickEvent -= CloseView;
            _eventSystem.GetComponent<EventSystemHandlers>().CancelEvent -= OnCancelEvent;
		}

        protected virtual void OnDestroy() { }

        void OnValidate()
        {
	        if (_eventSystem == null) {
		        _eventSystem = GetComponentInChildren<EventSystem>();
		        #if UNITY_EDITOR
		        if(_eventSystem == null) {
			        Debug.LogError($"No EventSystem found in children of '{name}', please add one.", this);
		        }
		        else
		        {
			        UnityEditor.EditorUtility.SetDirty(this);
		        }
		        #endif
	        }
        }

        void Reset()
        {
	        OnValidate();
        }

        #endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Open / Close ---------->>

		public static View InstantiateAndOpen(View prefab, View previous = null, CUIInteractable originButton = null, bool canCloseByReturnButton = true)
		{
			var view = Object.Instantiate(prefab);
			view.Open(previous, originButton, canCloseByReturnButton);
			return view;
		}

		void Open(View previous, CUIInteractable originButton, bool canCloseByReturnButton = true) {
			Debug.Log($"Opening UI {gameObject.name}");
			_previous = previous;
			if(_previous != null) {
				GetComponentInChildren<Canvas>(true).sortingOrder = 1 + _previous.GetComponentInChildren<Canvas>(true).sortingOrder;
				_previous.gameObject.SetActive(false);
			}
			_previousButton = originButton;
            _canCloseByReturnButton = canCloseByReturnButton;
            ETime.TimeScale = ShouldPauseTheGame ? 0f : 1f;
            DIContainer.Resolve<CBlockingEventsManager>().MenuRetainable.Retain(this);
			OpenEvent?.Invoke(this);
            gameObject.SetActive(true);
		}

		public void CloseView()
		{
			CloseInternal(false);
		}

		public void RecursiveCloseAllViews()
		{
			CloseInternal(true);
		}

        void CloseInternal(bool closeAll = false) {
			Debug.Log($"Closing UI {gameObject.name}", this);
			CloseEvent?.Invoke(this);
			if(_previous != null) {
				if(closeAll) _previous.RecursiveCloseAllViews();
				else _previous.ShowIfHidden(_previousButton);
			}
			else {
				ETime.TimeScale = 1f;
			}
			_blockingEventsManager?.MenuRetainable.Release(this);
			#if UNITY_ADDRESSABLES_EXIST
			CAssets.UnloadAsset(this.gameObject);
			#else
			gameObject.CDestroy();
			#endif
        }

		#endregion <<---------- Open / Close ---------->>




		#region <<---------- Visibility ---------->>

		void OnCancelEvent(BaseEventData obj)
		{
			if(CanCloseByReturnButton) CloseView();
		}
		
		void UpdateEventSystemAndCheckForObjectSelection(GameObject gameObjectToSelect) {
            if (gameObjectToSelect == null) return;
			UpdateEventSystemAndCheckForObjectSelection(gameObjectToSelect.GetComponent<CUIInteractable>());
        }

		void UpdateEventSystemAndCheckForObjectSelection(CUIInteractable interactableToSelect) {
            if (interactableToSelect == null) {
                Debug.LogError("Requested to select a null interactable! No object will be selected.");
                return;
            }

            EventSystem.current = _eventSystem;

            _eventSystem.SetSelectedGameObject(interactableToSelect.gameObject);
		}

		public void ShowIfHidden(CUIInteractable buttonToSelect) {
			gameObject.SetActive(true);
			if(buttonToSelect) UpdateEventSystemAndCheckForObjectSelection(buttonToSelect);
			ETime.TimeScale = ShouldPauseTheGame ? 0f : 1f;
		}

		#endregion <<---------- Visibility ---------->>

	}
}