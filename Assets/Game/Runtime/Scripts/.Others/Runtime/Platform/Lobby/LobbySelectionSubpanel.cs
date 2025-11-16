using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.UI;
using UnityEngine;

namespace Kumu.Kulitan.Lobby
{
    public class LobbySelectionSubpanel : MonoBehaviour
    {
        [SerializeField] private string hangoutPanelName;
        
        private void OnUIPanelDeactivated(IEvent<string> obj)
        {
            var evt = (UIPanelDeactivatedEvent)obj;

            if (evt.PanelName != hangoutPanelName)
            {
                return;
            }
            
            gameObject.SetActive(false);
        }
        
        private void OnEnable()
        {
            GlobalNotifier.Instance.SubscribeOn(UIPanelDeactivatedEvent.EVENT_NAME, OnUIPanelDeactivated);
        }

        private void OnDisable()
        {
            GlobalNotifier.Instance.UnSubscribeFor(UIPanelDeactivatedEvent.EVENT_NAME, OnUIPanelDeactivated);
        }
    }
}
