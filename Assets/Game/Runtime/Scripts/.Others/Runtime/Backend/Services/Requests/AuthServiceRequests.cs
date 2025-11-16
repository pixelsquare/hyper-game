using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class RegisterUserRequestOtpRequest : RequestCommon
    {
        public string mobile;
    }

    [Serializable]
    public class RegisterUserSendOtpRequest : RequestCommon
    {
        public string mobile;
        public string otp;
    }
    
    [Serializable]
    public class AutoSignInRequest : RequestCommon
    {
    }

    [Serializable]
    public class LoginUserRequestOtpRequest : RequestCommon
    {
        public string mobile;
        public string resendingToken;
    }

    [Serializable]
    public class LoginUserSendOtpRequest : RequestCommon
    {
        public string mobile;
        public string otp;
    }

    [Serializable]
    public class LinkUserRequestOtpRequest : RequestCommon
    {
        public string username;
    }

    [Serializable]
    public class LinkUserSendOtpRequest : RequestCommon
    {
        public string username;
        public string otp;
    }

    [Serializable]
    public class RefreshLinkRequestOtpRequest : RequestCommon
    {
    }

    [Serializable]
    public class RefreshLinkSendOtpRequest : RequestCommon
    {
        public string otp;
    }

    [Serializable]
    public class UnlinkUserRequest : RequestCommon
    {
    }

    [Serializable]
    public class LogoutUserRequest : RequestCommon
    {
        // empty
    }

    [Serializable]
    public class ResolvePlayerRequest : RequestCommon
    {
    }
    
    [Serializable]
    public class GetBadgeRequest : RequestCommon
    {
    }

    [Serializable]
    public class SignInRequest : RequestCommon
    {
        public object credential;
    }
}
