using Hawky.EventObserver;

namespace Hawky.EventObserver
{
    public partial class EventName
    {
        public const string GAMEFLOW_COMPLETED_STAGE = "GAMEFLOW_COMPLETED_STAGE";
        public const string GAMEFLOW_START_STAGE = "GAMEFLOW_START_STAGE";

        public const string GAMEFLOW_START_TUTORIAL = "GAMEFLOW_START_TUTORIAL";
        public const string GAMEFLOW_END_TUTORIAL = "GAMEFLOW_END_TUTORIAL";

        public const string GAMEFLOW_UNLOCKED = "GAMEFLOW_UNLOCKED";
    }
}


namespace Hawky.GameFlow
{
    public partial class GameFlowCompletedStageEvent : EventBase
    {
        public int stage;
        public long score;
        public bool isWin;
        public float duration;
    }

    public partial class GameFlowStartStageEvent : EventBase
    {
        public int stage;
    }

    public partial class GameFlowStartTutorialEvent : EventBase
    {
        public string tutorialId;
    }

    public partial class GameFlowEndTutorialEvent : EventBase
    {
        public string tutorialId;
        public bool success;
    }

    public partial class GameFlowUnlockedEvent : EventBase
    {
        public string contentId;
        public int currentLevel;
    }
}