using System;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    /// <summary>
    ///     Handles initialization for the device microphone permission.
    /// </summary>
    public class PermissionRequestHandler : MonoBehaviour
    {
        private const string IOS_PUSH_NOTIF_REMINDER_KEY = "NotifReminderShown";

        private bool PushNotificationReminder
        {
            get
            {
#if !UNITY_EDITOR && UNITY_IOS
                return Convert.ToBoolean(PlayerPrefs.GetInt(IOS_PUSH_NOTIF_REMINDER_KEY, 0));
#else
                return true;
#endif
            }
        }
        
        public bool IsPushNotificationEnabled()
        {
            return PushNotificationReminder || PermissionHelper.HasPushNotificationsEnabled();
        }

        public void OpenNotificationSettings()
        {
            PermissionHelper.OpenNotificationSettings();
        }

        public void SaveNotificationReminderSetting(bool hasShown)
        {
            PlayerPrefs.SetInt(IOS_PUSH_NOTIF_REMINDER_KEY, Convert.ToInt32(hasShown));
        }

        private void Awake()
        {
            PermissionHelper.RequestMicrophonePermission();
        }
    }
}
