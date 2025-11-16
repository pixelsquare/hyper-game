using Kumu.Extensions;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.Events;
using Photon.Realtime;
using Kumu.Kulitan.UI;

namespace Kumu.Kulitan.Multiplayer
{
    /// <summary>
    /// Connection Callback class used in MainScreen scene.
    /// </summary>
    public class MenuConnectionCallback : ConnectionCallback
    {
        [SerializeField] private UnityEvent onConnectedToMaster;
        
        public override void OnConnectedToMaster()
        {
            onConnectedToMaster?.Invoke();
            // sends a unity event trigger to any of the subscribed FSM
            GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("OnConnectedToMaster"));
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (ConnectionManager.Instance.IgnoreDisconnectCallback)
            {
                return;
            }

            //ensure client is disconnected before attempting reconnect
            ConnectionManager.Instance.DisconnectFromGame();

            if (cause == DisconnectCause.CustomAuthenticationFailed)
            {
                return;
            }

            var popup = (ConfirmationPopup) PopupManager.Instance.OpenConfirmationPopup("Error", "Connection failed", "Retry", "Sign out");
            popup.OnConfirm += () =>
            {
                ConnectionManager.Instance.ConnectToServer();
            };
            popup.OnCancel += () =>
            {
                SignOutManager.Instance.ReturnToSignInScreen();
            };
        }

        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            $"Custom Authentication failed: {debugMessage}".LogError();
            var popup = PopupManager.Instance.OpenErrorPopup("Error", "Custom Authentication Failed", "Okay");
            popup.OnClosed += ReturnToSignUp;
        }

        private void ReturnToSignUp()
        {
            SignOutManager.Instance.ReturnToSignInScreen();
        }
    }
}
