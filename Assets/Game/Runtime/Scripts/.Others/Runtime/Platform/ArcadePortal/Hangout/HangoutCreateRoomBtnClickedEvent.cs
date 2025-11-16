using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public class HangoutCreateRoomBtnClickedEvent : Event<string>
    {
        public const string EVENT_NAME = "HangoutCreateRoomBtnClickedEvent";

        public HangoutCreateRoomBtnClickedEvent(RoomDetails roomDetails, LevelConfigScriptableObject levelConfig) : base(EVENT_NAME)
        {
            RoomDetails = roomDetails;
            LevelConfig = levelConfig;
        }

        public RoomDetails RoomDetails { get; }
        public LevelConfigScriptableObject LevelConfig { get; }
    }
}
