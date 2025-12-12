using Hawki.EventObserver;
using Hawki.SaveData;
using TMPro;
using UnityEngine;

namespace Hawki.Setting.UI
{
    public class ChangeName : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputName;

        private void Awake()
        {
            if (_inputName == null)
            {
                Debug.LogError("ChangeName không có ImputName");
                return;
            }

            var settingData = SaveDataManager.Instance.GetData<SettingData>();

            _inputName.text = settingData.name;

            _inputName.onEndEdit.AddListener(OnEndEdit);
        }

        private void OnEndEdit(string arg0)
        {
            var settingData = SaveDataManager.Instance.GetData<SettingData>();
            
            settingData.name = arg0;

            settingData.Save();

            EventObs.Instance.ExcuteEvent(EventName.NAME_CHANGED, new NameChangedEvent()
            {
                name = settingData.name,
            });
        }
    }
}
