using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Messaging;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class FCMNotificationHandler : MonoBehaviour
    {
        private Slot<string> eventSlot;
        private bool isFirebaseInitialized;
        private HashSet<string> receivedMessageIds = new();

        private void InitFirebaseCloudMessaging()
        {
            if (isFirebaseInitialized)
            {
                "FirebaseCloudMessaging is already initialized!".LogError();
                return;
            }

            FirebaseMessaging.RequestPermissionAsync()
                             .ContinueWithOnMainThread(task => LogTaskCompletion(task, "RequestPermissionAsync"));
            
            isFirebaseInitialized = true;
        }

        #region Event Handlers

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (!isFirebaseInitialized)
            {
                return;
            }

            // Disable FCM notification handling if using mocks
#if !USES_MOCKS
            var message = e.Message;
            
            if (IgnorePreviouslyReceivedMessageIds(message))
            {
                return;
            }

            if (!string.IsNullOrEmpty(message.Error))
            {
                $"[FCMNotificationHandler] Message error: {message.Error}".Log();
                return;
            }
            
            $"[FCMNotificationHandler] Message received from {message.From}".Log();

            // Is data notification?
            if (message.Data.Count == 0)
            {
                "[FCMNotificationHandler] Data is empty.".Log();
                return;
            }

            // Uncomment to log message.Data content
            LogDataMessage(message);

            GlobalNotifier.Instance.Trigger(new RemoteDataNotificationReceivedEvent(new FCMRemoteDataNotification(message)));
#endif
        }

        private bool IgnorePreviouslyReceivedMessageIds(FirebaseMessage message)
        {
            if (receivedMessageIds.Contains(message.MessageId))
            {
                // ignore message
                return true;
            }

            receivedMessageIds.Add(message.MessageId);
            return false;
        }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }

        private void Start()
        {
            InitFirebaseCloudMessaging();
        }

        private void OnEnable()
        {
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        private void OnDisable()
        {
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }

        #endregion

        #region Debug

        private static StringBuilder sb = new();

        private static void LogDataMessage(FirebaseMessage message)
        {
            sb.Clear();
            sb.AppendLine($"[FCMNotificationHandler] Message received");
            sb.AppendLine($"Id: {message.MessageId}");
            sb.AppendLine($"Notification message contains data");
            foreach (var kvp in message.Data)
            {
                sb.AppendLine($"Key: {kvp.Key} - Value: {kvp.Value}");
            }
            sb.ToString().Log();
        }
        
        private bool LogTaskCompletion(Task task, string operation)
        {
            var complete = false;
            
            if (task.IsCanceled)
            {
                "[FCMNotificationHandler] RequestPermissionAsync task canceled.".Log();
            }
            else if (task.IsFaulted)
            {
                "[FCMNotificationHandler] RequestPermissionAsync task encountered an error.".LogError();
                
                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    var errorCode = "";
                    var firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        errorCode = $"Error.{((Error)firebaseEx.ErrorCode).ToString()}: ";
                    }
                    
                    $"[FCMNotificationHandler] Error: {errorCode} {exception}".Log();
                }
            }
            else if (task.IsCompleted)
            {
                "[FCMNotificationHandler] RequestPermissionAsync task completed".Log();
                complete = true;
            }
            
            return complete;
        }

        #endregion
    }
}
