using System;
using Hangout;
using System.Linq;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class InteractAvatarChanger : MonoBehaviour
    {
        [SerializeField] private RideItemState[] rideItems;
        [SerializeField] private AvatarItemType removeItems;
        [SerializeField] private EntityView entityView;
        
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private SwatchTable swatchTable;

        [Serializable]
        private struct RideItemState
        {
            public AvatarItemConfig avatarItem;
            public Color color;
        } 
        
        private void OnInteractLocal(EventOnObjectInteractedLocal eventData)
        {
            if (eventData.objInteracted != entityView.EntityRef)
            {
                return;
            }

            if (eventData.isInteracting)
            {
                WearRideItems();
            }
            else
            {
                ResetAvatarItems();
            }
        }
        
        private void WearRideItems() // local clients
        {
            var itemStates = (from equip in UserInventoryData.EquippedItems
                             where itemDatabase.HasItem(equip.itemId)
                             let idx = itemDatabase.GetItemIndex(equip.itemId)
                             let itemConfig = itemDatabase.GetItem(idx)
                             let itemType = AvatarItemUtil.ToAvatarItemType(itemConfig.GetTypeCode())
                             where (itemType & removeItems) == 0
                             let color = equip.hasColor ? equip.Color :
                                 swatchTable.TryGetDefaultColor(itemType, out var defaultColor) ? defaultColor :
                                 Color.white
                             select AvatarItemState.ToHangoutItemState(itemDatabase, equip, color)).ToList();

            foreach (var item in rideItems)
            {
                var hangoutItemState = new HangoutItemState()
                {
                    itemIdx = itemDatabase.GetItemIndex(item.avatarItem.Data.itemId),
                    r = FP.FromFloat_UNSAFE(item.color.r),
                    g = FP.FromFloat_UNSAFE(item.color.g),
                    b = FP.FromFloat_UNSAFE(item.color.b),
                    a = FP.FromFloat_UNSAFE(item.color.a),
                };
                
                itemStates.Add(hangoutItemState);
            }
            
            var loadAvatarCommand = new LoadAvatarCommand()
            {
                itemStates = itemStates.ToArray(),
            };
            
            QuantumRunner.Default.Game.SendCommand(loadAvatarCommand);
        }

        private void ResetAvatarItems() // local only
        {
            var localItems = from equip in UserInventoryData.EquippedItems
                             where itemDatabase.HasItem(equip.itemId)
                             let idx = itemDatabase.GetItemIndex(equip.itemId)
                             let itemConfig = itemDatabase.GetItem(idx)
                             let itemType = AvatarItemUtil.ToAvatarItemType(itemConfig.GetTypeCode()) 
                             let color = equip.hasColor ? equip.Color :
                                 swatchTable.TryGetDefaultColor(itemType, out var defaultColor) ? defaultColor :
                                 Color.white
                             select AvatarItemState.ToHangoutItemState(itemDatabase, equip, color);

            var loadAvatarCommand = new LoadAvatarCommand()
            {
                itemStates = localItems.ToArray(),
            };
            
            QuantumRunner.Default.Game.SendCommand(loadAvatarCommand);
        }

        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnObjectInteractedLocal>(this, OnInteractLocal);
        }
    }
}
