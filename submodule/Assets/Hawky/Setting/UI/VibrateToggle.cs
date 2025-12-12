using Hawky.EventObserver;
using Hawky.SaveData;
using Hawky.UI;

namespace Hawky.Setting.UI
{
    public class VibrateToggle : AutoToggle
    {
        protected override void Awake()
        {
            base.Awake();

            var settingData = SaveDataManager.Ins.GetData<SettingData>();

            this.SetData(() => settingData.vibrate);
            this.OnValueChange = ((value) =>
            {
                settingData.vibrate = value;
                settingData.Save();

                EventObs.Ins.ExcuteEvent(EventName.VIBRATE_CHANGED, new VibrateChangedEvent()
                {
                    isOn = settingData.vibrate
                });
            });
        }
    }
}
