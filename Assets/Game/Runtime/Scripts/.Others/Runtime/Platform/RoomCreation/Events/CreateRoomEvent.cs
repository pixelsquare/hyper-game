using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public class CreateRoomEvent : Event<string>
    { 
        public const string EVENT_NAME = "CreateRoomEvent";

        public CreateRoomEvent() : base(EVENT_NAME)
        {
        }
    }
}
