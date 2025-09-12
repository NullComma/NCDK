using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace EnigmaCore
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CSplashScreen : MonoBehaviour
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
        [SerializeField, Tooltip("The parent GameObject containing the splash Image objects as children.")]
        Transform splashImagesHolder;
        
        // Non-Serialized Fields
        [NonSerialized] CanvasGroup canvasGroup;
        [NonSerialized] List<Image> splashImages = new();
        [NonSerialized] bool allowSkip;
        [NonSerialized] Coroutine splashCoroutine;
        [NonSerialized] AsyncOperation sceneLoadOperation;

        void Awake()
        {
            if (!TryGetComponent(out canvasGroup))
            {
                Debug.LogError($"{nameof(CSplashScreen)} requires a CanvasGroup component.");
                enabled = false;
                return;
            }

            if (splashImagesHolder == null) splashImagesHolder = this.transform;

            var childTransforms = splashImagesHolder.GetComponentsInChildren<Image>(true);
            foreach (var image in childTransforms)
            {
                splashImages.Add(image);
                image.gameObject.SetActive(false);
            }
        }

        IEnumerator Start()
        {
            yield return null; // Wait one frame for all systems to initialize.

            sceneLoadOperation = SceneManager.LoadSceneAsync(sceneToLoadIndex);
            
            // Prevent the scene from activating as soon as it's finished loading.
            sceneLoadOperation.allowSceneActivation = false;

            // Start the visual splash screen sequence.
            splashCoroutine = StartCoroutine(SplashScreenSequence());
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
                              Gamepad.current.buttonEast.wasPressedThisFrame  ||
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
            allowSkip = true;

            foreach (var image in splashImages)
            {
                image.gameObject.SetActive(true);
                yield return Fade(1f, fadeInDuration);
                yield return new WaitForSeconds(holdDuration);
                if (image.gameObject.TryGetComponent(out ESplashScreenExtraTime extraTime))
                {
                    yield return new WaitForSeconds(extraTime.ExtraTime);
                }
                yield return Fade(0f, fadeOutDuration);
                image.gameObject.SetActive(false);
            }

            allowSkip = false;
            
            Debug.Log("Splash sequence finished. Activating next scene.");
            if (sceneLoadOperation != null)
            {
                sceneLoadOperation.allowSceneActivation = true;
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