namespace Kumu.Kulitan.Backend
{
    public class CreateUserProfileResult : ResultBase
    {
        public UserProfile UserProfile { get; }

        public CreateUserProfileResult() { }

        public CreateUserProfileResult(UserProfile userProfile)
        {
            UserProfile = userProfile;
        }
    }

    public class GetUserProfileResult : ResultBase
    {
        public UserProfile UserProfile { get; }

        public GetUserProfileResult() { }

        public GetUserProfileResult(UserProfile userProfile)
        {
            UserProfile = userProfile;
        }
    }
}
