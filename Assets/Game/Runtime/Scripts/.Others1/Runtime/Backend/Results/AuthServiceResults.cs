namespace Santelmo.Rinsurv.Backend
{
    public class SignInAnonymouslyResult : ResultBase
    {
        public UserInfo UserInfo { get; internal set; }
    }

    public class SignOutResult : ResultBase
    {
    }

    public class SignInGoogleResult : ResultBase
    {
        public UserInfo UserInfo { get; internal set; }
    }

    public class SignInFirebaseResult : ResultBase
    {
        public UserInfo UserInfo { get; internal set; }
    }

    public class SignInFacebookResult : ResultBase
    {
        public UserInfo UserInfo { get; internal set; }
    }
    
    public class SignInEmailResult : ResultBase
    {
        public UserInfo UserInfo { get; internal set; }
    }

    public class CreateUserWithEmailResult : ResultBase
    {
        public UserInfo UserInfo { get; internal set; }
    }

    public class SendEmailVerificationResult : ResultBase
    {
        public bool EmailSuccessful { get; internal set; }
    }   
    
    public class SendPasswordResetResult : ResultBase
    {
        public bool EmailSuccessful { get; internal set; }
    }
}
