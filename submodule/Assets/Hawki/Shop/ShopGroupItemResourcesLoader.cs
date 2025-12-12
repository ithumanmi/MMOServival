using Hawki.ResourcesLoader;

namespace Hawki.Shop
{
    public class ShopGroupItemResourcesLoader : ResourcesLoader<ShopGroupItemResourcesLoader, ShopGroupItem>
    {
        protected override string ResourcesPath()
        {
            return ResourcesLoaderLink.UI_SHOPGROUPITEM;
        }
    }
}