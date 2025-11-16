using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Avatar
{
    [Flags]
    public enum ItemSelectionMode
    {
        None = 0b0000,
        TrackPartSlot = 0b0001,
        ApplyModel = 0b0010,
        All = 0b0011,
    }

    public class ItemSelectionController : MonoBehaviour
    {
        [SerializeField] private OnItemSelect onItemSelect;
        [SerializeField] private PartReplacer partReplacer;
        [SerializeField] private AvatarCustomizer avatarCustomizer;
        [SerializeField] private DefaultItemData defaultItemData;
        [SerializeField] private SwatchTable swatchTable;

        private ItemDatabase itemDatabase => ItemDatabase.Current;
        private readonly Dictionary<string, AvatarItemConfig> selectedItems = new();
        private readonly Dictionary<string, UnityAction<AvatarItemConfig, bool>> onItemSelectActionMap = new();

        public IEnumerable<AvatarItemConfig> SelectedItems => selectedItems.Select(item => item.Value);

    #region Listener subscription

        public void AddListenerOnSelect(string itemId, UnityAction<AvatarItemConfig, bool> action)
        {
            onItemSelectActionMap[itemId] += action;
        }

        public void RemoveListenerOnSelect(string itemId, UnityAction<AvatarItemConfig, bool> action)
        {
            onItemSelectActionMap[itemId] -= action;
        }

    #endregion

        public bool IsItemSelected(string itemId)
        {
            return selectedItems.ContainsKey(itemId);
        }

        public async Task SelectItem(string itemId, ItemSelectionMode selectionMode = ItemSelectionMode.None)
        {
            if (!itemDatabase.TryGetItem(itemId, out var itemConfig))
            {
                return;
            }

            var alreadySelected = selectedItems.TryGetValue(itemId, out var selectedItem);
            if (alreadySelected)
            {
                return;
            }

            selectedItems.Add(itemId, itemConfig);
            await ItemSelection(itemConfig, true, selectionMode);

            if (AvatarItemUtil.HasItemTypeOverlap(selectedItems.Values))
            {
                GlobalNotifier.Instance.Trigger(new FatalErrorEvent());
            }
        }

        public async Task DeselectItem(string itemId, ItemSelectionMode selectionMode = ItemSelectionMode.None)
        {
            if (!itemDatabase.TryGetItem(itemId, out var itemData))
            {
                return;
            }

            var hasSelected = selectedItems.TryGetValue(itemId, out var selectedItem);

            if (!hasSelected)
            {
                return;
            }

            selectedItems.Remove(itemId);
            await ItemSelection(itemData, false, selectionMode);
        }

        /// <summary>
        /// Deselects all unequipped items.
        /// Retains equipped items.
        /// </summary>
        public async Task DeselectAllUnequippedItems()
        {
            var selectedItemsIds = selectedItems.Keys.ToArray();

            var equippedItems = UserInventoryData.EquippedItems;

            var tasks = new List<Task>();

            foreach (var selectedItem in selectedItemsIds)
            {
                if (equippedItems.Any(item => item.itemId.Equals(selectedItem)))
                {
                    continue;
                }

                DeselectItem(selectedItem);
                var itemConfig = itemDatabase.GetItem(selectedItem);
                var task = avatarCustomizer.OnItemSelected(itemConfig, false);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            selectedItems.Clear();

            partReplacer.Init();
            TrackDefaultItems();
            SelectEquippedItems(UserInventoryData.EquippedItems);
        }

        /// <summary>
        /// Create the action map and equip items.
        /// Note that while this is not referenced in other scripts, it is called via Bolt FSM flow.
        /// </summary>
        public void Init()
        {
            partReplacer.Init();
            TrackDefaultItems();
            CreateActionMap();
            SelectEquippedItems(UserInventoryData.EquippedItems);
        }

        public void TrackDefaultItems()
        {
            foreach (var pair in defaultItemData.AllItemPairs())
            {
                selectedItems.Add(pair.Value.Data.itemId, pair.Value);
            }
        }

        public void CreateActionMap()
        {
            onItemSelectActionMap.Clear();

            foreach (var config in itemDatabase.ItemConfigs)
            {
                onItemSelectActionMap.Add(config.Data.itemId, (item, isSelected) => onItemSelect?.Invoke(item, isSelected));
            }
        }

        // todo [jef] : move somewhere else
        public void SelectEquippedItems(AvatarItemState[] equippedItems)
        {
            foreach (var item in equippedItems)
            {
                if (!itemDatabase.TryGetItem(item.itemId, out var itemConfig))
                {
                    continue;
                }

                if (item.hasColor)
                {
                    itemConfig.SetStateColor(item.Color);
                }
                else if (swatchTable.TryGetDefaultColor(itemConfig.GetTypeCode(), out var defaultColor))
                {
                    itemConfig.SetStateColor(defaultColor);
                    var hex = ColorUtility.ToHtmlStringRGBA(defaultColor);
                }

                partReplacer.OnSelectedItem(itemConfig, true);
                avatarCustomizer.OnItemSelected(itemConfig, true);
                onItemSelect.Invoke(itemConfig, true);
                SelectItem(item.itemId);
            }
        }

        /// <summary>
        /// Used by AvatarCustomization FSM
        /// </summary>
        /// <returns></returns>
        public bool IsAllEquipped()
        {
            foreach (var pair in selectedItems)
            {
                if (!UserInventoryData.IsItemEquipped(pair.Value.Data.itemId))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Used by AvatarCustomization FSM
        /// </summary>
        /// <returns></returns>
        public bool IsAllOwned()
        {
            foreach (var pair in selectedItems)
            {
                if (!UserInventoryData.IsItemOwned(pair.Value.Data.itemId))
                {
                    return false;
                }
            }

            return true;
        }

        public void ClearSelectedItems()
        {
            selectedItems.Clear();
        }

        private async Task ItemSelection(AvatarItemConfig itemConfig, bool isSelected, ItemSelectionMode selectionMode)
        {
            var itemId = itemConfig.Data.itemId;

            if ((selectionMode & ItemSelectionMode.TrackPartSlot) > 0)
            {
                partReplacer.OnSelectedItem(itemConfig, isSelected);
            }

            if ((selectionMode & ItemSelectionMode.ApplyModel) > 0)
            {
                await avatarCustomizer.OnItemSelected(itemConfig, isSelected);
            }

            onItemSelectActionMap[itemId]?.Invoke(itemConfig, isSelected);
        }

        [Serializable]
        private class OnItemSelect : UnityEvent<AvatarItemConfig, bool> { }
    }
}
