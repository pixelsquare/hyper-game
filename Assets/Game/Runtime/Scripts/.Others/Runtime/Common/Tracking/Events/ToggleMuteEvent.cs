using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public class ToggleMuteEvent : Event<string>
    {
        public const string EVENT_ID = "MuteToggle";
        
        public ToggleMuteEvent(bool isMuted) : base(EVENT_ID)
        {
            IsMuted = isMuted;
        }
        
        public bool IsMuted { get; }
    }
}
