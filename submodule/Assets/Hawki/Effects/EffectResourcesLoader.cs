using Hawki.ResourcesLoader;

namespace Hawki.Effect
{
    public class EffectResourcesLoader : ResourcesLoader<EffectResourcesLoader, ResourcesPool>
    {
        protected override string ResourcesPath()
        {
            return "Prefabs/Effects";
        }
    }
}
