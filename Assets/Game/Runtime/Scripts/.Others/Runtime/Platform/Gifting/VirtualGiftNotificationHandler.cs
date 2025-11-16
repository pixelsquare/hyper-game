using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    // (cj) Only used while mocked?
    public class VirtualGiftNotificationHandler : MonoBehaviour
    {
        //keeping player map in case functionality to show gifts to other players is needed
        private SerializableDictionary<EntityRef, QuantumPlayerJoinedEvent> playerMapping;
        private Slot<string> eventSlot;

        public SerializableDictionary<EntityRef, QuantumPlayerJoinedEvent> PlayerMapping => playerMapping;

        private void OnPlayerJoin(IEvent<string> callback)
        {
            var joinEvent = (QuantumPlayerJoinedEvent) callback;
            playerMapping.Add(joinEvent.PlayerEntity, joinEvent);
        }
        
        private void OnPlayerLeave(IEvent<string> callback)
        {
            var leaveEvent = (QuantumPlayerRemovedEvent) callback;
            playerMapping.Remove(leaveEvent.EntityRef);
        }

        private void ReceiveGiftEventHandler(VirtualGiftEventInfo obj)
        {
            $"[VirtualGiftNotificationHandler] received {obj.gifts.Length}".Log();
        }

        private void Awake()
        {
            playerMapping = new SerializableDictionary<EntityRef, QuantumPlayerJoinedEvent>();
            
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerJoin);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerLeave);

            Services.VirtualGiftService.VirtualGiftReceivedEvent += ReceiveGiftEventHandler;
        }

        private void OnDestroy()
        {
            eventSlot.UnSubscribeAllOn(QuantumPlayerJoinedEvent.EVENT_NAME);
            eventSlot.UnSubscribeAllOn(QuantumPlayerRemovedEvent.EVENT_NAME);

            Services.VirtualGiftService.VirtualGiftReceivedEvent -= ReceiveGiftEventHandler;
        }
    }
}
