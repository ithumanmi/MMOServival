using Hawky.Localization;
using Hawky.SaveData;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hawky.Setting.UI
{
    public class LanguageDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropdown;

        private void Awake()
        {
            if (_dropdown == null)
            {
                Debug.LogError("LanguageDropdown kh�ng c� Dropdown");
                return;
            }

            var settingData = SaveDataManager.Ins.GetData<SettingData>();
            var allSuportedLanguage = Hawky.Localization.LocalizationManager.Ins.SupportedLanguages;

            _dropdown.ClearOptions();
            var _allOption = new List<TMP_Dropdown.OptionData>();

            int value = 0;

            foreach (var language in allSuportedLanguage)
            {
                var newOption = new LanguageOptionData();
                newOption.systemLanguage = language;
                newOption.text = Hawky.Localization.LocalizationManager.Ins.GetString($"STRING_NAME_{language.ToString().ToUpper()}");
                newOption.image = NationalFlagManager.Ins.GetFlag(language.ToString().ToLower());
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

            var settingData = SaveDataManager.Ins.GetData<SettingData>();
            settingData.language = languageOptionData.systemLanguage;

            settingData.Save();

            Hawky.Localization.LocalizationManager.Ins.CurrentLanguage = languageOptionData.systemLanguage;
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
