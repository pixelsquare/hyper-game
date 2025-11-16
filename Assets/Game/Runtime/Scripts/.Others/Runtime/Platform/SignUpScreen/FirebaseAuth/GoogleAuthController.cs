using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Google;
using Kumu.Extensions;

namespace Kumu.Kulitan.Backend
{
    public class GoogleAuthController : FirebaseAuthController
    {
        private string googleWebAPI = "859113658788-pp97ns521i8sq15n1nmb6e2ru36l4ril.apps.googleusercontent.com";
        private GoogleSignInConfiguration config;

        private static bool IsConfigured; 
        
        protected override void ShowSignInWindow()
        {
            HideError();

            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(AuthCallback);
        }
        
        private void AuthCallback(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                using IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator();
                if (enumerator.MoveNext()) 
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    $"Got Error: {error.Status} {error.Message}".LogError();
                } 
                else 
                {
                    $"Got Unexpected Exception?!? {task.Exception}".LogError();
                }
                return;
            } 
            else if(task.IsCanceled) 
            {
                "User cancelled login".Log();
                return;
            } 
 
            var signInRequest = new SignInRequest()
            {
                credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null)
            };
            SignInToFirebase(signInRequest);
        }

        protected override void Awake()
        {
            base.Awake();

            config = new GoogleSignInConfiguration 
            {
                WebClientId = googleWebAPI,
                RequestIdToken = true,
                UseGameSignIn = false,
            };

            GoogleSignIn.Configuration = config;
        }
    }
}
