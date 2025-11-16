using System.Collections.Generic;
using Facebook.Unity;
using Firebase.Auth;
using Kumu.Extensions;

namespace Kumu.Kulitan.Backend
{
    public class FacebookAuthController : FirebaseAuthController
    {
        protected override void ShowSignInWindow()
        {
            var permissions = new List<string>
            {
                "email"
            };
            
            FB.LogInWithReadPermissions(permissions, AuthCallback);
        }

        private void AuthCallback(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                var accessToken = AccessToken.CurrentAccessToken;
                var signInRequest = new SignInRequest
                {
                    credential = FacebookAuthProvider.GetCredential(accessToken.TokenString)
                };
                SignInToFirebase(signInRequest);
                return;
            }
            
            "User cancelled login".Log();
        }
    }
}
