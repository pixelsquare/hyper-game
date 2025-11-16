using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Extensions;
using UniRx;

namespace Kumu.Kulitan.Backend
{
    public class MockedSocialService : ISocialService
    {
        [Flags]
        public enum ResultErrorFlags
        {
            None = 0,
            UnknownError = 0xb1 << 1,
            NetworkUnknownError = 0xb1 << 2,
            NetworkTimeoutError = 0xb1 << 3,
            NetworkUnreachableError = 0xb1 << 4,
            InvalidDataError = 0xb1 << 5, // Data provided is invalid
            AppInMaintenanceError = 0x01 << 6,
            RoomIdDoesNotExist = 0xb1 << 7
        }

        private readonly Dictionary<string, OtherUserProfile> otherUserProfiles = new();
        private readonly ISubject<bool> onSocialActionMade = new Subject<bool>();

        /// <summary>
        /// Creates a stateful list of mocked user profiles to simulate functions that change follower/following/friend/favorite states
        /// </summary>
        public MockedSocialService()
        {
            MockedServicesUtil.GetMockedNames(out var mockedNames);
            var rand = new Random();

            for (var i = 0; i < 20; i++)
            {
                var mockedProfile = new UserProfile
                {
                    accountId = MockedServicesUtil.GetRandomAccountId(),
                    ageRange = rand.Next(0, 4),
                    gender = rand.Next(0, 3),
                    hasLinkedKumuAccount = false,
                    mobile = "+639000000000",
                    nickName = mockedNames[i],
                    playerId = (uint)rand.Next(1000, 9999),
                    userName = MockedServicesUtil.CreateUserNameWithRandomDiscriminator(mockedNames[i]).ToString()
                };

                switch (i)
                {
                    case >= 17:
                        otherUserProfiles[mockedProfile.accountId] = WrapProfile(mockedProfile, SocialState.None);
                        break;

                    case >= 12:
                        otherUserProfiles[mockedProfile.accountId] = WrapProfile(mockedProfile, SocialState.Friends | SocialState.Favorite);
                        break;

                    case >= 7:
                        otherUserProfiles[mockedProfile.accountId] = WrapProfile(mockedProfile, SocialState.Follower);
                        break;

                    default:
                        otherUserProfiles[mockedProfile.accountId] = WrapProfile(mockedProfile, SocialState.Following);
                        break;
                }
            }
        }

        public int ResponseTimeInMilliseconds { get; set; }

        public ResultErrorFlags ErrorFlags { get; set; }

        public async Task<ServiceResultWrapper<FindUserResult>> FindUserAsync(FindUserRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<FindUserResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<FindUserResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<FindUserResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<FindUserResult>(MockedErrors.appInMaintenanceError);
            }

            var matchingKeywordProfiles = otherUserProfiles.Values.Where(t =>
                    t.profile.userName.ToString().Contains(request.keyword)).ToArray();

            var result = new FindUserResult
            {
                Friends = matchingKeywordProfiles.Where(t => t.social_state.HasFlag(SocialState.Friends)).ToArray(),
                Followers = matchingKeywordProfiles.Where(t => t.social_state.HasFlag(SocialState.Follower)).ToArray(),
                Following = matchingKeywordProfiles.Where(t => t.social_state.HasFlag(SocialState.Following)).ToArray(),
                Unrelated = matchingKeywordProfiles.Where(t => t.social_state.HasFlag(SocialState.None)).ToArray()
            };

            return new ServiceResultWrapper<FindUserResult>(result);
        }

        public async Task<ServiceResultWrapper<FollowUserResult>> FollowUserAsync(FollowUserRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<FollowUserResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<FollowUserResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<FollowUserResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<FollowUserResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new FollowUserResult();

            // Mocked operation
            if (otherUserProfiles.Values.First(t => t.profile.accountId == request.userId) is var userProfileMatch)
            {
                var isFavorite = userProfileMatch.social_state.HasFlag(SocialState.Favorite);
                userProfileMatch.social_state = userProfileMatch.social_state.HasFlag(SocialState.Follower) ? SocialState.Friends : SocialState.Following;
                if (isFavorite)
                {
                    userProfileMatch.social_state |= SocialState.Favorite;
                }

                otherUserProfiles[userProfileMatch.profile.accountId] = userProfileMatch;
            }

            onSocialActionMade.OnNext(true);
            return new ServiceResultWrapper<FollowUserResult>(result);
        }

        public async Task<ServiceResultWrapper<UnfollowUserResult>> UnfollowUserAsync(UnfollowUserRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<UnfollowUserResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<UnfollowUserResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<UnfollowUserResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<UnfollowUserResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new UnfollowUserResult();

            // Mocked operation
            if (otherUserProfiles.Values.First(t => t.profile.accountId == request.userId) is var userProfileMatch)
            {
                var isFavorite = userProfileMatch.social_state.HasFlag(SocialState.Favorite);
                userProfileMatch.social_state = userProfileMatch.social_state.HasFlag(SocialState.Friends) ? SocialState.Follower : SocialState.None;
                if (isFavorite)
                {
                    userProfileMatch.social_state |= SocialState.Favorite;
                }

                otherUserProfiles[userProfileMatch.profile.accountId] = userProfileMatch;
            }

            onSocialActionMade.OnNext(true);
            return new ServiceResultWrapper<UnfollowUserResult>(result);
        }

        public async Task<ServiceResultWrapper<GetSocialRelationshipsResult>> GetSocialRelationshipsAsync(GetSocialRelationshipsRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetSocialRelationshipsResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetSocialRelationshipsResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<GetSocialRelationshipsResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetSocialRelationshipsResult>(MockedErrors.appInMaintenanceError);
            }

            var profiles = request.userConstraint switch
            {
                "friends" => otherUserProfiles.Values.Where(t => t.social_state.HasFlag(SocialState.Friends)).ToArray(),
                "followers" => otherUserProfiles.Values.Where(t => t.social_state.HasFlag(SocialState.Follower) || t.social_state.HasFlag(SocialState.Friends)).ToArray(),
                "following" => otherUserProfiles.Values.Where(t => t.social_state.HasFlag(SocialState.Following) || t.social_state.HasFlag(SocialState.Friends)).ToArray(),
                _ => null
            };

            var result = new GetSocialRelationshipsResult
            {
                OtherPlayerProfiles = profiles
            };

            return new ServiceResultWrapper<GetSocialRelationshipsResult>(result);
        }

        public async Task<ServiceResultWrapper<SetFavoriteResult>> SetFavoriteAsync(SetFavoriteRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<SetFavoriteResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<SetFavoriteResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<SetFavoriteResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<SetFavoriteResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new SetFavoriteResult();

            // Mocked operation
            if (otherUserProfiles.Values.First(t => t.profile.accountId == request.userId) is var userProfileMatch)
            {
                userProfileMatch.social_state |= SocialState.Favorite;
                otherUserProfiles[userProfileMatch.profile.accountId] = userProfileMatch;
            }
            
            onSocialActionMade.OnNext(true);
            return new ServiceResultWrapper<SetFavoriteResult>(result);
        }

        public async Task<ServiceResultWrapper<RemoveFavoriteResult>> RemoveFavoriteAsync(RemoveFavoriteRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<RemoveFavoriteResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<RemoveFavoriteResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<RemoveFavoriteResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<RemoveFavoriteResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new RemoveFavoriteResult();

            // Mocked operation
            if (otherUserProfiles.Values.First(t => t.profile.accountId == request.userId) is var userProfileMatch)
            {
                userProfileMatch.social_state &= ~SocialState.Favorite;
                otherUserProfiles[userProfileMatch.profile.accountId] = userProfileMatch;
            }
            
            onSocialActionMade.OnNext(true);
            return new ServiceResultWrapper<RemoveFavoriteResult>(result);
        }

        public async Task<ServiceResultWrapper<UpdateUserRoomResult>> UpdateUserRoomAsync(UpdateUserRoomRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<UpdateUserRoomResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<UpdateUserRoomResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<UpdateUserRoomResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<UpdateUserRoomResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new UpdateUserRoomResult();
            return new ServiceResultWrapper<UpdateUserRoomResult>(result);
        }

        public async Task<ServiceResultWrapper<RecordUserCreatedRoomResult>> RecordUserCreatedRoomAsync(RecordUserCreatedRoomRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<RecordUserCreatedRoomResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<RecordUserCreatedRoomResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<RecordUserCreatedRoomResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<RecordUserCreatedRoomResult>(MockedErrors.appInMaintenanceError);
            }

            $"[MockedSocialService] User has created room {request.roomId} recorded.".Log();

            var result = new RecordUserCreatedRoomResult();
            return new ServiceResultWrapper<RecordUserCreatedRoomResult>(result);
        }

        public async Task<ServiceResultWrapper<RecordUserEnteredRoomResult>> RecordUserEnteredRoomAsync(RecordUserEnteredRoomRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<RecordUserEnteredRoomResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<RecordUserEnteredRoomResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<RecordUserEnteredRoomResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<RecordUserEnteredRoomResult>(MockedErrors.appInMaintenanceError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.RoomIdDoesNotExist))
            {
                return new ServiceResultWrapper<RecordUserEnteredRoomResult>(MockedErrors.roomIdDoesNotExistError);
            }

            $"[MockedSocialService] User has entered room {request.roomId} recorded.".Log();

            var result = new RecordUserEnteredRoomResult();
            return new ServiceResultWrapper<RecordUserEnteredRoomResult>(result);
        }

        public async Task<ServiceResultWrapper<ClearUserRoomRecordResult>> ClearUserRoomRecordAsync(ClearUserRoomRecordRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<ClearUserRoomRecordResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<ClearUserRoomRecordResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<ClearUserRoomRecordResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<ClearUserRoomRecordResult>(MockedErrors.appInMaintenanceError);
            }

            "[MockedSocialService] Cleared user room record.".Log();

            var result = new ClearUserRoomRecordResult();
            return new ServiceResultWrapper<ClearUserRoomRecordResult>(result);
        }

        public async Task<ServiceResultWrapper<GetOtherUserProfileResult>> GetOtherUserProfileAsync(GetOtherUserProfileRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetOtherUserProfileResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetOtherUserProfileResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<GetOtherUserProfileResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetOtherUserProfileResult>(MockedErrors.appInMaintenanceError);
            }

            "[MockedSocialService] Get other user profile success.".Log();

            var result = new GetOtherUserProfileResult();
            return new ServiceResultWrapper<GetOtherUserProfileResult>(result);
        }

        public ISubject<bool> OnSocialActionMade => onSocialActionMade;

        private bool TryGetNetworkError(out ServiceError error)
        {
            error = null;

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkUnreachableError))
            {
                error = ServiceErrors.networkUnreachableError;
                return true;
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkTimeoutError))
            {
                error = ServiceErrors.networkTimeoutError;
                return true;
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkUnknownError))
            {
                error = ServiceErrors.networkUnknownError;
                return true;
            }

            return false;
        }

        private static OtherUserProfile WrapProfile(UserProfile profile, SocialState state)
        {
            return new OtherUserProfile
            {
                profile = profile,
                social_state = state
            };
        }
    }
}
