using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ItemEquipHud : BaseHud
    {
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private ItemButton _itemPrefab;

        [Inject] private DiContainer _diContainer;
        [Inject] private ItemDatabase _itemDatabase;
        [Inject] private IHudManager _hudManager;
        [Inject] private IPopupManager _popupManager;

        public new UniTask<IItem> Task { get; private set; }
        private ItemEquipPool ItemPool => _inventoryItemPool ??= new ItemEquipPool(_diContainer, _itemPrefab, _parentTransform);

        private IItem _currentItem;
        private IItem _selectedItem;
        private ItemEquipPool _inventoryItemPool;

        public ItemEquipHud Setup(IItem item)
        {
            _currentItem = item;
            Task = UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.WaitUntil(() => _selectedItem != null || _isClosed);
                return _selectedItem;
            });
            return this;
        }

        private void GenerateItems(IEnumerable<IItem> items)
        {
            ItemPool.ReturnAll();

            foreach (var item in items)
            {
                if (!item.GetType().IsSubclassOf(typeof(Emblem)))
                {
                    continue;
                }

                var itemObj = ItemPool.Rent();
                itemObj.Setup(item, async () =>
                {
                    var equippedItem = await _popupManager.ShowPopupAsync<ItemComparisonPopup>(PopupType.ItemComparison)
                                       .Setup(_currentItem, item)
                                       .Task;
                    
                    _selectedItem = equippedItem ?? item;
                    _hudManager.HideHud(HudType.ItemEquip);
                });
                itemObj.gameObject.SetActive(true);
            }
        }

        private void Start()
        {
            GenerateItems(_itemDatabase.AllItems);
        }

        private class ItemEquipPool : RinawaObjectPool<ItemButton>
        {
            public ItemEquipPool(DiContainer diContainer, ItemButton prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
