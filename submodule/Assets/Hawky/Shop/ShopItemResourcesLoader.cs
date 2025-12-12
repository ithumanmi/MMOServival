using Hawky.ResourcesLoader;

namespace Hawky.Shop
{
    public class ShopItemResourcesLoader : ResourcesLoader<ShopItemResourcesLoader, ShopItem>
    {
        protected override string ResourcesPath()
        {
            return ResourcesLoaderLink.UI_SHOPITEM;
        }
    }
}