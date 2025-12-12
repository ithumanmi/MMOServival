using System;

namespace Hawky.Shop
{
    public interface IShopHandler
    {
        void BuyProgress(ShopConfig config, ShopHandlerRequest request, Action<ShopHandlerResponse> onCompleted);
        bool CanHandle(ShopConfig config);
        string PriceText(ShopConfig config);
    }

    public class ShopHandlerRequest
    {
        public string position;
    }

    public class ShopHandlerResponse
    {
        public bool reuslt;
    }
}