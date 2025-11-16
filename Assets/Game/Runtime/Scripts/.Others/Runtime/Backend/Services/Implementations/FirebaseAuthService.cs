using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using KumuBackend = Kumu.Kulitan.Backend;
using Newtonsoft.Json;
using AppleAuth;
using AppleAuth.Enums;
using UnityEngine;
using AppleAuth.Extensions;
using Firebase;
using Google;

public class FirebaseAuthService : IAuthService
{
    public static IAppleAuthManager appleAuthManager;
    public static readonly FirebaseAuth firebaseAuth = FirebaseAuth.DefaultInstance;
    public static FirebaseUser firebaseUser;
    public static ForceResendingToken resendingToken;
    public static string verificationId;

    public static readonly string AppleUserIdKey = "appleIdKey";
    public static readonly ITokenCache<string> AppleUserIdCache = new EncryptedAppleIdKeyCachePrefs(AppleUserIdKey);

    public async Task<string> GetUserTokenAsync()
    {
        string userToken = null;

        await firebaseAuth.CurrentUser.TokenAsync(true).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                "Failed to fetch Firebase token.".LogError();
                userToken = null;
                return;
            }
            
            "Firebase token fetched.".Log();
            userToken = task.Result;
        });
        
        BackendUtil.SetAuthToken(userToken);
 
        return userToken;
    }
    
    public async Task<bool> IsUserValid()
    {
        var userToken = await GetUserTokenAsync();
        return !string.IsNullOrEmpty(userToken);
    }
    
    public async Task<ServiceResultWrapper<AutoSignInResult>> AutoSignInAsync(AutoSignInRequest request)
    {
        var result = new AutoSignInResult();
        firebaseUser = firebaseAuth.CurrentUser;

        if (firebaseUser != null && await IsUserValid())
        {
            if(appleAuthManager != null)
            {
                var isAppleUserSignedIn = await IsAppleUserSignedIn();
                if (!isAppleUserSignedIn)
                {
                    "apple id credential is revoked or not found, logging out".Log();
                    return new ServiceResultWrapper<AutoSignInResult>(ServiceErrors.unknownError);
                }
            }
            
            var resolvePlayerResult = await ResolvePlayerAsync(new ResolvePlayerRequest());

            if (resolvePlayerResult.HasError)
            {
                return new ServiceResultWrapper<AutoSignInResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "resolve player request failed"));
            }
            
            return new ServiceResultWrapper<AutoSignInResult>(result);
        }

        "auto-sign in failed".Log();
        return new ServiceResultWrapper<AutoSignInResult>(ServiceErrors.unknownError);
    }
    
    public async Task<ServiceResultWrapper<KumuBackend.SignInResult>> SignInAsync(SignInRequest request)
    {
        var errorMsg = "";
        var hasError = false;

        var cred = (Credential)request.credential;
        
        await firebaseAuth.SignInAndRetrieveDataWithCredentialAsync(cred).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                hasError = true;
                errorMsg = "SignInAndRetrieveDataWithCredentialAsync was canceled.";
                errorMsg.LogError();
                return;
            }

            if (task.IsFaulted)
            {
                hasError = true;
                errorMsg = "SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception;
                errorMsg.LogError();
                return;
            }

            firebaseUser = task.Result.User;
            $"User signed in successfully: {task.Result.User.DisplayName} {task.Result.User.Email}".Log();
        });

        if (hasError)
        {
            return new ServiceResultWrapper<KumuBackend.SignInResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, errorMsg));
        }
        
        await GetUserTokenAsync();
   
        var result = new KumuBackend.SignInResult();
        return new ServiceResultWrapper<KumuBackend.SignInResult>(result);
    }

    public async Task<ServiceResultWrapper<LoginUserRequestOtpResult>> LoginUserRequestOtpAsync(LoginUserRequestOtpRequest request)
    {   
        var provider = PhoneAuthProvider.GetInstance(firebaseAuth);
        var serviceResult = new ServiceResultWrapper<LoginUserRequestOtpResult>(ServiceErrors.unknownError);
        var tcs = new TaskCompletionSource<ServiceResultWrapper<LoginUserRequestOtpResult>>();
        provider.VerifyPhoneNumber(request.mobile, 60000, resendingToken,
            verificationCompleted: (credential) => {
                $"verification completed".Log();
                tcs.SetResult(serviceResult); // ??? why return an error response?
            },
            verificationFailed: (error) => {
                $"verification failed {error}".Log();
                tcs.SetResult(new ServiceResultWrapper<LoginUserRequestOtpResult>(new ServiceError(ServiceErrorCodes.FIREBASE_ERROR, error)));
            },
            codeSent: (id, token) => {
                $"code sent".Log();
                verificationId = id;
                resendingToken = token;
                var result = new LoginUserRequestOtpResult();
                tcs.SetResult(new ServiceResultWrapper<LoginUserRequestOtpResult>(result));
            });

        var responseData = await tcs.Task;
        return responseData;
    }

    public async Task<ServiceResultWrapper<LoginUserSendOtpResult>> LoginUserSendOtpAsync(LoginUserSendOtpRequest request)
    {
        var provider = PhoneAuthProvider.GetInstance(firebaseAuth);
        var credential = provider.GetCredential(verificationId, request.otp);

        var hasError = false;
        var errorCode = 0;
        var errorMessage = "";
        
        await firebaseAuth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                hasError = true;
                errorCode = GetErrorCode(task.Exception);
                errorMessage = GetErrorMessage(task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;
            firebaseUser = newUser;
            "User signed in successfully".Log();
            $"Phone number: {newUser.PhoneNumber}".Log();
        });

        if (hasError)
        {
            if ((AuthError)errorCode == AuthError.InvalidVerificationCode)
            {
                return new ServiceResultWrapper<LoginUserSendOtpResult>(new ServiceError(ServiceErrorCodes.INVALID_OTP, errorMessage));
            }

            if (errorCode == ServiceErrorCodes.FIREBASE_ERROR)
            {
                return new ServiceResultWrapper<LoginUserSendOtpResult>(new ServiceError(ServiceErrorCodes.FIREBASE_ERROR, errorMessage));
            }

            return new ServiceResultWrapper<LoginUserSendOtpResult>(
                new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, errorMessage));
        }


        await GetUserTokenAsync();
        
        var result = new LoginUserSendOtpResult();
        return new ServiceResultWrapper<LoginUserSendOtpResult>(result);
    }

    #region Obsolete Commands
    public Task<ServiceResultWrapper<RegisterUserRequestOtpResult>> RegisterUserRequestOtpAsync(RegisterUserRequestOtpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResultWrapper<RegisterUserSendOtpResult>> RegisterUserSendOtpAsync(RegisterUserSendOtpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResultWrapper<LinkUserRequestOtpResult>> LinkUserRequestOtpAsync(LinkUserRequestOtpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResultWrapper<LinkUserSendOtpResult>> LinkUserSendOtpAsync(LinkUserSendOtpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResultWrapper<RefreshLinkRequestOtpResult>> RefreshLinkRequestOtpAsync(RefreshLinkRequestOtpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResultWrapper<RefreshLinkSendOtpResult>> RefreshLinkSendOtpAsync(RefreshLinkSendOtpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResultWrapper<UnlinkUserResult>> UnlinkUserAsync(UnlinkUserRequest request)
    {
        throw new NotImplementedException();
    }
    #endregion

    public Task<ServiceResultWrapper<LogoutUserResult>> LogOutUserAsync(LogoutUserRequest request)
    {
        AppleUserIdCache.Delete();
        if (GoogleSignIn.Configuration != null)
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }
        firebaseAuth.SignOut();
        return Task.FromResult(new ServiceResultWrapper<LogoutUserResult>(new LogoutUserResult()));
    }
    
    public async Task<ServiceResultWrapper<ResolvePlayerResult>> ResolvePlayerAsync(ResolvePlayerRequest request)
    {
        var fullUrl = BackendUtil.GetFullUrl("/api/v3/player/auth/resolve");
        var reqConverter = new GenericServiceConverter<ResolvePlayerRequest>(BackendUtil.GlobalMapping);
        var reqBody = JsonConvert.SerializeObject(request, reqConverter);
        using var req = BackendUtil.Post(fullUrl, reqBody);

        try
        {
            using (var tokenSource = CTokenSource.Create())
            {
                if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                {
                    return new ServiceResultWrapper<ResolvePlayerResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                }
            }

            if (!BackendUtil.TryGetPayload<ResolvePlayerResult>(req, out var payload, out var errorWrapper))
            {
                return errorWrapper;
            }

            if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
            {
                return await TokenRefresh.RefreshTokensAsync(() => ResolvePlayerAsync(request));
            }

            var result = new ResolvePlayerResult();
            return new ServiceResultWrapper<ResolvePlayerResult>(result);
        }
        catch (Exception e)
        {
            return new ServiceResultWrapper<ResolvePlayerResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Resolve player failed", e.Message));
        }
    }

    public async Task<ServiceResultWrapper<GetBadgeResult>> GetBadgeAsync(GetBadgeRequest request)
    {
        var fullUrl = BackendUtil.GetFullUrl("/api/v3/player/auth/badge");
        using var req = BackendUtil.Get(fullUrl);

        try
        {
            using (var tokenSource = CTokenSource.Create())
            {
                if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                {
                    return new ServiceResultWrapper<GetBadgeResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                }
            }

            if (!BackendUtil.TryGetPayload<GetBadgeResult>(req, out var payload, out var errorWrapper))
            {
                return errorWrapper;
            }

            if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
            {
                return await TokenRefresh.RefreshTokensAsync(() => GetBadgeAsync(request));
            }

            var result = JsonConvert.DeserializeObject<GetBadgeResult>(payload.data.ToString());

            return new ServiceResultWrapper<GetBadgeResult>(result);
        }
        catch (Exception e)
        {
            return new ServiceResultWrapper<GetBadgeResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Get badge failed", e.Message));
        }
    }

    private async Task<bool> IsAppleUserSignedIn()
    {
        // apple sign in is not compatible with this device
        if (appleAuthManager == null)
        {
            "apple sign in is not compatible with this device".Log();
            return true;
        }

        // user has not signed in with apple
        if (!AppleUserIdCache.IsValid())
        {
            "user has not used apple sign-in".Log();
            return true;
        }

        var appleUserId = AppleUserIdCache.LoadToken();

        var tcs = new TaskCompletionSource<bool>();

        appleAuthManager.GetCredentialState(
            appleUserId,
            state =>
            {
                switch (state)
                {
                    // If it's authorized, login with that user id
                    case CredentialState.Authorized:
                        tcs.SetResult(true);
                        return;

                    // If it was revoked, or not found, we need a new sign in with apple attempt
                    // Discard previous apple user id
                    case CredentialState.Revoked:
                    case CredentialState.NotFound:
                        AppleUserIdCache.Delete();
                        tcs.SetResult(false);
                        return;
                }
            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                $"Error while trying to get credential state {authorizationErrorCode} {error}".LogWarning();
                tcs.SetResult(false);

            });

        var result = await tcs.Task;
        $"{result} this is the result.".Log();
        return result;
    }
    
    private static int GetErrorCode(AggregateException exception)
    {
        if (exception == null)
        {
            return 0;
        }

        var fbEx = exception.GetBaseException() as FirebaseException;
        if (fbEx is FirebaseException firebaseEx)
        {
            return ServiceErrorCodes.FIREBASE_ERROR;
        }

        "not a firebase exception".Log();
        return 0;
    }

    private static string GetErrorMessage(AggregateException exception)
    {
        if (exception == null)
        {
            return "unknown error";
        }

        var fbEx = exception.GetBaseException() as FirebaseException;
        if (fbEx is not FirebaseException firebaseEx)
        {
            "not a firebase exception".Log();
            return exception.ToString();
        }

        var errorCode = (AuthError)firebaseEx.ErrorCode;
        return GetErrorMessage(errorCode);
    }

    private static string GetErrorMessage(AuthError errorCode)
    {
        $"error code is {errorCode}".LogError();
        string message;
        switch (errorCode)
        {
            case AuthError.InvalidVerificationCode:
                message = "Code is invalid.";
                break;
            case AuthError.ExpiredActionCode:
            case AuthError.SessionExpired:
                message = "Code has expired.";
                break;
            case AuthError.QuotaExceeded:
                message = "Too many requests. Please try again later.";
                break;
            default:
                message = "Unknown error";
                break;
        }

        return message;
    }
}
