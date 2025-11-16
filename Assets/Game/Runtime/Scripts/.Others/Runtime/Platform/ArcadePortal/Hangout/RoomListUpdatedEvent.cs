using System.Collections.Generic;
using Photon.Realtime;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public class RoomListUpdatedEvent : Event<string>
    {
        public const string EVENT_NAME = "RoomListUpdatedEvent";

        public RoomListUpdatedEvent(Dictionary<string, RoomInfo> roomList) : base(EVENT_NAME)
        {
            RoomList = roomList;
        }

        public Dictionary<string, RoomInfo> RoomList { get; }
    }
}
