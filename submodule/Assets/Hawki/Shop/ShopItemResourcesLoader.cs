using Hawki.ResourcesLoader;

namespace Hawki.Shop
{
    public class ShopItemResourcesLoader : ResourcesLoader<ShopItemResourcesLoader, ShopItem>
    {
        protected override string ResourcesPath()
        {
            return ResourcesLoaderLink.UI_SHOPITEM;
        }
    }
}