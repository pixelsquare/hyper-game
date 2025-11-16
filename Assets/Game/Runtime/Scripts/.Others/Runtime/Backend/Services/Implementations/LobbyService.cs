using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public class LobbyService : ILobbyService
    {
        /// <summary>
        /// How long in seconds until the lobby cache expires
        /// </summary>
        private const uint CACHE_DURATION = 1800;

        private long cacheTimestamp;
        private LobbyConfig[] cacheLobbies;

        public async Task<ServiceResultWrapper<GetLobbyConfigResult>> GetLobbyConfigsAsync(GetLobbyConfigsRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetLobbyConfigsRequester(request,
                () => cacheTimestamp,
                value => cacheTimestamp = value, 
                () => cacheLobbies, 
                value => cacheLobbies = value, 
                CACHE_DURATION);
            return await requester.ExecuteRequestAsync();
#else
            var timespan = new System.TimeSpan(System.DateTime.Now.Ticks - cacheTimestamp);
            if (timespan.Seconds < CACHE_DURATION && cacheLobbies != null)
            {
                var cachedResult = new GetLobbyConfigResult()
                {
                    lobbyConfigs = cacheLobbies,
                };
                return new ServiceResultWrapper<GetLobbyConfigResult>(cachedResult);
            }

            var fullUrl = BackendUtil.GetFullUrl("/api/v3/lobby");
            using var req = BackendUtil.Get(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetLobbyConfigResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<GetLobbyConfigResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetLobbyConfigsAsync(request));
                }

                var data = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
                {
                    lobbies = System.Array.Empty<LobbyConfig>()
                });

                var result = new GetLobbyConfigResult
                {
                    lobbyConfigs = data.lobbies,
                };

                cacheTimestamp = System.DateTime.Parse(payload.request_completed_timestamp).Ticks;
                cacheLobbies = data.lobbies;

                return new ServiceResultWrapper<GetLobbyConfigResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetLobbyConfigResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }
    }
}
