using System.Threading.Tasks;
#if !USE_OLD_API_METHODS
using System.Linq;
#endif

namespace Kumu.Kulitan.Backend
{
    public class ModerationService : IModerationService
    {
        public async Task<ServiceResultWrapper<ReportUserResult>> ReportUserAsync(ReportUserRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new ReportUserRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/report/player");

            var reqConverter = new GenericServiceConverter<ReportUserRequest>(BackendUtil.GlobalMapping);
            var reqBody = Newtonsoft.Json.JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<ReportUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<ReportUserResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => ReportUserAsync(request));
                }

                var result = new ReportUserResult();
                return new ServiceResultWrapper<ReportUserResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<ReportUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<ReportHangoutResult>> ReportHangoutAsync(ReportHangoutRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new ReportHangoutRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/report/room");

            var reqConverter = new GenericServiceConverter<ReportHangoutRequest>(BackendUtil.GlobalMapping);
            var reqBody = Newtonsoft.Json.JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<ReportHangoutResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<ReportHangoutResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => ReportHangoutAsync(request));
                }

                var result = new ReportHangoutResult();
                return new ServiceResultWrapper<ReportHangoutResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<ReportHangoutResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<BlockPlayerResult>> BlockPlayerAsync(BlockPlayerRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new BlockPlayersRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl($"/api/v3/social/{request.userId}/block");
            using var req = BackendUtil.Post(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<BlockPlayerResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<BlockPlayerResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => BlockPlayerAsync(request));
                }

                var result = new BlockPlayerResult();
                return new ServiceResultWrapper<BlockPlayerResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<BlockPlayerResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<UnblockPlayerResult>> UnblockPlayerAsync(UnblockPlayerRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new UnblockPlayersRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl($"/api/v3/social/{request.userId}/block");
            using var req = BackendUtil.Delete(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<UnblockPlayerResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<UnblockPlayerResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => UnblockPlayerAsync(request));
                }

                var result = new UnblockPlayerResult();
                return new ServiceResultWrapper<UnblockPlayerResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<UnblockPlayerResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<GetBlockedPlayersResult>> GetBlockedPlayersAsync(GetBlockedPlayersRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetBlockedPlayersRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/social/blocked");
            using var req = BackendUtil.Get(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetBlockedPlayersResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<GetBlockedPlayersResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetBlockedPlayersAsync(request));
                }

                var dataMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(payload.data.ToString());

                var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);
                var userProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile[]>(dataMap["blocked"].ToString(), resConverter);
                var blockedUids = userProfile.Select(a => a.accountId).ToArray();

                var result = new GetBlockedPlayersResult
                {
                    blockedUserIds = blockedUids
                };

                return new ServiceResultWrapper<GetBlockedPlayersResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetBlockedPlayersResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }
    }
}
