using System.Threading.Tasks;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedUserProfileServiceMono : MockedServiceMono, IUserProfileService
    {
        [SerializeField] private MockedUserProfileService.ResultErrorFlags errorFlags;

        [SerializeField] private bool isProfileReady = true;

        [SerializeField] private bool hasLinkedKumuAccount = true;

        [SerializeField] private int responseTimeInMilliseconds = 1;

        private readonly MockedUserProfileService service = new();

        public Task<ServiceResultWrapper<GetUserProfileResult>> GetUserProfileAsync(GetUserProfileRequest request)
        {
            ConfigService();
            return service.GetUserProfileAsync(request);
        }

        public Task<ServiceResultWrapper<CreateUserProfileResult>> CreateUserProfileAsync(CreateUserProfileRequest request)
        {
            ConfigService();
            return service.CreateUserProfileAsync(request);
        }

        private void ConfigService()
        {
            service.ErrorFlags = errorFlags;
            service.IsProfileReady = isProfileReady;
            service.HasLinkedKumuAccount = hasLinkedKumuAccount;
            service.ResponseTimeInMilliseconds = responseTimeInMilliseconds;
        }
    }
}
