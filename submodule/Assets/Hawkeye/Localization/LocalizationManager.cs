using Hawkeye.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Hawkeye.Localization
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        #region Localization

        private Dictionary<SystemLanguage, Dictionary<string, string>> dicAllData = new Dictionary<SystemLanguage, Dictionary<string, string>>();

        private SystemLanguage _currentLanguage = SystemLanguage.English;

        public System.Action<SystemLanguage> OnLanguageChanged;
        public SystemLanguage CurrentLanguage
        {
            get => this._currentLanguage;
            set
            {
                if (this._currentLanguage == value)
                {
                    return;
                }

                if (this.dicAllData.ContainsKey(value))
                {
                    if (this._currentLanguage != value)
                    {
                        this._currentLanguage = value;
                        this.OnLanguageChanged?.Invoke(value);
                    }
                }
                else
                {
                    Debug.Log("No language data found: " + value);
                }
            }
        }

        public void Init(List<SystemLanguage> languages, SystemLanguage defaultLangauge)
        {
            this._currentLanguage = defaultLangauge;

            SupportedLanguages.Clear();
            SupportedLanguages.AddRange(languages);

            InitLocalizationConfig();
        }

        public static readonly List<SystemLanguage> SupportedLanguages = new List<SystemLanguage>()
        {
            SystemLanguage.English,
            SystemLanguage.Vietnamese,
            SystemLanguage.Japanese,
            SystemLanguage.French,
            SystemLanguage.Chinese,
            SystemLanguage.Danish,
            SystemLanguage.Russian,
            SystemLanguage.Portuguese,
            SystemLanguage.Spanish,
            SystemLanguage.Italian,
            SystemLanguage.German,
            SystemLanguage.Korean,
        };


        private void InitLocalizationConfig()
        {            this.dicAllData.Clear();
            for (var i = 0; i < SupportedLanguages.Count; i++)
            {
                this.dicAllData.Add(SupportedLanguages[i], new Dictionary<string, string>());
            }

            LocalizationConfig LocalizationConfig = ConfigManager.Instance.GetData<LocalizationConfig>();

            if (LocalizationConfig != null)
            {
                var checkList = LocalizationConfig.LocalizationConfigData;
                
                if (checkList != null)
                {
                    foreach (var pair in checkList)
                    {
                        foreach (var language in SupportedLanguages)
                        {
                            var field = pair.GetType().GetField(language.ToString().ToLower());

                            if (field != null)
                            {
                                var value = field.GetValue(pair);

                                if (value == null)
                                {
                                    this.dicAllData[language][pair.key] = string.Empty;
                                }
                                else
                                {
                                    this.dicAllData[language][pair.key] = value.ToString();
                                }
                            }
                            else
                            {
                                //Debug.LogWarning($"Field {language} does not exist for key {pair.key}");
                            }
                        }
                    }
                }
            }
        }

        private string GetString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            if (this.dicAllData[this.CurrentLanguage].TryGetValue(key, out var value))
            {
                return value;
            }

            return $"<{key}>";
        }


        public string GetString(string key, params object[] paramaters)
        {
            var localizedString = this.GetString(key);

            if (paramaters != null && paramaters.Length > 0)
            {
                localizedString = string.Format(localizedString, paramaters);
            }

            return localizedString;
        }

        #endregion
    }
}
