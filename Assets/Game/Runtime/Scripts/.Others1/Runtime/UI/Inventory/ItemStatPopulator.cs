using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ItemStatPopulator : MonoBehaviour
    {
        [SerializeField] private Transform _itemStatParentTransform;
        [SerializeField] private ItemStatPanel _itemStatPrefab;

        private ItemStatPool ItemStatsPool => _itemStatsPool ??= new ItemStatPool(_diContainer, _itemStatPrefab, _itemStatParentTransform);

        [Inject] private DiContainer _diContainer;

        private ItemStatPool _itemStatsPool;

        public void Setup(IEnumerable<ItemStats> itemStats)
        {
            ItemStatsPool.ReturnAll();

            foreach (var itemStat in itemStats)
            {
                var statObj = ItemStatsPool.Rent();
                statObj.Setup(itemStat.icon, itemStat.name, (int)itemStat.value, (int)itemStat.change);
                statObj.gameObject.SetActive(true);
            }
        }

        private class ItemStatPool : RinawaObjectPool<ItemStatPanel>
        {
            public ItemStatPool(DiContainer diContainer, ItemStatPanel prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
