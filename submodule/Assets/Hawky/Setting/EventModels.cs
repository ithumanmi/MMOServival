namespace Hawky.EventObserver
{
    public partial class EventName
    {
        public const string SOUND_CHANGED = "SOUND_CHANGED";
        public const string MUSIC_CHANGED = "MUSIC_CHANGED";
        public const string VIBRATE_CHANGED = "VIBRATE_CHANGED";
        public const string NAME_CHANGED = "NAME_CHANGED";
    }
    public class SoundChangedEvent : EventBase
    {
        public bool isOn;
    }

    public class MusicChangedEvent : EventBase
    {
        public bool isOn;
    }

    public class VibrateChangedEvent : EventBase
    {
        public bool isOn;
    }

    public class NameChangedEvent : EventBase
    {
        public string name;
    }
}
