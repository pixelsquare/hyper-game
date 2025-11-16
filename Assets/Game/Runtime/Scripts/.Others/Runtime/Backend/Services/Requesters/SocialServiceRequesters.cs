using System;
using System.Collections.Generic;
using System.Linq;
using Kumu.Extensions;
using Kumu.Kulitan.Avatar;
using Newtonsoft.Json;
using UniRx;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    internal class GetOtherUserProfileRequester : Requester<GetOtherUserProfileRequest, GetOtherUserProfileResult>
    {
        public GetOtherUserProfileRequester(GetOtherUserProfileRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var extraParams = RequestObject.profileConstraints.Select(constraint => new ValueTuple<string, string>("profile_constraints[]", constraint)).ToList();
            return BackendUtil.Get(BackendUtil.GetFullUrl($"/api/v3/social/{RequestObject.userId}", extraParams));
        }

        public override GetOtherUserProfileResult FormResultFromPayload(HttpResponseObject payload)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(payload.data.ToString());
            var userProfile = new UserProfile();
            var socialState = SocialState.None;
            var roomId = string.Empty;
            var equippedItems = Array.Empty<AvatarItemState>();

            if (dataMap != null && dataMap.TryGetValue("other_player_profile", out var otherUserProfileObj) && otherUserProfileObj != null)
            {
                var otherUserProfileMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(otherUserProfileObj.ToString());

                if (otherUserProfileMap != null && otherUserProfileMap.TryGetValue("profile", out var profileObj) && profileObj != null)
                {
                    var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);
                    userProfile = JsonConvert.DeserializeObject<UserProfile>(profileObj.ToString(), resConverter);
                    var profileMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(profileObj.ToString(), resConverter);
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
                    var resConverter = new GenericServiceConverter<AvatarItemState[]>(BackendUtil.GlobalMapping);
                    equippedItems = JsonConvert.DeserializeObject<AvatarItemState[]>(equippedItemsObj.ToString(), resConverter);
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

            return result;
        }
    }

    internal class ClearUserRoomRecordRequester : Requester<ClearUserRoomRecordRequest, ClearUserRoomRecordResult>
    {
        public ClearUserRoomRecordRequester(ClearUserRoomRecordRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Delete(BackendUtil.GetFullUrl("/api/v3/room/clear"));
        }

        public override ClearUserRoomRecordResult FormResultFromPayload(HttpResponseObject payload)
        {
            var result = new ClearUserRoomRecordResult();
            "[SocialService] Cleared user room record.".Log();

            return result;
        }
    }

    internal class RecordUserEnteredRoomRequester : Requester<RecordUserEnteredRoomRequest, RecordUserEnteredRoomResult>
    {
        public RecordUserEnteredRoomRequester(RecordUserEnteredRoomRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var reqConverter = new GenericServiceConverter<RecordUserEnteredRoomRequest>(BackendUtil.GlobalMapping);

            return BackendUtil.Put(BackendUtil.GetFullUrl("/api/v3/room/enter"), JsonConvert.SerializeObject(RequestObject, reqConverter));
        }

        public override RecordUserEnteredRoomResult FormResultFromPayload(HttpResponseObject payload)
        {
            var result = new RecordUserEnteredRoomResult();
            $"[SocialService] User has entered room {RequestObject.roomId} recorded.".Log();

            return result;
        }
    }

    internal class RecordUserCreatedRoomRequester : Requester<RecordUserCreatedRoomRequest, RecordUserCreatedRoomResult>
    {
        public RecordUserCreatedRoomRequester(RecordUserCreatedRoomRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var reqConverter = new GenericServiceConverter<RecordUserCreatedRoomRequest>(BackendUtil.GlobalMapping);
            return BackendUtil.Post(BackendUtil.GetFullUrl("/api/v3/room/create"), JsonConvert.SerializeObject(RequestObject, reqConverter));
        }

        public override RecordUserCreatedRoomResult FormResultFromPayload(HttpResponseObject payload)
        {
            var result = new RecordUserCreatedRoomResult();
            $"[SocialService] User has created room {RequestObject.roomId} recorded.".Log();

            return result;
        }
    }

    internal class UpdateUserRoomRequester : Requester<UpdateUserRoomRequest, UpdateUserRoomResult>
    {
        public UpdateUserRoomRequester(UpdateUserRoomRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            throw new NotImplementedException();
        }

        public override UpdateUserRoomResult FormResultFromPayload(HttpResponseObject payload)
        {
            throw new NotImplementedException();
        }
    }

    internal class RemoveFavoriteRequester : Requester<RemoveFavoriteRequest, RemoveFavoriteResult>
    {
        private readonly ISubject<bool> onSocialActionMade;

        public RemoveFavoriteRequester(RemoveFavoriteRequest requestObject, ISubject<bool> subject) : base(requestObject)
        {
            onSocialActionMade = subject;
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Delete(BackendUtil.GetFullUrl($"/api/v3/social/{RequestObject.userId}/favorite"));
        }

        public override RemoveFavoriteResult FormResultFromPayload(HttpResponseObject payload)
        {
            onSocialActionMade?.OnNext(true);
            var result = new RemoveFavoriteResult();

            return result;
        }
    }

    internal class SetFavoritesRequester : Requester<SetFavoriteRequest, SetFavoriteResult>
    {
        private readonly ISubject<bool> onSocialActionMade;

        public SetFavoritesRequester(SetFavoriteRequest requestObject, ISubject<bool> subject) : base(requestObject)
        {
            onSocialActionMade = subject;
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Post(BackendUtil.GetFullUrl($"/api/v3/social/{RequestObject.userId}/favorite"));
        }

        public override SetFavoriteResult FormResultFromPayload(HttpResponseObject payload)
        {
            onSocialActionMade?.OnNext(true);
            var result = new SetFavoriteResult();

            return result;
        }
    }

    internal class GetSocialRelationshipsRequester : Requester<GetSocialRelationshipsRequest, GetSocialRelationshipsResult>
    {
        public GetSocialRelationshipsRequester(GetSocialRelationshipsRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var dynamicUrl = $"/api/v3/social?user_constraint={RequestObject.userConstraint}&page={RequestObject.pageToRequest}";

            if (RequestObject.profileConstraints is { } profileConstraints && profileConstraints.Any())
            {
                dynamicUrl = profileConstraints.Aggregate(dynamicUrl, (current, constraint) => current + $"&profile_constraints[]={constraint}");
            }

            return BackendUtil.Get(BackendUtil.GetFullUrl(dynamicUrl));
        }

        public override GetSocialRelationshipsResult FormResultFromPayload(HttpResponseObject payload)
        {
            var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);

            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(payload.data.ToString());
            var otherPlayerProfiles = JsonConvert.DeserializeObject<OtherUserProfile[]>(dataMap["other_player_profiles"].ToString(), resConverter);
            var meta = JsonConvert.DeserializeObject<SocialRelationshipsResponseMeta>(dataMap["meta"].ToString());

            var result = new GetSocialRelationshipsResult
            {
                OtherPlayerProfiles = otherPlayerProfiles,
                CurrentPage = meta.current_page,
                HasNextPage = meta.current_page < meta.last_page
            };

            return result;
        }
    }

    internal class UnfollowUserRequester : Requester<UnfollowUserRequest, UnfollowUserResult>
    {
        private readonly ISubject<bool> onSocialActionMade;

        public UnfollowUserRequester(UnfollowUserRequest requestObject, ISubject<bool> subject) : base(requestObject)
        {
            onSocialActionMade = subject;
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Delete(BackendUtil.GetFullUrl($"/api/v3/social/{RequestObject.userId}/follow"));
        }

        public override UnfollowUserResult FormResultFromPayload(HttpResponseObject payload)
        {
            onSocialActionMade?.OnNext(true);
            var result = new UnfollowUserResult();

            return result;
        }
    }

    internal class FollowUserRequester : Requester<FollowUserRequest, FollowUserResult>
    {
        private readonly ISubject<bool> onSocialActionMade;

        public FollowUserRequester(FollowUserRequest requestObject, ISubject<bool> subject) : base(requestObject)
        {
            onSocialActionMade = subject;
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Post(BackendUtil.GetFullUrl($"/api/v3/social/{RequestObject.userId}/follow"));
        }

        public override FollowUserResult FormResultFromPayload(HttpResponseObject payload)
        {
            onSocialActionMade?.OnNext(true);
            var result = new FollowUserResult();

            return result;
        }
    }

    internal class FindUserRequester : Requester<FindUserRequest, FindUserResult>
    {
        public FindUserRequester(FindUserRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var dynamicUrl = $"/api/v3/social/search?keyword={RequestObject.keyword}";

            if (RequestObject.profileConstraints is { } profileConstraints && profileConstraints.Any())
            {
                dynamicUrl = profileConstraints.Aggregate(dynamicUrl, (current, constraint) => current + $"&profile_constraints[]={constraint}");
            }

            if (RequestObject.userConstraints is { } userConstraints && userConstraints.Any())
            {
                dynamicUrl = userConstraints.Aggregate(dynamicUrl, (current, constraint) => current + $"&user_constraints[]={constraint}");
            }

            return BackendUtil.Get(BackendUtil.GetFullUrl(dynamicUrl));
        }

        public override FindUserResult FormResultFromPayload(HttpResponseObject payload)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, object>>>(payload.data.ToString());
            var resConverter = new GenericServiceConverter<UserProfile>(BackendUtil.GlobalMapping);

            var friends = RequestObject.userConstraints.Contains("friends") ? JsonConvert.DeserializeObject<OtherUserProfile[]>(dataMap["friends"]["other_player_profiles"].ToString(), resConverter) : null;
            var followers = RequestObject.userConstraints.Contains("followers") ? JsonConvert.DeserializeObject<OtherUserProfile[]>(dataMap["followers"]["other_player_profiles"].ToString(), resConverter) : null;
            var following = RequestObject.userConstraints.Contains("following") ? JsonConvert.DeserializeObject<OtherUserProfile[]>(dataMap["following"]["other_player_profiles"].ToString(), resConverter) : null;
            var unrelated = RequestObject.userConstraints.Contains("unrelated") ? JsonConvert.DeserializeObject<OtherUserProfile[]>(dataMap["unrelated"]["other_player_profiles"].ToString(), resConverter) : null;

            var result = new FindUserResult
            {
                Friends = friends,
                Followers = followers,
                Following = following,
                Unrelated = unrelated
            };

            return result;
        }
    }
}
