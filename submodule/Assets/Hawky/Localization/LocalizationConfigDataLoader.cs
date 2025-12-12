using Hawky.Config;

namespace Hawky.Localization
{
    public class LocalizationConfigDataLoader : ConfigDataLoader<LocalizationConfig>
    {
        protected override string ResourcesPath()
        {
            return "LocalizationConfig";
        }
    }
}
