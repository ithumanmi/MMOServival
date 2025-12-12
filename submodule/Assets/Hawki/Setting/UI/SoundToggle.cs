using Hawki.EventObserver;
using Hawki.SaveData;
using Hawki.Sound;
using Hawki.UI;

namespace Hawki.Setting.UI
{
    public class SoundToggle : AutoToggle
    {
        protected override void Awake()
        {
            base.Awake();

            var settingData = SaveDataManager.Instance.GetData<SettingData>();

            this.SetData(() => settingData.sound);
            this.OnValueChange = ((value) =>
            {
                settingData.sound = value;
                settingData.Save();

                SoundManager.Instance.sound = value ? 1 : 0;

                EventObs.Instance.ExcuteEvent(EventName.MUSIC_CHANGED, new SoundChangedEvent()
                {
                    isOn = settingData.sound
                });
            });
        }
    }
}
