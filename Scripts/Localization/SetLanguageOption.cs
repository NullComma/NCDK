using System;

using TMPro;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace EnigmaCore.UI
{
    public class SetLanguageOption : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _localeName;
        [NonSerialized] Locale _localeToSet;

        public void Init(Locale locale)
        {
            _localeToSet = locale;
            _localeName.text = locale.LocaleName;
            gameObject.SetActive(true);
        }

        public void SetLocale()
        {
            if (_localeToSet == null)
            {
                Debug.LogError("Locale to set is null!");
                return;
            }
            LocalizationSettings.SelectedLocale = _localeToSet;
        }

		 
    }
}