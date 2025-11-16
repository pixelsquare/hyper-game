using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    internal class GetLobbyConfigsRequester : Requester<GetLobbyConfigsRequest, GetLobbyConfigResult>
    {
        private Func<long> timestampGetter;
        private Action<long> timestampSetter;
        private Func<LobbyConfig[]> lobbiesGetter;
        private Action<LobbyConfig[]> lobbiesSetter;
        
        private uint CacheDuration { get; }

        public GetLobbyConfigsRequester(GetLobbyConfigsRequest requestObject, 
            Func<long> cacheTimestampGetter, 
            Action<long> cacheTimestampSetter,
            Func<LobbyConfig[]> cacheLobbiesGetter,
            Action<LobbyConfig[]> cacheLobbiesSetter,
            uint cacheDuration) : base(requestObject)
        {
            timestampGetter = cacheTimestampGetter;
            timestampSetter = cacheTimestampSetter;
            lobbiesGetter = cacheLobbiesGetter;
            lobbiesSetter = cacheLobbiesSetter;
            CacheDuration = cacheDuration;
        }

        public override bool UseCachedResult(out GetLobbyConfigResult result)
        {
            result = default;
            
            var timespan = new TimeSpan(DateTime.Now.Ticks - timestampGetter.Invoke());
            if (timespan.Seconds >= CacheDuration || lobbiesGetter.Invoke() == null)
            {
                return false;
            }

            result = new GetLobbyConfigResult
            {
                lobbyConfigs = lobbiesGetter.Invoke()
            };
            return true;
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Get(BackendUtil.GetFullUrl("/api/v3/lobby"));
        }

        public override GetLobbyConfigResult FormResultFromPayload(HttpResponseObject payload)
        {
            var data = JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
            {
                lobbies = Array.Empty<LobbyConfig>()
            });

            var result = new GetLobbyConfigResult
            {
                lobbyConfigs = data.lobbies,
            };

            timestampSetter.Invoke(DateTime.Parse(payload.request_completed_timestamp).Ticks);
            lobbiesSetter.Invoke(data.lobbies);

            return result;
        }
    }
}
