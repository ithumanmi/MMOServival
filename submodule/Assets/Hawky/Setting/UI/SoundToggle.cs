using Hawky.EventObserver;
using Hawky.SaveData;
using Hawky.Sound;
using Hawky.UI;

namespace Hawky.Setting.UI
{
    public class SoundToggle : AutoToggle
    {
        protected override void Awake()
        {
            base.Awake();

            var settingData = SaveDataManager.Ins.GetData<SettingData>();

            this.SetData(() => settingData.sound);
            this.OnValueChange = ((value) =>
            {
                settingData.sound = value;
                settingData.Save();

                SoundManager.Instance.sound = value ? 1 : 0;

                EventObs.Ins.ExcuteEvent(EventName.MUSIC_CHANGED, new SoundChangedEvent()
                {
                    isOn = settingData.sound
                });
            });
        }
    }
}
