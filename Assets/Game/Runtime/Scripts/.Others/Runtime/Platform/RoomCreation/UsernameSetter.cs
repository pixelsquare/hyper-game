using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;
using Kumu.Kulitan.UI;

namespace Kumu.Kulitan.Multiplayer
{
    public class UsernameSetter : MonoBehaviour
    {
        private Slot<string> eventSlot = new Slot<string>(GlobalNotifier.Instance);

        private static bool wasDisplayed;
        
        public void ShowUsernamePopup()
        {
            if (wasDisplayed)
            {
                return;
            }
            
            PopupManager.Instance.ShowUsernamePopup((username) =>
            {
                ConnectionManager.Client.NickName = username;
                wasDisplayed = true;
            });
        }

        private void HandleTutorialDismissedEvent(IEvent<string> obj)
        {
            ShowUsernamePopup();
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(Tutorial.TutorialDismissedEvent.EVENT_NAME, HandleTutorialDismissedEvent);
        }

        private void OnDisable()
        {
            eventSlot.UnSubscribeFor(Tutorial.TutorialDismissedEvent.EVENT_NAME, HandleTutorialDismissedEvent);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
