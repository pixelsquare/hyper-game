using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Common
{
    public class FSMRecieveUnityEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<string> onUnityEventReceived;

        private string unityEventToTrigger = "";
        private Slot<string> eventSlot;

        private void OnEventReceived(IEvent<string> callback)
        {
            var eventCallback = (FSMSendUnityEvent)callback;
            unityEventToTrigger = eventCallback.EventName;
            onUnityEventReceived?.Invoke(unityEventToTrigger);
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(FSMSendUnityEvent.EVENT_NAME, OnEventReceived);
        }

        private void OnDisable()
        {
            eventSlot.UnSubscribeFor(FSMSendUnityEvent.EVENT_NAME, OnEventReceived);
        }
    }
}
