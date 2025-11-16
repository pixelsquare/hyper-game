using System.Threading.Tasks;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedLobbyServiceMono : MockedServiceMono, ILobbyService
    {
        [SerializeField] private MockedLobbyService.ResultErrorFlags errorFlags;
        [SerializeField] private int responseTimeInMilliseconds = 1;

        private readonly MockedLobbyService service = new();

        public async Task<ServiceResultWrapper<GetLobbyConfigResult>> GetLobbyConfigsAsync(GetLobbyConfigsRequest request)
        {
            ConfigService();
            return await service.GetLobbyConfigsAsync(request);
        }

        private void ConfigService()
        {
            service.ErrorFlags = errorFlags;
            service.ResponseTimeInMilliseconds = responseTimeInMilliseconds;
        }
    }
}
