
using UnityEngine;

namespace EnigmaCore.UI
{
    public class LanguageSelectorView : View
    {

        [SerializeField] SetLanguageOption _languageOptionPrefab;
        [SerializeField] Transform _languageOptionsContainer;

        protected override void Awake()
        {
            base.Awake();
            _languageOptionPrefab.gameObject.SetActive(false);
            var locales = UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.Locales;
            foreach (var locale in locales)
            {
                var option = Instantiate(_languageOptionPrefab, _languageOptionsContainer);
                option.Init(locale);
            }
        }

    }
}