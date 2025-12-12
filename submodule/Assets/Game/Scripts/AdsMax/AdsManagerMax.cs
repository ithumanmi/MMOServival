using System;
using System.Collections;
using System.Collections.Generic;
using NFramework;
using UnityEngine;

namespace Antada.Libs
{
    public interface IAdUnitMaxProvider
    {
        string SDKKey();
        AdUnits AdUnitsAndroid();
        AdUnits AdUnitsIOS();
    }

    public interface ILoggerImpressionMax
    {
        void AdRevenuePaidEvent(MaxSdkBase.AdInfo impressionData);
    }

    public interface ILoggerIntersitialMax
    {
        void LogEventIntersApiCalled(MaxSdkBase.AdInfo adInfo);
        void LogEventIntersDisplayed(MaxSdkBase.AdInfo adInfo);
        void LogEventInterLoadFailed(MaxSdkBase.ErrorInfo errorInfo);
        void LogEventInterHidden(MaxSdkBase.AdInfo adInfo);
        void LogEventIntersShow();
    }

    public interface ILoggerRewardedMax
    {
        void LogEventRewardedApiCalled(MaxSdkBase.AdInfo adInfo);
        void LogEventRewardedDisplayed(MaxSdkBase.AdInfo adInfo);
        void LogEventRewardedShow();
    }

    public class AdUnits
    {
        public string intersitialAdunit;
        public string bannerAdunit;
        public string rewardedAdunit;
        public string appOpenAdunit;
    }

    public class AdsManagerMax : SingletonMono<AdsManagerMax>, ISaveable
    {
        [SerializeField] private SaveData _saveData;
        public static Action<bool> OnIsRemovedAdsChanged;
        public IAdUnitMaxProvider Delegate;
        public ILoggerImpressionMax loggerImpressionMax;
        private List<ILoggerIntersitialMax> _loggersIntersitial;
        private List<ILoggerRewardedMax> _loggersRewardeds;
        private bool isInitialize = false;
        private MaxSdkBase.BannerPosition _bannerPosition;
        private Action<bool> _OnRewardsAction;
        private Action _OnRewardsHiddenAction;
        private Action<bool> _OnInterstitialAction;
        private Action<bool> _OnAppOpenAdAction;
        public static Action OnLoadBannerAdsFail;
        private bool _isAdShowing;

        public bool IsInitialized()
        {
            return isInitialize;
        }

        private string GetSDKKey()
        {
            return Delegate.SDKKey();
        }

        public string BannerAdUnit()
        {
#if UNITY_ANDROID
            return Delegate.AdUnitsAndroid().bannerAdunit;
#elif UNITY_IOS || UNITY_IPHONE
            return Delegate.AdUnitsIOS().bannerAdunit;
#else
            return "unsupport";
#endif
        }

        private string IntersitialAdUnit()
        {
#if UNITY_ANDROID
            return Delegate.AdUnitsAndroid().intersitialAdunit;
#elif UNITY_IOS || UNITY_IPHONE
            return Delegate.AdUnitsIOS().intersitialAdunit;
#else
            return "unsupport";
#endif
        }

        private string RewardedAdUnit()
        {
#if UNITY_ANDROID
            return Delegate.AdUnitsAndroid().rewardedAdunit;
#elif UNITY_IOS || UNITY_IPHONE
            return Delegate.AdUnitsIOS().rewardedAdunit;
#else
            return "unsupport";
#endif
        }

        private string AppOpenAdUnit()
        {
#if UNITY_ANDROID
            return Delegate.AdUnitsAndroid().appOpenAdunit;
#elif UNITY_IOS || UNITY_IPHONE
            return Delegate.AdUnitsIOS().appOpenAdunit;
#else
            return "unsupport";
#endif
        }

        public bool IsAdShowing
        {
            get => _isAdShowing;
        }

        public Rect BannerAdRect
        {
            get
            {
                var density = MaxSdkUtils.GetScreenDensity();
                var bannerRect = MaxSdk.GetBannerLayout(BannerAdUnit());
                bannerRect.width *= density;
                bannerRect.height *= density;
                return bannerRect;
            }
        }

        public void AddIntersitialLogger(ILoggerIntersitialMax logger)
        {
            if (_loggersIntersitial == null)
            {
                _loggersIntersitial = new List<ILoggerIntersitialMax>();
            }
            _loggersIntersitial.Add(logger);
        }

        public void AddRewardedLogger(ILoggerRewardedMax logger)
        {
            if (_loggersRewardeds == null)
            {
                _loggersRewardeds = new List<ILoggerRewardedMax>();
            }
            _loggersRewardeds.Add(logger);
        }

        public bool IsRemovedAds
        {
            get => _saveData.isRemovedAds;
            set
            {
                if (_saveData.isRemovedAds != value)
                {
                    _saveData.isRemovedAds = value;
                    DataChanged = true;
                    OnIsRemovedAdsChanged?.Invoke(value);

                    if (value)
                        HideBanner();
                }
            }
        }

        public void Init(Action OnConsentCompleted = null, MaxSdkBase.BannerPosition bannerPosition = MaxSdkBase.BannerPosition.BottomCenter)
        {
            _bannerPosition = bannerPosition;
#if !NO_ADS
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
#if DEVELOPMENT
                MaxSdk.ShowMediationDebugger();
                MaxSdk.SetCreativeDebuggerEnabled(true);
                MaxSdk.ShowCreativeDebugger();
#endif
                Debug.Log("AdsManager: Max SDK Initialized");
                isInitialize = true;
                OnConsentCompleted?.Invoke();
                if (!string.IsNullOrEmpty(BannerAdUnit()))
                {
                    InitializeBannerAds();
                }
                if (!string.IsNullOrEmpty(IntersitialAdUnit()))
                {
                    InitializeInterstitialAds();
                }
                if (!string.IsNullOrEmpty(RewardedAdUnit()))
                {
                    InitializeRewardedAds();
                }
                if (!string.IsNullOrEmpty(AppOpenAdUnit()))
                {
                    InitializeAppOpenAds();
                }
            };

            MaxSdk.SetSdkKey(GetSDKKey());
            MaxSdk.InitializeSdk();
#else
            isInitialize = true;
            OnConsentCompleted?.Invoke();
#endif
        }


        #region Banner
        private int bannerRetry = 0;
        public void InitializeBannerAds()
        {
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.IsTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(BannerAdUnit(), _bannerPosition);

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(BannerAdUnit(), Color.clear);

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
        }

        public void ShowBanner()
        {
#if NO_ADS
            return;
#endif
            if (IsRemovedAds)
                return;
            MaxSdk.ShowBanner(BannerAdUnit());
        }

        public void HideBanner()
        {
            MaxSdk.HideBanner(BannerAdUnit());
        }

        private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
        }

        private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.Log("AdsManager: OnBannerAdLoadFailedEvent" + errorInfo.Message);
            bannerRetry++;
            //OnLoadBannerAdsFail?.Invoke();
            float retryDelay = Mathf.Pow(2, Mathf.Min(3, bannerRetry));
            Invoke("InitializeBannerAds", (float)retryDelay);
        }

        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("AdsManager: OnBannerAdClickedEvent");
        }

        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (loggerImpressionMax != null)
                loggerImpressionMax.AdRevenuePaidEvent(adInfo);
        }

        private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("AdsManager: OnBannerAdExpandedEvent");
        }

        private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("AdsManager: OnBannerAdCollapsedEvent");
        }

#endregion
#region Interstitial
        private int interstitialRetryAttempt;

        public void InitializeInterstitialAds()
        {
            // Attach callback
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;

            // Load the first interstitial
            LoadInterstitial();
        }

        private void OnInterstitialAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            if (loggerImpressionMax != null)
                loggerImpressionMax.AdRevenuePaidEvent(arg2);
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

            // Reset retry attempt
            interstitialRetryAttempt = 0;
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            if (_loggersIntersitial != null)
            {
                foreach (var logger in _loggersIntersitial)
                {
                    logger.LogEventInterLoadFailed(errorInfo);
                }
            }
            // Interstitial ad failed to load 
            Debug.Log("Adsmanager: OnInterstitialLoadFailedEvent" + errorInfo.Message);

            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
            interstitialRetryAttempt++;
            float retryDelay = Mathf.Pow(2, Mathf.Min(6, interstitialRetryAttempt));
            Invoke("LoadInterstitial", (float)retryDelay);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _OnInterstitialAction?.Invoke(true);
            if (_loggersIntersitial != null)
            {
                foreach (var logger in _loggersIntersitial)
                {
                    logger.LogEventIntersDisplayed(adInfo);
                }
            }
            _isAdShowing = true;
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            Debug.Log("Adsmanager: OnInterstitialAdFailedToDisplayEvent" + errorInfo.Message);
            _OnInterstitialAction?.Invoke(false);
            LoadInterstitial();
            _isAdShowing = false;
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad.
            if (_loggersIntersitial != null)
            {
                foreach (var logger in _loggersIntersitial)
                {
                    logger.LogEventInterHidden(adInfo);
                }
            }
            LoadInterstitial();
            _isAdShowing = false;
        }

        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(IntersitialAdUnit());
        }

        public bool IsInterstitialReady()
        {
            if (MaxSdk.IsInterstitialReady(IntersitialAdUnit()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ShowInterstitial(Action<bool> action)
        {
            _OnInterstitialAction = action;
#if NO_ADS
            _OnInterstitialAction?.Invoke(true);
            return;
#endif
            if (IsRemovedAds)
            {
                _OnInterstitialAction?.Invoke(false);
                return;
            }

            if (!IsInterstitialReady())
            {
                _OnInterstitialAction?.Invoke(false);
                return;
            }
            if (_loggersIntersitial != null)
            {
                foreach (var logger in _loggersIntersitial)
                {
                    logger.LogEventIntersShow();
                }
            }
            _ShowInterstitial();
            LoadInterstitial();
            _isAdShowing = true;
        }

        private void _ShowInterstitial()
        {
            if (isInitialize && IsInterstitialReady())
            {
                MaxSdk.ShowInterstitial(IntersitialAdUnit());
            }
        }
#endregion
#region AppOpenAds
        int appOpenRetryAttempt;
        private void InitializeAppOpenAds()
        {
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenAdLoadFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAppOpenDisplayFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAppOpenRevenuePaiedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenAdDisplayedEvent;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenAdLoadedEvent;

            MaxSdk.LoadAppOpenAd(AppOpenAdUnit());
        }

        private void LoadAppOpenAd()
        {
            MaxSdk.LoadAppOpenAd(AppOpenAdUnit());
        }

        private void OnAppOpenAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            appOpenRetryAttempt = 0;
        }

        private void OnAppOpenAdDisplayedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            _OnAppOpenAdAction?.Invoke(true);
        }

        private void OnAppOpenRevenuePaiedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            if (loggerImpressionMax != null)
                loggerImpressionMax.AdRevenuePaidEvent(arg2);
        }

        private void OnAppOpenDisplayFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2, MaxSdkBase.AdInfo arg3)
        {
            _OnAppOpenAdAction?.Invoke(false);
        }

        private void OnAppOpenAdLoadFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2)
        {
            appOpenRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(3, appOpenRetryAttempt));
            Invoke("LoadAppOpenAd", (float)retryDelay);
        }

        private void OnAppOpenDismissedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            LoadAppOpenAd();
        }

        private bool IsAppOpenReady()
        {
            if (MaxSdk.IsAppOpenAdReady(AppOpenAdUnit()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void _ShowAppOpenAd()
        {
            if (isInitialize && MaxSdk.IsAppOpenAdReady(AppOpenAdUnit()))
            {
                MaxSdk.ShowAppOpenAd(AppOpenAdUnit());
            }
        }


        public void ShowAppOpenAd(Action<bool> action)
        {
            _OnAppOpenAdAction = action;
#if NO_ADS
            _OnAppOpenAdAction?.Invoke(true);
            return;
#endif
            if (IsRemovedAds)
            {
                _OnAppOpenAdAction?.Invoke(true);
                return;
            }

            if (!IsAppOpenReady())
            {
                _OnAppOpenAdAction?.Invoke(false);
                return;
            }
            _ShowAppOpenAd();
            LoadAppOpenAd();
        }
#endregion
#region Rewarded Ads
        int rewardedRetryAttempt;

        public void InitializeRewardedAds()
        {
            // Attach callback
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            // Load the first rewarded ad
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(RewardedAdUnit());
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

            // Reset retry attempt
            rewardedRetryAttempt = 0;
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

            rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));
            Invoke("LoadRewardedAd", (float)retryDelay);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (_loggersRewardeds != null)
            {
                foreach (var logger in _loggersRewardeds)
                {
                    logger.LogEventRewardedDisplayed(adInfo);
                }
            }
            _isAdShowing = true;
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            LoadRewardedAd();
            _OnRewardsAction?.Invoke(false);
            _isAdShowing = false;
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            _OnRewardsHiddenAction?.Invoke();
            LoadRewardedAd();
            _isAdShowing = false;
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
            _OnRewardsAction?.Invoke(true);
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
            if (loggerImpressionMax != null)
                loggerImpressionMax.AdRevenuePaidEvent(adInfo);
        }

        public bool IsRewardAdsAvailible()
        {
            if (MaxSdk.IsRewardedAdReady(RewardedAdUnit()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void _ShowRewardedAds()
        {
            if (isInitialize && MaxSdk.IsRewardedAdReady(RewardedAdUnit()))
            {
                MaxSdk.ShowRewardedAd(RewardedAdUnit());
            }
        }

        public void ShowRewardedAds(Action<bool> action, Action actionHiddenAds = null)
        {
            _OnRewardsAction = action;
            _OnRewardsHiddenAction = actionHiddenAds;
#if NO_ADS
            _OnRewardsAction?.Invoke(true);
            return;
#endif
            if (!IsRewardAdsAvailible())
            {
                _OnRewardsAction?.Invoke(false);
            }
            if (_loggersRewardeds != null)
            {
                foreach (var logger in _loggersRewardeds)
                {
                    logger.LogEventRewardedShow();
                }
            }
            _isAdShowing = true;
            _ShowRewardedAds();
            LoadRewardedAd();
        }
#endregion
#region ISaveable
        [Serializable]
        public class SaveData
        {
            public bool isRemovedAds;
        }

        public string SaveKey => "AdsManagerMax";

        public bool DataChanged { get; set; }

        public object GetData() => _saveData;

        public void SetData(string data)
        {
            if (string.IsNullOrEmpty(data))
                _saveData = new SaveData();
            else
                _saveData = JsonUtility.FromJson<SaveData>(data);
        }

        public void OnAllDataLoaded() { }

        public void DefaultData()
        {
            _saveData = new SaveData();
            IsRemovedAds = false;
        }
#endregion
    }
}

