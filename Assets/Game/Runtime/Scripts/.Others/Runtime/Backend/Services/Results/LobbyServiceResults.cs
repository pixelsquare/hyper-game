using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class GetLobbyConfigResult : ResultBase
    {
        public LobbyConfig[] lobbyConfigs;
    }

    [Serializable]
    public struct LobbyConfig
    {
        public string id;
        public string name;
    }
}
