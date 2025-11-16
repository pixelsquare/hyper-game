using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.UI;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitLinkUserRequestOtp : UnitServiceBase<LinkUserRequestOtpResult>
    {
        protected override Task<ServiceResultWrapper<LinkUserRequestOtpResult>> GetServiceOperation()
        {
            var newLinkUserOtpRequest = new LinkUserRequestOtpRequest();
            newLinkUserOtpRequest.username = KumuLinkUserScreen.kumuUsername;
            return Services.AuthService.LinkUserRequestOtpAsync(newLinkUserOtpRequest);
        }
    }
}
