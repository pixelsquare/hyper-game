using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    internal class GetRTMTokenRequester : Requester<GetRTMTokenRequest, GetRTMTokenResult>
    {
        public GetRTMTokenRequester(GetRTMTokenRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Post(BackendUtil.GetFullUrl("/api/v3/multiplayer/agora/rtm"), JsonConvert.SerializeObject(RequestObject));
        }

        public override GetRTMTokenResult FormResultFromPayload(HttpResponseObject payload)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, AgoraRtmToken>>(payload.data.ToString());

            if (dataMap == null || !dataMap.TryGetValue("agora_rtm_token", out var tokenObj))
            {
                throw new InvalidDataException("Invalid data.");
            }

            var result = new GetRTMTokenResult
            {
                token = tokenObj.token
            };

            return result;
        }
    }

    internal class GetRTCTokenRequester : Requester<GetRTCTokenRequest, GetRTCTokenResult>
    {
        public GetRTCTokenRequester(GetRTCTokenRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Post(BackendUtil.GetFullUrl("/api/v3/multiplayer/agora/rtc"), JsonConvert.SerializeObject(RequestObject));
        }

        public override GetRTCTokenResult FormResultFromPayload(HttpResponseObject payload)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, AgoraRtcToken>>(payload.data.ToString());

            if (dataMap == null || !dataMap.TryGetValue("agora_rtc_token", out var tokenObj))
            {
                throw new InvalidDataException("Invalid data.");
            }

            var result = new GetRTCTokenResult
            {
                token = tokenObj.token
            };

            return result;
        }
    }
}
