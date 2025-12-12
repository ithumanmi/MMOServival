#if ANTADA_FALCON

using Hawki_Antada_Falcon.AppsFlyer;
using System.Collections.Generic;

namespace Hawki.AllConfig
{
    public partial class ConfigAll
    {
        public List<AppsFlyerKeyConfig> AppsFlyerKeyConfig = new List<AppsFlyerKeyConfig>
        {
            new AppsFlyerKeyConfig
            {

            }
        };
    }
}

namespace Hawki_Antada_Falcon.AppsFlyer
{
    public class AppsFlyerKeyConfig
    {
        public string appId = "";
        public string devKey = "UAVHtuYSgwSPXxXQVDGA65";
    }
}

#endif
