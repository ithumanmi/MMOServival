using Hawki.Config;

namespace Hawki.AllConfig
{
    public class AllConfigDataLoader : ConfigDataLoader<ConfigAll>
    {
        protected override string ResourcesPath()
        {
            return "AllConfig";
        }
    }
}
