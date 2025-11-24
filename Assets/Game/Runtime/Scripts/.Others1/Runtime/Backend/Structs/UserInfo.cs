using System;
using Firebase.Auth;
using Google;

namespace Santelmo.Rinsurv.Backend
{
    public class UserInfo
    {
        public string DisplayName { get; internal set; }
        public string Email { get; internal set; }
        public string PhoneNumber { get; internal set; }
        public Uri PhotoUrl { get; internal set; }
        public string ProviderId { get; internal set; }
        public string UserId { get; internal set; }
        public bool IsAnonymous { get; internal set; }
        public bool IsEmailVerified { get; internal set; }
        public string GoogleToken { get; internal set; }


        internal static UserInfo FromFirebaseUser(FirebaseUser firebaseUser)
        {
            var userInfo = new UserInfo();
            userInfo.DisplayName = firebaseUser.DisplayName;
            userInfo.Email = firebaseUser.Email;
            userInfo.PhoneNumber = firebaseUser.PhoneNumber;
            userInfo.PhotoUrl = firebaseUser.PhotoUrl;
            userInfo.ProviderId = firebaseUser.ProviderId;
            userInfo.UserId = firebaseUser.UserId;
            userInfo.IsAnonymous = firebaseUser.IsAnonymous;
            userInfo.IsEmailVerified = firebaseUser.IsEmailVerified;
            return userInfo;
        }

        internal static UserInfo FromGoogleSignInUser(GoogleSignInUser googleUser)
        {
            var userInfo = new UserInfo();
            userInfo.DisplayName = googleUser.DisplayName;
            userInfo.Email = googleUser.Email;
            userInfo.UserId = googleUser.UserId;
            userInfo.GoogleToken = googleUser.IdToken;
            userInfo.PhotoUrl = googleUser.ImageUrl;

            return userInfo;
        }
    }
}
