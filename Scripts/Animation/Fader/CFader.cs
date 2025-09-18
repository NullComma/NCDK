using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EnigmaCore {
	[Obsolete("Use Fader on timeline instead")]
	public class CFader : MonoBehaviour {

		#region <<---------- Properties ---------->>
		
		CanvasGroup _fadeCanvasGroup;
		float TargetAlpha;
		float TargetFadeTime;
		bool IgnoreTimeScale;
		static CFader _instance;
		
		#endregion <<---------- Properties ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>

		void Awake() {
			if (_instance != null) _instance.gameObject.CDestroy();
			_instance = this;
			gameObject.layer = 5; // UI
			gameObject.CDontDestroyOnLoad();
            gameObject.hideFlags = HideFlags.DontSaveInEditor;

			// canvas
			var goCanvas = gameObject.AddComponent<Canvas>();
			goCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			goCanvas.sortingOrder = -1;
			try {
				goCanvas.sortingLayerID++;
			} catch (Exception e) {
				Debug.LogException(e);
			}

			// canvas group
			_fadeCanvasGroup = gameObject.AddComponent<CanvasGroup>();
			_fadeCanvasGroup.blocksRaycasts = false;
			_fadeCanvasGroup.interactable = false;

			// image black
			var imgComp = gameObject.AddComponent<Image>();
			imgComp.color = Color.black;
			imgComp.maskable = false;
			imgComp.raycastTarget = false;
			
			Application.quitting += ApplicationOnQuittingEvent;
		}
		void ApplicationOnQuittingEvent()
		{
			Application.quitting -= ApplicationOnQuittingEvent;
			if (this == null) return;
			gameObject.CDestroy();
		}

		void Update() => UpdateOpacity();

		#endregion <<---------- MonoBehaviour ---------->>
		
		
		

		#region <<---------- General ---------->>
		
		public void FadeToBlack(float fadeTime, bool ignoreTimeScale = true) {
            //Debug.Log($"Requesting fade to black, time '{fadeTime}' seconds with ignoreTimeScale set to '{ignoreTimeScale}'");
			TargetAlpha = 1f;
			TargetFadeTime = fadeTime;
			IgnoreTimeScale = ignoreTimeScale;
            UpdateOpacity();
        }

		public void FadeToTransparent(float fadeTime, bool ignoreTimeScale = true) {
            //Debug.Log($"Requesting fade to transparent, time '{fadeTime}' seconds with ignoreTimeScale set to '{ignoreTimeScale}'");
			TargetAlpha = 0f;
			TargetFadeTime = fadeTime;
			IgnoreTimeScale = ignoreTimeScale;
            UpdateOpacity();
		}

		private void UpdateOpacity() {
            if (TargetFadeTime <= 0f) {
                _fadeCanvasGroup.alpha = TargetAlpha;
                return;
            }
            float currentAlpha = _fadeCanvasGroup.alpha.CImprecise();
			if (Mathf.Approximately(TargetAlpha, currentAlpha)) return;
			float delta = IgnoreTimeScale ? Time.unscaledDeltaTime : ETime.DeltaTimeScaled;
			float step = delta / TargetFadeTime;
			if (currentAlpha > TargetAlpha) step *= -1f;
			_fadeCanvasGroup.alpha = currentAlpha + step;
		}
        
        #endregion <<---------- General ---------->>




        #region <<---------- Debug ---------->>

        public void DebugEnableFader() {
            _fadeCanvasGroup.enabled = true;
        }
        
        public void DebugDisableFader() {
            _fadeCanvasGroup.enabled = false;
        }

        #endregion <<---------- Debug ---------->>

	}
}
