
using UnityEngine;
#if UNITY_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace EnigmaCore.UI
{
    public class UnityLocalizationLanguageSelectorView : View
    {

        [SerializeField] SetLanguageOption _languageOptionPrefab;
        [SerializeField] Transform _languageOptionsContainer;

#if UNITY_LOCALIZATION
        [SerializeField] LocalizedString _languageTitleString;

        protected override void Awake()
        {
            base.Awake();
            _languageOptionPrefab.gameObject.SetActive(false);
            var locales = UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.Locales;
            foreach (var locale in locales)
            {
                var option = Instantiate(_languageOptionPrefab, _languageOptionsContainer);
                option.Init(locale, _languageTitleString);
            }
        }
#endif
    }
}