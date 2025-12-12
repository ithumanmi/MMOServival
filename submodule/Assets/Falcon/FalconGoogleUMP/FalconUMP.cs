using System;
using System.Collections;
using Falcon.FalconCore.Scripts;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

namespace Falcon.FalconGoogleUMP
{
    public class FalconUMP
    {
        private const string TCF_TL_CONSENT = "IABTCF_AddtlConsent";
        private const string IS_CONSENT_VALUE = "2878";
        private const string RESET_CMP = "f_reset_cmp";
        
        private enum ConfigResult
        {
            Default = 0, //Không show consent form
            MediationWithoutCmp = 1,
            MediationWithCmp = 2
        }

        private static int _configResult = 0;

        private static Action<bool> _onSetIronSourceConsent;
        private static Action _onInitializeAdmob;
        private static Action _onShowPopupATT;
        private static Action<bool> _callback;
        private static bool _onRemoteConfig;
        public static bool isInitAdmob = false;

        public static void ShowConsentForm(Action<bool> onSetIronSourceConsent, Action onInitializeAdmob, Action onShowPopupATT, Action<bool> callback)
        {
            _onSetIronSourceConsent = onSetIronSourceConsent;
            _onInitializeAdmob = onInitializeAdmob;
            _onShowPopupATT = onShowPopupATT;
            _callback = callback;
            AfterGetRemoteConfig();
        }

        public static IEnumerator CoWaitRemoteConfig()
        {
            FalconConfig.OnUpdateFromNet += (_, _) => { _onRemoteConfig = true; };
            float waitTime = 5f;
            while (!_onRemoteConfig && waitTime > 0)
            {
                waitTime -= Time.unscaledDeltaTime;
                yield return null;
            }

            _configResult = FalconConfig.Instance<FalconUMPConfig>().f_ump_consent;
            Debug.LogError("FalconUMP _configResult: " + _configResult);
            var resetConfig = FalconConfig.Instance<FalconUMPConfig>().f_reset_cmp;
            if (resetConfig && !PlayerPrefs.HasKey(RESET_CMP))
            {
                PlayerPrefs.SetInt(RESET_CMP, 1);
                ConsentInformation.Reset();
            }
            Debug.LogError("FalconUMP > CoWaitRemoteConfig: " + _configResult);
            yield return null;
        }

        private static void AfterGetRemoteConfig()
        {
            Debug.LogError("FalconUMP > AfterGetRemoteConfig: " + _configResult);
            
            if (_configResult == (int)ConfigResult.Default || _configResult < (int)ConfigResult.Default || _configResult > (int)ConfigResult.MediationWithCmp)
            {
                _onSetIronSourceConsent?.Invoke(true);
                _callback?.Invoke(true);
                _onInitializeAdmob?.Invoke();
                _onShowPopupATT?.Invoke();
                return;
            }

            if (_configResult == (int)ConfigResult.MediationWithoutCmp)
            {
                _onSetIronSourceConsent?.Invoke(true);
            }

            //Config = 2: làm đúng luật.
            //Show popup CMP, sau khi nhận giá trị consent trả về thì khởi tạo GMA
            //Set consent cho IronSource tùy thuộc vào giá trị lấy được từ CMP -> Khởi tạo IronSource

            // Set tag for under age of consent.
            // Here false means users are not under age of consent.
            ConsentRequestParameters request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
            };

            // Check the current consent information status.
            Debug.LogError("FalconUMP > AfterGetRemoteConfig: ConsentInformation.ConsentStatus" +
                           ConsentInformation.ConsentStatus);
            ConsentInformation.Update(request, OnConsentInfoUpdated);
        }

        private static void OnConsentInfoUpdated(FormError consentError)
        {
            Debug.LogError("FalconUMP > OnConsentInfoUpdated === " + _configResult);
            if (consentError != null)
            {
                // Handle the error.
                Debug.LogError("FalconUMP > OnConsentInfoUpdated === consentError: " + consentError);

                if (_configResult == (int)ConfigResult.MediationWithCmp)
                {
                    CallIronSourceSetConsentEvent();
                    _onShowPopupATT?.Invoke();
                }

                return;
            }

            // If the error is null, the consent information state was updated.
            // You are now ready to check if a form is available.
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
            {
                Debug.LogError("FalconUMP Show and gather");
                if (_configResult == (int)ConfigResult.MediationWithCmp)
                {
                    CallIronSourceSetConsentEvent();
                    _onShowPopupATT?.Invoke();
                }

                if (_configResult == (int)ConfigResult.MediationWithoutCmp)
                {
                    _callback?.Invoke(true);
                }

                if (formError != null)
                {
                    // Consent gathering failed.
                    Debug.LogError("FalconUMP > OnConsentInfoUpdated === LoadAndShowConsentFormIfRequired FormError: " + formError);
                    return;
                }

                // Consent has been gathered.
                if (ConsentInformation.CanRequestAds())
                {
                    _onInitializeAdmob?.Invoke();
                }
            });
        }

        /// <summary>
        /// Check if it's necessary to show the Privacy Options Form
        /// </summary>
        public static bool RequirePrivacyOptionsForm => ConsentInformation.PrivacyOptionsRequirementStatus ==
                                                        PrivacyOptionsRequirementStatus.Required;

        /// <summary>
        /// Show Privacy Options form if required
        /// </summary>
        public static void ShowPrivacyOptionsForm()
        {
            ConsentForm.ShowPrivacyOptionsForm((FormError formError) =>
            {
                if (formError != null)
                {
                    Debug.LogError("FalconUMP > ShowPrivacyOptionsForm === Error: " + formError);
                }
            });
        }

        private static void CallIronSourceSetConsentEvent()
        {
            string tcf = PlayerPrefs.GetString(TCF_TL_CONSENT, string.Empty);
            _onSetIronSourceConsent?.Invoke(tcf.Contains(IS_CONSENT_VALUE));
            _callback?.Invoke(tcf.Contains(IS_CONSENT_VALUE));
        }
    }
}