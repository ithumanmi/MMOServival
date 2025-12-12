namespace Hawky.ResourcesLoader
{
    public class CommonResourcesLoader : ResourcesLoader<CommonResourcesLoader, ResourcesPool>
    {
        protected override string ResourcesPath()
        {
            return "Prefabs";
        }
    }
}
