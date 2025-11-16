using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class PermissionHelper
    {
        public static void RequestMicrophonePermission()
        {
            if (Microphone.devices.Length > 0 && !Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }

#if !UNITY_EDITOR && UNITY_ANDROID
			if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
			{
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
			}
#endif
        }

        public static bool HasPushNotificationsEnabled()
        {
#if !UNITY_EDITOR && UNITY_IOS
            var notifSettings = Unity.Notifications.iOS.iOSNotificationCenter.GetNotificationSettings();
            return notifSettings.AuthorizationStatus == Unity.Notifications.iOS.AuthorizationStatus.Authorized;
#endif
            return true;
        }

        public static void OpenNotificationSettings()
        {
#if !UNITY_EDITOR && UNITY_IOS
            Unity.Notifications.iOS.iOSNotificationCenter.OpenNotificationSettings();
#elif !UNITY_EDITOR && UNITY_ANDROID
            Unity.Notifications.Android.AndroidNotificationCenter.OpenNotificationSettings();
#endif
        }
    }
}
