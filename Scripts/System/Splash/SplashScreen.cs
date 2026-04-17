using System;
using System.Collections;
using System.Collections.Generic;
using NullCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using NullCore.Refs;

#endif


namespace NullCore
{
    public class SplashScreen : ValidatedMonoBehaviour
    {
        [Header("Timing Settings")]
        [SerializeField, Tooltip("Duration in seconds for each image to fade in.")]
        float fadeInDuration = 0.5f;
        [SerializeField, Tooltip("Duration in seconds for each image to stay visible.")]
        float holdDuration = 1.5f;
        [SerializeField, Tooltip("Duration in seconds for each image to fade out.")]
        float fadeOutDuration = 0.5f;

        [Header("Scene Loading")]
        [SerializeField, Min(1), Tooltip("The build index of the scene to load after the splash sequence.")]
        int sceneToLoadIndex = 1;

        [Header("Object References")]
        [SerializeField, Tooltip("The splash screen objects to display sequentially.")]
        List<GameObject> splashObjects = new();

        [Tooltip("The pre-splash loading indicator to display before start showing logos.")]
        [SerializeField] GameObject preSplashIndicator;

        [SerializeField, Scene(Flag.Editable)] CanvasGroup canvasGroup;

        // Non-Serialized Fields
        [NonSerialized] bool allowSkip;
        [NonSerialized] Coroutine splashCoroutine;
        [NonSerialized] AsyncOperation sceneLoadOperation;
        [NonSerialized] BlockingEventsManager blockingEventsManager;

        void Awake()
        {
            blockingEventsManager = ServiceLocator.Resolve<BlockingEventsManager>();
            foreach (var splashObj in splashObjects)
            {
                if (splashObj != null)
                {
                    splashObj.SetActive(false);
                }
            }
        }

        void OnEnable()
        {
            blockingEventsManager.MenuRetainable.Retain(this);
        }

        void OnDisable()
        {
            blockingEventsManager.MenuRetainable.Release(this);
        }

        IEnumerator Start()
        {
            float startTime = Time.realtimeSinceStartup;
            Debug.Log($"[SplashScreen] Start sequence @ {startTime:F2}s");

            // 1. Wait for the engine core to be ready (EApplication initialization)
            yield return new WaitUntil(() => EApplication.IsInitialized);

            if (preSplashIndicator != null) preSplashIndicator.SetActive(false);
            canvasGroup.gameObject.SetActive(true);


            // 3. Start loading the next scene ONLY after initialization is complete
            sceneLoadOperation = SceneManager.LoadSceneAsync(sceneToLoadIndex);

            // Prevent the scene from activating as soon as it's finished loading.
            sceneLoadOperation.allowSceneActivation = false;

            // 4. Start the visual splash screen sequence.
            // This is safe now because all LocalizedString components on splash objects will find the system ready.
            splashCoroutine = StartCoroutine(SplashScreenSequence());

            allowSkip = true;
            yield return null;
        }

        void Update()
        {
            if (!allowSkip) return;

#if ENABLE_INPUT_SYSTEM
            bool keyboardSkip = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool gamepadSkip = false;
            if (Gamepad.current != null)
            {
                gamepadSkip = Gamepad.current.buttonSouth.wasPressedThisFrame ||
                              Gamepad.current.buttonEast.wasPressedThisFrame ||
                              Gamepad.current.startButton.wasPressedThisFrame;
            }
            bool mouseSkip = false;
            if (Mouse.current != null)
            {
                mouseSkip = Mouse.current.leftButton.wasPressedThisFrame ||
                            Mouse.current.rightButton.wasPressedThisFrame;
            }

            if (keyboardSkip || gamepadSkip || mouseSkip)
            {
                Skip();
            }
#else
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Skip();
            }
#endif
        }

        void Skip()
        {
            Debug.Log("Splash screen skipped by user.");
            allowSkip = false;

            if (splashCoroutine != null)
            {
                StopCoroutine(splashCoroutine);
            }
            StartCoroutine(FadeOutAndLoadScene(fadeOutDuration * 0.25f));
        }

        IEnumerator SplashScreenSequence()
        {
            canvasGroup.alpha = 0f;

            foreach (var splashObj in splashObjects)
            {
                splashObj.SetActive(true);
                yield return Fade(1f, fadeInDuration);
                yield return new WaitForSeconds(holdDuration);
                if (splashObj.TryGetComponent(out ESplashScreenExtraTime extraTime))
                {
                    yield return new WaitForSeconds(extraTime.ExtraTime);
                }
                yield return Fade(0f, fadeOutDuration);
                splashObj.SetActive(false);
            }

            allowSkip = false;

            Debug.Log($"Splash sequence finished. Activating next scene. (Scene load status: {sceneLoadOperation?.progress * 100:F0}%)");
            if (sceneLoadOperation != null)
            {
                var sceneActivationStart = Time.realtimeSinceStartup;
                sceneLoadOperation.allowSceneActivation = true;

                // Track how long it takes for the scene to actually become active
                while (!sceneLoadOperation.isDone)
                {
                    yield return null;
                }
                Debug.Log($"[SplashScreen] Scene activation completed in {Time.realtimeSinceStartup - sceneActivationStart:F2}s");
            }
        }

        IEnumerator FadeOutAndLoadScene(float duration)
        {
            yield return Fade(0f, duration);
            if (sceneLoadOperation != null)
            {
                sceneLoadOperation.allowSceneActivation = true;
            }
        }

        IEnumerator Fade(float targetAlpha, float duration)
        {
            float startAlpha = canvasGroup.alpha;
            float time = 0;
            while (time < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = targetAlpha;
        }
    }
}