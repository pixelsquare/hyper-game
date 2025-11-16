using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Lobby;
using Kumu.Kulitan.Multiplayer;
using Photon.Realtime;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitJoinLobby : Unit
    {
        [DoNotSerialize] public ControlInput enter;
        [DoNotSerialize] public ControlOutput exit;
        [DoNotSerialize] public ValueInput lobbyConfigs;
        
        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), JoinLobby);
            exit = ControlOutput(nameof(exit));
            lobbyConfigs = ValueInput<LobbyConfig[]>(nameof(lobbyConfigs));

            Succession(enter, exit);
        }

        private ControlOutput JoinLobby(Flow flow)
        {
            var lobby = RoomConnectionDetails.Instance.myLobby;

            if (lobby == null)
            {
                var lobbyConfigValue = flow.GetValue<LobbyConfig[]>(lobbyConfigs);
                var defaultLobbyConfig = lobbyConfigValue[0];
                var lobbyId = LobbyUtil.WrapVersionLobbyId(defaultLobbyConfig.id);          
                    
                lobby = new TypedLobby(lobbyId, LobbyType.Default);
                RoomConnectionDetails.Instance.lobbyLabel = defaultLobbyConfig.name;
            }
            
            ConnectionManager.Client.OpJoinLobby(lobby);
            RoomConnectionDetails.Instance.myLobby = lobby;
            
            return exit;
        }
    }
}
