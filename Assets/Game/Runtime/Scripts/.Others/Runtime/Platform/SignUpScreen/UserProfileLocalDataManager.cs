using Kumu.Kulitan.Common;
using UniRx;

namespace Kumu.Kulitan.Backend
{
    public class UserProfileLocalDataManager : Singleton<UserProfileLocalDataManager>
    {
        private UserProfile userProfile;
        private readonly ISubject<UserProfile> userProfileUpdated = new Subject<UserProfile>();
        
        /// <summary>
        /// Fires when the local user profile is updated
        /// </summary>
        public ISubject<UserProfile> UserProfileUpdated => userProfileUpdated;

        public void UpdateLocalUserProfile(UserProfile userProfile)
        {
            this.userProfile = userProfile;
            userProfileUpdated.OnNext(userProfile);
        }

        public UserProfile GetLocalUserProfile()
        {
            return userProfile;
        }
    }
}
