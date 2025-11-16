using System.Threading.Tasks;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitCheckSignedInUser : UnitServiceBase<AutoSignInResult>
    {
        protected override Task<ServiceResultWrapper<AutoSignInResult>> GetServiceOperation()
        {
            var request = new AutoSignInRequest();
            var service = Services.AuthService;
            return service.AutoSignInAsync(request);
        }
    }
}
