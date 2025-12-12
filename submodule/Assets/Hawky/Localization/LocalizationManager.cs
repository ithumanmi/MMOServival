using Hawky.Config;
using Hawky.EventObserver;
using System.Collections.Generic;
using UnityEngine;

namespace Hawky.Localization
{
    public partial class LocalizationKey
    {

    }

    public interface ILocalizationDataProvider
    {
        public List<SystemLanguage> Languages();
        public SystemLanguage Default();
    }

    public class LocalizationManager : RuntimeSingleton<LocalizationManager>, IAllSingletonAwakeComplete
    {
        #region Localization

        private Dictionary<SystemLanguage, Dictionary<string, string>> _dicAllData = new Dictionary<SystemLanguage, Dictionary<string, string>>();

        private SystemLanguage _currentLanguage = SystemLanguage.English;

        private List<SystemLanguage> _supportedLanguages = new List<SystemLanguage>();
        public SystemLanguage CurrentLanguage
        {
            get => this._currentLanguage;
            set
            {
                if (this._currentLanguage == value)
                {
                    return;
                }

                if (this._dicAllData.ContainsKey(value))
                {
                    if (this._currentLanguage != value)
                    {
                        this._currentLanguage = value;
                        EventObs.Ins.ExcuteEvent(EventName.LANGUAGE_CHANGED, new LanguageChangedEvent
                        {
                            language = this._currentLanguage,
                        });
                    }
                }
                else
                {
                    Debug.Log("No language data found: " + value);
                }
            }
        }

        public List<SystemLanguage> SupportedLanguages => this._supportedLanguages;

        public void OnAllSingletonInitComplete()
        {
            var instance = SingletonManager.Ins.FindFirst<ILocalizationDataProvider>();

            _supportedLanguages.Clear();
            if (instance == null)
            {
                _currentLanguage = SystemLanguage.English;
                _supportedLanguages.Add(SystemLanguage.English);
            }
            else
            {
                _currentLanguage = instance.Default();
                _supportedLanguages.AddRange(instance.Languages());
            }

            InitLocalizationConfig();
        }


        private void InitLocalizationConfig()
        {
            this._dicAllData.Clear();
            for (var i = 0; i < _supportedLanguages.Count; i++)
            {
                this._dicAllData.Add(_supportedLanguages[i], new Dictionary<string, string>());
            }

            LocalizationConfig LocalizationConfig = ConfigManager.Ins.GetData<LocalizationConfig>();

            if (LocalizationConfig != null)
            {
                var checkList = LocalizationConfig.LocalizationConfigData;

                if (checkList != null)
                {
                    foreach (var pair in checkList)
                    {
                        foreach (var language in _supportedLanguages)
                        {
                            var field = pair.GetType().GetField(language.ToString().ToLower());

                            if (field != null)
                            {
                                var value = field.GetValue(pair);

                                if (value == null)
                                {
                                    this._dicAllData[language][pair.key] = string.Empty;
                                }
                                else
                                {
                                    this._dicAllData[language][pair.key] = value.ToString();
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"Field {language} does not exist for key {pair.key}");
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

            if (this._dicAllData[this.CurrentLanguage].TryGetValue(key, out var value))
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
