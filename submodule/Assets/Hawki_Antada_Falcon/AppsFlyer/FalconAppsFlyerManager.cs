#if ANTADA_FALCON

using Antada.Libs;
using AppsFlyerSDK;
using Hawki;
using Hawki.AllConfig;
using Hawki.Config;
using Hawki.MyCoroutine;
using System.Collections;
using System.Linq;

namespace Hawki_Antada_Falcon.AppsFlyer
{
    public class FalconAppsFlyerManager : IStartBehaviour, IAppsFlyerKey
    {
        public string GetAppID()
        {
            var config = ConfigManager.Instance.GetData<ConfigAll>().AppsFlyerKeyConfig.Last();

            return config.appId;
        }

        public string GetDevKey()
        {
            var config = ConfigManager.Instance.GetData<ConfigAll>().AppsFlyerKeyConfig.Last();

            return config.devKey;
        }

        public void Start()
        {
            CoroutineManager.Instance.Start(TryInitAppsFlyer());
        }

        IEnumerator TryInitAppsFlyer()
        {
            while (true)
            {
                var appsFlyers = AppsFlyerManager.I;

                if (appsFlyers != null)
                {
                    appsFlyers.Delegate = this;
                    appsFlyers.Init();

                    AppsFlyerAdRevenue.start();
#if DEVELOPMENT
                    AppsFlyerAdRevenue.setIsDebug(true);
#endif
                    break;
                };

                yield return null;
            }
        }
    }
}

#endif
