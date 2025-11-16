using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class PartReplacer : MonoBehaviour
    {
        [SerializeField] private ItemCategorySelector itemCategorySelector;
        [SerializeField] private DefaultItemData defaultItemData;
        [SerializeField] private ItemSelection itemSelection;
        [SerializeField] private ItemSelectionController selectionController;

        private Dictionary<AvatarItemType, AvatarItemConfig> partSelectedItemTable = new();

        /// <summary>
        /// todo [jef] : move to visual scripting
        /// </summary>
        public void Init()
        {
            partSelectedItemTable.Clear();
            
            foreach (var pair in defaultItemData.AllItemPairs())
            {
                var itemId = pair.Value.Data.itemId;
                var instance = ItemDatabase.Current.GetItem(itemId);
                partSelectedItemTable.Add(pair.Key, instance);
            }
        }

        public async void OnSelectedItem(AvatarItemConfig currentItem, bool isSelected)
        {
            var key = currentItem.State.ItemType;
            var hasPrevOfSameType = partSelectedItemTable.TryGetValue(key, out var previousItem);         
            
            if (isSelected)
            {
                var mainTask = SelectPart(currentItem, NotifyMode.After, ItemSelectionMode.ApplyModel);
                
                if (hasPrevOfSameType)
                {
                    selectionController.DeselectItem(previousItem.Data.itemId);
                    RecalculateSelectedParts(currentItem, previousItem);
                }
                else
                {
                    await mainTask;
                    DeselectItemByTags(currentItem, NotifyMode.After);
                    DeselectItemsByTagsReverse(currentItem, NotifyMode.Before);
                }
            }
            else if (IsCurrentPart(currentItem))
            {
                var mainTask = SelectDefaultItemFromTags(currentItem);

                if (defaultItemData.TryGetItem(key, out var defaultConfig))
                {
                    if (defaultConfig != currentItem)
                    {
                        selectionController.DeselectItem(currentItem.Data.itemId);
                        SelectPart(defaultConfig, NotifyMode.After, ItemSelectionMode.ApplyModel);
                    }
                    else
                    {
                        itemSelection?.NotifyItem(defaultConfig.Data.itemId, true);
                    }
                }
                else
                {
                    await mainTask;
                    DeselectPart(key, NotifyMode.After, ItemSelectionMode.ApplyModel);
                }
            }
            else
            {
                await DeselectPart(key, NotifyMode.After, ItemSelectionMode.None);
            }
        }

        /// <summary>
        /// Used in FSM
        /// </summary>
        public bool TryGetCurrentItemOfPart(AvatarItemType itemType, out AvatarItemConfig itemData)
        {
            return partSelectedItemTable.TryGetValue(itemType, out itemData);
        }

        private async Task SelectPart(AvatarItemConfig itemData, NotifyMode notifyMode, ItemSelectionMode selectionMode = ItemSelectionMode.All)
        {
            var key = itemData.State.ItemType;

            if (notifyMode.Equals(NotifyMode.Before))
            {
                await selectionController.SelectItem(itemData.Data.itemId, selectionMode);
            }

            if (!partSelectedItemTable.ContainsKey(key))
            {
                partSelectedItemTable.Add(key, itemData);
            }
            else
            {
                partSelectedItemTable[key] = itemData;
            }

            if (notifyMode.Equals(NotifyMode.After))
            {
                await selectionController.SelectItem(itemData.Data.itemId, selectionMode);
            }
        }

        private async Task DeselectPart(AvatarItemType key, NotifyMode notifyMode, ItemSelectionMode selectionMode = ItemSelectionMode.All)
        {
            if (partSelectedItemTable.TryGetValue(key, out var itemData))
            {
                if (notifyMode.Equals(NotifyMode.Before))
                {
                    await selectionController.DeselectItem(itemData.Data.itemId, selectionMode);
                }

                partSelectedItemTable.Remove(key);

                if (notifyMode.Equals(NotifyMode.After))
                {
                    await selectionController.DeselectItem(itemData.Data.itemId, selectionMode);
                }
            }
        }

        private bool IsCurrentPart(AvatarItemConfig itemConfig)
        {
            var key = itemConfig.State.ItemType;
            
            if (partSelectedItemTable.TryGetValue(key, out var value))
            {
                return value.Data.itemId.Equals(itemConfig.Data.itemId);
            }

            return false;
        }

        private async Task SelectDefaultItemFromTags(AvatarItemConfig itemData)
        {
            if (!itemCategorySelector.TryGetValue(itemData.Data, out var itemTag))
            {
                return;
            }

            var allTasks = new List<Task>();

            foreach (AvatarItemType itemType in Enum.GetValues(typeof(AvatarItemType)))
            {
                var isFlagged = (itemType & itemTag.DeselectedTypes) > 0;

                if (!isFlagged)
                {
                    continue;
                }
                
                if (partSelectedItemTable.ContainsKey(itemType))
                {
                    continue;
                }

                if (defaultItemData.TryGetItem(itemType, out var defaultConfig))
                {
                    var task = SelectPart(defaultConfig, NotifyMode.After, ItemSelectionMode.ApplyModel);
                    allTasks.Add(task);
                }
            }

            await Task.WhenAll(allTasks);
        }

        private async Task RecalculateSelectedParts(AvatarItemConfig current, AvatarItemConfig previous)
        {
            var hasCurrentTag = itemCategorySelector.TryGetValue(current.Data, out var currentTag);
            var hasPreviousTag = itemCategorySelector.TryGetValue(previous.Data, out var previousTag);
            var currentFlags = hasCurrentTag ? currentTag.DeselectedTypes : AvatarItemType.None;
            var previousFlags = hasPreviousTag ? previousTag.DeselectedTypes : AvatarItemType.None;

            var deselectedFlags = (previousFlags ^ currentFlags) & currentFlags;
            var selectedFlags = (previousFlags ^ currentFlags) & previousFlags;

            await SelectDefaultItemsByType(selectedFlags);
            DeselectItemsByType(deselectedFlags, NotifyMode.After);
        }

        private void DeselectItemsByType(AvatarItemType deselectedFlags, NotifyMode notifyMode)
        {
            foreach (AvatarItemType itemType in Enum.GetValues(typeof(AvatarItemType)))
            {
                if ((itemType & deselectedFlags) == 0)
                {
                    continue;
                }

                DeselectPart(itemType, notifyMode);
            }
        }

        private async Task SelectDefaultItemsByType(AvatarItemType selectedFlags)
        {
            var tasks = new List<Task>();
            foreach (AvatarItemType itemType in Enum.GetValues(typeof(AvatarItemType)))
            {
                if ((itemType & selectedFlags) == 0)
                {
                    continue;
                }

                if (defaultItemData.TryGetItem(itemType, out var defaultConfig))
                {
                    var task = SelectPart(defaultConfig, NotifyMode.Before, ItemSelectionMode.ApplyModel);
                    tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks);
        }

        private void DeselectItemByTags(AvatarItemConfig itemData, NotifyMode notifyMode)
        {
            var hasTag = itemCategorySelector.TryGetValue(itemData.Data, out var itemTag);

            if (!hasTag)
            {
                return;
            }

            var deselectFlags = itemTag.DeselectedTypes;

            var tableCopy = new Dictionary<AvatarItemType, AvatarItemConfig>(partSelectedItemTable);

            foreach (var pair in tableCopy)
            {
                var slot = pair.Key;
                var isOccupied = (slot & deselectFlags) == slot;

                if (isOccupied)
                {
                    DeselectPart(slot, notifyMode);
                }
            }
        }

        private void DeselectItemsByTagsReverse(AvatarItemConfig currentItem, NotifyMode notifyMode)
        {
            var slot = currentItem.State.ItemType;
            var tableCopy = new Dictionary<AvatarItemType, AvatarItemConfig>(partSelectedItemTable);

            foreach (var pair in tableCopy)
            {
                var hasTag = itemCategorySelector.TryGetValue(pair.Value.Data, out var itemTag);

                if (!hasTag)
                {
                    continue;
                }

                var deselectFlags = itemTag.DeselectedTypes;

                var isOccupied = (slot & deselectFlags) > 0;
                if (isOccupied)
                {
                    DeselectPart(pair.Key, notifyMode);
                }
            }
        }
        
        private enum NotifyMode
        {
            None,
            Before,
            After,
        }
    }
}
