using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    public class UIPanelDeactivatedEventReceiver : MonoBehaviour
    {
        [SerializeField] private string panelName = "PanelName";
        [SerializeField] private UnityEvent onEventReceived;
        
        private Slot<string> eventSlot;
        
        private void OnEventReceived(IEvent<string> callback)
        {
            var eventCallback = (UIPanelDeactivatedEvent)callback;
            if (!eventCallback.PanelName.Equals(panelName))
            {
                return;
            }
            onEventReceived?.Invoke();
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(UIPanelDeactivatedEvent.EVENT_NAME, OnEventReceived);
        }

        private void OnDisable()
        {
            eventSlot.UnSubscribeFor(UIPanelDeactivatedEvent.EVENT_NAME, OnEventReceived);
        }
    }
}
