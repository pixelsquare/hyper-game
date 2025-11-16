using UnityEngine;
using UnityEngine.Events;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Common;

namespace  Kumu.Kulitan.Multiplayer
{
    public class EmptyRoomListChecker : MonoBehaviour
    {
        [SerializeField] private UnityEvent onRoomListEmpty;
        [SerializeField] private UnityEvent onRoomListNotEmpty;
        
        private Slot<string> eventSlot;

        private void OnRoomListUpdated(IEvent<string> callback)
        {
            var roomListUpdatedCallback = (RoomListUpdatedEvent)callback;
            if (roomListUpdatedCallback.RoomList.Count <= 0)
            {
                onRoomListEmpty?.Invoke();
                return;
            }
            onRoomListNotEmpty?.Invoke();
        }

        private void OnEnable()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(RoomListUpdatedEvent.EVENT_NAME, OnRoomListUpdated);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }
    }
}
