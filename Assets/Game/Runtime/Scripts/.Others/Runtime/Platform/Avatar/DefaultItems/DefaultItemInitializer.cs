using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Backend;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    ///     OBSOLETE: Keeping for this for reference.
    ///     Handles initializing default equipped item id values for the current user.
    /// </summary>
    public class DefaultItemInitializer : MonoBehaviour
    {
        [SerializeField] private DefaultItemData defaultItemData;
        [SerializeField] private SwatchTable swatchTable;
        [SerializeField] private ItemCategorySelector itemCategorySelector;

        private ItemDatabase itemDatabase;

        /// <summary>
        /// Invoked via Bolt FSM.
        /// </summary>
        public void InitDefaultItems()
        {
            itemDatabase = ItemDatabase.Current;
            InitDefaultItemsAsync();
        }

        private async void InitDefaultItemsAsync()
        {
            var op = await Services.InventoryService.GetInventoryAsync(new GetInventoryRequest());

            if (!op.HasError)
            {
                UserInventoryData.EquippedItems = op.Result.equippedItems;
                UserInventoryData.UserWallet = new UserWallet(op.Result.walletBalance);

                if (AvatarSaveLoadUtil.TryConvertToAvatarItemState(op.Result.ownedItemIds, out var ownedItems))
                {
                    UserInventoryData.OwnedItems = ownedItems;
                }
            }

            var equippedItems = LoadEquippedItems();
            var deselectedFlags = GetDeselectedTypes(equippedItems);

            AddDefaultItemsToEquipped(equippedItems, deselectedFlags);

            var itemsToEquip = from pair in equippedItems
                    where itemDatabase.HasItem(pair.Value.itemId)
                    select itemDatabase.GetItem(pair.Value.itemId);

            // AvatarSaveLoadUtil.EquipItems(itemsToEquip, false);
            // AvatarSaveLoadUtil.PurchaseItems(itemsToEquip, userItemData.OwnedItems);
        }

        private Dictionary<AvatarItemType, AvatarItemState> LoadEquippedItems()
        {
            var equipped = new Dictionary<AvatarItemType, AvatarItemState>();
            var equippedItems = UserInventoryData.EquippedItems;

            foreach (var item in equippedItems)
            {
                if (itemDatabase.TryGetItem(item.itemId, out var itemConfig))
                {
                    if (item.hasColor)
                    {
                        itemConfig.SetStateColor(item.Color);
                    }

                    equipped.Add(itemConfig.State.ItemType, itemConfig.State);
                }
            }

            return equipped;
        }

        private AvatarItemType GetDeselectedTypes(Dictionary<AvatarItemType, AvatarItemState> equipped)
        {
            var deselectedFlags = AvatarItemType.None;

            foreach (var pair in equipped)
            {
                var itemId = pair.Value.itemId;
                if (!itemDatabase.TryGetItem(itemId, out var itemConfig))
                {
                    continue;
                }

                if (!itemCategorySelector.TryGetValue(itemConfig.Data, out var itemTagConfig))
                {
                    continue;
                }

                deselectedFlags |= itemTagConfig.DeselectedTypes;
            }

            return deselectedFlags;
        }

        /// <summary>
        /// Iterates through each default item config, and adds them to the equipped list if the AvatarItemType is available,
        /// meaning the part is not equipped and there are no other parts that fill it.
        /// Skipping of parts is determined by the ItemTag of the currently equipped item, if any.
        /// E.g.: UpperBody is not equipped, and FullBody is not equipped, will grant and equip the default UpperBody item to the player.
        /// Conversely, if an UpperBody is not equipped, and a FullBody is equipped, granting and equipping for the default UpperBody item will be skipped.
        /// </summary>
        /// <param name="equipped">Dictionary of Avatar Items to process.</param>
        /// <param name="deselectedFlags"></param>
        private void AddDefaultItemsToEquipped(Dictionary<AvatarItemType, AvatarItemState> equipped, AvatarItemType deselectedFlags)
        {
            foreach (var pair in defaultItemData.AllItemPairs())
            {
                if (equipped.TryGetValue(pair.Key, out _))
                {
                    continue;
                }

                var defaultItem = pair.Value;
                var flagged = (defaultItem.State.ItemType & deselectedFlags) > 0; // bitmask operation to check if default item's type is included in flag

                if (flagged)
                {
                    continue;
                }

                if (swatchTable.TryGetDefaultColor(pair.Key, out var color))
                {
                    defaultItem.SetStateColor(color);
                }

                equipped.Add(pair.Key, defaultItem.State);
            }
        }
    }
}
