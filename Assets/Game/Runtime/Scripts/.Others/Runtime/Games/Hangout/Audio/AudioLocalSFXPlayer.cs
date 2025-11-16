using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Gifting;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class AudioLocalSFXPlayer : MonoBehaviour
    {
        private SerializableDictionary<string, VirtualGiftingSpatialSFXMarker> playerMapping;
        private Slot<string> eventSlot;

        public void PlaySFX(string accountId, AudioClip clip)
        {
            playerMapping[accountId].MySource.PlayOneShot(clip);
        }
        
        private void OnPlayerJoin(IEvent<string> callback)
        {
            var joinEvent = (QuantumPlayerJoinedEvent) callback;
            playerMapping.Add(joinEvent.AccountId, joinEvent.PlayerTransform.gameObject.GetComponentInChildren<VirtualGiftingSpatialSFXMarker>());
        }
        
        private void OnPlayerLeave(IEvent<string> callback)
        {
            var leaveEvent = (QuantumPlayerRemovedEvent) callback;
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var playerData = f.GetPlayerData(leaveEvent.PlayerRef);
            playerMapping.Remove(playerData.accountId);
        }
        
        private void Awake()
        {
            playerMapping = new SerializableDictionary<string, VirtualGiftingSpatialSFXMarker>();
            
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerJoin);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerLeave);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
