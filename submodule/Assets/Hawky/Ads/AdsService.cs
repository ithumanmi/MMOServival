using Hawky.MyCoroutine;
using Hawky.SaveData;
using Hawky.Shop;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hawky.Ads
{
    public class AdsService : RuntimeSingleton<AdsService>, IShopHandler, ILockBannerRegister, IStartBehaviour
    {
        public ShowAdsResult currentResult;

        private bool _adsInProgress = false;
        private float _lastTimeShowIntersitial;

        public void Start()
        {
            var adsData = SaveDataManager.Ins.GetData<AdsData>();
            if (adsData.noAds)
            {
                RegisterLockBanner(this);
            }
        }

        public bool InProgress()
        {
            return _adsInProgress;
        }

        public bool IsNoAds()
        {
            var adsData = SaveDataManager.Ins.GetData<AdsData>();

            return adsData.noAds;
        }

        public IEnumerator WaitForComplete()
        {
            yield return new WaitWhile(() => currentResult == null || currentResult.result == AdsResultId.RESULT_INPROGRESS);
        }

        public void ShowIntersitial(ShowInterstitialRequest request)
        {
            currentResult = new ShowAdsResult();
            currentResult.result = AdsResultId.RESULT_INPROGRESS;

            var adsData = SaveDataManager.Ins.GetData<AdsData>();
            if (adsData.noAds)
            {
                currentResult.result = AdsResultId.RESULT_OK;
                return;
            }
            ;

            var interstitialHanlder = SingletonManager.Ins.FindFirst<IShowInterstitialAdsHandler>();

            if (interstitialHanlder == null || Time.realtimeSinceStartup - _lastTimeShowIntersitial < interstitialHanlder.Interval())
            {
                currentResult.result = AdsResultId.RESULT_BACK;
                return;
            }

            _adsInProgress = true;
            _lastTimeShowIntersitial = Time.realtimeSinceStartup;
            interstitialHanlder.ShowInterstitial(request, (showAdsResult) =>
            {
                currentResult.result = showAdsResult.result;
                _adsInProgress = false;
            });
        }

        public void ShowReward(ShowRewardRequest request)
        {
            currentResult = new ShowAdsResult();
            currentResult.result = AdsResultId.RESULT_INPROGRESS;

#if HAWKy_CHEAT
            currentResult.result = AdsResultId.RESULT_OK;
            return;
#endif

            var rewardHanlder = SingletonManager.Ins.FindFirst<IShowRewardAdsHandler>();

            if (rewardHanlder == null)
            {
                currentResult.result = AdsResultId.RESULT_OK;
                return;
            }

            _adsInProgress = false;
            rewardHanlder.ShowReward(request, (showAdsResult) =>
            {
                currentResult.result = showAdsResult.result;
                _adsInProgress = false;
            });
        }

        bool _lastValue = false;
        private HashSet<IBannerRegister> _showingBannerRegisters = new HashSet<IBannerRegister>();
        private HashSet<ILockBannerRegister> _lockingBannerRegisters = new HashSet<ILockBannerRegister>();

        public void RegisterShowBanner(IBannerRegister register)
        {
            _showingBannerRegisters.Add(register);
            UpdateBanner();
        }

        public void UnRegistShowBanner(IBannerRegister register)
        {
            _showingBannerRegisters.Remove(register);
            UpdateBanner();
        }

        public void RegisterLockBanner(ILockBannerRegister register)
        {
            _lockingBannerRegisters.Add(register);
            UpdateBanner();
        }

        public void UnRegistLockBanner(ILockBannerRegister register)
        {
            _lockingBannerRegisters.Remove(register);
            UpdateBanner();
        }

        public void LockBannerBySelf()
        {
            RegisterLockBanner(this);
            UpdateBanner();
        }

        private void UpdateBanner()
        {
            var adsData = SaveDataManager.Ins.GetData<AdsData>();
            ShowBanner(_lockingBannerRegisters.Count == 0 && _showingBannerRegisters.Count > 0 && !adsData.noAds);
        }

        private void ShowBanner(bool showValue)
        {
            if (_lastValue == showValue)
            {
                return;
            }

            var bannerHandler = SingletonManager.Ins.FindFirst<IShowBannerAdsHanlder>();

            if (bannerHandler == null)
            {
                return;
            }

            bannerHandler.ShowBanner(new ShowBannerRequest
            {
                value = showValue,
            }, (result) =>
            {
                if (result)
                {
                    _lastValue = showValue;
                }
            });
        }

        public void BuyProgress(ShopConfig config, ShopHandlerRequest request, Action<ShopHandlerResponse> onCompleted)
        {
            var response = new ShopHandlerResponse();

            CoroutineManager.Ins.Start(AdsProgress());

            IEnumerator AdsProgress()
            {
                AdsService.Ins.ShowReward(new ShowRewardRequest
                {
                    position = request.position
                });

                yield return AdsService.Ins.WaitForComplete();

                response.reuslt = AdsService.Ins.currentResult.result == AdsResultId.RESULT_OK;

                onCompleted?.Invoke(response);
            }
        }

        public bool CanHandle(ShopConfig config)
        {
            return config.price == PriceId.ADS;
        }

        public string PriceText(ShopConfig config)
        {
            return config.price;
        }
    }

    public class ShowAdsResult
    {
        public int result { get; internal set; }
    }
}