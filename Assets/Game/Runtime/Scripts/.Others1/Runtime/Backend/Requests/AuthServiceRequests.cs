using Firebase.Auth;

namespace Santelmo.Rinsurv.Backend
{
    public class SignInAnonymouslyRequest : RequestCommon
    {
    }

    public class SignOutRequest : RequestCommon
    {
    }

    public class SignInGoogleRequest : RequestCommon
    {
    }

    public class SignInFirebaseRequest : RequestCommon
    {
        public Credential Credential { get; }

        public SignInFirebaseRequest(Credential credential)
        {
            Credential = credential;
        }
    }

    public class SignInFacebookRequest : RequestCommon
    {
    }

    public class SignInEmailRequest : RequestCommon
    {
        public string Email { get; }
        public string Password { get; }
        
        public Credential Credential { get; }

        public SignInEmailRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public SignInEmailRequest(Credential credential)
        {
            Credential = credential;
        }
    }
    
    public class CreateUserWithEmailRequest : RequestCommon
    {
        public string Email { get; }
        public string Password { get; }

        public CreateUserWithEmailRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
    
    public class SendPasswordResetRequest : RequestCommon
    { 
        public string Email { get; }

        public SendPasswordResetRequest(string email)
        {
            Email = email;
        }
    }
}

