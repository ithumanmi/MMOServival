using Hawki.EventObserver;
using Hawki.SaveData;
using Hawki.UI;

namespace Hawki.Setting.UI
{
    public class VibrateToggle : AutoToggle
    {
        protected override void Awake()
        {
            base.Awake();

            var settingData = SaveDataManager.Instance.GetData<SettingData>();

            this.SetData(() => settingData.vibrate);
            this.OnValueChange = ((value) =>
            {
                settingData.vibrate = value;
                settingData.Save();

                EventObs.Instance.ExcuteEvent(EventName.VIBRATE_CHANGED, new VibrateChangedEvent()
                {
                    isOn = settingData.vibrate
                });
            });
        }
    }
}
