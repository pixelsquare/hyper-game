using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftingEffectHandlerBase : MonoBehaviour
    {
        [SerializeField] private Transform screenVfxParent; // TODO: use this when screenspace vfx is available
        private SerializableDictionary<string, VirtualGiftingEffectPoolController> playerMapping;
        private Slot<string> eventSlot;
        private string localAccountId;
        
        private void OnPlayerJoin(IEvent<string> callback)
        {
            var joinEvent = (QuantumPlayerJoinedEvent) callback;
            var playerParentMaker = joinEvent.PlayerTransform.gameObject
                .GetComponentInChildren<VirtualGiftingEffectPoolController>();
            playerMapping.Add(joinEvent.AccountId, playerParentMaker);
            playerParentMaker.InitVfxPool();
            if (joinEvent.IsLocal)
            {
                localAccountId = joinEvent.AccountId;
            }
        }
        
        private void OnPlayerLeave(IEvent<string> callback)
        {
            var leaveEvent = (QuantumPlayerRemovedEvent) callback;
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var playerData = f.GetPlayerData(leaveEvent.PlayerRef);
            playerMapping.Remove(playerData.accountId);
        }
        
        private void OnVirtualGiftPlayVFXEvent(IEvent<string> obj)
        {
            var callbackObj = (OnVirtualGiftPlayVFX) obj;
            
            // skip if giftee is no longer in the room
            if (!playerMapping.ContainsKey(callbackObj.AccountId))
            {
                return;
            }

            //skip if screen space and local user did not get that gift
            if (callbackObj.VgEffectData.vfxData.gifteeVFXConfig.vfxType == VirtualGiftVFXConfig.VFXType.ScreenSpace && 
                callbackObj.AccountId != localAccountId)
            {
                return;
            }

            // TODO: check if vfx is screen space
            var vfxPoolController = playerMapping[callbackObj.AccountId];
            vfxPoolController.QueueVgEffect(callbackObj.VgEffectData);
        }

        private void Awake()
        {
            playerMapping = new SerializableDictionary<string, VirtualGiftingEffectPoolController>();
            
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(OnVirtualGiftPlayVFX.EVENT_NAME, OnVirtualGiftPlayVFXEvent);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerJoin);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerLeave);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
