using Hawky.EventObserver;

namespace Hawky.Loading
{
    public partial class LoadingControllerCreatorId
    {
        public const string DEFAULT = "DEFAULT";
    }

    public partial class LoadingHandlerId
    {
        public const string DEFAULT = "DEFAULT";
    }

    public partial class LoadingControllerId
    {
        public const string DEFAULT = "DEFAULT";
    }

    public class LoadingEventBase : EventBase
    {
        public string loadingId;
        public LoadingEventBase(string loadingId)
        {
            this.loadingId = loadingId;
        }
    }

    public class LoadingBeginEvent : LoadingEventBase
    {
        public string text;
        public string totalPointFirstSession;
        public string totalPoint;

        public LoadingBeginEvent(string loadingId, string text, string totalPointFirstSession, string totalPoint) : base(loadingId)
        {
            this.text = text;
            this.totalPointFirstSession = totalPointFirstSession;
            this.totalPoint = totalPoint;
        }
    }

    public class LoadingUpdateEvent : LoadingEventBase
    {
        public string text;
        public int totalPoint;
        public int currentPoint;

        public int totalPointInSession;
        public int currentPointInSession;

        public LoadingUpdateEvent(string loadingId, string text, int totalPoint, int currentPoint, int totalPointInSession, int currentPointInSession) : base(loadingId)
        {
            this.text = text;
            this.totalPoint = totalPoint;
            this.currentPoint = currentPoint;
            this.totalPointInSession = totalPointInSession;
            this.currentPointInSession = currentPointInSession;
        }
    }

    public class LoadingEndEvent : LoadingEventBase
    {

        public LoadingEndEvent(string loadingId) : base(loadingId)
        {

        }
    }
}

namespace Hawky.EventObserver
{
    public partial class EventName
    {
        public const string LOADING_BEGIN = "LOADING_BEGIN";
        public const string LOADING_UPDATE = "LOADING_UPDATE";
        public const string LOADING_END = "LOADING_END";
    }
}