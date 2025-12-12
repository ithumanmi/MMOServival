using Hawki.ResourcesLoader;
using Hawki.UI;

public class ShopItemListItemViewByTypeResourcesLoader : ResourcesLoader<ShopItemListItemViewByTypeResourcesLoader, ListItemView>
{
    protected override string ResourcesPath()
    {
        return ResourcesLoaderLink.UI_SHOPITEMLISTITEMVIEWBYTYPE;
    }
}
