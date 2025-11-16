using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public struct UserProfile
    {
        public string accountId;
        public uint playerId;
        public string userName;
        public string nickName;
        public string mobile;
        public string roomId;

        /// <summary>
        /// 0 = 13 & below, 1 = 14 - 17, 2 = 18 - 24, 4 = 25 - 35, 5 = 36 & above
        /// </summary>
        public int? ageRange;

        /// <summary>
        /// 0 = unspecified, 1 = male, 2 = female, 3 = non-binary
        /// </summary>
        public int? gender;

        /// <summary>
        /// Indicates if this account has a linked Kumu account.
        /// </summary>
        public bool hasLinkedKumuAccount;

        public int? followingCount;
        public int? followerCount;
        public int? friendCount;

        /// <remarks>
        /// The display count has to take into account the number of friends the user has.
        /// </remarks>
        public int FollowingCountToDisplay => (followingCount ?? 0) + (friendCount ?? 0);
        /// <remarks>
        /// The display count has to take into account the number of friends the user has.
        /// </remarks>
        public int FollowerCountToDisplay => (followerCount ?? 0) + (friendCount ?? 0);
    }
}
