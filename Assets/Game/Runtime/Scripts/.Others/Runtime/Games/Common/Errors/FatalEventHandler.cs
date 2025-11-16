using Kumu.Kulitan.Events;
using Kumu.Kulitan.UI;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class FatalEventHandler : MonoBehaviour
    {
        private Slot<string> eventSlot;

        private void ShowPopup(string title, string message, string button)
        {
            var popup = PopupManager.Instance.OpenErrorPopup(title, message, button, 2);
            popup.OnClosed += OnClosedHandler;
        }

        private void OnClosedHandler()
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
            #endif
        }

        #region Event handlers

        private void OnTokenExpiredEventHandler(IEvent<string> obj)
        {
            ShowPopup("Exiting Ube", "Access token has expired.", "Ok");
        }

        private void OnFatalErrorEventHandler(IEvent<string> obj)
        {
            ShowPopup("Exiting Ube", "A fatal error has occured.", "Ok");
        }

        private void OnAppInMaintenanceEventHandler(IEvent<string> obj)
        {
            ShowPopup("Exiting Ube", "App is currently in maintenance. Please come back later.", "Ok");
        }

        #endregion

        #region Monobehaviours

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(FatalErrorEvent.EVENT_NAME, OnFatalErrorEventHandler);
            eventSlot.SubscribeOn(AppInMaintenanceEvent.EVENT_NAME, OnAppInMaintenanceEventHandler);
            eventSlot.SubscribeOn(TokenExpiredErrorEvent.EVENT_NAME, OnTokenExpiredEventHandler);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }

        #endregion
    }
}
