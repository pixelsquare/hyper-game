using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ItemPassivePopulator : MonoBehaviour
    {
        [SerializeField] private Transform _itemStatParentTransform;
        [SerializeField] private ItemPassivePanel _itemPassivePrefab;

        private ItemStatUpgradePool ItemPassivePool => _itemPassivePool ??= new ItemStatUpgradePool(_diContainer, _itemPassivePrefab, _itemStatParentTransform);

        [Inject] private DiContainer _diContainer;
        private ItemStatUpgradePool _itemPassivePool;

        public void Setup(IEnumerable<ItemStats> itemStats)
        {
            ItemPassivePool.ReturnAll();

            foreach (var itemStat in itemStats)
            {
                var statObj = ItemPassivePool.Rent();
                statObj.Setup(itemStat.icon, itemStat.name, (int)itemStat.value, (int)itemStat.change);
                statObj.gameObject.SetActive(true);
            }
        }

        private class ItemStatUpgradePool : RinawaObjectPool<ItemPassivePanel>
        {
            public ItemStatUpgradePool(DiContainer diContainer, ItemPassivePanel prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
