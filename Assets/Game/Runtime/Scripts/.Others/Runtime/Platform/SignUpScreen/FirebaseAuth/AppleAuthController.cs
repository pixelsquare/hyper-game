using System;
using System.Collections.Generic;
using AppleAuth;
using Kumu.Extensions;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using System.Text;
using AppleAuth.Extensions;
using System.Security.Cryptography;

namespace Kumu.Kulitan.Backend
{
    public class AppleAuthController : FirebaseAuthController
    {
        private IAppleAuthManager appleAuthManager;

        protected override void ShowSignInWindow()
        {
            HideError();

            var rawNonce = GenerateRandomString(32);
            var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName, nonce);

            if (appleAuthManager == null)
            {
                "no apple auth manager present".LogError();
                return;
            }

            "login with apple id called".Log();
            appleAuthManager.LoginWithAppleId(loginArgs, credential =>
            {
                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    // Apple User ID
                    // You should save the user ID somewhere in the device
                    var userId = appleIdCredential.User;
                    FirebaseAuthService.AppleUserIdCache.SaveToken(userId);

                    // Email (Received ONLY in the first login)
                    var email = appleIdCredential.Email;

                    // Full name (Received ONLY in the first login)
                    var fullName = appleIdCredential.FullName;

                    // Identity token
                    var identityToken = Encoding.UTF8.GetString(
                                    appleIdCredential.IdentityToken,
                                    0,
                                    appleIdCredential.IdentityToken.Length);

                    // Authorization code
                    var authorizationCode = Encoding.UTF8.GetString(
                                    appleIdCredential.AuthorizationCode,
                                    0,
                                    appleIdCredential.AuthorizationCode.Length);


                    // And now you have all the information to create/login a user in your system
                    "apple sign in successful".Log();
                    var signInRequest = new SignInRequest()
                    {
                        credential = Firebase.Auth.OAuthProvider.GetCredential("apple.com", identityToken, rawNonce, authorizationCode)
                    };
                    SignInToFirebase(signInRequest);
                }
                else
                {
                    "No credentials".Log();
                }
            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                ShowError($"Failed to sign in with Apple: {authorizationErrorCode}");
            });
        }

        private static string GenerateSHA256NonceFromRawNonce(string rawNonce)
        {
            var sha = new SHA256Managed();
            var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
            var hash = sha.ComputeHash(utf8RawNonce);

            var result = string.Empty;
            for (var i = 0; i < hash.Length; i++)
            {
                result += hash[i].ToString("x2");
            }

            return result;
        }

        private static string GenerateRandomString(int length)
        {
            if (length <= 0)
            {
                throw new Exception("Expected nonce to have positive length");
            }

            const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
            var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
            var result = string.Empty;
            var remainingLength = length;

            var randomNumberHolder = new byte[1];
            while (remainingLength > 0)
            {
                var randomNumbers = new List<int>(16);
                for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
                {
                    cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                    randomNumbers.Add(randomNumberHolder[0]);
                }

                for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
                {
                    if (remainingLength == 0)
                    {
                        break;
                    }

                    var randomNumber = randomNumbers[randomNumberIndex];
                    if (randomNumber < charset.Length)
                    {
                        result += charset[randomNumber];
                        remainingLength--;
                    }
                }
            }

            return result;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                appleAuthManager = FirebaseAuthService.appleAuthManager;
                return;
            }

            // disable apple button on non-ios devices
            signInButton.gameObject.SetActive(false);
        }


        private void Update()
        {
            // Updates the AppleAuthManager instance to execute
            // pending callbacks inside Unity's execution loop
            if (appleAuthManager != null)
            {
                appleAuthManager.Update();
            }
        }
    }
}
