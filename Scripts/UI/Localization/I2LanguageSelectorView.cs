using UnityEngine;

namespace NullCore.UI
{
    /// <summary>
    /// Language selector view that uses I2 Localization to populate the list of available languages.
    /// Equivalent of UnityLocalizationLanguageSelectorView but for I2.Loc.
    /// </summary>
#if I2LOC
    public class I2LanguageSelectorView : View
    {
        [SerializeField] SetLanguageOption _languageOptionPrefab;
        [SerializeField] Transform _languageOptionsContainer;

        protected override void Awake()
        {
            base.Awake();
            _languageOptionPrefab.gameObject.SetActive(false);

            var languages = I2.Loc.LocalizationManager.GetAllLanguages();
            foreach (var language in languages)
            {
                var option = Instantiate(_languageOptionPrefab, _languageOptionsContainer);
                option.Init(language);
            }
        }
    }
#else
    // Stub so the class exists in all compilation contexts.
    public class I2LanguageSelectorView : View { }
#endif
}
