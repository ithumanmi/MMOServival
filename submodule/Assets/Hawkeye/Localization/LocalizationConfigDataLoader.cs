using Hawkeye.Config;

namespace Hawkeye.Localization
{
    public class LocalizationConfigDataLoader : ConfigDataLoader<LocalizationConfig>
    {
        protected override string ResourcesPath()
        {
            return "LocalizationConfig";
        }
    }
}
