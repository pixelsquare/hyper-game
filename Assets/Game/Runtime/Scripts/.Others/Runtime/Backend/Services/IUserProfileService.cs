using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public interface IUserProfileService
    {
        public Task<ServiceResultWrapper<GetUserProfileResult>> GetUserProfileAsync(GetUserProfileRequest request);

        public Task<ServiceResultWrapper<CreateUserProfileResult>> CreateUserProfileAsync(CreateUserProfileRequest request);
    }
}
