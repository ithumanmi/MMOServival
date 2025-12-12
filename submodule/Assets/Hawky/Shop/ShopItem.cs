using Hawky.Ads;
using Hawky.Inventory;
using Hawky.ResourcesLoader;
using Hawky.SaveData;
using Hawky.Sound;
using Hawky.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hawky.Shop
{
    public class ShopItem : ResourcesPool
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _limitText;
        [SerializeField] private TMP_Text _priceText;

        [SerializeField] private ListItemView _listItemView;
        [SerializeField] private string _position;

        private ShopConfig _currentConfig;
        private ShopItemUnitData _currentUnitData;

        public Action<BuyShopResponse> OnBuySuccess;

        public string Id => _currentConfig == null ? string.Empty : _currentConfig.shopId;

        private void Awake()
        {
            if (_buyButton != null)
            {
                _buyButton.onClick.AddListener(OnTapBuy);
            }
        }

        protected override void OnFree()
        {
            base.OnFree();

            _currentConfig = null;
            _currentUnitData = null;
            OnBuySuccess = null;
        }

        public void SetPosition(string position)
        {
            _position = position;
        }

        public void Refresh()
        {
            var remain = _currentConfig.dailyLimit - _currentUnitData.buyAmountDaily;

            var handle = SingletonManager.Ins.FindFirst<IShopHandler>();

            if (_iconImage != null)
            {
                _iconImage.sprite = ShopIconManager.Ins.GetIcon(_currentConfig.shopId.ToLower());
            }

            if (_nameText != null)
            {
                _nameText.text = Hawky.Localization.LocalizationManager.Ins.GetString($"STRING_SHOP_ITEM_NAME_{_currentConfig.shopId.ToUpper()}");
            }

            if (_listItemView != null)
            {
                var rewards = ItemService.Parse(_currentConfig.rewards);

                _listItemView.Init(rewards);
            }

            if (_limitText != null)
            {
                _limitText.text = $"{remain}/{_currentConfig.dailyLimit}";
            }

            if (_priceText != null && handle != null)
            {
                _priceText.text = handle != null ? handle.PriceText(_currentConfig) : _currentConfig.price;
            }

            if (_buyButton != null)
            {
                _buyButton.interactable = remain > 0;
            }


            if (_currentConfig.shopId == ShopId.NO_ADS)
            {
                var adsData = SaveDataManager.Ins.GetData<AdsData>();
                if (adsData.noAds)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void Init(ShopConfig shopConfig)
        {
            _currentConfig = shopConfig;
            _currentUnitData = ShopService.Ins.GetShopDataById(shopConfig.shopId);

            Refresh();
        }

        protected virtual void OnInit()
        {

        }

        private void OnTapBuy()
        {
            SoundManager.Instance.PlaySound(SoundId.Click_Button);

            if (_currentConfig == null)
            {
                return;
            }

            var handlers = SingletonManager.Ins.FindAll<IShopHandler>();

            var handler = handlers.Find(x => x.CanHandle(_currentConfig));

#if !HAWKY_CHEAT
            if (handler == null)
            {
                Debug.LogError("Chua có đứa nào xử lý mua hàng hết, hãy tạo 1 class RuntimeSingleTon kế thừa IShopHandler để xử lý");
                return;
            }
#endif

            MyCoroutine.CoroutineManager.Ins.Start(BuyProgress(handler));
        }

        private IEnumerator BuyProgress(IShopHandler handler)
        {
            if (AdsService.Ins.InProgress())
            {
                yield break;
            }

            var result = -1;

#if !HAWKY_CHEAT
            handler.BuyProgress(_currentConfig, new ShopHandlerRequest
            {
                position = GetPosition(),
            }, (rs) =>
            {
                result = rs.reuslt ? 1 : 0;
            });
#else
            result = 1;
#endif

            yield return new WaitWhile(() => result == -1);

            if (result == 1)
            {
                var response = ShopService.Ins.BuyShop(new BuyShopRequest
                {
                    shopId = _currentConfig.shopId
                });

                if (response.shopId == ShopId.NO_ADS)
                {
                    var adsData = SaveDataManager.Ins.GetData<AdsData>();
                    adsData.noAds = true;
                    adsData.Save();

                    AdsService.Ins.LockBannerBySelf();
                }

                OnBuySuccess?.Invoke(response);
            }
        }

        public string GetPosition()
        {
            if (!string.IsNullOrEmpty(_position))
            {
                return _position;
            }

            return ShopPosition.Shop;
        }
    }
}
