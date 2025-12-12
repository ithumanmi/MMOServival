using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Ump.Api;
using System;
using NFramework;

namespace Antada.Libs
{
    public class AdMobUMPManager
    {
        private const string IS_CONSENT_VALUE = "2878";
        private const string TCF_TL_CONSENT = "IABTCF_AddtlConsent";
        private static Action<bool> _onSetMediationConsent;
        private static Action _onInitializeAdmob;
        private static Action _onShowPopupATT;

        public static void ShowConsentForm(Action<bool> OnSetMediationConsent, Action OnInitializeAdmob, Action OnShowPopupATT,bool isConsentReset = false)
        {
            _onInitializeAdmob = OnInitializeAdmob;
            _onSetMediationConsent = OnSetMediationConsent;
            _onShowPopupATT = OnShowPopupATT;
            if (isConsentReset == true)
            {
                ResetConsent();
            }
            Request();
        }

        public static void ShowPrivacyOptionsForm()
        {
            ConsentForm.ShowPrivacyOptionsForm((FormError formError) =>
            {
                if (formError != null)
                {
                    Debug.LogError("UMP Form > ShowPrivacyOptionsForm === Error: " + formError);
                }
            });
        }

        /// <summary>
        /// Check if it's necessary to show the Privacy Options Form
        /// </summary>
        public static bool RequirePrivacyOptionsForm => ConsentInformation.PrivacyOptionsRequirementStatus ==
                                                        PrivacyOptionsRequirementStatus.Required;

        public static void ResetConsent()
        {
            ConsentInformation.Reset();
        }

        private static void Request()
        {
            // Set tag for under age of consent.
            // Here false means users are not under age of consent.
            ConsentRequestParameters request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
            };

            // Check the current consent information status.
            ConsentInformation.Update(request, OnConsentInfoUpdated);
        }


        private static void OnConsentInfoUpdated(FormError consentError)
        {
            if (consentError != null)
            {
                // Handle the error.
                Debug.LogError("UMP > OnConsentInfoUpdated === Error: " + consentError);
                _onSetMediationConsent?.Invoke(GetConsentValue());
                _onShowPopupATT?.Invoke();
                return;
            }

            // If the error is null, the consent information state was updated.
            // You are now ready to check if a form is available.
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
            {
                _onSetMediationConsent?.Invoke(GetConsentValue());
                _onShowPopupATT?.Invoke();
                if (formError != null)
                {
                    // Consent gathering failed.
                    Debug.LogError("UMP > OnConsentInfoUpdated === LoadAndShowConsentFormIfRequired FormError: " + formError);
                    return;
                }

                // Consent has been gathered.
                if (ConsentInformation.CanRequestAds())
                {
                    _onInitializeAdmob?.Invoke();
                }
            });

        }

        private static bool GetConsentValue()
        {
            string tcf = PlayerPrefs.GetString(TCF_TL_CONSENT, string.Empty);
            Debug.LogError("TCF_TL_CONSENT: " + tcf);
            return tcf.Contains(IS_CONSENT_VALUE);
        }
    }

}

