using System;
using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public class MockedAuthService : IAuthService
    {
        [Flags]
        public enum ResultErrorFlags
        {
            None = 0,
            UnknownError = 0b1 << 1,
            NetworkUnreachableError = 0b1 << 2,
            NetworkTimeoutError = 0b1 << 3,
            NetworkUnknownError = 0b1 << 4,
            InvalidDataError = 0b1 << 5,
            MobileAlreadyRegisteredError = 0b1 << 6,
            MobileNumberNotRegisteredError = 0b1 << 7,
            InvalidOtpError = 0b1 << 9,
            BadgeError = 0b1 << 10
        }

        public int ResponseTimeInMilliseconds { get; set; }

        public ResultErrorFlags ErrorFlags { get; set; }

        public async Task<ServiceResultWrapper<RegisterUserRequestOtpResult>> RegisterUserRequestOtpAsync(RegisterUserRequestOtpRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<RegisterUserRequestOtpResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<RegisterUserRequestOtpResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<RegisterUserRequestOtpResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.MobileAlreadyRegisteredError))
            {
                return new ServiceResultWrapper<RegisterUserRequestOtpResult>(MockedErrors.mobileAlreadyRegisteredError);
            }

            return new ServiceResultWrapper<RegisterUserRequestOtpResult>(new RegisterUserRequestOtpResult());
        }

        public async Task<ServiceResultWrapper<RegisterUserSendOtpResult>> RegisterUserSendOtpAsync(RegisterUserSendOtpRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<RegisterUserSendOtpResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<RegisterUserSendOtpResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<RegisterUserSendOtpResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.MobileAlreadyRegisteredError))
            {
                return new ServiceResultWrapper<RegisterUserSendOtpResult>(MockedErrors.mobileAlreadyRegisteredError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidOtpError))
            {
                return new ServiceResultWrapper<RegisterUserSendOtpResult>(MockedErrors.invalidOtpError);
            }

            MockedServicesUtil.SetMockedInitialProfileInPrefs(request.mobile);

            return new ServiceResultWrapper<RegisterUserSendOtpResult>(new RegisterUserSendOtpResult());
        }

        public async Task<ServiceResultWrapper<LoginUserRequestOtpResult>> LoginUserRequestOtpAsync(LoginUserRequestOtpRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<LoginUserRequestOtpResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<LoginUserRequestOtpResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<LoginUserRequestOtpResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.MobileNumberNotRegisteredError))
            {
                return new ServiceResultWrapper<LoginUserRequestOtpResult>(MockedErrors.mobileNotRegisteredError);
            }

            return new ServiceResultWrapper<LoginUserRequestOtpResult>(new LoginUserRequestOtpResult());
        }

        public Task<string> GetUserTokenAsync()
        {
            return Task.FromResult("sample token");
        }

        public async Task<ServiceResultWrapper<LoginUserSendOtpResult>> LoginUserSendOtpAsync(LoginUserSendOtpRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<LoginUserSendOtpResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<LoginUserSendOtpResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<LoginUserSendOtpResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.MobileNumberNotRegisteredError))
            {
                return new ServiceResultWrapper<LoginUserSendOtpResult>(MockedErrors.mobileNotRegisteredError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidOtpError))
            {
                return new ServiceResultWrapper<LoginUserSendOtpResult>(MockedErrors.invalidOtpError);
            }

            return new ServiceResultWrapper<LoginUserSendOtpResult>(new LoginUserSendOtpResult());
        }

        public async Task<ServiceResultWrapper<LinkUserRequestOtpResult>> LinkUserRequestOtpAsync(LinkUserRequestOtpRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<LinkUserRequestOtpResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<LinkUserRequestOtpResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<LinkUserRequestOtpResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.MobileNumberNotRegisteredError))
            {
                return new ServiceResultWrapper<LinkUserRequestOtpResult>(MockedErrors.mobileNotRegisteredError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidOtpError))
            {
                return new ServiceResultWrapper<LinkUserRequestOtpResult>(MockedErrors.invalidOtpError);
            }

            return new ServiceResultWrapper<LinkUserRequestOtpResult>(new LinkUserRequestOtpResult());
        }

        public async Task<ServiceResultWrapper<LinkUserSendOtpResult>> LinkUserSendOtpAsync(LinkUserSendOtpRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<LinkUserSendOtpResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<LinkUserSendOtpResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<LinkUserSendOtpResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.MobileNumberNotRegisteredError))
            {
                return new ServiceResultWrapper<LinkUserSendOtpResult>(MockedErrors.mobileNotRegisteredError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidOtpError))
            {
                return new ServiceResultWrapper<LinkUserSendOtpResult>(MockedErrors.invalidOtpError);
            }

            return new ServiceResultWrapper<LinkUserSendOtpResult>(new LinkUserSendOtpResult());
        }

        public async Task<ServiceResultWrapper<RefreshLinkRequestOtpResult>> RefreshLinkRequestOtpAsync(RefreshLinkRequestOtpRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<RefreshLinkRequestOtpResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<RefreshLinkRequestOtpResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<RefreshLinkRequestOtpResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.MobileNumberNotRegisteredError))
            {
                return new ServiceResultWrapper<RefreshLinkRequestOtpResult>(MockedErrors.mobileNotRegisteredError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidOtpError))
            {
                return new ServiceResultWrapper<RefreshLinkRequestOtpResult>(MockedErrors.invalidOtpError);
            }

            return new ServiceResultWrapper<RefreshLinkRequestOtpResult>(new RefreshLinkRequestOtpResult());
        }

        public async Task<ServiceResultWrapper<RefreshLinkSendOtpResult>> RefreshLinkSendOtpAsync(RefreshLinkSendOtpRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<RefreshLinkSendOtpResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<RefreshLinkSendOtpResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<RefreshLinkSendOtpResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.MobileNumberNotRegisteredError))
            {
                return new ServiceResultWrapper<RefreshLinkSendOtpResult>(MockedErrors.mobileNotRegisteredError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidOtpError))
            {
                return new ServiceResultWrapper<RefreshLinkSendOtpResult>(MockedErrors.invalidOtpError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidOtpError))
            {
                return new ServiceResultWrapper<RefreshLinkSendOtpResult>(MockedErrors.invalidOtpError);
            }

            return new ServiceResultWrapper<RefreshLinkSendOtpResult>(new RefreshLinkSendOtpResult());
        }

        public async Task<ServiceResultWrapper<UnlinkUserResult>> UnlinkUserAsync(UnlinkUserRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<UnlinkUserResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<UnlinkUserResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<UnlinkUserResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.MobileNumberNotRegisteredError))
            {
                return new ServiceResultWrapper<UnlinkUserResult>(MockedErrors.mobileNotRegisteredError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidOtpError))
            {
                return new ServiceResultWrapper<UnlinkUserResult>(MockedErrors.invalidOtpError);
            }

            return new ServiceResultWrapper<UnlinkUserResult>(new UnlinkUserResult());
        }

        public async Task<ServiceResultWrapper<LogoutUserResult>> LogOutUserAsync(LogoutUserRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<LogoutUserResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<LogoutUserResult>(error);
            }

            return new ServiceResultWrapper<LogoutUserResult>(new LogoutUserResult());
        }

        public async Task<ServiceResultWrapper<GetBadgeResult>> GetBadgeAsync(GetBadgeRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.BadgeError))
            {
                return new ServiceResultWrapper<GetBadgeResult>(new ServiceError(1000, "Some error occured"));
            }

            return new ServiceResultWrapper<GetBadgeResult>(new GetBadgeResult
            {
                badge = "live"
            });
        }

        private bool TryGetNetworkError(out ServiceError error)
        {
            error = null;

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkUnreachableError))
            {
                error = ServiceErrors.networkUnreachableError;
                return true;
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkTimeoutError))
            {
                error = ServiceErrors.networkTimeoutError;
                return true;
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkUnknownError))
            {
                error = ServiceErrors.networkUnknownError;
                return true;
            }

            return false;
        }

        public Task<ServiceResultWrapper<SignInResult>> SignInAsync(SignInRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResultWrapper<AutoSignInResult>> AutoSignInAsync(AutoSignInRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<AutoSignInResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<AutoSignInResult>(error);
            }

            return new ServiceResultWrapper<AutoSignInResult>(new AutoSignInResult());
        }

        public async Task<ServiceResultWrapper<ResolvePlayerResult>> ResolvePlayerAsync(ResolvePlayerRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<ResolvePlayerResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<ResolvePlayerResult>(error);
            }

            return new ServiceResultWrapper<ResolvePlayerResult>(new ResolvePlayerResult());
        }

        public Task<bool> IsUserValid()
        {
            return Task.FromResult(true);
        }
    }
}
