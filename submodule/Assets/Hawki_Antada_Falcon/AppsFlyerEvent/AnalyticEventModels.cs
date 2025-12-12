#if ANTADA_FALCON

using System.Collections.Generic;
using System.Reflection;

namespace Hawki_Antada_Falcon.AppsFlyerEvent
{
    public class BaseAnalyticEvent
    {
        protected string eventName;
        public string EventName => this.eventName;
        public virtual Dictionary<string, string> GetParams()
        {
            var dict = new Dictionary<string, string>();
            var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (var i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name == nameof(eventName) || fields[i].Name == nameof(EventName))
                {
                    continue;
                }

                dict.Add(ToCamelCase(fields[i].Name), fields[i].GetValue(this).ToString());
            }

            return dict;
        }

        private string ToCamelCase(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }
    }

    public class CustomAnalyticEvent : BaseAnalyticEvent
    {
        public CustomAnalyticEvent(string eventName)
        {
            this.eventName = eventName;
        }
    }

    public class TutorialCompletionAnalyticEvent : BaseAnalyticEvent
    {
        public string af_success;
        public string af_tutorial_id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="af_success">whether the user completed the tutorial</param>
        /// <param name="af_tutorial_id">tutorial ID</param>
        public TutorialCompletionAnalyticEvent(string af_success, string af_tutorial_id)
        {
            this.eventName = AnalyticEventName.AF_TUTORIAL_COMPLETION;

            this.af_success = af_success;
            this.af_tutorial_id = af_tutorial_id;
        }
    }

    public class LevelAchievedAnalyticEvent : BaseAnalyticEvent
    {
        public string af_level;
        public string af_score;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="af_level">level the user achieved</param>
        /// <param name="af_score">score associated with user's achievement</param>
        public LevelAchievedAnalyticEvent(string af_level, string af_score)
        {
            this.eventName = AnalyticEventName.AF_LEVEL_ACHIEVED;

            this.af_level = af_level;
            this.af_score = af_score;
        }
    }

    public class AchievementUnlockedAnalyticEvent : BaseAnalyticEvent
    {
        public string content_id;
        public string af_level;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content_id">unlocked content ID (Challenge#3)</param>
        /// <param name="af_level">current level of user</param>
        public AchievementUnlockedAnalyticEvent(string content_id, string af_level)
        {
            this.eventName = AnalyticEventName.AF_ACHIEVEMENT_UNLOCKED;

            this.content_id = content_id;
            this.af_level = af_level;
        }
    }

    public class PurchaseAnalyticEvent : BaseAnalyticEvent
    {
        public string af_currency;
        public string af_content_id;
        public string af_revenue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="af_currency_code">localized currency code</param>
        /// <param name="af_content_id">product id on respective store</param>
        /// <param name="af_purchase_price">product price has been localized*0.63</param>
        public PurchaseAnalyticEvent(string af_currency, string af_content_id, string af_revenue)
        {
            this.eventName = AnalyticEventName.AF_PURCHASE;

            this.af_currency = af_currency;
            this.af_content_id = af_content_id;
            this.af_revenue = af_revenue;
        }
    }

    public class InterShowAnalyticEvent : BaseAnalyticEvent
    {
        public InterShowAnalyticEvent()
        {
            this.eventName = AnalyticEventName.AF_INTERS_SHOW;
        }
    }

    public class InterDisplayedAnalyticEvent : BaseAnalyticEvent
    {
        public InterDisplayedAnalyticEvent()
        {
            this.eventName = AnalyticEventName.AF_INTERS_DISPLAYED;
        }
    }

    public class RewardShowAnalyticEvent : BaseAnalyticEvent
    {
        public RewardShowAnalyticEvent()
        {
            this.eventName = AnalyticEventName.AF_REWARDED_SHOW;
        }
    }

    public class RewardDisplayedAnalyticEvent : BaseAnalyticEvent
    {
        public RewardDisplayedAnalyticEvent()
        {
            this.eventName = AnalyticEventName.AF_REWARDED_DISPLAYED;
        }
    }
}

#endif