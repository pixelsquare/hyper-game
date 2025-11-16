using System.Threading.Tasks;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedAuthServiceMono : MockedServiceMono, IAuthService
    {
        [SerializeField] private MockedAuthService.ResultErrorFlags errorFlags;

        [SerializeField] private int responseTimeInMilliseconds = 1;

        private readonly MockedAuthService service = new();

        public async Task<ServiceResultWrapper<RegisterUserRequestOtpResult>> RegisterUserRequestOtpAsync(RegisterUserRequestOtpRequest request)
        {
            ConfigService();
            return await service.RegisterUserRequestOtpAsync(request);
        }

        public async Task<ServiceResultWrapper<RegisterUserSendOtpResult>> RegisterUserSendOtpAsync(RegisterUserSendOtpRequest request)
        {
            ConfigService();
            return await service.RegisterUserSendOtpAsync(request);
        }

        public async Task<ServiceResultWrapper<LoginUserRequestOtpResult>> LoginUserRequestOtpAsync(LoginUserRequestOtpRequest request)
        {
            ConfigService();
            return await service.LoginUserRequestOtpAsync(request);
        }

        public async Task<string> GetUserTokenAsync()
        {
            ConfigService();
            return await service.GetUserTokenAsync();
        }

        public async Task<ServiceResultWrapper<LoginUserSendOtpResult>> LoginUserSendOtpAsync(LoginUserSendOtpRequest request)
        {
            ConfigService();
            return await service.LoginUserSendOtpAsync(request);
        }

        public async Task<ServiceResultWrapper<LinkUserRequestOtpResult>> LinkUserRequestOtpAsync(LinkUserRequestOtpRequest request)
        {
            ConfigService();
            return await service.LinkUserRequestOtpAsync(request);
        }

        public async Task<ServiceResultWrapper<LinkUserSendOtpResult>> LinkUserSendOtpAsync(LinkUserSendOtpRequest request)
        {
            ConfigService();
            return await service.LinkUserSendOtpAsync(request);
        }

        public async Task<ServiceResultWrapper<RefreshLinkRequestOtpResult>> RefreshLinkRequestOtpAsync(RefreshLinkRequestOtpRequest request)
        {
            ConfigService();
            return await service.RefreshLinkRequestOtpAsync(request);
        }

        public async Task<ServiceResultWrapper<RefreshLinkSendOtpResult>> RefreshLinkSendOtpAsync(RefreshLinkSendOtpRequest request)
        {
            ConfigService();
            return await service.RefreshLinkSendOtpAsync(request);
        }

        public async Task<ServiceResultWrapper<UnlinkUserResult>> UnlinkUserAsync(UnlinkUserRequest request)
        {
            ConfigService();
            return await service.UnlinkUserAsync(request);
        }

        public async Task<ServiceResultWrapper<LogoutUserResult>> LogOutUserAsync(LogoutUserRequest request)
        {
            ConfigService();
            return await service.LogOutUserAsync(request);
        }

        public async Task<ServiceResultWrapper<GetBadgeResult>> GetBadgeAsync(GetBadgeRequest request)
        {
            ConfigService();
            return await service.GetBadgeAsync(request);
        }

        private void ConfigService()
        {
            service.ErrorFlags = errorFlags;
            service.ResponseTimeInMilliseconds = responseTimeInMilliseconds;
        }

        private void Start()
        {
            BackendUtil.ClearAllTokens();
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public async Task<ServiceResultWrapper<SignInResult>> SignInAsync(SignInRequest request)
        {
            ConfigService();
            return await service.SignInAsync(request);
        }

        public async Task<ServiceResultWrapper<AutoSignInResult>> AutoSignInAsync(AutoSignInRequest request)
        {
            ConfigService();
            return await service.AutoSignInAsync(request);

        }

        public async Task<ServiceResultWrapper<ResolvePlayerResult>> ResolvePlayerAsync(ResolvePlayerRequest request)
        {
            ConfigService();
            return await service.ResolvePlayerAsync(request);
        }

        public Task<bool> IsUserValid()
        {
            ConfigService();
            return ((IAuthService)service).IsUserValid();
        }
    }
}
