using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public interface IModerationService
    {
        public Task<ServiceResultWrapper<ReportUserResult>> ReportUserAsync(ReportUserRequest request);

        public Task<ServiceResultWrapper<ReportHangoutResult>> ReportHangoutAsync(ReportHangoutRequest request);

        public Task<ServiceResultWrapper<BlockPlayerResult>> BlockPlayerAsync(BlockPlayerRequest request);

        public Task<ServiceResultWrapper<UnblockPlayerResult>> UnblockPlayerAsync(UnblockPlayerRequest request);

        public Task<ServiceResultWrapper<GetBlockedPlayersResult>> GetBlockedPlayersAsync(GetBlockedPlayersRequest request);
    }
}
