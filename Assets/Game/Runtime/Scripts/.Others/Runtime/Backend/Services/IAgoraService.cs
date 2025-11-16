using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public interface IAgoraService
    {
        public Task<ServiceResultWrapper<GetRTCTokenResult>> GetRTCTokenAsync(GetRTCTokenRequest request);

        public Task<ServiceResultWrapper<GetRTMTokenResult>> GetRTMTokenAsync(GetRTMTokenRequest request);
    }
}
