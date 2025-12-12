#if ANTADA_FALCON

using Antada.Libs;
using Hawki;
using Hawki.Ads;
using Hawki.AllConfig;
using Hawki.Config;
using Hawki.MyCoroutine;
using Hawki.SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hawki_Antada_Falcon.Ads
{
    public class FalconAdsManager : RuntimeSingleton<FalconAdsManager>,
        IStartBehaviour, 
        IAdUnitIronSourceProvider, 
        IShowInterstitialAdsHandler, 
        IShowRewardAdsHandler,
        IShowBannerAdsHanlder,
        IAdsUnitBannerAdmobProvider,
        IAdsUnitAppOpenAdsAdmobProvider,
        IAdsAdmobController
    {

        private bool _isAdsShowing = false;

        private AdsManager _adsManager;
        private AdsManagerAdmob _adsManagerAdmob;
        private bool _bannerLoaded = false;
        private bool _currentBannerShowValue;
        private bool _consentGiven;
        public void Start()
        {
            FalconUMP.ShowConsentForm((consent) =>
            {
                _consentGiven = true;
            });

            CoroutineManager.Instance.Start(TryInitAds());
            CoroutineManager.Instance.Start(TryInitAdsAdMobs());
        }

        IEnumerator TryInitAds()
        {
            yield return new WaitUntil(() => _consentGiven);

            while (true)
            {
                var adsManager = AdsManager.I;

                if (adsManager != null)
                {
                    var adsData = SaveDataManager.Instance.GetData<AdsData>();

                    adsManager.Delegate = this;
                    adsManager.DefaultData(adsData.noAds);
                    adsManager.Init();

                    _adsManager = adsManager;
                    break;
                };

                yield return null;
            }
        }

        IEnumerator TryInitAdsAdMobs()
        {
            yield return new WaitUntil(() => _consentGiven);
            while (true)
            {
                var adsManagerAdmob = AdsManagerAdmob.I;

                if (adsManagerAdmob != null)
                {
                    adsManagerAdmob.bannerAdDelegate = this;
                    adsManagerAdmob.appOpenAdDelegate = this;
                    adsManagerAdmob.controllerDelegate = this;
                    AdsManagerAdmob.BannerAdLoadedStatus += OnBannerAdLoadedStatus;
                    adsManagerAdmob.LoadAppOpenAd();

                    _adsManagerAdmob = adsManagerAdmob;
                    break;
                };

                yield return null;
            }
        }

        public string AppKeyAndroid()
        {
            var config = ConfigManager.Instance.GetData<ConfigAll>().AdUnitIronSourceConfig.Last();

            return config.appKeyAndroid;
        }

        public string AppKeyiOS()
        {
            var config = ConfigManager.Instance.GetData<ConfigAll>().AdUnitIronSourceConfig.Last();

            return config.appKeyIos;
        }

        public void ShowInterstitial(ShowInterstitialRequest request, Action<ShowAdsResult> callBack)
        {
            if (_adsManager == null)
            {
                callBack?.Invoke(new ShowAdsResult
                {
                    result = AdsResultId.RESULT_BACK,
                });
                Debug.LogError("AdManager is Null");
                return;
            }

            var showAdsResult = new ShowAdsResult();
            _isAdsShowing = true;
            _adsManager.ShowInterstitial((result) =>
            {
                showAdsResult.result = result ? AdsResultId.RESULT_OK : AdsResultId.RESULT_BACK;
                _isAdsShowing = false;
                callBack?.Invoke(showAdsResult);
            }, request.position, request.placementId);
        }

        float IShowInterstitialAdsHandler.Interval()
        {
            var config = ConfigManager.Instance.GetData<ConfigAll>().AdsConfig.Last();

            return config.intervalIntersitial;
        }

        public void ShowReward(ShowRewardRequest request, Action<ShowAdsResult> callBack)
        {
            if (_adsManager == null)
            {
                callBack?.Invoke(new ShowAdsResult
                {
                    result = AdsResultId.RESULT_BACK,
                });
                Debug.LogError("AdManager is Null");
                return;
            }
            var showAdsResult = new ShowAdsResult();
            _isAdsShowing = true;
            _adsManager.ShowReward((result) =>
            {
                showAdsResult.result = result ? AdsResultId.RESULT_OK : AdsResultId.RESULT_BACK;
                _isAdsShowing = false;
                callBack?.Invoke(showAdsResult);
            }, request.position, request.placementId);
        }

        public List<string> AdUnitIdAndroidOpenAd()
        {
            var config = ConfigManager.Instance.GetData<ConfigAll>().AdUnitOpenAdsConfig;

            return config.Where(x => !string.IsNullOrEmpty(x.androidOpenAds)).Select(x => x.androidOpenAds).ToList();
        }

        public List<string> AdUnitIdiOSOpenAd()
        {
            var config = ConfigManager.Instance.GetData<ConfigAll>().AdUnitOpenAdsConfig;

            return config.Where(x => !string.IsNullOrEmpty(x.iosOpenAds)).Select(x => x.androidOpenAds).ToList();
        }

        public bool CanShowInteruptOpenAds()
        {
            return true;
        }

        public bool isRemovedAds()
        {
            return AdsService.Instance.IsNoAds();
        }

        public bool isAdShowing()
        {
            return _isAdsShowing;
        }

        public string AdUnitIdAndroidBannerAd()
        {
            var config = ConfigManager.Instance.GetData<ConfigAll>().AdUnitBannerAdsConfig;

            var detail = config.LastOrDefault();

            return detail.androidBannerAds;
        }

        public string AdUnitIdiOSBannerAd()
        {
            var config = ConfigManager.Instance.GetData<ConfigAll>().AdUnitBannerAdsConfig;

            var detail = config.LastOrDefault();

            return detail.iosBannerAds;
        }

        public void ShowBanner(ShowBannerRequest request, Action<bool> callback)
        {
            if (_adsManagerAdmob == null)
            {
                callback?.Invoke(false);
                Debug.LogError("AdsManagerAdmob is Null");
                return;
            }

            _currentBannerShowValue = request.value;
            if (_bannerLoaded == false && request.value)
            {
                _adsManagerAdmob.LoadBannerAdMob();
                _bannerLoaded = true;
                callback?.Invoke(true);
                return;
            }

            UpdateShow();
            callback?.Invoke(true);
        }

        private void UpdateShow()
        {
            if (_currentBannerShowValue)
            {
                _adsManagerAdmob.ShowBannerAdMob();
            }
            else
            {
                _adsManagerAdmob.HideBannerAdMob();
            }
        }

        private void OnBannerAdLoadedStatus(bool obj)
        {
            if (obj)
            {
                UpdateShow();
            }
        }
    }
}

#endif
