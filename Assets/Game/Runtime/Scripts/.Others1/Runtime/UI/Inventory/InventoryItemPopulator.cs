using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class InventoryItemPopulator : MonoBehaviour
    {
        [SerializeField] private GameObject _emptyOverlay;
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private ItemButton _itemPrefab;

        [Inject] private DiContainer _diContainer;
        [Inject] private IInventoryModule _inventoryModule;
        [Inject] private InventoryItemDetail _inventoryItemDetail;

        private InventoryItemPool ItemPool => _inventoryItemPool ??= new InventoryItemPool(_diContainer, _itemPrefab, _parentTransform);

        private InventoryItemPool _inventoryItemPool;
        private readonly List<ItemButton> _itemButtons = new();

        private readonly ReactiveProperty<bool> _overlayActiveProp = new(false);

        public void GenerateItems(IEnumerable<IItem> items)
        {
            var itemArray = items as IItem[] ?? items.ToArray();
            var itemCount = itemArray.Length;

            _overlayActiveProp.Value = itemCount == 0;

            _itemButtons.Clear();
            ItemPool.ReturnAll();

            foreach (var item in itemArray)
            {
                var itemObj = ItemPool.Rent();

                switch (item)
                {
                    case null: continue;
                    case Emblem:
                        itemObj.Setup(item, () =>
                        {
                            _itemButtons.ForEach(x => x.SetButtonSelected(false));
                            _inventoryItemDetail.Setup(item);
                        });
                        break;
                    default:
                        var count = _inventoryModule.GetItemCount(item);
                        itemObj.Setup(item, count, () =>
                        {
                            _itemButtons.ForEach(x => x.SetButtonSelected(false));
                            _inventoryItemDetail.Setup(item);
                        });
                        break;
                }

                itemObj.gameObject.SetActive(true);
                _itemButtons.Add(itemObj);
            }

            var selectedItemButton = _itemButtons.Count > 0 ? _itemButtons.First() : null;
            _inventoryItemDetail.Setup(selectedItemButton?.Item);
            selectedItemButton?.SetButtonSelected(true);
        }

        private void OnEnable()
        {
            _overlayActiveProp.Subscribe(x => _emptyOverlay.SetActive(x));
        }

        private void OnDestroy()
        {
            _overlayActiveProp.Dispose();
        }

        private class InventoryItemPool : RinawaObjectPool<ItemButton>
        {
            public InventoryItemPool(DiContainer diContainer, ItemButton prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
