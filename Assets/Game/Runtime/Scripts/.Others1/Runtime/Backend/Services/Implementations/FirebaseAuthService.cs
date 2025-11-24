using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Facebook.Unity;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using UnityEngine;

namespace Santelmo.Rinsurv.Backend
{
    public class FirebaseLoginService : IAuthService
    {
        public string UserId => _firebaseAuth.CurrentUser.UserId;
        public bool IsAlreadySigned => _firebaseAuth.CurrentUser != null;

        private FirebaseUser _firebaseUser;
        private CancellationTokenSource cts;
        private readonly FirebaseAuth _firebaseAuth = FirebaseAuth.DefaultInstance;

        /// <summary>
        /// Instantiates a Google Sign In pop-up gets the Google token then it proceeds to sign you in Firebase.
        /// </summary>
        /// <param name="request">Setting up the Configuration parameter is needed</param>
        /// <returns>This returns a SignInGoogleResult with a <see cref="UserInfo"/>. </returns>
        public async UniTask<ServiceResultWrapper<SignInGoogleResult>> SignInGoogleAsync(SignInGoogleRequest request)
        {
            try
            {
                var error = CheckInternetAvailability();

                if (error != null)
                {
                    return new ServiceResultWrapper<SignInGoogleResult>(error);
                }

                UserInfo userInfo = null;
                var requestFailed = false;
                _firebaseUser = _firebaseAuth.CurrentUser;
                
                if (IsAlreadySigned)
                {
                    // Check if we're already signed up.
                    userInfo = UserInfo.FromFirebaseUser(_firebaseUser);
                }
                else
                {
                    using (cts = new CancellationTokenSource(TimeSpan.FromSeconds(BackendUtil.TimeoutSec)))
                    {
                        GoogleSignIn.Configuration = AuthUtil.GetGoogleConfig();
                        GoogleSignIn.DefaultInstance.EnableDebugLogging(true);
                        var signIn = GoogleSignIn.DefaultInstance.SignIn();
                        
                        // A way we could fetch and return the error and the result inside the google task.
                        var taskComplete = new TaskCompletionSource<SignInGoogleResult>();
                        
                        // Attempt sign in with google
                        await signIn.ContinueWithOnMainThread(async task =>
                        {
                            requestFailed = task.IsCanceled || task.IsFaulted;

                            if (requestFailed)
                            {
                                Debug.LogError($"Request failed: {task.Exception?.Message}");
                                if (task.Exception != null) 
                                    taskComplete.SetException(task.Exception);
                                return;
                            }

                            userInfo = UserInfo.FromGoogleSignInUser(task.Result);
                            var firebaseResult = await SignInFirebaseAsync(new SignInFirebaseRequest(AuthUtil.GetGoogleCredentials(userInfo.GoogleToken)));
                            userInfo = firebaseResult.Result.UserInfo;
                            taskComplete.SetResult(new SignInGoogleResult{UserInfo = userInfo});
                            
                        }).AsUniTask().AttachExternalCancellation(cts.Token);
                        
                        await taskComplete.Task;

                        if (taskComplete.Task.Exception != null && !string.IsNullOrEmpty(taskComplete.Task.Exception.Message))
                        {
                            return new ServiceResultWrapper<SignInGoogleResult>(new ServiceError(0, "Email Sign In Failed", taskComplete.Task.Exception.Message));
                        }

                    }
                }

                return new ServiceResultWrapper<SignInGoogleResult>(new SignInGoogleResult
                    {UserInfo = userInfo});
            }
            catch (Exception e)
            {
                return new ServiceResultWrapper<SignInGoogleResult>(new ServiceError(0, "Google Sign In Failed.", e.Message));
            }
        }
        
        /// <summary>
        /// Opens up a Facebook sign-in window. gets the Facebook token then it proceeds to sign you in Firebase.
        /// </summary>
        /// <param name="request">No required setup in request for now.</param>
        /// <returns>This returns a SignInFacebookResult with a <see cref="UserInfo"/>. </returns>
        public async UniTask<ServiceResultWrapper<SignInFacebookResult>> SignInFacebookAsync(SignInFacebookRequest request)
        {
            try
            {
                var error = CheckInternetAvailability();

                if (error != null)
                {
                    return new ServiceResultWrapper<SignInFacebookResult>(error);
                }

                UserInfo userInfo = null;
                _firebaseUser = _firebaseAuth.CurrentUser;

                if (IsAlreadySigned)
                {
                    // Check if we're already signed up.
                    userInfo = UserInfo.FromFirebaseUser(_firebaseUser);
                }
                else
                {
                    using (cts = new CancellationTokenSource(TimeSpan.FromSeconds(BackendUtil.TimeoutSec)))
                    {
                        var perms = new List<string>(){"email"};
                        
                        // A way we could fetch and return the error and the result inside the facebook task.
                        var taskComplete = new TaskCompletionSource<SignInFacebookResult>();
                        FB.LogInWithReadPermissions(perms, async result =>
                        {
                            if (FB.IsLoggedIn)
                            {
                                var accessToken = AccessToken.CurrentAccessToken;
                                
                                var firebaseResult = await SignInFirebaseAsync(new SignInFirebaseRequest(AuthUtil.GetFacebookCredentials(accessToken.TokenString)));
                                userInfo = firebaseResult.Result.UserInfo;
                                taskComplete.SetResult(new SignInFacebookResult{UserInfo = userInfo});
                            } 
                            else 
                            {
                                Debug.Log($"User cancelled login: {result?.Error}");
                                var exception = new Exception(result?.Error);
                                taskComplete.SetException(exception);
                            }
                        });
                        
                        await taskComplete.Task;

                       if (taskComplete.Task.Exception != null && !string.IsNullOrEmpty(taskComplete.Task.Exception.Message))
                       {
                           return new ServiceResultWrapper<SignInFacebookResult>(new ServiceError(0, "Facebook Sign In Failed", taskComplete.Task.Exception.Message));
                       }
                    }
                }

                return new ServiceResultWrapper<SignInFacebookResult>(new SignInFacebookResult {UserInfo = userInfo});
            }
            catch (Exception e)
            {
                return new ServiceResultWrapper<SignInFacebookResult>(new ServiceError(0, "Facebook Sign In Failed.", e.Message));
            }
        }
        
        /// <summary>
        /// This signs you in via email, This will not work if the user isn't registered yet.
        /// Use <see cref="CreateUserWithEmailAsync"/> to create, send email verification and auto sign in to Firebase
        /// </summary>
        /// <param name="request">In SignInEmailRequest, Email and Password should be configured</param>
        /// <returns>This returns a SignInEmailResult with a <see cref="UserInfo"/>.</returns>
        public async UniTask<ServiceResultWrapper<SignInEmailResult>> SignInEmailAsync(SignInEmailRequest request)
        {
            try
            {
                var error = CheckInternetAvailability();

                if (error != null)
                {
                    return new ServiceResultWrapper<SignInEmailResult>(error);
                }

                UserInfo userInfo = null;
                var requestFailed = false;
                _firebaseUser = _firebaseAuth.CurrentUser;
                
                //TODO: Find a way to update Firebase User's IsVerified when it's already logged in.
                if (IsAlreadySigned)
                {
                    // Check if we're already signed up.
                    userInfo = UserInfo.FromFirebaseUser(_firebaseUser);
                }
                else
                {
                    using (cts = new CancellationTokenSource(TimeSpan.FromSeconds(BackendUtil.TimeoutSec)))
                    {
                        var credential = request.Credential ?? AuthUtil.GetEmailCredentials(request.Email, request.Password);
                        
                        // A way we could fetch and return the error and the result inside the firebase task.
                        var taskComplete = new TaskCompletionSource<SignInEmailResult>();
                        
                        await _firebaseAuth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                        {
                            requestFailed = task.IsCanceled || task.IsFaulted;

                            if (requestFailed)
                            {
                                Debug.LogError($"Request failed: {task.Exception?.Message}");
                                if (task.Exception != null) 
                                    taskComplete.SetException(task.Exception);
                                
                                return;
                            }
                            
                            _firebaseUser = task.Result.User;
                            userInfo = UserInfo.FromFirebaseUser(_firebaseUser);
                            taskComplete.SetResult(new SignInEmailResult{UserInfo = userInfo});
                            
                        }).AsUniTask().AttachExternalCancellation(cts.Token);;
                        await taskComplete.Task;

                        if (taskComplete.Task.Exception != null && !string.IsNullOrEmpty(taskComplete.Task.Exception.Message))
                        {
                            return new ServiceResultWrapper<SignInEmailResult>(new ServiceError(0, "Email Sign In Failed", taskComplete.Task.Exception.Message));
                        }

                        if (_firebaseUser.IsEmailVerified)
                            return new ServiceResultWrapper<SignInEmailResult>(new SignInEmailResult {UserInfo = userInfo});
                        
                        var emailVerification = await SendEmailVerificationAsync();

                        if (emailVerification.Error != null && !string.IsNullOrEmpty(emailVerification.Error.Message))
                        {
                            return new ServiceResultWrapper<SignInEmailResult>(new ServiceError(0,
                                emailVerification.Error.Message, emailVerification.Error.RawError));
                        }
                    }
                    
                }

                return new ServiceResultWrapper<SignInEmailResult>(new SignInEmailResult
                    {UserInfo = userInfo});
            }
            catch (Exception e)
            {
                return new ServiceResultWrapper<SignInEmailResult>(new ServiceError(0, "Email Sign In Failed.", e.Message));
            }
        }
        
        /// <summary>
        /// Used to sign in to firebase. Credentials must be first setup depending on which type of sign in.
        /// </summary>
        /// <param name="request">Must get credentials via <see cref="GetFirebaseCredentials"/>, a token depending on which platform is needed.</param>
        /// <returns>Returns a SignInFirebaseResult which contains a UserInfo</returns>
        public async UniTask<ServiceResultWrapper<SignInFirebaseResult>> SignInFirebaseAsync(SignInFirebaseRequest request)
        {
            try
            {
                var error = CheckInternetAvailability();

                if (error != null)
                {
                    return new ServiceResultWrapper<SignInFirebaseResult>(error);
                }

                UserInfo userInfo = null;
                var requestFailed = false;
                _firebaseUser = _firebaseAuth.CurrentUser;

                if (IsAlreadySigned)
                {
                    // Check if we're already signed up.
                    userInfo = UserInfo.FromFirebaseUser(_firebaseUser);
                }
                else
                {
                    using (cts = new CancellationTokenSource(TimeSpan.FromSeconds(BackendUtil.TimeoutSec)))
                    {
                        var credential = request.Credential;
                        
                        // A way we could fetch and return the error and the result inside the firebase task.
                        var taskComplete = new TaskCompletionSource<SignInFirebaseResult>();

                        // Validate credentials
                        await _firebaseAuth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                        {
                            requestFailed = task.IsCanceled || task.IsFaulted;
                            if (requestFailed)
                            {
                                Debug.LogError($"Request failed: {task.Exception?.Message}");
                                if (task.Exception != null) 
                                    taskComplete.SetException(task.Exception);
                                
                                return;
                            }

                            _firebaseUser = task.Result;
                            userInfo = UserInfo.FromFirebaseUser(_firebaseUser);
                            taskComplete.SetResult(new SignInFirebaseResult{UserInfo = userInfo});
                            
                        }).AsUniTask().AttachExternalCancellation(cts.Token);
                        await taskComplete.Task;
                        
                        if (taskComplete.Task.Exception != null && !string.IsNullOrEmpty(taskComplete.Task.Exception.Message))
                        {
                            return new ServiceResultWrapper<SignInFirebaseResult>(new ServiceError(0, "Firebase Sign In Failed", taskComplete.Task.Exception.Message));
                        }
                    }
                }
                
                if (requestFailed || !await IsUserValid())
                {
                    return new ServiceResultWrapper<SignInFirebaseResult>(ServiceErrorHelper.UnknownError);
                }

                return new ServiceResultWrapper<SignInFirebaseResult>(new SignInFirebaseResult {UserInfo = userInfo});
            }
            catch (Exception e)
            {
                return new ServiceResultWrapper<SignInFirebaseResult>(new ServiceError(0, "Firebase Sign In Failed.",
                    e.Message));
            }
        }
        
        public async UniTask<ServiceResultWrapper<SignInAnonymouslyResult>> SignInAnonymouslyAsync(SignInAnonymouslyRequest request)
        {
            try
            {
                var error = CheckInternetAvailability();

                if (error != null)
                {
                    return new ServiceResultWrapper<SignInAnonymouslyResult>(error);
                }

                UserInfo userInfo = null;
                var requestFailed = false;
                _firebaseUser = _firebaseAuth.CurrentUser;

                if (IsAlreadySigned)
                {
                    // Check if we're already signed up.
                    userInfo = UserInfo.FromFirebaseUser(_firebaseUser);
                }
                else
                {
                    using (cts = new CancellationTokenSource(TimeSpan.FromSeconds(BackendUtil.TimeoutSec)))
                    {
                        await _firebaseAuth.SignInAnonymouslyAsync().ContinueWith(task =>
                        {
                            requestFailed = task.IsCanceled || task.IsFaulted;

                            if (requestFailed)
                            {
                                return;
                            }

                            _firebaseUser = task.Result.User;
                            userInfo = UserInfo.FromFirebaseUser(_firebaseUser);
                        }).AsUniTask().AttachExternalCancellation(cts.Token);
                    }
                }

                // Need to check again for user token to verify firebase user.
                // And check if we're offline.
                if (requestFailed || !await IsUserValid())
                {
                    return new ServiceResultWrapper<SignInAnonymouslyResult>(ServiceErrorHelper.UnknownError);
                }

                return new ServiceResultWrapper<SignInAnonymouslyResult>(
                    new SignInAnonymouslyResult {UserInfo = userInfo});
            }
            catch (Exception e)
            {
                return new ServiceResultWrapper<SignInAnonymouslyResult>(
                    new ServiceError(0, "Login Failed.", e.Message));
            }
        }

        public UniTask<ServiceResultWrapper<SignOutResult>> SignOutAsync(SignOutRequest request)
        {
            try
            {
                var error = CheckInternetAvailability();

                if (error != null)
                {
                    return UniTask.FromResult(new ServiceResultWrapper<SignOutResult>(error));
                }

                _firebaseAuth.SignOut();
                _firebaseAuth.Dispose();
                _firebaseUser.Dispose();
                return UniTask.FromResult(new ServiceResultWrapper<SignOutResult>(new SignOutResult()));
            }
            catch (Exception e)
            {
                return UniTask.FromResult(
                    new ServiceResultWrapper<SignOutResult>(new ServiceError(0, "Sign Out Failed.", e.Message)));
            }
        }
        
        /// <summary>
        /// Creates the user, sends an email verification the proceeds to login to Firebase.
        /// Currently, the user will login even if email is not yet validated. (need to discuss further flow)
        /// Make sure the email you input wasn't used in testing <see cref="SignInGoogleAsync"/> before. It will say the email is already used.
        /// </summary>
        /// <param name="request">In CreateUserWithEmailResult, Email and Password should be configured</param>
        /// <returns>This returns a SignInEmailResult with a <see cref="UserInfo"/>.</returns>
        public async UniTask<ServiceResultWrapper<CreateUserWithEmailResult>> CreateUserWithEmailAsync(CreateUserWithEmailRequest request)
        {
            try
            {
                var error = CheckInternetAvailability();

                if (error != null)
                {
                    return new ServiceResultWrapper<CreateUserWithEmailResult>(error);
                }

                UserInfo userInfo = null;
                var requestFailed = false;
                _firebaseUser = _firebaseAuth.CurrentUser;

                if (IsAlreadySigned)
                {
                    // Check if we're already signed up.
                    userInfo = UserInfo.FromFirebaseUser(_firebaseUser);
                }
                else
                {
                    using (cts = new CancellationTokenSource(TimeSpan.FromSeconds(BackendUtil.TimeoutSec)))
                    {
                        // A way we could fetch and return the error and the result inside the firebase task.
                        var taskComplete = new TaskCompletionSource<CreateUserWithEmailResult>();
                        
                        await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(request.Email,request.Password).ContinueWithOnMainThread(async task =>
                        {
                            requestFailed = task.IsCanceled || task.IsFaulted;

                            if (requestFailed)
                            {
                                Debug.LogError($"Request failed: {task.Exception?.Message}");
                                if (task.Exception != null) 
                                    taskComplete.SetException(task.Exception);
                                
                                return;
                            }

                            var createResult = task.Result;
                            var signInResult = await SignInEmailAsync(new SignInEmailRequest(createResult.Credential));
                            userInfo = signInResult.Result.UserInfo;
                            taskComplete.SetResult(new CreateUserWithEmailResult{UserInfo = userInfo});
                            
                        }).AsUniTask().AttachExternalCancellation(cts.Token);;
                        await taskComplete.Task;

                        if (taskComplete.Task.Exception != null && !string.IsNullOrEmpty(taskComplete.Task.Exception.Message))
                        {
                            return new ServiceResultWrapper<CreateUserWithEmailResult>(new ServiceError(0, "Create User with Email Failed", taskComplete.Task.Exception.Message));
                        }

                        var emailVerification = await SendEmailVerificationAsync();
                        
                        if (emailVerification.Error!= null && !string.IsNullOrEmpty(emailVerification.Error.Message))
                        {
                            return new ServiceResultWrapper<CreateUserWithEmailResult>(new ServiceError(0, emailVerification.Error.Message, emailVerification.Error.RawError));
                        }
                    }
                }

                return new ServiceResultWrapper<CreateUserWithEmailResult>(new CreateUserWithEmailResult
                    {UserInfo = userInfo});
            }
            catch (Exception e)
            {
                return new ServiceResultWrapper<CreateUserWithEmailResult>(new ServiceError(0, "Create User with Email Failed.", e.Message));
            }
        }
        
        /// <summary>
        /// Sends an email verification link to the user.
        /// </summary>
        /// <returns>Returns a flag if the email is sent sucessfully.</returns>
        public async UniTask<ServiceResultWrapper<SendEmailVerificationResult>> SendEmailVerificationAsync()
        {
            try
            {
                var error = CheckInternetAvailability();

                if (error != null)
                {
                    return new ServiceResultWrapper<SendEmailVerificationResult>(error);
                }

                UserInfo userInfo = null;
                var requestFailed = false;
                _firebaseUser = _firebaseAuth.CurrentUser;
             
                if (_firebaseUser == null)
                {
                    return new ServiceResultWrapper<SendEmailVerificationResult>(new ServiceError(0, "Send Email Verification Failed! Firebase User null!", "FIREBASE USER NULL"));
                }
                
                using (cts = new CancellationTokenSource(TimeSpan.FromSeconds(BackendUtil.TimeoutSec)))
                {
                    var sendEmailResult = _firebaseUser.SendEmailVerificationAsync();
                    if (sendEmailResult.Exception?.GetBaseException() is FirebaseException firebaseException)
                    {
                        var authError = (AuthError) firebaseException.ErrorCode;
                        var errorMsg = "Unknown Error";
                        switch (authError)
                        {
                            case AuthError.InvalidRecipientEmail:
                                errorMsg = "The email entered is invalid";
                                break;
                            case AuthError.Cancelled:
                                errorMsg = "Email verification was cancelled";
                                break;
                            case AuthError.TooManyRequests:
                                errorMsg = "Too many requests";
                                break;
                        }
                        
                        return new ServiceResultWrapper<SendEmailVerificationResult>(new ServiceError(0, errorMsg, firebaseException.Message));
                    }
                    Debug.Log("EMAIL SENT");
                    return new ServiceResultWrapper<SendEmailVerificationResult>(new SendEmailVerificationResult {EmailSuccessful = true});
                }
            }
            catch (Exception e)
            {
                return new ServiceResultWrapper<SendEmailVerificationResult>(new ServiceError(0, "Send Email Verification Failed!", e.Message));
            }
        }
        
        /// <summary>
        /// Sends a reset password email to the user.
        /// </summary>
        /// <param name="request">In SendPasswordResetRequest, email variable shouldn't be empty.</param>
        /// <returns>Returns a flag if the password reset email was sent successfully.</returns>
        public async UniTask<ServiceResultWrapper<SendPasswordResetResult>> SendPasswordResetAsync(SendPasswordResetRequest request)
        {
            try
            {
                var error = CheckInternetAvailability();

                if (error != null)
                {
                    return new ServiceResultWrapper<SendPasswordResetResult>(error);
                }

                var requestFailed = false;

                using (cts = new CancellationTokenSource(TimeSpan.FromSeconds(BackendUtil.TimeoutSec)))
                {
                    // A way we could fetch and return the error and the result inside the firebase task.
                    var taskComplete = new TaskCompletionSource<SendPasswordResetResult>();
                    
                    await _firebaseAuth.SendPasswordResetEmailAsync(request.Email).ContinueWith(task =>
                    {
                        requestFailed = task.IsCanceled || task.IsFaulted;

                        if (requestFailed)
                        {
                            Debug.LogError($"Request failed: {task.Exception?.Message}");
                            if (task.Exception != null) 
                                taskComplete.SetException(task.Exception);
                        }
                        
                        taskComplete.SetResult(new SendPasswordResetResult{EmailSuccessful = true});
                        
                    }).AsUniTask().AttachExternalCancellation(cts.Token);;
                    await taskComplete.Task;

                    if (taskComplete.Task.Exception != null && !string.IsNullOrEmpty(taskComplete.Task.Exception.Message))
                    {
                        return new ServiceResultWrapper<SendPasswordResetResult>(new ServiceError(0, "Password Reset Failed.", taskComplete.Task.Exception.Message));
                    }
                    
                }
                
                return new ServiceResultWrapper<SendPasswordResetResult>(new SendPasswordResetResult{EmailSuccessful = true});
            }
            catch (Exception e)
            {
                return new ServiceResultWrapper<SendPasswordResetResult>(new ServiceError(0, "Password Reset Failed.", e.Message));
            }
        }
        
        private async UniTask<bool> IsUserValid()
        {
            var userToken = await GetUserTokenAsync();
            return !string.IsNullOrEmpty(userToken);
        }

        private async UniTask<string> GetUserTokenAsync()
        {
            string userToken = null;

            await _firebaseUser.TokenAsync(true).ContinueWith(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    return;
                }

                userToken = task.Result;
            });

            return userToken;
        }

        private ServiceError CheckInternetAvailability()
        {
            return Application.internetReachability == NetworkReachability.NotReachable ? new ServiceError(0, "No Internet Connection!") : null;
        }
    }
}
