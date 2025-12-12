using Hawky.Setting;
using System.Collections.Generic;

namespace Hawky.AllConfig
{
    public partial class ConfigAll
    {
        public List<SettingConfig> SettingConfig = new List<SettingConfig>
        {
            new SettingConfig()
        };
    }
}

namespace Hawky.Setting
{
    public class SettingConfig
    {
        public string termsOfUse = URL.DEFAULT_TERM_OF_USE;
        public string privacyPolicy = URL.DEFAULT_PRIVACY_POLICY;
    }
}