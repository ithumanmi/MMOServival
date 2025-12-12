using Hawky.EventObserver;
using TMPro;
using UnityEngine;

namespace Hawky.Localization.UI
{
    public class LocalizedText : MonoBehaviour, IRegister
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

        public void SetKey(string key)
        {
            Key = key;
        }

        private void OnEnable()
        {
            UpdateText();

            EventObs.Ins.AddRegister(EventName.LANGUAGE_CHANGED, this);
        }

        private void OnDisable()
        {
            EventObs.Ins.RemoveRegister(EventName.LANGUAGE_CHANGED, this);
        }

        private void UpdateText()
        {
            if (_target != null)
            {
                _target.SetText($"{GetFontString()}{LocalizationManager.Ins.GetString(_key)}{GetBackString()}");
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

        public void OnEvent(string eventId, EventBase data)
        {
            switch (eventId)
            {
                case EventName.LANGUAGE_CHANGED:
                    UpdateText();
                    break;
            }
        }
    }
}
