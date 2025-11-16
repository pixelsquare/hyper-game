using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public class HangoutJoinRoomBtnClickedEvent : Event<string>
    { 
        public const string EVENT_NAME = "HangoutRoomJoinedEvent";

        public HangoutJoinRoomBtnClickedEvent(RoomDetails roomDetails, string previewIconAddress, LevelConfigScriptableObject levelConfig) : base(EVENT_NAME)
        {
            RoomDetails = roomDetails;
            PreviewIconAddress = previewIconAddress;
            LevelConfig = levelConfig;
        }

        public RoomDetails RoomDetails { get; private set; }
        public string PreviewIconAddress { get; private set; }

        public LevelConfigScriptableObject LevelConfig { get; private set; }
    }
}
