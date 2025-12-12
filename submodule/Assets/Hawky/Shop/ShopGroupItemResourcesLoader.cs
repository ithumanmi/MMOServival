using Hawky.ResourcesLoader;

namespace Hawky.Shop
{
    public class ShopGroupItemResourcesLoader : ResourcesLoader<ShopGroupItemResourcesLoader, ShopGroupItem>
    {
        protected override string ResourcesPath()
        {
            return ResourcesLoaderLink.UI_SHOPGROUPITEM;
        }
    }
}