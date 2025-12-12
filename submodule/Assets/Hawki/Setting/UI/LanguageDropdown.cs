using Hawki.Localization;
using Hawki.SaveData;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hawki.Setting.UI
{
    public class LanguageDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropdown;

        private void Awake()
        {
            if (_dropdown == null)
            {
                Debug.LogError("LanguageDropdown không có Dropdown");
                return;
            }

            var settingData = SaveDataManager.Instance.GetData<SettingData>();
            var allSuportedLanguage = Hawki.Localization.LocalizationManager.Instance.SupportedLanguages;

            _dropdown.ClearOptions();
            var _allOption = new List<TMP_Dropdown.OptionData>();

            int value = 0;

            foreach (var language in allSuportedLanguage)
            {
                var newOption = new LanguageOptionData();
                newOption.systemLanguage = language;
                newOption.text = Hawki.Localization.LocalizationManager.Instance.GetString($"STRING_NAME_{language.ToString().ToUpper()}");
                newOption.image = NationalFlagManager.Instance.GetFlag(language.ToString().ToLower());
                _allOption.Add(newOption);

                if (language == settingData.language)
                {
                    value = allSuportedLanguage.IndexOf(language);
                }
            }

            _dropdown.AddOptions(_allOption);
            _dropdown.SetValueWithoutNotify(value);

            _dropdown.onValueChanged.AddListener(OnDropdownChange);
        }

        private void OnDropdownChange(int id)
        {
            var languageOptionData = _dropdown.options[id] as LanguageOptionData;

            var settingData = SaveDataManager.Instance.GetData<SettingData>();
            settingData.language = languageOptionData.systemLanguage;

            settingData.Save();

            Hawki.Localization.LocalizationManager.Instance.CurrentLanguage = languageOptionData.systemLanguage;
        }

        private void OnEnable()
        {

        }

        public class LanguageOptionData : TMP_Dropdown.OptionData
        {
            public SystemLanguage systemLanguage;
        }
    }

    
}
