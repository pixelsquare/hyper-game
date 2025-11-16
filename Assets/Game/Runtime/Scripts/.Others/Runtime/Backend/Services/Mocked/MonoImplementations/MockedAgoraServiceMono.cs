using System.Threading.Tasks;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedAgoraServiceMono : MockedServiceMono, IAgoraService
    {
        [SerializeField] private MockedAgoraService.ResultErrorFlags errorFlags;

        [SerializeField] private int responseTimeInMilliseconds = 1;

        private readonly MockedAgoraService service = new();
        
        public Task<ServiceResultWrapper<GetRTCTokenResult>> GetRTCTokenAsync(GetRTCTokenRequest request)
        {
            ConfigService();
            return service.GetRTCTokenAsync(request);
        }

        public Task<ServiceResultWrapper<GetRTMTokenResult>> GetRTMTokenAsync(GetRTMTokenRequest request)
        {
            ConfigService();
            return service.GetRTMTokenAsync(request);
        }
        
        private void ConfigService()
        {
            service.ErrorFlags = errorFlags;
            service.ResponseTimeInMilliseconds = responseTimeInMilliseconds;
        }
    }
}
