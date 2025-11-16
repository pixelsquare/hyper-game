using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using Photon.Realtime;

namespace Kumu.Kulitan.Hangout
{
    public class RoomConnectionDetails : SingletonMonoBehaviour<RoomConnectionDetails>
    {
        public string roomName;
        public string sceneName;
        public LevelConfigScriptableObject levelConfig;
        public int maxPlayers;
        public string clientId;
        public TypedLobby myLobby;
        public EnterRoomParams enterRoomParams;
        public string lobbyLabel;
        public RoomExitMode roomExitMode;
        public bool hasPromptedRoomExit;
    }

    public enum RoomExitMode
    {
        Normal,
        OnHostLeft,
    }
}
