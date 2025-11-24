using Firebase.Auth;
using Google;
using UnityEngine;

namespace Santelmo.Rinsurv.Backend
{
    public static class AuthUtil
    {
        /// <summary>
        /// Retrieves the Google configuration use for Google sign in.
        /// </summary>
        /// <returns>Returns a GoogleSignInConfiguration</returns>
        public static GoogleSignInConfiguration GetGoogleConfig()
        {
            GoogleSignInConfiguration signInConfiguration = new GoogleSignInConfiguration();
            signInConfiguration.UseGameSignIn = false;
            signInConfiguration.RequestIdToken = true;

            var config = Resources.Load<GoogleServicesConfig>("GoogleServicesConfig");
            if (config == null)
            {
                Debug.LogError($"Google services file cannot be found!");
                return null;
            }

            signInConfiguration.WebClientId = config.GoogleWebClientId;
            return signInConfiguration;
        }

        /// <summary>
        /// Retrieves the Firebase Credentials for Google that is used for signing in.
        /// </summary>
        /// <param name="idToken">Token ID that will be use for verification.</param>
        /// <returns>Returns the credentials needed.</returns>
        public static Credential GetGoogleCredentials(string idToken)
        {
            return GoogleAuthProvider.GetCredential(idToken, null);
        }

        /// <summary>
        /// Retrieves the Facebook Credentials for Facebook that is used for signing in.
        /// </summary>
        /// <param name="accessToken">Access token that will be use for verification.</param>
        /// <returns>Returns the credentials needed.</returns>
        public static Credential GetFacebookCredentials(string accessToken)
        {
            return FacebookAuthProvider.GetCredential(accessToken);
        }

        /// <summary>
        /// Retrieves the Firebase Credentials for Google that is used for signing in.
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>Returns the credentials needed.</returns>
        public static Credential GetEmailCredentials(string email, string password)
        {
            return EmailAuthProvider.GetCredential(email,password);
        }
    }
}
