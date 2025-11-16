using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public interface IAuthService
    {
        public Task<ServiceResultWrapper<RegisterUserRequestOtpResult>> RegisterUserRequestOtpAsync(RegisterUserRequestOtpRequest request);

        public Task<ServiceResultWrapper<RegisterUserSendOtpResult>> RegisterUserSendOtpAsync(RegisterUserSendOtpRequest request);

        public Task<ServiceResultWrapper<LoginUserRequestOtpResult>> LoginUserRequestOtpAsync(LoginUserRequestOtpRequest request);

        public Task<ServiceResultWrapper<SignInResult>> SignInAsync(SignInRequest request);

        public Task<ServiceResultWrapper<AutoSignInResult>> AutoSignInAsync(AutoSignInRequest request);

        public Task<ServiceResultWrapper<ResolvePlayerResult>> ResolvePlayerAsync(ResolvePlayerRequest request);

        public Task<string> GetUserTokenAsync();

        public Task<bool> IsUserValid();

        public Task<ServiceResultWrapper<LoginUserSendOtpResult>> LoginUserSendOtpAsync(LoginUserSendOtpRequest request);

        public Task<ServiceResultWrapper<LinkUserRequestOtpResult>> LinkUserRequestOtpAsync(LinkUserRequestOtpRequest request);

        public Task<ServiceResultWrapper<LinkUserSendOtpResult>> LinkUserSendOtpAsync(LinkUserSendOtpRequest request);

        public Task<ServiceResultWrapper<RefreshLinkRequestOtpResult>> RefreshLinkRequestOtpAsync(RefreshLinkRequestOtpRequest request);

        public Task<ServiceResultWrapper<RefreshLinkSendOtpResult>> RefreshLinkSendOtpAsync(RefreshLinkSendOtpRequest request);

        public Task<ServiceResultWrapper<UnlinkUserResult>> UnlinkUserAsync(UnlinkUserRequest request);

        public Task<ServiceResultWrapper<LogoutUserResult>> LogOutUserAsync(LogoutUserRequest request);

        public Task<ServiceResultWrapper<GetBadgeResult>> GetBadgeAsync(GetBadgeRequest request);
    }

    public interface IInputFormatValidator
    {
        public bool IsValid(string value);
    }

    public interface IInputFormatValidator<T> where T : struct
    {
        public bool IsValid(string value, out T details);
    }
}
