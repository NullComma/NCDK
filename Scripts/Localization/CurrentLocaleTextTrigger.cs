using System;
using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace EnigmaCore.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CurrentLocaleTextTrigger : MonoBehaviour
    {

        [NonSerialized] TextMeshProUGUI _textToSet;

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
    }
}