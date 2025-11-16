using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitGetLobbyConfigs : UnitServiceBase<GetLobbyConfigResult>
    {
        [DoNotSerialize] public ValueOutput lobbyConfigs;

        protected override Task<ServiceResultWrapper<GetLobbyConfigResult>> GetServiceOperation()
        {
            var request = new GetLobbyConfigsRequest();
            return Services.LobbyService.GetLobbyConfigsAsync(request);
        }

        protected override void Definition()
        {
            base.Definition();
            lobbyConfigs = ValueOutput<LobbyConfig[]>(nameof(LobbyConfig));
        }

        protected override void BeforeExit()
        {
            flow.SetValue(lobbyConfigs, task.Result?.Result?.lobbyConfigs);
        }
    }
}
