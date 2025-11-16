using Kumu.Kulitan.Events;
using Kumu.Kulitan.UI;

namespace Kumu.Kulitan.Multiplayer
{
    public class RoomLayoutSelectedEvent : Event<string>
    { 
        public const string EVENT_NAME = "RoomLayoutSelectedEvent";

        private RoomLayoutButton layoutButton;
        
        public RoomLayoutSelectedEvent(RoomLayoutButton btn) : base(EVENT_NAME)
        {
            layoutButton = btn;
        }

        public RoomLayoutButton LayoutButton => layoutButton;
    }
}
