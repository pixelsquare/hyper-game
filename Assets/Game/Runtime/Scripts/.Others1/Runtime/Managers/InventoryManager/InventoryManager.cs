using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Santelmo.Rinsurv
{
    using SaveKey = GameConstants.SaveKeys;

    public class InventoryManager : IInventoryManager
    {
        private readonly ItemDatabase _itemDatabase;
        private readonly IInventoryModule _inventoryModule;
        private readonly ISaveManager _saveManager;

        [Inject]
        public InventoryManager(ItemDatabase itemDatabase, IInventoryModule inventoryModule, ISaveManager saveManager)
        {
            _itemDatabase = itemDatabase;
            _inventoryModule = inventoryModule;
            _saveManager = saveManager;
        }

        public async UniTask InitializeAsync()
        {
            if (GameUtil.ShouldSyncUp(_saveManager))
            {
                FillDefaultItems();
                await SyncUpAsync();
                return;
            }

            await SyncDownAsync();
        }

        public void AddItemsToInventory(IEnumerable<IItem> items)
        {
            foreach (var item in items)
            {
                _inventoryModule.AddItem(item, 1);
            }
        }

        public async UniTask SyncUpAsync()
        {
            // FIX: [TONY] Should be placed and initialized on backend ideally.
            await _inventoryModule.WriteItemCountsAsync();
        }

        public async UniTask SyncDownAsync()
        {
            await _inventoryModule.ReadFromCloudAsync();
        }

        private void FillDefaultItems()
        {
            // TODO: Default items should be placed here.
            AddItemsToInventory(_itemDatabase.AllItems);
        }
    }
}
