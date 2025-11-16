using System.Threading.Tasks;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedModerationServiceMono : MockedServiceMono, IModerationService
    {
        [SerializeField] private MockedModerationService.ResultErrorFlags errorFlags;

        [SerializeField] private int responseTimeInMilliseconds = 1;

        private readonly MockedModerationService service = new();

        public async Task<ServiceResultWrapper<ReportUserResult>> ReportUserAsync(ReportUserRequest request)
        {
            ConfigService();
            return await service.ReportUserAsync(request);
        }

        public async Task<ServiceResultWrapper<ReportHangoutResult>> ReportHangoutAsync(ReportHangoutRequest request)
        {
            ConfigService();
            return await service.ReportHangoutAsync(request);
        }

        public async Task<ServiceResultWrapper<BlockPlayerResult>> BlockPlayerAsync(BlockPlayerRequest request)
        {
            ConfigService();
            return await service.BlockPlayerAsync(request);
        }

        public async Task<ServiceResultWrapper<UnblockPlayerResult>> UnblockPlayerAsync(UnblockPlayerRequest request)
        {
            ConfigService();
            return await service.UnblockPlayerAsync(request);
        }

        public async Task<ServiceResultWrapper<GetBlockedPlayersResult>> GetBlockedPlayersAsync(GetBlockedPlayersRequest request)
        {
            ConfigService();
            return await service.GetBlockedPlayersAsync(request);
        }

        private void ConfigService()
        {
            service.ErrorFlags = errorFlags;
            service.ResponseTimeInMilliseconds = responseTimeInMilliseconds;
        }
    }
}
