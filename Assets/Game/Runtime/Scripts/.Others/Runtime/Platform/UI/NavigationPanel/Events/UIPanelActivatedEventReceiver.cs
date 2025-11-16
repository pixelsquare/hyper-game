using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    public class UIPanelActivatedEventReceiver : MonoBehaviour
    {
        [SerializeField] private string panelName = "PanelName";
        [SerializeField] private UnityEvent onEventReceived;
        
        private Slot<string> eventSlot;
        
        private void OnEventReceived(IEvent<string> callback)
        {
            var eventCallback = (UIPanelActivatedEvent)callback;
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
            eventSlot.SubscribeOn(UIPanelActivatedEvent.EVENT_NAME, OnEventReceived);
        }

        private void OnDisable()
        {
            eventSlot.UnSubscribeFor(UIPanelActivatedEvent.EVENT_NAME, OnEventReceived);
        }
    }
}
