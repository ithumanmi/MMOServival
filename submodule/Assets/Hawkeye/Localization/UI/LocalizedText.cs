using System;
using TMPro;
using UnityEngine;

namespace Hawkeye.Localization.UI
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private string _fontString;
        [SerializeField] private string _backString;
        [SerializeField] private TextMeshProUGUI _target;

        public string Key 
        { 
            get
            {
                return _key;
            } 
            set
            {
                _key = value;
                UpdateText();
            }
        }

        private void OnEnable()
        {
            UpdateText();

            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDisable()
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(SystemLanguage language)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (_target != null)
            {
                _target.SetText($"{GetFontString()}{LocalizationManager.Instance.GetString(_key)}{GetBackString()}");
            }
        }

        private void OnValidate()
        {
            if (_target == null)
            {
                _target = GetComponent<TextMeshProUGUI>();
            }
        }

        private string GetFontString()
        {
            if (string.IsNullOrEmpty(_fontString))
            {
                return string.Empty;
            }

            return _fontString;
        }

        private string GetBackString()
        {
            if (string.IsNullOrEmpty(_backString))
            {
                return string.Empty;
            }

            return _backString;
        }
    }
}
