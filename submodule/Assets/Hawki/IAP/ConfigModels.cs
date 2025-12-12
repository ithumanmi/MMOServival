using Hawki.IAP;
using System.Collections.Generic;

namespace Hawki.AllConfig
{
    public partial class ConfigAll
    {
        public List<IAPProductConfig> IAPProductConfig = new List<IAPProductConfig>();
    }
}

namespace Hawki.IAP
{
    public partial class IAPProductConfig
    {
        public string id;
        public float defaultPrice;
    }
}