using System;
using I2.Loc;
using NCDK.Refs;
using TMPro;
using UnityEngine;

namespace NCDK.UI
{
    public class SetLanguageOption : ValidatedMonoBehaviour
    {
        [SerializeField, Child] TextMeshProUGUI _localizable;
        [NonSerialized] string languageText;

        public void Init(string language)
        {
            languageText = language;
            _localizable.text = LocalizationManager.GetTranslation("language title", overrideLanguage: language) ?? language;
            gameObject.SetActive(true);
        }

        public void SetLocale()
        {
            LocalizationManager.CurrentLanguage = languageText;
        }
    }
}