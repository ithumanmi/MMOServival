#if ANTADA_IAP

using Antada.Libs;
using Hawki;
using Hawki.AllConfig;
using Hawki.Config;
using Hawki.IAP;
using Hawki.MyCoroutine;
using Hawki.Shop;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hawki_Antada_IAP.IAP
{
    public class AntadaIAPManager : RuntimeSingleton<AntadaIAPManager>, IStartBehaviour, IProductProvider, IShopHandler
    {
        public void Start()
        {
            CoroutineManager.Instance.Start(TryInitIAP());
        }

        IEnumerator TryInitIAP()
        {
            while (true)
            {
                var iapManager = IAPManager.I;

                if (iapManager != null)
                {
                    iapManager.Delegate = this;
                    iapManager.Init();
                    break;
                };

                yield return null;
            }
        }


        public void BuyProgress(ShopConfig config, ShopHandlerRequest request,  Action<ShopHandlerResponse> onCompleted)
        {
            var response = new ShopHandlerResponse();
#if UNITY_EDITOR
            response.reuslt = true;
            onCompleted?.Invoke(response);
            return;
#endif
            var progressing = true;
            var result = false;

            IAPManager.I.Purchase(config.iapId, (x) =>
            {
                progressing = false; 
                result = x;
            }, request.position);

            CoroutineManager.Instance.Start(WaitToResponse());

            IEnumerator WaitToResponse()
            {
                yield return new WaitWhile(() => progressing);

                response.reuslt = result;
                onCompleted?.Invoke(response);
            }
        }

        public bool CanHandle(ShopConfig config)
        {
            return !string.IsNullOrEmpty(config.iapId);
        }

        public List<IAPProductSO> GetIAPProductSOs()
        {
            List<IAPProductSO> _products;
            _products = new List<IAPProductSO>();

            foreach (var productConfig in ConfigManager.Instance.GetData<ConfigAll>().IAPProductConfig)
            {
                var ins = ScriptableObject.CreateInstance<IAPProductSO>();

                ins.id = productConfig.id;
                ins.androidProductId = productConfig.androidId;
                ins.iosProductId = productConfig.iosId;
                ins.productType = productConfig.productType;
                ins.defaultPrice = productConfig.defaultPrice;

                _products.Add(ins);
            }

            return _products;
        }

        public string PriceText(ShopConfig config)
        {
            var iapManager = IAPManager.I;

            if (iapManager == null)
            {
                return config.price;
            }

            var product = iapManager.GetProductSO(config.iapId);

            if (product == null)
            {
                return config.price;
            }

            return product.PriceString;
        }

        public void ProcessRestore(IAPProductSO productSO)
        {
            if (productSO.id == ProductId.NO_ADS)
            {
                AdsManager.I.IsRemovedAds = true;
            }
        }
    }
}

#endif