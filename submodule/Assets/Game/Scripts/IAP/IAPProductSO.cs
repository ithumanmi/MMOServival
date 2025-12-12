using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Antada.Libs
{

    [CreateAssetMenu(menuName = "ScriptableObject/IAPBundleSO")]
    public class IAPProductSO : ScriptableObject
    {
        public event Action<string> OnPriceStringChanged;
        public string id;
        public string androidProductId;
        public string iosProductId;
        public ProductType productType;
        public float defaultPrice;
        private string _priceString;
        public string PriceString
        {
            get
            {
                if (string.IsNullOrEmpty(_priceString))
                    return $"{defaultPrice}$";
                else
                    return _priceString;
            }
            set
            {
                _priceString = value;
                OnPriceStringChanged?.Invoke(value);
            }
        }
    }
}

