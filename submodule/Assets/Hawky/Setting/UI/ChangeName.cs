using Hawky.EventObserver;
using Hawky.SaveData;
using TMPro;
using UnityEngine;

namespace Hawky.Setting.UI
{
    public class ChangeName : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputName;

        private void Awake()
        {
            if (_inputName == null)
            {
                Debug.LogError("ChangeName kh�ng c� ImputName");
                return;
            }

            var settingData = SaveDataManager.Ins.GetData<SettingData>();

            _inputName.text = settingData.name;

            _inputName.onEndEdit.AddListener(OnEndEdit);
        }

        private void OnEndEdit(string arg0)
        {
            var settingData = SaveDataManager.Ins.GetData<SettingData>();

            settingData.name = arg0;

            settingData.Save();

            EventObs.Ins.ExcuteEvent(EventName.NAME_CHANGED, new NameChangedEvent()
            {
                name = settingData.name,
            });
        }
    }
}
