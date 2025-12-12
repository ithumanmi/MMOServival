#if ANTADA_FALCON

using Hawki_Antada_Falcon.Ads;
using System.Collections.Generic;

namespace Hawki.AllConfig
{
    public partial class ConfigAll
    {
        public List<AdUnitIronSourceConfig> AdUnitIronSourceConfig = new List<AdUnitIronSourceConfig>
        {
            new AdUnitIronSourceConfig()
        };

        public List<AdUnitOpenAdsConfig> AdUnitOpenAdsConfig = new List<AdUnitOpenAdsConfig>();
        public List<AdUnitBannerAdsConfig> AdUnitBannerAdsConfig = new List<AdUnitBannerAdsConfig>
        {
            new AdUnitBannerAdsConfig()
        };
        public List<AdsConfig> AdsConfig = new List<AdsConfig>
        {
            new AdsConfig()
        };
    }
}

namespace Hawki_Antada_Falcon.Ads
{
    public class AdUnitIronSourceConfig
    {
        public string appKeyAndroid = "";
        public string appKeyIos = "";
    }

    public class AdUnitOpenAdsConfig
    {
        public string androidOpenAds;
        public string iosOpenAds;
    }

    public class AdUnitBannerAdsConfig
    {
        public string androidBannerAds = "";
        public string iosBannerAds = "";
    }

    public class AdsConfig
    {
        public float intervalIntersitial = 0;
    }
}

#endif
