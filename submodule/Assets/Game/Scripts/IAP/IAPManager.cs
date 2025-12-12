using NFramework;
using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Antada.Libs
{
    public interface IProductProvider
    {
        List<IAPProductSO> GetIAPProductSOs();
        void ProcessRestore(IAPProductSO productSO);
    }

    public interface IIAPLooger
    {
        void IAPLog(Product product, string whereToBuy);
    }

    public interface IIAPActionProvider
    {
        void LoadingPayment();
        void HideLoadingPayment();
    }

    public class IAPManager : SingletonMono<IAPManager>, IDetailedStoreListener
    {
        public IProductProvider Delegate;
        public List<IIAPLooger> iAPLoogers = new List<IIAPLooger>();
        public IIAPActionProvider IAPActions;
        private IStoreController _controller;
        private IExtensionProvider _extensions;
        private IAppleExtensions _appleExtensions;
        private IGooglePlayStoreExtensions _googleExtensions;
        private bool _isPurchaseInProgress;
        private Action<bool> _purchaseAction;
        private Dictionary<string, IAPProductSO> _idToIAPProductSODic = new Dictionary<string, IAPProductSO>();
        private string _whereToBuy;
        public bool IsInitialized() => _controller != null && _extensions != null;

        public void Init()
        {
            if (IsInitialized())
                return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (var iapProductSO in Delegate.GetIAPProductSOs())
            {
                var id = iapProductSO.id;
                _idToIAPProductSODic.Add(id, iapProductSO);
                builder.AddProduct(id, iapProductSO.productType, new IDs()
                {
                    {iapProductSO.androidProductId, GooglePlay.Name},
                    {iapProductSO.iosProductId, AppleAppStore.Name }
                });
            }
            UnityPurchasing.Initialize(this, builder);
        }

        public void AddLogger(IIAPLooger logger)
        {
            if (iAPLoogers == null)
            {
                iAPLoogers = new List<IIAPLooger>();
            }
            iAPLoogers.Add(logger);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            NFramework.Logger.Log("IAP Initialized");
            _controller = controller;
            _extensions = extensions;

            foreach (var iapProductSO in Delegate.GetIAPProductSOs())
            {
                var product = _controller.products.WithID(iapProductSO.id);
                if (product != null)
                    iapProductSO.PriceString = product.metadata.localizedPriceString;
            }
        }

        public IAPProductSO GetProductSO(string iapId)
        {
            if (_idToIAPProductSODic.TryGetValue(iapId, out var product))
            {
                return product;
            }

            return null;
        }

        public bool HasReceipt(string iapId)
        {
            foreach (var item in _controller.products.all)
            {
                if (item.definition.id == iapId)
                {
                    return item.hasReceipt;
                }
            }

            return false;
        }

        // This method will called when purchase completed and when init completed to restore purchase
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var product = purchaseEvent.purchasedProduct;
            if (_idToIAPProductSODic.TryGetValue(product.definition.id, out var iapProductSO))
            {
                if (_isPurchaseInProgress)
                {
                    NFramework.Logger.Log($"Purchase OK: {product.hasReceipt} {product.transactionID} {product.definition.id}");
                    _isPurchaseInProgress = false;
                    IAPActions?.HideLoadingPayment();
                    _purchaseAction?.Invoke(true);
                    if (iAPLoogers != null)
                    {
                        foreach (var logger in iAPLoogers)
                        {
                            logger.IAPLog(product, _whereToBuy);
                        }
                    }
                }
                else
                {
                    this.Delegate.ProcessRestore(iapProductSO);
                    // We suppose its a restore if the method is called while _isPurchaseInProgress == false
                    NFramework.Logger.Log($"Restore OK: {product.hasReceipt} {product.transactionID} {product.definition.id}");
                }
            }
            else
            {
                IAPActions?.HideLoadingPayment();
                NFramework.Logger.LogError($"Cannot find iapProductSO id:{product.definition.id}", this);
            }


            return PurchaseProcessingResult.Complete;
        }

        public void Purchase(IAPProductSO iapProductSO, Action<bool> callback, string whereToBuy)
        {
            Purchase(iapProductSO.id, callback, whereToBuy);
        }

        public void Purchase(string iapProductSO, Action<bool> callback, string whereToBuy)
        {
            IAPActions?.LoadingPayment();
            _purchaseAction = callback;
            _whereToBuy = whereToBuy;
            if (DeviceInfo.IsDevelopment)
            {
                IAPActions?.HideLoadingPayment();
                callback?.Invoke(true);
                return;
            }

            if (_isPurchaseInProgress)
            {
                IAPActions?.HideLoadingPayment();
                callback?.Invoke(false);
                NFramework.Logger.Log("Another Purchase in progress");
                return;
            }

            if (IsInitialized())
            {
                _isPurchaseInProgress = true;
                var product = _controller.products.WithID(iapProductSO);
                if (product != null && product.availableToPurchase)
                {
                    NFramework.Logger.Log($"Purchasing product asynchronously: '{product.definition.storeSpecificId}'");
                    _controller.InitiatePurchase(product);
                }
                else
                {
                    IAPActions?.HideLoadingPayment();
                    callback?.Invoke(false);
                    NFramework.Logger.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    _isPurchaseInProgress = false;
                }
            }
            else
            {
                IAPActions?.HideLoadingPayment();
                callback?.Invoke(false);
                NFramework.Logger.LogError($"Not IsInitialized");
                return;
            }
        }

        public SubscriptionInfo GetSubscriptionInfo(IAPProductSO productSO)
        {
            if (_controller == null)
            {
                return null;
            }

            foreach (var item in _controller.products.all)
            {
                if(item.definition.id == productSO.id)
                {
                    if (item.hasReceipt)
                    {
                        SubscriptionManager p = new SubscriptionManager(item, null);
                        SubscriptionInfo info = p.getSubscriptionInfo();
                        return info;
                    }
                }
            }
            return null;
        }

        public SubscriptionInfo GetSubscriptionInfo(string productId)
        {
            foreach (var item in _controller.products.all)
            {
                if(item.definition.id == productId)
                {
                    if (item.hasReceipt)
                    {
                        SubscriptionManager p = new SubscriptionManager(item, null);
                        SubscriptionInfo info = p.getSubscriptionInfo();
                        return info;
                    }
                }
            }
            return null;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            NFramework.Logger.Log("IAP init failed");
            switch (error)
            {
                case InitializationFailureReason.AppNotKnown:
                    NFramework.Logger.LogError("Is your App correctly uploaded on the relevant publisher console?");
                    break;
                case InitializationFailureReason.PurchasingUnavailable:
                    // Ask the user if billing is disabled in device settings.
                    NFramework.Logger.Log("Billing disabled!");
                    break;
                case InitializationFailureReason.NoProductsAvailable:
                    // Developer configuration error; check product metadata.
                    NFramework.Logger.Log("No products available for purchase!");
                    break;
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            NFramework.Logger.Log("IAP init failed: " + message);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            NFramework.Logger.Log("IAP Purchase Failed: " + failureDescription.message);
            _isPurchaseInProgress = false;
            IAPActions?.HideLoadingPayment();
            _purchaseAction?.Invoke(false);

        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            NFramework.Logger.Log("IAP Purchase Failed:");
            _isPurchaseInProgress = false;
            IAPActions?.HideLoadingPayment();
            _purchaseAction?.Invoke(false);
        }


        public void RestorePurchases(Action<bool> callback)
        {
            _extensions.GetExtension<IAppleExtensions>().RestoreTransactions((result, errorMessage) =>
            {
                if (result)
                {
                    NFramework.Logger.Log("Restore purchases succeeded.");
                    callback?.Invoke(true);
                }
                else
                {
                    NFramework.Logger.Log($"Restore purchases failed. Error message: {errorMessage}");
                    callback?.Invoke(false);
                }
            });
        }

    }
}


