using System.Linq;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftingChatLink : MonoBehaviour
    {
        [SerializeField] private ChatGroupManager groupManager;
        private SerializableDictionary<string, string> playerMapping;
        private Slot<string> eventSlot;

        private void OnPlayerJoin(IEvent<string> callback)
        {
            var joinEvent = (QuantumPlayerJoinedEvent) callback;
            playerMapping.Add(joinEvent.AccountId, joinEvent.Nickname);
        }
        
        private void OnPlayerLeave(IEvent<string> callback)
        {
            var leaveEvent = (QuantumPlayerRemovedEvent) callback;
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var playerData = f.GetPlayerData(leaveEvent.PlayerRef);
            playerMapping.Remove(playerData.accountId);
        }
        
        private void OnVirtualGiftReceived(IEvent<string> obj)
        {
            var callback = (OnVirtualGiftPlayEffectEvent)obj;
            
            var gifterExists = playerMapping.TryGetValue(callback.VgEffectData.gifterId, out var gifter);
            if (!gifterExists)
            {
                $"Gifter {gifter} is not in the room!".Log();
                return;
            }
            
            var giftId = callback.VgEffectData.giftId;
            var giftCount = callback.VgEffectData.count;
            var giftees = callback.VgEffectData.giftees;
            var giftType = callback.VgEffectData.giftType;
            var giftString = $"{giftCount} {VirtualGiftDatabase.Current.GetGift(giftId).Data.giftName}";

            var gifteeNicknames = giftees.Where(g => playerMapping.ContainsKey(g))
                                         .Select(g => playerMapping[g])
                                         .ToArray();

            switch (giftType)
            {
                case VirtualGiftData.VGType.Individual:
                    groupManager.AddTextToDisplay($"{gifter} gifted {giftString} to {string.Join(", ", gifteeNicknames)}!", ChatMessage.MessageType.Info);
                    break;
                
                case VirtualGiftData.VGType.Shared:
                    groupManager.AddTextToDisplay($"{gifter} gifted {giftString} to EVERYONE in the room!", ChatMessage.MessageType.Info);
                    break;
                
                case VirtualGiftData.VGType.Area:
                    //todo - to be implemented
                    break;
                
                case VirtualGiftData.VGType.Collectible:
                    //todo - to be implemented
                    break;
                
                case VirtualGiftData.VGType.Interactable:
                    //todo - to be implemented
                    break;
                default:
                    $"Unexpected VGType! Type:{(int)giftType}".LogError();
                    break;
            }
        }

        private void OnVGFailedToSendEventHandler(IEvent<string> obj)
        {
            groupManager.AddTextToDisplay("Virtual Gift failed to send.", ChatMessage.MessageType.Error);
        }

        private void Awake()
        {
            playerMapping = new SerializableDictionary<string, string>();
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerJoin);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerLeave);
            eventSlot.SubscribeOn(VGFailedToSendEvent.EVENT_NAME, OnVGFailedToSendEventHandler);
            eventSlot.SubscribeOn(OnVirtualGiftPlayEffectEvent.EVENT_NAME, OnVirtualGiftReceived);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }
    }
}
