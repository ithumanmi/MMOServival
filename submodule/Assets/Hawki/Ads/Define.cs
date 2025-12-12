using System;

namespace Hawki.Ads
{
    public class ShowInterstitialRequest
    {
        public string position = ShowInterstitialPositionId.DEFAULT;
        public string placementId;
    }

    public class ShowRewardRequest
    {
        public string position = ShowInterstitialPositionId.DEFAULT;
        public string placementId;
    }

    public class ShowBannerRequest
    {
        public bool value;
    }

    public partial class ShowInterstitialPositionId
    {
        public const string DEFAULT = "Unknown";
    }

    public partial class ShowRewardPositionId
    {
        public const string DEFAULT = "Unknown";
    }

    public interface IShowInterstitialAdsHandler
    {
        void ShowInterstitial(ShowInterstitialRequest request, Action<ShowAdsResult> callBack);
        float Interval();
    }

    public interface IShowRewardAdsHandler
    {
        void ShowReward(ShowRewardRequest request, Action<ShowAdsResult> callBack); 
    }


    public interface IShowBannerAdsHanlder
    {
        void ShowBanner(ShowBannerRequest request, Action<bool> callback);
    }

    public class AdsResultId
    {
        public const int RESULT_INPROGRESS = 0;
        public const int RESULT_OK = 1;
        public const int RESULT_BACK = 2;
    }

    public interface IBannerRegister
    {

    }

    public interface ILockBannerRegister
    {

    }
}