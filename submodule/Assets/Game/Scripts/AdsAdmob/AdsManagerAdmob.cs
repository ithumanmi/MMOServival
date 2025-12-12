using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using NFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antada.Libs
{
    public interface IAdsAdmobController
    {
        bool CanShowInteruptOpenAds();
        bool isRemovedAds();
        bool isAdShowing();
    }

    public interface IAdsUnitAppOpenAdsAdmobProvider
    {
        List<string> AdUnitIdAndroidOpenAd();
        List<string> AdUnitIdiOSOpenAd();
    }

    public interface IAdsUnitBannerAdmobProvider
    {
        string AdUnitIdAndroidBannerAd();
        string AdUnitIdiOSBannerAd();
    }

    public interface INativeAdsUnitAdmobProvider
    {
        List<string> AdUnitAndroidNativeAd();
        List<string> AdUnitiOSNativeAd();
    }

    public interface ILoggerOpenAd
    {
        void LogEventOpenAdLoadFail(LoadAdError error);
        void LogEventOpenAdLoadSuccess();
        void LogEventOpenAdClick();
        void LogEventOpenAdShow();
        void LogEventOnAdPaid(AdValue adValue);
    }

    public class AdsManagerAdmob : SingletonMono<AdsManagerAdmob>
    {
        public INativeAdsUnitAdmobProvider nativeAdDelegate;
        public IAdsUnitBannerAdmobProvider bannerAdDelegate;
        public IAdsUnitAppOpenAdsAdmobProvider appOpenAdDelegate;
        public IAdsAdmobController controllerDelegate;
        public List<ILoggerOpenAd> loggerOpenAds;
        public static Action<bool> OnIsRemovedAdsChanged;
        public static Action<bool> BannerAdLoadedStatus;
        [SerializeField] private bool _raiseAdEventsOnUnityMainThread;
        [SerializeField] private bool _iOSAppPauseOnBackground = true;
        private bool _isAdShowing;
        public static bool? _isInitialized;
        private int _tierOpenAdIndex = 0;
        private int _tierNativeAdIndex = 0;
        private NativeAd nativeAd;

        private string GetAppOpenAdUnit(int index)
        {
#if UNITY_ANDROID
            return appOpenAdDelegate == null ? "" : appOpenAdDelegate.AdUnitIdAndroidOpenAd()[index];
#elif UNITY_IOS
            return appOpenAdDelegate == null ? "" : appOpenAdDelegate.AdUnitIdiOSOpenAd()[index];
#else
            return "unused";
#endif
        }

        private string GetBannerAdUnit()
        {
#if UNITY_ANDROID
            return bannerAdDelegate == null ? "" : bannerAdDelegate.AdUnitIdAndroidBannerAd();
#elif UNITY_IOS
            return bannerAdDelegate == null ? "" : bannerAdDelegate.AdUnitIdiOSBannerAd();
#else
            return "unused";
#endif
        }

        private string GetNativeAdUnit(int index)
        {
#if UNITY_ANDROID
            return nativeAdDelegate == null ? "" : nativeAdDelegate.AdUnitAndroidNativeAd()[index];
#elif UNITY_IOS
            return nativeAdDelegate == null ? "" : nativeAdDelegate.AdUnitiOSNativeAd()[index];
#else
            return "unused";
#endif
        }

        private AppOpenAd _appOpenAd;
        private DateTime _expireTime;
        private int _interstitialAdsCount;

        public bool IsAppOpenAvailable
        {
            get
            {
                return _appOpenAd != null
                       && _appOpenAd.CanShowAd()
                       && DateTime.Now < _expireTime;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
            MobileAds.RaiseAdEventsOnUnityMainThread = _raiseAdEventsOnUnityMainThread;
            MobileAds.SetiOSAppPauseOnBackground(_iOSAppPauseOnBackground);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        }

        public void Init(Action<bool> initstatus = null)
        {
            InitializeGoogleMobileAds(initstatus);
        }

        private void InitializeGoogleMobileAds(Action<bool> initAction = null)
        {
            // The Google Mobile Ads Unity plugin needs to be run only once and before loading any ads.
            if (_isInitialized.HasValue)
            {
                return;
            }


            _isInitialized = false;

            // Initialize the Google Mobile Ads Unity plugin.
            Debug.Log("Google Mobile Ads Initializing.");
            MobileAds.Initialize((InitializationStatus initstatus) =>
            {
                if (initstatus == null)
                {
                    Debug.LogError("Google Mobile Ads initialization failed.");
                    _isInitialized = null;
                    initAction?.Invoke(false);
                    return;
                }
                initAction?.Invoke(true);
                Debug.Log("Google Mobile Ads initialization complete.");
                _isInitialized = true;
            });
        }

        public void AddILoggerOpenAd(ILoggerOpenAd logger)
        {
            if (loggerOpenAds == null)
            {
                loggerOpenAds = new List<ILoggerOpenAd>();
            }

            loggerOpenAds.Add(logger);
        }

        #region AppOpenAd
        public void LoadAppOpenAd()
        {
            // Clean up the old ad before loading a new one.
            if (_appOpenAd != null)
            {
                _appOpenAd.Destroy();
                _appOpenAd = null;
            }

            Debug.Log("Loading the app open ad.");

            string adunit = GetAppOpenAdUnit(_tierOpenAdIndex);
            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            AppOpenAd.Load(adunit, adRequest,
                (AppOpenAd ad, LoadAdError error) =>
                {
                    // If the operation failed, an error is returned.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("App open ad failed to load an ad with error : " + error);
                        if (loggerOpenAds != null)
                        {
                            foreach (var logger in loggerOpenAds)
                            {
                                logger.LogEventOpenAdLoadFail(error);
                            }
                        }
                        return;
                    }

                    // If the operation completed successfully, no error is returned.
                    Debug.Log("App open ad loaded with response : " + ad.GetResponseInfo());
                    if (loggerOpenAds != null)
                    {
                        foreach (var logger in loggerOpenAds)
                        {
                            logger.LogEventOpenAdLoadSuccess();
                        }
                    }

                    // App open ads can be preloaded for up to 4 hours.
                    _expireTime = DateTime.Now + TimeSpan.FromHours(4);
                    _appOpenAd = ad;
                    _tierOpenAdIndex = 0;
                    RegisterEventHandlers(ad);
                });
        }
        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("App open ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
                if (loggerOpenAds != null)
                {
                    foreach (var logger in loggerOpenAds)
                    {
                        logger.LogEventOnAdPaid(adValue);
                    }
                }
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("App open ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("App open ad was clicked.");
                if (loggerOpenAds != null)
                {
                    foreach (var logger in loggerOpenAds)
                    {
                        logger.LogEventOpenAdClick();
                    }
                }
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                if (loggerOpenAds != null)
                {
                    foreach (var logger in loggerOpenAds)
                    {
                        logger.LogEventOpenAdShow();
                    }
                }
            };
            // Raised when the ad closed full screen content.
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("App open ad full screen content closed.");

                // Reload the ad so that we can show another as soon as possible.
                LoadAppOpenAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("App open ad failed to open full screen content " +
                               "with error : " + error);


                // Reload the ad so that we can show another as soon as possible.
                LoadAppOpenAd();
            };
        }

        public void ShowAppOpenAd()
        {
#if UNITY_EDITOR || NO_ADS
            return;
#endif
            if (controllerDelegate.isRemovedAds())
                return;

            if (_appOpenAd != null && _appOpenAd.CanShowAd())
            {
                Debug.Log("Showing app open ad.");
                _appOpenAd.Show();
            }
            else
            {
                LoadAppOpenAd();
                Debug.LogError("App open ad is not ready yet.");
            }
        }
        private void OnAppStateChanged(AppState state)
        {
            Debug.Log("App State changed to : " + state);

            // if the app is Foregrounded and the ad is available, show it.
            if (state == AppState.Foreground)
            {
                if (IsAppOpenAvailable && !controllerDelegate.isRemovedAds() && !controllerDelegate.isAdShowing())
                {
#if UNITY_EDITOR || NO_ADS
                    return;
#endif
                    if (!controllerDelegate.CanShowInteruptOpenAds())
                        return;
                    ShowAppOpenAd();
                }
            }
        }
 #endregion

        #region Native Ads

        public void LoadNativeAd()
        {
            string adunit = GetNativeAdUnit(_tierNativeAdIndex);
            AdLoader adLoader = new AdLoader.Builder(adunit)
                .ForNativeAd()
                .Build();
            adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
            adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
            adLoader.LoadAd(new AdRequest());
        }

        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.Log("Native ad loaded falied: Retrying...");
            LoadNativeAd();
        }

        private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
        {
            Debug.Log("Native ad loaded.");
            this.nativeAd = args.nativeAd;
        }


        public NativeAd GetNativeAd()
        {
            return this.nativeAd;
        }

        #endregion
        #region Banner Admob Ad
        private BannerView _bannerView;
        private void CreateBannerView(AdPosition adPosition, AdSize.Type type)
        {
            Debug.Log("Creating banner view.");

            // If we already have a banner, destroy the old one.
            if (_bannerView != null)
            {
                DestroyAd();
            }
            AdSize adSize;
            if (type == AdSize.Type.AnchoredAdaptive)
            {
                adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            }
            else
            {
                adSize = AdSize.Banner;
            }
                
            // Create a 320x50 banner at top of the screen.
            _bannerView = new BannerView(GetBannerAdUnit(), adSize, adPosition);
            

            // Listen to events the banner may raise.
            ListenToAdEvents();

            Debug.Log("Banner view created.");
        }
        public void LoadBannerAdMob(Dictionary<string, string> extras = null, AdPosition adPosition = AdPosition.Bottom, AdSize.Type adType = AdSize.Type.Standard)
        {
            if (controllerDelegate.isRemovedAds())
                return;

            // Create an instance of a banner view first.
            if (_bannerView == null)
            {
                CreateBannerView(adPosition, adType);
            }

            // Create our request used to load the ad.
            var adRequest = new AdRequest();
            if (extras != null)
            {
                foreach (var extra in extras)
                {
                    adRequest.Extras.Add(extra.Key, extra.Value);
                }
            }

            // Send the request to load the ad.
            Debug.Log("Loading banner ad.");
            _bannerView.LoadAd(adRequest);
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowBannerAdMob()
        {
#if UNITY_EDITOR || NO_ADS
            return;
#endif
            if (controllerDelegate.isRemovedAds())
                return;

            if (_bannerView != null)
            {
                Debug.Log("Showing banner view.");
                _bannerView.Show();
            }
        }

        /// <summary>
        /// Hides the ad.
        /// </summary>
        public void HideBannerAdMob()
        {
            if (_bannerView != null)
            {
                Debug.Log("Hiding banner view.");
                _bannerView.Hide();
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// When you are finished with a BannerView, make sure to call
        /// the Destroy() method before dropping your reference to it.
        /// </summary>
        public void DestroyAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("Destroying banner view.");
                _bannerView.Destroy();
                _bannerView = null;
            }
        }

        /// <summary>
        /// Logs the ResponseInfo.
        /// </summary>
        public void LogResponseInfo()
        {
            if (_bannerView != null)
            {
                var responseInfo = _bannerView.GetResponseInfo();
                if (responseInfo != null)
                {
                    UnityEngine.Debug.Log(responseInfo);
                }
            }
        }

        /// <summary>
        /// Listen to events the banner may raise.
        /// </summary>
        private void ListenToAdEvents()
        {
            // Raised when an ad is loaded into the banner view.
            _bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("Banner view loaded an ad with response : "
                    + _bannerView.GetResponseInfo());
                BannerAdLoadedStatus?.Invoke(true);

            };
            // Raised when an ad fails to load into the banner view.
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogError("Banner view failed to load an ad with error : " + error);
                BannerAdLoadedStatus?.Invoke(false);
            };
            // Raised when the ad is estimated to have earned money.
            _bannerView.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Banner view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            _bannerView.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            _bannerView.OnAdClicked += () =>
            {
                Debug.Log("Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            _bannerView.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            _bannerView.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Banner view full screen content closed.");
            };
        }
        #endregion
    }
}

