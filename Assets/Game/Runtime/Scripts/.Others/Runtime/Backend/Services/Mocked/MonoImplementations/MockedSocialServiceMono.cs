using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedSocialServiceMono : MockedServiceMono, ISocialService
    {
        [SerializeField] private MockedSocialService.ResultErrorFlags errorFlags;

        [SerializeField] private int responseTimeInMilliseconds = 1;

        [SerializeField] private string roomId;

        private MockedSocialService service;

        public string RoomID => roomId;

        private void Awake()
        {
            service = new MockedSocialService();
        }

        public Task<ServiceResultWrapper<FindUserResult>> FindUserAsync(FindUserRequest request)
        {
            ConfigService();
            return service.FindUserAsync(request);
        }

        public Task<ServiceResultWrapper<FollowUserResult>> FollowUserAsync(FollowUserRequest request)
        {
            ConfigService();
            return service.FollowUserAsync(request);
        }

        public Task<ServiceResultWrapper<UnfollowUserResult>> UnfollowUserAsync(UnfollowUserRequest request)
        {
            ConfigService();
            return service.UnfollowUserAsync(request);
        }

        public Task<ServiceResultWrapper<GetSocialRelationshipsResult>> GetSocialRelationshipsAsync(GetSocialRelationshipsRequest request)
        {
            ConfigService();
            return service.GetSocialRelationshipsAsync(request);
        }

        public Task<ServiceResultWrapper<SetFavoriteResult>> SetFavoriteAsync(SetFavoriteRequest request)
        {
            ConfigService();
            return service.SetFavoriteAsync(request);
        }

        public Task<ServiceResultWrapper<RemoveFavoriteResult>> RemoveFavoriteAsync(RemoveFavoriteRequest request)
        {
            ConfigService();
            return service.RemoveFavoriteAsync(request);
        }

        public Task<ServiceResultWrapper<UpdateUserRoomResult>> UpdateUserRoomAsync(UpdateUserRoomRequest request)
        {
            ConfigService();
            return service.UpdateUserRoomAsync(request);
        }

        public Task<ServiceResultWrapper<RecordUserCreatedRoomResult>> RecordUserCreatedRoomAsync(RecordUserCreatedRoomRequest request)
        {
            ConfigService();
            return service.RecordUserCreatedRoomAsync(request);
        }

        public Task<ServiceResultWrapper<RecordUserEnteredRoomResult>> RecordUserEnteredRoomAsync(RecordUserEnteredRoomRequest request)
        {
            ConfigService();
            return service.RecordUserEnteredRoomAsync(request);
        }

        public Task<ServiceResultWrapper<ClearUserRoomRecordResult>> ClearUserRoomRecordAsync(ClearUserRoomRecordRequest request)
        {
            ConfigService();
            return service.ClearUserRoomRecordAsync(request);
        }

        public Task<ServiceResultWrapper<GetOtherUserProfileResult>> GetOtherUserProfileAsync(GetOtherUserProfileRequest request)
        {
            ConfigService();
            return service.GetOtherUserProfileAsync(request);
        }

        public ISubject<bool> OnSocialActionMade => service.OnSocialActionMade;

        private void ConfigService()
        {
            service.ErrorFlags = errorFlags;
            service.ResponseTimeInMilliseconds = responseTimeInMilliseconds;
        }
    }
}
