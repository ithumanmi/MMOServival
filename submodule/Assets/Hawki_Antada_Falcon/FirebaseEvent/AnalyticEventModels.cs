#if ANTADA_FALCON

using Antada.Libs;

namespace Hawki_Antada_Falcon.FirebaseEvent
{
    public class SimpleAnalyticEvent : AnalyticEventBase
    {
        public SimpleAnalyticEvent(string eventName)
        {
            this.eventName = eventName;
        }
    }

    public class LevelStartAnalyticEvent : AnalyticEventBase
    {
        public int level;
        public string current_gold;

        public LevelStartAnalyticEvent(int level, string current_gold)
        {
            this.eventName = AnalyticEventName.LEVEL_START;

            this.level = level;
            this.current_gold = current_gold;
        }
    }

    public class LevelCompleteAnalyticEvent : AnalyticEventBase
    {
        public int level;
        public string timePlayed;

        public LevelCompleteAnalyticEvent(int level, string timePlayed)
        {
            this.eventName = AnalyticEventName.LEVEL_COMPLETED;

            this.level = level;
            this.timePlayed = timePlayed;
        }
    }

    public class LevelFailAnalyticEvent : AnalyticEventBase
    {
        public int level;
        public long failcount;

        public LevelFailAnalyticEvent(int level, long failcount)
        {
            this.eventName = AnalyticEventName.LEVEL_FAIL;

            this.level = level;
            this.failcount = failcount;
        }
    }

    public class EarnVirtualCurrencyAnalyticEvent : AnalyticEventBase
    {
        public string virtual_currency_name;
        public long value;
        public string source;

        public EarnVirtualCurrencyAnalyticEvent(string virtual_currency_name, long value, string source)
        {
            this.eventName = AnalyticEventName.EARN_VIRTUAL_CURRENCY;

            this.virtual_currency_name = virtual_currency_name;
            this.value = value;
            this.source = source;
        }
    }

    public class SpendVirtualCurrencyAnalyticEvent : AnalyticEventBase
    {
        public string virtual_currency_name;
        public long value;
        public string item_name;

        public SpendVirtualCurrencyAnalyticEvent(string virtual_currency_name, long value, string item_name)
        {
            this.eventName = AnalyticEventName.SPEND_VIRTUAL_CURRENCY;

            this.virtual_currency_name = virtual_currency_name;
            this.value = value;
            this.item_name = item_name;
        }
    }
}

#endif
