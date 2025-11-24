using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ActiveLegacyIconPopulator : MonoBehaviour
    {
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private LegacyIconProgress _legacyIconProgressPrefab;

        private LegacyPool ActiveLegacyPool => _legacyPool ??= new LegacyPool(_diContainer, _legacyIconProgressPrefab, _parentTransform);

        [Inject] private DiContainer _diContainer;
        [Inject] private LegacySlotTracker _legacySlotTracker;

        private LegacyPool _legacyPool;

        private void PopulateActiveLegacies(IEnumerable<ILegacy> legacies)
        {
            foreach (var legacy in legacies)
            {
                var legacyPanel = ActiveLegacyPool.Rent();
                legacyPanel.Setup(legacy);
            }
        }

        private void OnEnable()
        {
            PopulateActiveLegacies(_legacySlotTracker.LegacyActives.Values);
        }

        private void OnDisable()
        {
            ActiveLegacyPool.ReturnAll();
        }

        private class LegacyPool : RinawaObjectPool<LegacyIconProgress>
        {
            public LegacyPool(DiContainer diContainer, LegacyIconProgress prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
