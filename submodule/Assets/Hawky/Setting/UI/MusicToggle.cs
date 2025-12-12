using Hawky.EventObserver;
using Hawky.SaveData;
using Hawky.Sound;
using Hawky.UI;

namespace Hawky.Setting.UI
{
    public class MusicToggle : AutoToggle
    {
        protected override void Awake()
        {
            base.Awake();

            var settingData = SaveDataManager.Ins.GetData<SettingData>();

            this.SetData(() => settingData.music);
            this.OnValueChange = ((value) =>
            {
                settingData.music = value;
                settingData.Save();

                SoundManager.Instance.music = value ? 1 : 0;

                EventObs.Ins.ExcuteEvent(EventName.MUSIC_CHANGED, new MusicChangedEvent()
                {
                    isOn = settingData.music
                });
            });
        }
    }
}
