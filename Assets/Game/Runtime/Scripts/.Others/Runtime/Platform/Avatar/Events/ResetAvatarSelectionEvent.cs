using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public class ResetAvatarSelectionEvent : Event<string>
    {
        public const string EVENT_NAME = "ResetAvatarSelectionEvent";

        public ResetAvatarSelectionEvent() : base(EVENT_NAME)
        {
        }
    }
}
