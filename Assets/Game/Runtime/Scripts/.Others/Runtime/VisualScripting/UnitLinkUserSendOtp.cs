using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.UI;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitLinkUserSendOtp : UnitServiceBase<LinkUserSendOtpResult>
    {
        protected override Task<ServiceResultWrapper<LinkUserSendOtpResult>> GetServiceOperation()
        {
            var newLinkUserSendOtpRequest = new LinkUserSendOtpRequest
            {
                username = KumuLinkUserScreen.kumuUsername,
                otp = KumuLinkUserVerificationScreen.otpVerifCode
            };

            return Services.AuthService.LinkUserSendOtpAsync(newLinkUserSendOtpRequest);
        }
    }
}
