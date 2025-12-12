using Hawki.Setting;
using System.Collections.Generic;

namespace Hawki.AllConfig
{
    public partial class ConfigAll
    {
        public List<SettingConfig> SettingConfig = new List<SettingConfig>
        {
            new SettingConfig()
        };
    }
}

namespace Hawki.Setting
{
    public class SettingConfig
    {
        public string termsOfUse = URL.DEFAULT_TERM_OF_USE;
        public string privacyPolicy = URL.DEFAULT_PRIVACY_POLICY;
    }
}