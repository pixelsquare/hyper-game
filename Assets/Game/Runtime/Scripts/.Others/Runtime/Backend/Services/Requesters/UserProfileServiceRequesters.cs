using System.Collections.Generic;
using Kumu.Extensions;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    internal class CreateUserProfileRequester : Requester<CreateUserProfileRequest, CreateUserProfileResult>
    {
        public CreateUserProfileRequester(CreateUserProfileRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var reqConverter = new GenericServiceConverter<CreateUserProfileRequest>(BackendUtil.GlobalMapping);
            return BackendUtil.Post(BackendUtil.GetFullUrl("/api/v3/player/profile"), JsonConvert.SerializeObject(RequestObject, reqConverter));
        }

        public override CreateUserProfileResult FormResultFromPayload(HttpResponseObject payload)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(payload.data.ToString());
            var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);
            var userProfile = JsonConvert.DeserializeObject<UserProfile>(dataMap["profile"].ToString(), resConverter);
            var userProfileMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(dataMap["profile"].ToString());
            userProfile.userName = UserName.FromString(userProfileMap?["username"].ToString());

            $"retrieved from server {userProfile.playerId}".Log();

            return new CreateUserProfileResult(userProfile);
        }
    }

    internal class GetUserProfileRequester : Requester<GetUserProfileRequest, GetUserProfileResult>
    {
        public GetUserProfileRequester(GetUserProfileRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Get(BackendUtil.GetFullUrl("/api/v3/player/profile"));
        }

        public override GetUserProfileResult FormResultFromPayload(HttpResponseObject payload)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(payload.data.ToString());
            var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);
            var userProfile = JsonConvert.DeserializeObject<UserProfile>(dataMap["profile"].ToString(), resConverter);
            var userProfileMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(dataMap["profile"].ToString());
            if (userProfileMap?["username"] != null)
            {
                userProfile.userName = userProfileMap["username"].ToString();
            }

            $"retrieved from server {userProfile.playerId}".Log();

            return new GetUserProfileResult(userProfile);
        }
    }
}
