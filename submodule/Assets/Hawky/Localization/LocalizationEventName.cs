using UnityEngine;

namespace Hawky.EventObserver
{
    public partial class EventName
    {
        public const string LANGUAGE_CHANGED = "LANGUAGE_CHANGED";
    }

    public class LanguageChangedEvent : EventBase
    {
        public SystemLanguage language;
    }
}