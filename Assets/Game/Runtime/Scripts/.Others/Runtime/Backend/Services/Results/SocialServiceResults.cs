namespace Kumu.Kulitan.Backend
{
    public class FindUserResult : ResultBase
    {
        public OtherUserProfile[] Followers;
        public OtherUserProfile[] Following;
        public OtherUserProfile[] Friends;
        public OtherUserProfile[] Unrelated;
    }
    
    public class FollowUserResult : ResultBase
    {
    }

    public class UnfollowUserResult : ResultBase
    {
    }

    public class GetSocialRelationshipsResult : ResultBase
    {
        public OtherUserProfile[] OtherPlayerProfiles;
        public int CurrentPage;
        public bool HasNextPage;
    }

    public class SetFavoriteResult : ResultBase
    {
    }

    public class RemoveFavoriteResult : ResultBase
    {
    }

    public class UpdateUserRoomResult : ResultBase
    {
    }

    public class RecordUserCreatedRoomResult : ResultBase
    {
    }

    public class RecordUserEnteredRoomResult : ResultBase
    {
    }

    public class ClearUserRoomRecordResult : ResultBase
    {
    }

    public class GetOtherUserProfileResult : ResultBase
    {
        public OtherUserProfile otherUserProfile;
    }
}
