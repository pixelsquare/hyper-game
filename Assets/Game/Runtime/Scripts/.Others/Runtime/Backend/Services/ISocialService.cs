using System.Threading.Tasks;
using UniRx;

namespace Kumu.Kulitan.Backend
{
    public interface ISocialService
    {
        public Task<ServiceResultWrapper<FindUserResult>> FindUserAsync(FindUserRequest request);

        public Task<ServiceResultWrapper<FollowUserResult>> FollowUserAsync(FollowUserRequest request);

        public Task<ServiceResultWrapper<UnfollowUserResult>> UnfollowUserAsync(UnfollowUserRequest request);

        public Task<ServiceResultWrapper<GetSocialRelationshipsResult>> GetSocialRelationshipsAsync(GetSocialRelationshipsRequest request);

        public Task<ServiceResultWrapper<SetFavoriteResult>> SetFavoriteAsync(SetFavoriteRequest request);

        public Task<ServiceResultWrapper<RemoveFavoriteResult>> RemoveFavoriteAsync(RemoveFavoriteRequest request);

        public Task<ServiceResultWrapper<UpdateUserRoomResult>> UpdateUserRoomAsync(UpdateUserRoomRequest request);

        public Task<ServiceResultWrapper<RecordUserCreatedRoomResult>> RecordUserCreatedRoomAsync(RecordUserCreatedRoomRequest request);

        public Task<ServiceResultWrapper<RecordUserEnteredRoomResult>> RecordUserEnteredRoomAsync(RecordUserEnteredRoomRequest request);

        public Task<ServiceResultWrapper<ClearUserRoomRecordResult>> ClearUserRoomRecordAsync(ClearUserRoomRecordRequest request);

        public Task<ServiceResultWrapper<GetOtherUserProfileResult>> GetOtherUserProfileAsync(GetOtherUserProfileRequest request);
        
        /// <summary>
        /// Fired when Follow/Unfollow/Favorite/Unfavorite actions are made
        /// </summary>
        public ISubject<bool> OnSocialActionMade { get; }
    }
}
