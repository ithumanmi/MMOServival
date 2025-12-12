namespace Hawky.Inventory
{
    public partial class ItemType
    {
        public const string CUR = "CUR";
    }

    public partial class ItemId
    {
        public const string NOADS = "NOADS";
    }

    public partial class AddItemPosition
    {
        public const string DEFAULT = "Unknown";
        public const string Shop = "Shop";
    }

    public partial class UseItemPosition
    {
        public const string DEFAULT = "Unknown";
    }

    public partial class UseItemNewReward
    {
        public const string DEFAULT = "Unknown";
    }
}

namespace Hawky.EventObserver
{
    public partial class EventName
    {
        public const string EARN_VIRTUAL_CURRENCY = "EARN_VIRTUAL_CURRENCY";
        public const string SPEND_VIRTUAL_CURRENCY = "SPEND_VIRTUAL_CURRENCY";
    }
}