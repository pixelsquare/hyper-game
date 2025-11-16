using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Common
{
    public class EventReceiverCallbackMono : MonoBehaviour
    {
        [SerializeField] private string eventName;
        
        private Slot<string> eventSlot;

        public UnityEvent onReceiveEvent;

        private void Listener(IEvent<string> obj)
        {
            onReceiveEvent.Invoke();
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(eventName, Listener);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
