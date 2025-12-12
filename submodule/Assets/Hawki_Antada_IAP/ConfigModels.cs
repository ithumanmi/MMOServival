#if ANTADA_IAP

using UnityEngine.Purchasing;

namespace Hawki.IAP
{
    public partial class IAPProductConfig
    {
        public ProductType productType;
        public string androidId;
        public string iosId;
    }
}

namespace Hawki.Shop
{
    public partial class ShopConfig
    {
        public string iapId;
    }
}

#endif