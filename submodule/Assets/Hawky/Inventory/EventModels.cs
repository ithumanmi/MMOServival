using Hawky.EventObserver;

namespace Hawky.Inventory
{
    public class EarnVirtualCurrencyEvent : EventBase
    {
        public string itemId;
        public float itemAmount;
        public float newAmount;
        public string position;

        public EarnVirtualCurrencyEvent(string itemId, float itemAmount, float newAmount, string position)
        {
            this.itemId = itemId;
            this.itemAmount = itemAmount;
            this.newAmount = newAmount;
            this.position = position;
        }
    }

    public class SpendVirtualCurrencyEvent : EventBase
    {
        public string itemId;
        public float itemAmount;
        public float newAmount;
        public string position;
        public string newReward;

        public SpendVirtualCurrencyEvent(string itemId, float itemAmount, float newAmount, string position, string newReward)
        {
            this.itemId = itemId;
            this.itemAmount = itemAmount;
            this.newAmount = newAmount;
            this.newReward = newReward;
            this.position = position;
        }
    }
}