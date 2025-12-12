using Hawky.IAP;
using System.Collections.Generic;

namespace Hawky.AllConfig
{
    public partial class ConfigAll
    {
        public List<IAPProductConfig> IAPProductConfig = new List<IAPProductConfig>();
    }
}

namespace Hawky.IAP
{
    public partial class IAPProductConfig
    {
        public string id;
        public float defaultPrice;
    }
}