using System;
using System.Threading.Tasks;
using Kumu.Extensions;
using Newtonsoft.Json;

namespace Kumu.Kulitan.Backend
{
    [Obsolete]
    public class AuthService : IAuthService
    {
        [Obsolete]
        public async Task<ServiceResultWrapper<RegisterUserRequestOtpResult>> RegisterUserRequestOtpAsync(RegisterUserRequestOtpRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/auth/register/send-otp");

            var reqConverter = new GenericServiceConverter<RegisterUserRequestOtpRequest>(BackendUtil.GlobalMapping);
            var reqBody = JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<RegisterUserRequestOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<RegisterUserRequestOtpResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => RegisterUserRequestOtpAsync(request));
                }

                var result = new RegisterUserRequestOtpResult();
                return new ServiceResultWrapper<RegisterUserRequestOtpResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<RegisterUserRequestOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Register Request OTP Failed", e.Message));
            }
        }

        [Obsolete]
        public async Task<ServiceResultWrapper<RegisterUserSendOtpResult>> RegisterUserSendOtpAsync(RegisterUserSendOtpRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/auth/register/verify-otp");

            var reqConverter = new GenericServiceConverter<RegisterUserSendOtpRequest>(BackendUtil.GlobalMapping);
            var reqBody = JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<RegisterUserSendOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<RegisterUserSendOtpResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => RegisterUserSendOtpAsync(request));
                }

                if (!BackendUtil.TryWriteToken(payload.data.ToString()))
                {
                    "Access token not found!".LogError();
                    return new ServiceResultWrapper<RegisterUserSendOtpResult>(ServiceErrors.unknownError);
                }

                var result = new RegisterUserSendOtpResult();
                return new ServiceResultWrapper<RegisterUserSendOtpResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<RegisterUserSendOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Register Send OTP Failed", e.Message));
            }
        }

        [Obsolete]
        public async Task<ServiceResultWrapper<LoginUserRequestOtpResult>> LoginUserRequestOtpAsync(LoginUserRequestOtpRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/auth/login/send-otp");

            var reqConverter = new GenericServiceConverter<LoginUserRequestOtpRequest>(BackendUtil.GlobalMapping);
            var reqBody = JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<LoginUserRequestOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<LoginUserRequestOtpResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => LoginUserRequestOtpAsync(request));
                }

                var result = new LoginUserRequestOtpResult();
                return new ServiceResultWrapper<LoginUserRequestOtpResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<LoginUserRequestOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Login Request OTP Failed", e.Message));
            }
        }

        [Obsolete]
        public Task<string> GetUserTokenAsync()
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public async Task<ServiceResultWrapper<LoginUserSendOtpResult>> LoginUserSendOtpAsync(LoginUserSendOtpRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/auth/login/verify-otp");

            var reqConverter = new GenericServiceConverter<LoginUserSendOtpRequest>(BackendUtil.GlobalMapping);
            var reqBody = JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<LoginUserSendOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<LoginUserSendOtpResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => LoginUserSendOtpAsync(request));
                }

                if (!BackendUtil.TryWriteToken(payload.data.ToString()))
                {
                    "Access token not found!".LogError();
                    return new ServiceResultWrapper<LoginUserSendOtpResult>(ServiceErrors.unknownError);
                }

                var result = new LoginUserSendOtpResult();
                return new ServiceResultWrapper<LoginUserSendOtpResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<LoginUserSendOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Login Send OTP Failed", e.Message));
            }
        }

        [Obsolete]
        public async Task<ServiceResultWrapper<LinkUserRequestOtpResult>> LinkUserRequestOtpAsync(LinkUserRequestOtpRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/kumu/send-otp");

            var reqBody = JsonConvert.SerializeObject(request);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<LinkUserRequestOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<LinkUserRequestOtpResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshSsoTokensAsync(() => LinkUserRequestOtpAsync(request));
                }

                var result = new LinkUserRequestOtpResult();
                return new ServiceResultWrapper<LinkUserRequestOtpResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<LinkUserRequestOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Link Request OTP Failed", e.Message));
            }
        }

        [Obsolete]
        public async Task<ServiceResultWrapper<LinkUserSendOtpResult>> LinkUserSendOtpAsync(LinkUserSendOtpRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/kumu/verify-otp");

            var reqBody = JsonConvert.SerializeObject(request);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<LinkUserSendOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<LinkUserSendOtpResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshSsoTokensAsync(() => LinkUserSendOtpAsync(request));
                }

                if (!BackendUtil.TryWriteSsoToken(payload.data.ToString()))
                {
                    "SSO token not found!".LogError();
                    return new ServiceResultWrapper<LinkUserSendOtpResult>(ServiceErrors.unknownError);
                }

                var result = new LinkUserSendOtpResult();
                return new ServiceResultWrapper<LinkUserSendOtpResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<LinkUserSendOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Link Send OTP Failed", e.Message));
            }
        }

        [Obsolete]
        public async Task<ServiceResultWrapper<RefreshLinkRequestOtpResult>> RefreshLinkRequestOtpAsync(RefreshLinkRequestOtpRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/kumu/relink/send-otp");

            var reqBody = JsonConvert.SerializeObject(request);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<RefreshLinkRequestOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<RefreshLinkRequestOtpResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshSsoTokensAsync(() => RefreshLinkRequestOtpAsync(request));
                }

                var result = new RefreshLinkRequestOtpResult();
                return new ServiceResultWrapper<RefreshLinkRequestOtpResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<RefreshLinkRequestOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Refresh Request OTP Failed", e.Message));
            }
        }

        [Obsolete]
        public async Task<ServiceResultWrapper<RefreshLinkSendOtpResult>> RefreshLinkSendOtpAsync(RefreshLinkSendOtpRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/kumu/relink/verify-otp");

            var reqBody = JsonConvert.SerializeObject(request);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<RefreshLinkSendOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<RefreshLinkSendOtpResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshSsoTokensAsync(() => RefreshLinkSendOtpAsync(request));
                }

                if (!BackendUtil.TryWriteSsoToken(payload.data.ToString()))
                {
                    "SSO token not found!".LogError();
                    return new ServiceResultWrapper<RefreshLinkSendOtpResult>(ServiceErrors.unknownError);
                }

                var result = new RefreshLinkSendOtpResult();
                return new ServiceResultWrapper<RefreshLinkSendOtpResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<RefreshLinkSendOtpResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Refresh Send OTP Failed", e.Message));
            }
        }

        [Obsolete]
        public async Task<ServiceResultWrapper<UnlinkUserResult>> UnlinkUserAsync(UnlinkUserRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/kumu");
            using var req = BackendUtil.Delete(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<UnlinkUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<UnlinkUserResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshSsoTokensAsync(() => UnlinkUserAsync(request));
                }

                BackendUtil.ClearSsoTokens();

                var result = new UnlinkUserResult();
                return new ServiceResultWrapper<UnlinkUserResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<UnlinkUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Unlinking Failed", e.Message));
            }
        }

        [Obsolete]
        public async Task<ServiceResultWrapper<LogoutUserResult>> LogOutUserAsync(LogoutUserRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/auth/logout");
            using var req = BackendUtil.Post(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<LogoutUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<LogoutUserResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => LogOutUserAsync(request));
                }

                if (payload.error_code == ServiceErrorCodes.JWT_TOKEN_INVALID)
                {
                    return new ServiceResultWrapper<LogoutUserResult>(new ServiceError(ServiceErrorCodes.JWT_TOKEN_INVALID, payload.message));
                }

                BackendUtil.ClearAllTokens();

                var result = new LogoutUserResult();
                return new ServiceResultWrapper<LogoutUserResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<LogoutUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Logout Failed", e.Message));
            }
        }

        [Obsolete]
        public async Task<ServiceResultWrapper<GetBadgeResult>> GetBadgeAsync(GetBadgeRequest request)
        {
            var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/auth/badge");
            using var req = BackendUtil.Get(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetBadgeResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<GetBadgeResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetBadgeAsync(request));
                }

                var result = JsonConvert.DeserializeObject<GetBadgeResult>(payload.data.ToString());

                return new ServiceResultWrapper<GetBadgeResult>(result);
            }
            catch (Exception e)
            {
                return new ServiceResultWrapper<GetBadgeResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Get badge failed", e.Message));
            }
        }

        public Task<ServiceResultWrapper<SignInResult>> SignInAsync(SignInRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResultWrapper<ResolvePlayerResult>> ResolvePlayerAsync(ResolvePlayerRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResultWrapper<AutoSignInResult>> AutoSignInAsync(AutoSignInRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserValid()
        {
            throw new NotImplementedException();
        }
    }
}
