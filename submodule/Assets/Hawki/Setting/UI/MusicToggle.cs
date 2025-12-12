using Hawki.EventObserver;
using Hawki.SaveData;
using Hawki.Sound;
using Hawki.UI;

namespace Hawki.Setting.UI
{
    public class MusicToggle : AutoToggle
    {
        protected override void Awake()
        {
            base.Awake();

            var settingData = SaveDataManager.Instance.GetData<SettingData>();

            this.SetData(() => settingData.music);
            this.OnValueChange = ((value) =>
            {
                settingData.music = value;
                settingData.Save();

                SoundManager.Instance.music = value ? 1 : 0;

                EventObs.Instance.ExcuteEvent(EventName.MUSIC_CHANGED, new MusicChangedEvent()
                {
                    isOn = settingData.music
                });
            });
        }
    }
}
