using Hawki.Config;

namespace Hawki.Localization
{
    public class LocalizationConfigDataLoader : ConfigDataLoader<LocalizationConfig>
    {
        protected override string ResourcesPath()
        {
            return "LocalizationConfig";
        }
    }
}
