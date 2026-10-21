using UnityEngine;

namespace NCDK.UI
{
    /// <summary>
    /// Language selector view that uses I2 Localization to populate the list of available languages.
    /// Equivalent of UnityLocalizationLanguageSelectorView but for I2.Loc.
    /// </summary>
    public class I2LanguageSelectorView : View
    {
#if I2LOC
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
#endif
    }
}
