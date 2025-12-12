#if ANTADA_FALCON

namespace Hawki_Antada_Falcon.FirebaseEvent
{
    public partial class AnalyticEventName
    {
        public const string STAGE_PASSED = "stage_passed_{0}";
        public const string LEVEL_START = "level_start";
        public const string LEVEL_COMPLETED = "level_complete";
        public const string LEVEL_FAIL = "level_fail";
        public const string EARN_VIRTUAL_CURRENCY = "earn_virtual_currency";
        public const string SPEND_VIRTUAL_CURRENCY = "spend_virtual_currency";

        public const string ADS_REWARD_LOAD = "ads_reward_load";
        public const string ADS_REWARD_CLICK = "ads_reward_click";
        public const string ADS_REWARD_SHOW_SUCCESS = "ads_reward_show_success";
        public const string ADS_REWARD_SHOW_FAIL = "ads_reward_show_fail";
        public const string ADS_REWARD_COMPLETE = "ads_reward_complete";

        public const string ADS_INTER_LOAD_SUCCESS = "ad_inter_load_success";
        public const string ADS_INTER_LOAD_FAIL = "ad_inter_load_fail";
        public const string ADS_INTER_SHOW = "ad_inter_show";
        public const string ADS_INTER_CLICK = "ad_inter_click";
    }
}

#endif
