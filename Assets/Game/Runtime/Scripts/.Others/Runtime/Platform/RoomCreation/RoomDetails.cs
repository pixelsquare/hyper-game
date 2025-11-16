using System;

namespace Kumu.Kulitan.Multiplayer
{
    [Serializable]
    public struct RoomDetails
    {
        public string roomId;
        public string roomName;
        public string layoutName;
        public string sceneName;
        public string previewIconAddress;
        public bool isFriendsOnly;
    }
}
