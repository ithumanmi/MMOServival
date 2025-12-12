using NFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Antada.Libs {
    public class GDPRUI : BaseUIView
    {
        [SerializeField] private Button _btnContinute;
        [SerializeField] private Button _btnNoThanks;
        [SerializeField] private Button _btnPrivacyPolicy;
        private void Awake()
        {
            _btnContinute.onClick.AddListener(OnContinuteClick);
            _btnNoThanks.onClick.AddListener(OnNoThanksClick);
            _btnPrivacyPolicy.onClick.AddListener(OnPrivacyPolicyClick);
        }

        private void OnNoThanksClick()
        {
            AdsManager.I.ConsentStatus = AdsManager.GDPRStatus.NO;
            CloseSelf(true);
            FirebaseManager.I.LogEventIgnoreAllNull(new EventGDPR()
            {
                gdpr_type = "no_thanks"
            });
        }

        private void OnContinuteClick()
        {
            AdsManager.I.ConsentStatus = AdsManager.GDPRStatus.YES;
            CloseSelf(true);
            FirebaseManager.I.LogEventIgnoreAllNull(new EventGDPR()
            {
                gdpr_type = "yes"
            });
        }

        private void OnPrivacyPolicyClick()
        {
            Application.OpenURL("https://falcongames.com.vn/policy/en/privacy-policy.html");
        }
    }


}
