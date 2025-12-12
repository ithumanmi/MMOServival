using Hawky.Config;

namespace Hawky.AllConfig
{
    public class AllConfigDataLoader : ConfigDataLoader<ConfigAll>
    {
        protected override string ResourcesPath()
        {
            return "AllConfig";
        }
    }
}
