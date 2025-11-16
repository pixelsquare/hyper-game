using Firebase.RemoteConfig;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    /// <summary>
    /// Hides the facebook sign in button gameobject base on remote configuration.
    /// </summary>
    public class FacebookSignInHider : MonoBehaviour
    {
        private FirebaseRemoteConfig RemoteConfig => FirebaseRemoteConfig.DefaultInstance;

        private void Start()
        {
             var hideFacebook = RemoteConfig.GetValue(Constants.HIDE_FACEBOOK_SIGNIN_KEY).BooleanValue;
             $"Hiding facebook sign-in button: {hideFacebook}".Log();
             gameObject.SetActive(!hideFacebook);
        }
    }
}
