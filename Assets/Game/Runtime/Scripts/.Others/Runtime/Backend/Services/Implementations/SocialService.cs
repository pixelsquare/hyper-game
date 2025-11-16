using System;
using System.Threading.Tasks;
using UniRx;
#if !USE_OLD_API_METHODS
using System.Linq;
using Kumu.Extensions;
#endif

namespace Kumu.Kulitan.Backend
{
    public class SocialService : ISocialService
    {
        private readonly ISubject<bool> onSocialActionMade = new Subject<bool>();

        public async Task<ServiceResultWrapper<FindUserResult>> FindUserAsync(FindUserRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new FindUserRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var dynamicUrl = $"/api/v3/social/search?keyword={request.keyword}";

            if (request.profileConstraints is { } profileConstraints && profileConstraints.Any())
            {
                dynamicUrl = profileConstraints.Aggregate(dynamicUrl, (current, constraint) => current + $"&profile_constraints[]={constraint}");
            }

            if (request.userConstraints is { } userConstraints && userConstraints.Any())
            {
                dynamicUrl = userConstraints.Aggregate(dynamicUrl, (current, constraint) => current + $"&user_constraints[]={constraint}");
            }

            var fullUrl = BackendUtil.GetFullUrl(dynamicUrl);
            using var req = BackendUtil.Get(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<FindUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<FindUserResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => FindUserAsync(request));
                }

                var dataMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, System.Collections.Generic.IDictionary<string, object>>>(payload.data.ToString());
                var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);

                var friends = request.userConstraints.Contains("friends") ? Newtonsoft.Json.JsonConvert.DeserializeObject<OtherUserProfile[]>(dataMap["friends"]["other_player_profiles"].ToString(), resConverter) : null;
                var followers = request.userConstraints.Contains("followers") ? Newtonsoft.Json.JsonConvert.DeserializeObject<OtherUserProfile[]>(dataMap["followers"]["other_player_profiles"].ToString(), resConverter) : null;
                var following = request.userConstraints.Contains("following") ? Newtonsoft.Json.JsonConvert.DeserializeObject<OtherUserProfile[]>(dataMap["following"]["other_player_profiles"].ToString(), resConverter) : null;
                var unrelated = request.userConstraints.Contains("unrelated") ? Newtonsoft.Json.JsonConvert.DeserializeObject<OtherUserProfile[]>(dataMap["unrelated"]["other_player_profiles"].ToString(), resConverter) : null;

                var result = new FindUserResult
                {
                    Friends = friends,
                    Followers = followers,
                    Following = following,
                    Unrelated = unrelated
                };

                return new ServiceResultWrapper<FindUserResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<FindUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<FollowUserResult>> FollowUserAsync(FollowUserRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new FollowUserRequester(request, onSocialActionMade);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl($"/api/v3/social/{request.userId}/follow");
            using var req = BackendUtil.Post(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<FollowUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<FollowUserResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => FollowUserAsync(request));
                }

                onSocialActionMade?.OnNext(true);
                var result = new FollowUserResult();
                return new ServiceResultWrapper<FollowUserResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<FollowUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<UnfollowUserResult>> UnfollowUserAsync(UnfollowUserRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new UnfollowUserRequester(request, onSocialActionMade);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl($"/api/v3/social/{request.userId}/follow");
            using var req = BackendUtil.Delete(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<UnfollowUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<UnfollowUserResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => UnfollowUserAsync(request));
                }

                onSocialActionMade?.OnNext(true);
                var result = new UnfollowUserResult();
                return new ServiceResultWrapper<UnfollowUserResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<UnfollowUserResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<GetSocialRelationshipsResult>> GetSocialRelationshipsAsync(GetSocialRelationshipsRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetSocialRelationshipsRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var dynamicUrl = $"/api/v3/social?user_constraint={request.userConstraint}&page={request.pageToRequest}";

            if (request.profileConstraints is { } profileConstraints && profileConstraints.Any())
            {
                dynamicUrl = profileConstraints.Aggregate(dynamicUrl, (current, constraint) => current + $"&profile_constraints[]={constraint}");
            }

            var fullUrl = BackendUtil.GetFullUrl(dynamicUrl);
            using var req = BackendUtil.Get(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetSocialRelationshipsResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<GetSocialRelationshipsResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetSocialRelationshipsAsync(request));
                }

                var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);

                var dataMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(payload.data.ToString());
                var otherPlayerProfiles = Newtonsoft.Json.JsonConvert.DeserializeObject<OtherUserProfile[]>(dataMap["other_player_profiles"].ToString(), resConverter);
                var meta = Newtonsoft.Json.JsonConvert.DeserializeObject<SocialRelationshipsResponseMeta>(dataMap["meta"].ToString());

                var result = new GetSocialRelationshipsResult
                {
                    OtherPlayerProfiles = otherPlayerProfiles,
                    CurrentPage = meta.current_page,
                    HasNextPage = meta.current_page < meta.last_page
                };

                return new ServiceResultWrapper<GetSocialRelationshipsResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetSocialRelationshipsResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<SetFavoriteResult>> SetFavoriteAsync(SetFavoriteRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new SetFavoritesRequester(request, onSocialActionMade);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl($"/api/v3/social/{request.userId}/favorite");
            using var req = BackendUtil.Post(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<SetFavoriteResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<SetFavoriteResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => SetFavoriteAsync(request));
                }
                
                onSocialActionMade?.OnNext(true);
                var result = new SetFavoriteResult();
                return new ServiceResultWrapper<SetFavoriteResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<SetFavoriteResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<RemoveFavoriteResult>> RemoveFavoriteAsync(RemoveFavoriteRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new RemoveFavoriteRequester(request, onSocialActionMade);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl($"/api/v3/social/{request.userId}/favorite");
            using var req = BackendUtil.Delete(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<RemoveFavoriteResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<RemoveFavoriteResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => RemoveFavoriteAsync(request));
                }

                onSocialActionMade?.OnNext(true);
                var result = new RemoveFavoriteResult();
                return new ServiceResultWrapper<RemoveFavoriteResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<RemoveFavoriteResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<UpdateUserRoomResult>> UpdateUserRoomAsync(UpdateUserRoomRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new UpdateUserRoomRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            throw new NotImplementedException();
#endif
        }

        public async Task<ServiceResultWrapper<RecordUserCreatedRoomResult>> RecordUserCreatedRoomAsync(RecordUserCreatedRoomRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new RecordUserCreatedRoomRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/room/create");

            var reqConverter = new GenericServiceConverter<RecordUserCreatedRoomRequest>(BackendUtil.GlobalMapping);
            var reqBody = Newtonsoft.Json.JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<RecordUserCreatedRoomResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<RecordUserCreatedRoomResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => RecordUserCreatedRoomAsync(request));
                }

                var result = new RecordUserCreatedRoomResult();
                $"[SocialService] User has created room {request.roomId} recorded.".Log();
                return new ServiceResultWrapper<RecordUserCreatedRoomResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<RecordUserCreatedRoomResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Create RTM Failed", e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<RecordUserEnteredRoomResult>> RecordUserEnteredRoomAsync(RecordUserEnteredRoomRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new RecordUserEnteredRoomRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/room/enter");

            var reqConverter = new GenericServiceConverter<RecordUserEnteredRoomRequest>(BackendUtil.GlobalMapping);
            var reqBody = Newtonsoft.Json.JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Put(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<RecordUserEnteredRoomResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<RecordUserEnteredRoomResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => RecordUserEnteredRoomAsync(request));
                }

                if (payload.error_code == ServiceErrorCodes.ROOM_ID_DOES_NOT_EXIST)
                {
                    return new ServiceResultWrapper<RecordUserEnteredRoomResult>(new ServiceError(ServiceErrorCodes.ROOM_ID_DOES_NOT_EXIST, "Room does not exist."));
                }

                var result = new RecordUserEnteredRoomResult();
                $"[SocialService] User has entered room {request.roomId} recorded.".Log();
                return new ServiceResultWrapper<RecordUserEnteredRoomResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<RecordUserEnteredRoomResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Create RTM Failed", e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<ClearUserRoomRecordResult>> ClearUserRoomRecordAsync(ClearUserRoomRecordRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new ClearUserRoomRecordRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/room/clear");

            var reqConverter = new GenericServiceConverter<RecordUserEnteredRoomRequest>(BackendUtil.GlobalMapping);
            using var req = BackendUtil.Delete(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<ClearUserRoomRecordResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<ClearUserRoomRecordResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => ClearUserRoomRecordAsync(request));
                }

                var result = new ClearUserRoomRecordResult();
                "[SocialService] Cleared user room record.".Log();
                return new ServiceResultWrapper<ClearUserRoomRecordResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<ClearUserRoomRecordResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Create RTM Failed", e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<GetOtherUserProfileResult>> GetOtherUserProfileAsync(GetOtherUserProfileRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetOtherUserProfileRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var extraParams = request.profileConstraints.Select(constraint => new ValueTuple<string, string>("profile_constraints[]", constraint)).ToList();
            var fullUrl = BackendUtil.GetFullUrl($"/api/v3/social/{request.userId}", extraParams);
            using var req = BackendUtil.Get(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetOtherUserProfileResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<GetOtherUserProfileResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetOtherUserProfileAsync(request));
                }

                var dataMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(payload.data.ToString());
                var userProfile = new UserProfile();
                var socialState = SocialState.None;
                var roomId = string.Empty;
                var equippedItems = Array.Empty<Avatar.AvatarItemState>();

                if (dataMap != null && dataMap.TryGetValue("other_player_profile", out var otherUserProfileObj) && otherUserProfileObj != null)
                {
                    var otherUserProfileMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(otherUserProfileObj.ToString());

                    if (otherUserProfileMap != null && otherUserProfileMap.TryGetValue("profile", out var profileObj) && profileObj != null)
                    {
                        var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);
                        userProfile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(profileObj.ToString(), resConverter);
                        var profileMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(profileObj.ToString(), resConverter);
                        userProfile.userName = profileMap["username"].ToString();
                    }

                    if (otherUserProfileMap != null && otherUserProfileMap.TryGetValue("social_state", out var socialStateObj) && socialStateObj != null)
                    {
                        socialState = (SocialState)Convert.ToInt32(socialStateObj);
                    }

                    if (otherUserProfileMap != null && otherUserProfileMap.TryGetValue("room_id", out var roomIdObj) && roomIdObj != null)
                    {
                        roomId = Convert.ToString(roomIdObj);
                    }

                    if (otherUserProfileMap != null && otherUserProfileMap.TryGetValue("equipped_items", out var equippedItemsObj) && equippedItemsObj != null)
                    {
                        var resConverter = new GenericServiceConverter<Avatar.AvatarItemState[]>(BackendUtil.GlobalMapping);
                        equippedItems = Newtonsoft.Json.JsonConvert.DeserializeObject<Avatar.AvatarItemState[]>(equippedItemsObj.ToString(), resConverter);
                    }
                }

                var result = new GetOtherUserProfileResult
                {
                    otherUserProfile = new OtherUserProfile
                    {
                        profile = userProfile,
                        social_state = socialState,
                        room_id = roomId,
                        equipped_items = equippedItems
                    }
                };

                return new ServiceResultWrapper<GetOtherUserProfileResult>(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetOtherUserProfileResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public ISubject<bool> OnSocialActionMade => onSocialActionMade;
    }

    /// <summary>
    /// Meta object containing pagination details for get social relationship responses
    /// </summary>
    [Serializable]
    public class SocialRelationshipsResponseMeta
    {
        public int total;
        public int current_page;
        public int last_page;
        public int per_page;
    }
}
