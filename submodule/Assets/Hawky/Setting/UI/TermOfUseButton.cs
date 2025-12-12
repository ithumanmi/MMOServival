using Hawky.AllConfig;
using Hawky.Config;
using Hawky.UI;
using System.Linq;
using UnityEngine;

namespace Hawky.Setting.UI
{
    [RequireComponent(typeof(OpenUrlButton))]
    public class TermOfUseButton : MonoBehaviour
    {
        private void Awake()
        {
            var button = GetComponent<OpenUrlButton>();

            var settingConfig = ConfigManager.Ins.GetData<ConfigAll>().SettingConfig.Last();

            button.SetUrl(settingConfig.termsOfUse);
        }

        private void OnValidate()
        {
            var button = GetComponent<OpenUrlButton>();
            if (string.IsNullOrEmpty(button.Url))
            {
                button.SetUrl(URL.DEFAULT_TERM_OF_USE);
            }
        }
    }
}
