using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class LegacyIconCounter : MonoBehaviour
    {
        [SerializeField] private RinawaText _legacyCountText;

        [SerializeField] private Transform _parentTransform;
        [SerializeField] private LegacyIcon _legacyIconPrefab;

        private LegacyIconPool IconPool => _legacyIconPool ??= new LegacyIconPool(_diContainer, _legacyIconPrefab, _parentTransform);

        [Inject] private DiContainer _diContainer;
        [Inject] private LegacySlotTracker _legacySlotTracker;

        private LegacyIconPool _legacyIconPool;

        private readonly ReactiveProperty<string> _legacyCountProp = new(null);

        private void PopulateLegacies(IEnumerable<ILegacy> legacies)
        {
            var count = 0;

            foreach (var legacy in legacies)
            {
                if (count < 5)
                {
                    var legacyIcon = IconPool.Rent();
                    legacyIcon.Setup(legacy.LegacyIconName);
                    legacyIcon.transform.SetAsFirstSibling();
                }

                count++;
            }

            _legacyCountProp.Value = count <= 0 ? $"{count}" : $"+{count}";
        }

        private void OnEnable()
        {
            _legacyCountProp.Subscribe(x =>
            {
                _legacyCountText.text = x;
                _legacyCountText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            });
        }

        private void OnDestroy()
        {
            _legacyCountProp.Dispose();
        }

        private void Start()
        {
            var legacies = new List<ILegacy>();
            legacies.AddRange(_legacySlotTracker.LegacyActives.Values);
            legacies.AddRange(_legacySlotTracker.LegacyPassives.Values);
            PopulateLegacies(legacies);
        }

        private class LegacyIconPool : RinawaObjectPool<LegacyIcon>
        {
            public LegacyIconPool(DiContainer diContainer, LegacyIcon prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
