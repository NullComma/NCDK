using System;
using System.Collections;

using TMPro;

using UnityEngine;
#if UNITY_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif
namespace EnigmaCore.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CurrentLocaleTextTrigger : MonoBehaviour
    {

        [NonSerialized] TextMeshProUGUI _textToSet;
#if UNITY_LOCALIZATION
        void Awake()
        {
            TryGetComponent(out _textToSet);
        }

        IEnumerator Start()
        {
            yield return LocalizationSettings.InitializationOperation;
            OnSelectedLocaleChanged(LocalizationSettings.SelectedLocale);
            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        }

        private void OnSelectedLocaleChanged(Locale locale)
        {
            _textToSet.text = locale.LocaleName;
        }
#endif
    }
}