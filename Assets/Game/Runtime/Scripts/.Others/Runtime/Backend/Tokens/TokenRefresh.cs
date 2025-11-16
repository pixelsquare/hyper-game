using System;
using System.Threading;
using System.Threading.Tasks;
using Kumu.Extensions;
using Kumu.Kulitan.Common;

namespace Kumu.Kulitan.Backend
{
    public static class TokenRefresh
    {
        private const int REFRESH_ATTEMPT = 3;

        public static async Task<ServiceResultWrapper<T>> RefreshTokensAsync<T>(
            Func<Task<ServiceResultWrapper<T>>> callback) where T : ResultBase
        {
            var authService = Services.AuthService;
            var isUserValid = await authService.IsUserValid();

            if (isUserValid)
            {
                return await Task.FromResult(new ServiceResultWrapper<T>(default(T)));
            }
            
            return new ServiceResultWrapper<T>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to refresh token, signing out."));

        }

        public static async Task<ServiceResultWrapper<T>> RefreshSsoTokensAsync<T>(Func<Task<ServiceResultWrapper<T>>> callback) where T : ResultBase
        {
            var refreshAttemptCount = 0;
            var isTokenRefreshed = false;
            var isTokenExpired = false;

            if (string.IsNullOrEmpty(BackendUtil.SsoRefreshToken))
            {
                "Invalid refresh token!".LogWarning();
                return new ServiceResultWrapper<T>(ServiceErrors.unknownError);
            }

            do
            {
                refreshAttemptCount++;

                var fullUrl = BackendUtil.GetFullUrl("/api/v1/player/kumu/refresh");
                using var req = BackendUtil.Post(fullUrl);
                req.SetRequestHeader("X-Kumu-Refresh-Token", BackendUtil.SsoRefreshToken);

                try
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        if (!await BackendUtil.TrySendWebRequest(req, cts.Token))
                        {
                            return new ServiceResultWrapper<T>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                        }
                    }

                    if (!BackendUtil.TryGetPayload<T>(req, out var payload, out var errorWrapper))
                    {
                        return errorWrapper;
                    }

                    if (payload.error_code == ServiceErrorCodes.TOKEN_EXPIRED)
                    {
                        "Token expired!".Log();
                        isTokenExpired = true;
                    }

                    // Success! Save new access token here!
                    else if (payload.error_code == 0 && BackendUtil.TryWriteSsoToken(payload.data.ToString()))
                    {
                        "Token refreshed!".Log();
                        isTokenRefreshed = true;
                        isTokenExpired = false;
                        break;
                    }
                    else
                    {
                        return new ServiceResultWrapper<T>(new ServiceError(payload.error_code, payload.message));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return new ServiceResultWrapper<T>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to refresh sso token.", e.Message));
                }

                // Attempt delay goes from 2, 4, 8 seconds with a max of 8 seconds.
                var attemptDelay = (int)Math.Pow(2, Math.Min(refreshAttemptCount, REFRESH_ATTEMPT)) * 1000;
                $"Failed to refresh sso token. Retrying again in {attemptDelay / 1000} seconds. ({refreshAttemptCount}/{REFRESH_ATTEMPT})".Log();
                await Task.Delay(attemptDelay);
            }
            while (refreshAttemptCount < REFRESH_ATTEMPT);

            if (isTokenExpired)
            {
                GlobalNotifier.Instance.Trigger(new TokenExpiredErrorEvent());
            }

            if (!isTokenRefreshed)
            {
                "Failed to refresh token!".LogError();
                return new ServiceResultWrapper<T>(ServiceErrors.unknownError);
            }

            return callback != null ? await callback.Invoke() : await Task.FromResult(new ServiceResultWrapper<T>(default(T)));
        }
    }
}
