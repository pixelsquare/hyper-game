using System.Threading.Tasks;
#if !USE_OLD_API_METHODS
using Kumu.Extensions;
#endif

namespace Kumu.Kulitan.Backend
{
    public class UserProfileService : IUserProfileService
    {
        public async Task<ServiceResultWrapper<GetUserProfileResult>> GetUserProfileAsync(GetUserProfileRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetUserProfileRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/player/profile");
            using var req = BackendUtil.Get(fullUrl);
            
            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetUserProfileResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<GetUserProfileResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                $"payload: {payload.message}".Log();
                
                var dataMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(payload.data.ToString());

                var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);
                var userProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(dataMap["profile"].ToString(), resConverter);
                var userProfileMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(dataMap["profile"].ToString());
                if (userProfileMap?["username"] != null)
                {
                    userProfile.userName = userProfileMap["username"].ToString();
                }

                var result = new GetUserProfileResult(userProfile);

                return new ServiceResultWrapper<GetUserProfileResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetUserProfileResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Get Profile Failed", e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<CreateUserProfileResult>> CreateUserProfileAsync(CreateUserProfileRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new CreateUserProfileRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/player/profile");

            var reqConverter = new GenericServiceConverter<CreateUserProfileRequest>(BackendUtil.GlobalMapping);
            var reqBody = Newtonsoft.Json.JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<CreateUserProfileResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<CreateUserProfileResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => CreateUserProfileAsync(request));
                }

                var dataMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(payload.data.ToString());

                var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);
                var userProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(dataMap["profile"].ToString(), resConverter);
                var userProfileMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(dataMap["profile"].ToString());
                userProfile.userName = UserName.FromString(userProfileMap?["username"].ToString());

                $"retrieved from server {userProfile.playerId}".Log();

                var result = new CreateUserProfileResult(userProfile);
                return new ServiceResultWrapper<CreateUserProfileResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<CreateUserProfileResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Create Profile Failed", e.Message));
            }
#endif
        }
    }
}
