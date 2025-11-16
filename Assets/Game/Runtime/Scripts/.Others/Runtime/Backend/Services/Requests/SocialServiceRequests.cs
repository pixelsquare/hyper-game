using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class FindUserRequest : RequestCommon
    {
        public string keyword;
        public string[] userConstraints; // multiple selections allowed but must be at least length = 1, also must be distinct
        public string[] profileConstraints; // can be empty, will return minimal profile in that case, also must be distinct
    }

    [Serializable]
    public class FollowUserRequest : RequestCommon
    {
        public string userId;
    }

    [Serializable]
    public class UnfollowUserRequest : RequestCommon
    {
        public string userId;
    }

    [Serializable]
    public class GetSocialRelationshipsRequest : RequestCommon
    {
        public string userConstraint; // only 1 selection allowed. Cannot select "unrelated".
        public string[] profileConstraints = Array.Empty<string>();
        public int pageToRequest;
    }

    [Serializable]
    public class SetFavoriteRequest : RequestCommon
    {
        public string userId;
    }

    [Serializable]
    public class RemoveFavoriteRequest : RequestCommon
    {
        public string userId;
    }

    [Serializable]
    public class UpdateUserRoomRequest : RequestCommon
    {
        public string userId;
        public string roomId;
    }

    [Serializable]
    public class RecordUserCreatedRoomRequest : RequestCommon
    {
        public string roomId;
        public bool friendsOnly;
    }

    [Serializable]
    public class RecordUserEnteredRoomRequest : RequestCommon
    {
        public string roomId;
    }

    [Serializable]
    public class ClearUserRoomRecordRequest : RequestCommon
    {
    }

    [Serializable]
    public class GetOtherUserProfileRequest : RequestCommon
    {
        public string userId;
        public string[] profileConstraints;
    }
}
