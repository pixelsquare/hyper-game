using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public class AgoraService : IAgoraService
    {
        public async Task<ServiceResultWrapper<GetRTCTokenResult>> GetRTCTokenAsync(GetRTCTokenRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetRTCTokenRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/multiplayer/agora/rtc");

            var reqBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetRTCTokenResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<GetRTCTokenResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetRTCTokenAsync(request));
                }

                var dataMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, AgoraRtcToken>>(payload.data.ToString());

                if (dataMap == null || !dataMap.TryGetValue("agora_rtc_token", out var tokenObj))
                {
                    return new ServiceResultWrapper<GetRTCTokenResult>(new ServiceError(ServiceErrorCodes.INVALID_DATA, "Invalid Data"));
                }

                var result = new GetRTCTokenResult
                {
                    token = tokenObj.token
                };

                return new ServiceResultWrapper<GetRTCTokenResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetRTCTokenResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Creating RTC Failed", e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<GetRTMTokenResult>> GetRTMTokenAsync(GetRTMTokenRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetRTMTokenRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/multiplayer/agora/rtm");

            var reqBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetRTMTokenResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<GetRTMTokenResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetRTMTokenAsync(request));
                }

                var dataMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, AgoraRtmToken>>(payload.data.ToString());

                if (dataMap == null || !dataMap.TryGetValue("agora_rtm_token", out var tokenObj))
                {
                    return new ServiceResultWrapper<GetRTMTokenResult>(new ServiceError(ServiceErrorCodes.INVALID_DATA, "Invalid Data"));
                }

                var result = new GetRTMTokenResult
                {
                    token = tokenObj.token
                };

                return new ServiceResultWrapper<GetRTMTokenResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetRTMTokenResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Create RTM Failed", e.Message));
            }
#endif
        }
    }
}
