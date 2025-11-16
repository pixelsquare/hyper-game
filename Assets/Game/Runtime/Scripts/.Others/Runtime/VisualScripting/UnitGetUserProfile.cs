using System.Threading.Tasks;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitGetUserProfile : UnitServiceBase<GetUserProfileResult>
    {
        protected override Task<ServiceResultWrapper<GetUserProfileResult>> GetServiceOperation()
        {
            return Services.UserProfileService.GetUserProfileAsync(new GetUserProfileRequest());
        }
    }
}
