using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class PassiveLegacyPopulator : MonoBehaviour
    {
        [SerializeField] private bool _hideLegacyType;
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private LegacyPanel _legacyPanelPrefab;

        private LegacyPool PassiveLegacyPool => _legacyPool ??= new LegacyPool(_diContainer, _legacyPanelPrefab, _parentTransform);

        [Inject] private DiContainer _diContainer;
        [Inject] private LegacySlotTracker _legacySlotTracker;

        private LegacyPool _legacyPool;

        private void PopulateActiveLegacies(IEnumerable<ILegacy> legacies)
        {
            foreach (var legacy in legacies)
            {
                var legacyPanel = PassiveLegacyPool.Rent();
                legacyPanel.Setup(legacy, _hideLegacyType);
            }
        }

        private void OnEnable()
        {
            PopulateActiveLegacies(_legacySlotTracker.LegacyPassives.Values);
        }

        private void OnDisable()
        {
            PassiveLegacyPool.ReturnAll();
        }

        private class LegacyPool : RinawaObjectPool<LegacyPanel>
        {
            public LegacyPool(DiContainer diContainer, LegacyPanel prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
