using Kumu.Kulitan.Hangout;

namespace Kumu.Kulitan.Lobby
{
    public static class LobbyUtil
    {
        public static string WrapVersionLobbyId(string lobbyId)
        {
            return $"ube_{lobbyId}_{VersionObject.FetchVersionString()}";
        }
    }
}
