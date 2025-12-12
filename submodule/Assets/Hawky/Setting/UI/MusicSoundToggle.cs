using Hawky.EventObserver;
using Hawky.SaveData;
using Hawky.Sound;
using Hawky.UI;

namespace Hawky.Setting.UI
{
    public class MusicSoundToggle : AutoToggle
    {
        protected override void Awake()
        {
            base.Awake();

            var settingData = SaveDataManager.Ins.GetData<SettingData>();

            this.SetData(() => settingData.music);
            this.OnValueChange += ((value) =>
            {
                settingData.music = value;
                settingData.sound = value;
                settingData.Save();

                SoundManager.Instance.PlaySound(SoundId.Click_Button);

                SoundManager.Instance.music = value ? 1 : 0;
                SoundManager.Instance.sound = value ? 1 : 0;

                EventObs.Ins.ExcuteEvent(EventName.MUSIC_CHANGED, new MusicChangedEvent()
                {
                    isOn = settingData.music
                });

                EventObs.Ins.ExcuteEvent(EventName.SOUND_CHANGED, new SoundChangedEvent()
                {
                    isOn = settingData.music
                });
            });
        }
    }
}
