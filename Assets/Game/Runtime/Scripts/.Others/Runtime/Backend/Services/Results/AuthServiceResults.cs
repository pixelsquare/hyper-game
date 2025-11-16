namespace Kumu.Kulitan.Backend
{
    public class ServiceResultWrapper<T> where T : ResultBase
    {
        public T Result { get; }
        
        public ServiceError Error { get; }

        public bool HasError => Error != null;

        public ServiceResultWrapper(T result)
        {
            Result = result;
        }

        public ServiceResultWrapper(ServiceError error)
        {
            Error = error;
        }
    }
    
    public abstract class ResultBase
    {
    }

    public class RegisterUserRequestOtpResult : ResultBase
    {
    }

    public class RegisterUserSendOtpResult : ResultBase
    {
    }

    public class LoginUserRequestOtpResult : ResultBase
    {
    }
    
    public class AutoSignInResult : ResultBase
    {
        public string authToken;
    }

    public class LoginUserSendOtpResult : ResultBase
    {
    }

    public class LinkUserRequestOtpResult : ResultBase
    {
    }

    public class LinkUserSendOtpResult : ResultBase
    {
    }

    public class RefreshLinkRequestOtpResult : ResultBase
    {
    }

    public class RefreshLinkSendOtpResult : ResultBase
    {
    }

    public class UnlinkUserResult : ResultBase
    {
    }

    public class LogoutUserResult : ResultBase
    {
    }
    
    public class ResolvePlayerResult : ResultBase
    {
    }

    public class GetBadgeResult : ResultBase
    {
        public string badge;
    }
    
    public class SignInResult : ResultBase
    {
    }

}
