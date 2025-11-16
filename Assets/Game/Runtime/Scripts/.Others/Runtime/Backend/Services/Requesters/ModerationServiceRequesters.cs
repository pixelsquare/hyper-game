using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    internal class GetBlockedPlayersRequester : Requester<GetBlockedPlayersRequest, GetBlockedPlayersResult>
    {
        public GetBlockedPlayersRequester(GetBlockedPlayersRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Get(BackendUtil.GetFullUrl("/api/v3/social/blocked"));
        }

        public override GetBlockedPlayersResult FormResultFromPayload(HttpResponseObject payload)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(payload.data.ToString());

            var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);
            var userProfile = JsonConvert.DeserializeObject<UserProfile[]>(dataMap["blocked"].ToString(), resConverter);
            var blockedUids = userProfile.Select(a => a.accountId).ToArray();

            var result = new GetBlockedPlayersResult
            {
                blockedUserIds = blockedUids
            };

            return result;
        }
    }

    internal class UnblockPlayersRequester : Requester<UnblockPlayerRequest, UnblockPlayerResult>
    {
        public UnblockPlayersRequester(UnblockPlayerRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var fullUrl = BackendUtil.GetFullUrl($"/api/v3/social/{RequestObject.userId}/block");
            return BackendUtil.Delete(fullUrl);
        }

        public override UnblockPlayerResult FormResultFromPayload(HttpResponseObject payload)
        {
            return new UnblockPlayerResult();
        }
    }

    internal class BlockPlayersRequester : Requester<BlockPlayerRequest, BlockPlayerResult>
    {
        public BlockPlayersRequester(BlockPlayerRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var fullUrl = BackendUtil.GetFullUrl($"/api/v3/social/{RequestObject.userId}/block");
            return BackendUtil.Post(fullUrl);
        }

        public override BlockPlayerResult FormResultFromPayload(HttpResponseObject payload)
        {
            return new BlockPlayerResult();
        }
    }

    internal class ReportHangoutRequester : Requester<ReportHangoutRequest, ReportHangoutResult>
    {
        public ReportHangoutRequester(ReportHangoutRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var reqConverter = new GenericServiceConverter<ReportHangoutRequest>(BackendUtil.GlobalMapping);
            return BackendUtil.Post(BackendUtil.GetFullUrl("/api/v3/report/room"), JsonConvert.SerializeObject(RequestObject, reqConverter));
        }

        public override ReportHangoutResult FormResultFromPayload(HttpResponseObject payload)
        {
            return new ReportHangoutResult();
        }
    }

    internal class ReportUserRequester : Requester<ReportUserRequest, ReportUserResult>
    {
        public ReportUserRequester(ReportUserRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var reqConverter = new GenericServiceConverter<ReportUserRequest>(BackendUtil.GlobalMapping);
            return BackendUtil.Post(BackendUtil.GetFullUrl("/api/v3/report/player"), JsonConvert.SerializeObject(RequestObject, reqConverter));
        }

        public override ReportUserResult FormResultFromPayload(HttpResponseObject payload)
        {
            return new ReportUserResult();
        }
    }
}
