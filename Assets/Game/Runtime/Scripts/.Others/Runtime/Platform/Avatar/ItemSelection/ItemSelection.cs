using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Kulitan.Hangout;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Avatar
{
    public class ItemSelection : MonoBehaviour
    {
        [SerializeField] private AvatarItem prefab;
        [SerializeField] private Transform contentContainer;
        [SerializeField] private ItemSelectionController selectionController;
        [SerializeField] private AssetReferenceT<ItemDatabase> databaseRef;
        [SerializeField] private AvatarItemGridAdapter gridAdapter;
        [SerializeField] private PartReplacer partReplacer;

        // private ObjectPool<AvatarItem> pool;

        private List<AvatarItemConfig> ItemCatalog => ItemDatabase.Current.ItemConfigs;

        public Dictionary<string, AvatarItem> ItemDisplays { get; private set; } = new();

        /// <summary>
        /// Invoked via Bolt FSM
        /// </summary>
        public void Initialize()
        {
            // pool = new ObjectPool<AvatarItem>(CreatePoolInstance);
        }

        /// <summary>
        /// Refreshes the Item Selection area with the currently cached avatar items.
        /// </summary>
        public void LoadItems()
        {
            GenerateItems(ItemCatalog);
        }

        /// <summary>
        /// Populates the Item Selection area with the avatar items.
        /// </summary>
        /// <param name="itemCatalog">Collection of all items to load</param>
        public void LoadItems(List<AvatarItemConfig> itemCatalog)
        {
            GenerateItems(ItemCatalog);
        }

        /// <summary>
        /// Filters the Item Selection area 
        /// </summary>
        /// <param name="itemType">AvatarItemType to filter</param>
        public void FilterItems(AvatarItemType itemType)
        {
            // TODO: should also be filtered via backend?

            var filter = from x in ItemCatalog
                         where (x.State.ItemType & itemType) > 0
                         select x;

            GenerateItems(filter);
        }

        /// <summary>
        /// Filter Item Selection area by owned items.
        /// </summary>
        public void FilterItemsByOwned(AvatarItemType itemType)
        {
            var filter = from item in ItemCatalog
                         where item != null 
                            && UserInventoryData.IsItemOwned(item.Data.itemId)
                            && (item.State.ItemType & itemType) > 0
                         select item;

            GenerateItems(filter);
        }

        /// <summary>
        /// Filters Item Selection area by newest item batch.
        /// </summary>
        public void FilterItemsByNew(AvatarItemType itemType)
        {
            var filter = from item in ItemCatalog
                         group item by item.Data.itemBatch into batch
                         select batch;
            var newItems = filter.OrderBy(batch => batch.Key)
                                 .First()
                                 .Where(item => (item.State.ItemType & itemType) > 0)
                                 .Select(item => item);
            GenerateItems(newItems);
        }

        /// <summary>
        /// Filter Item Selection area by batch.
        /// </summary>
        /// <param name="itemBatch">String identifier of the batch to be filtered.</param>
        public void FilterItemsByBatch(string itemBatch)
        {
            var filter = from item in ItemCatalog
                         where item.Data.itemBatch.Equals(itemBatch)
                         select item;

            GenerateItems(filter);
        }

        /// <summary>
        /// Filter Item Selection area by Item Tag.
        /// </summary>
        /// <param name="itemCategory">ItemCategoryConfig to filter against.</param>
        public void FilterItemsByItemCategory(ItemCategoryConfig itemCategory)
        {
            var filter = from item in ItemCatalog
                where (item.State.ItemType & itemCategory.ItemType) > 0
                where item.Data.itemCategory == itemCategory.ItemCategory.ToLower()
                select item;

            GenerateItems(filter);
        }

        /// <summary>
        /// Toggle all owned item displays to show owned icon.
        /// Invoked via Bolt FSM.
        /// </summary>
        public void NotifyOwnedItems()
        {
            gridAdapter.NotifyOwnedItems();
        }

        /// <summary>
        /// Toggle all item displays for new items to show notification.
        /// </summary>
        public void NotifyNewItems()
        {
            var batches = from item in ItemCatalog
                           group item by item.Data.itemBatch into batch
                           orderby batch.Key descending
                           select batch;
            var newItems = batches.First()
                                  .Select(item => item);

            gridAdapter.NotifyNewItems(newItems);
        }

        public void NotifyItem(AvatarItemConfig itemConfig, bool isOn)
        {
            NotifyItem(itemConfig.Data.itemId, isOn);
        }

        public void NotifyItem(string itemId, bool isOn)
        {
            if (ItemDisplays.TryGetValue(itemId, out var avatarItem))
            {
                avatarItem.ToggleWithoutNotify(isOn);
            }
        }

        /// <summary>
        /// Clears the Item Selection area.
        /// </summary>
        private void ClearItems()
        {
            if (gridAdapter.Data != null)
            {
                gridAdapter.RemoveItemsFrom(0, gridAdapter.Data.Count);
            }
        }

        /// <summary>
        /// Generates Avatar Item UI element displays.
        /// </summary>
        private async void GenerateItems(IEnumerable<AvatarItemConfig> itemConfigs)
        {
            ClearItems();
            selectionController.CreateActionMap();

            var sortByBatch = from item in itemConfigs
                              where !item.Unpurchaseable
                              orderby item.Data.itemBatch descending
                              select item;

            var itemListCopy = new List<AvatarItemConfig>(sortByBatch);
            itemListCopy.RemoveAll(item =>
                    !UserInventoryData.IsItemOwned(item.Data.itemId)
                 && string.IsNullOrEmpty(item.Data.cost.code));

            await Task.Delay(2); //wait one frame to ensure grid adapter is initialized - prevents race condition
            
            gridAdapter.AddItemsAt(gridAdapter.Data.Count, itemListCopy);
            
            ItemDisplays = (from holder in gridAdapter.activeHolders
                           select holder.myAvatarItem)
                           .ToDictionary(avatarItem => avatarItem.AvatarItemId);

            NotifyNewItems();
        }

        public void OnItemSelected(AvatarItemConfig itemConfig, bool isSelected)
        {   
            partReplacer.OnSelectedItem(itemConfig, isSelected);
        }
    }
}
