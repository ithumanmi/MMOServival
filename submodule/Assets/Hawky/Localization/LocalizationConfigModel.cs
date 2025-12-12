using Hawky.Config;
using System.Collections.Generic;

namespace Hawky.Localization
{
    public class LocalizationConfig : ConfigDataBase
    {
        public List<LocalizationConfigData> LocalizationConfigData;
    }


    public class LocalizationConfigData
    {
        public string key;
        public string english;
        public string vietnamese;
        public string japanese;
        public string french;
        public string chinese;
        public string danish;
        public string korean;
        public string german;
    }
}
