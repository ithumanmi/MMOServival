using Hawky.ResourcesLoader;

namespace Hawky.Effect
{
    public class EffectResourcesLoader : ResourcesLoader<EffectResourcesLoader, ResourcesPool>
    {
        protected override string ResourcesPath()
        {
            return "Prefabs/Effects";
        }
    }
}
