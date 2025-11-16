using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public class HangoutJoinRoomFailedEvent : Event<string>
    { 
        public const string EVENT_NAME = "HangoutJoinRoomFailedEvent";

        public HangoutJoinRoomFailedEvent() : base(EVENT_NAME)
        {
        }
    }
}
