using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public class LoadHangoutEvent : Event<string>
    { 
        public const string EVENT_NAME = "LoadHangoutEvent";

        public LoadHangoutEvent(bool hasMinigameMatchmakingFailed = false) : base(EVENT_NAME)
        {
            HasMinigameMatchmakingFailed = hasMinigameMatchmakingFailed;
        }

        public bool HasMinigameMatchmakingFailed { get; }
    }
}
