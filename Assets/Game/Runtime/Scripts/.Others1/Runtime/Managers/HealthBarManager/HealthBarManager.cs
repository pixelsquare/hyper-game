using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Santelmo.Rinsurv
{
    public class HealthBarManager : IHealthBarManager
    {
        private class UnitHealthBar
        {
            public HealthBar healthBar;
            public IDisposable[] disposables;
        }

        private readonly DiContainer _diContainer;
        private readonly Transform _parentTransform;
        private readonly HealthBar _healthBarPrefab;

        private readonly Dictionary<Health, UnitHealthBar> _unitHealthMap = new();

        public HealthBarManager(DiContainer diContainer, HealthBar healthBarPrefab, Transform parentTransform)
        {
            _diContainer = diContainer;
            _parentTransform = parentTransform;
            _healthBarPrefab = healthBarPrefab;
        }

        public void Add(Health health, Vector3 offset)
        {
            var healthBar = _diContainer.InstantiatePrefabForComponent<HealthBar>(_healthBarPrefab, _parentTransform);
            healthBar.SetActive(false);

            var disposables = new IDisposable[2];
            disposables[0] = Observable.IntervalFrame(1)
                                       .Subscribe(_ => { healthBar.transform.position = health.transform.position + offset; });
            disposables[1] = health.ObserveEveryValueChanged(x => x.PercentProgress)
                                   .Subscribe(x => healthBar.SetHealth(x));

            var unitHealthBar = new UnitHealthBar
            {
                healthBar = healthBar,
                disposables = disposables
            };

            _unitHealthMap.Add(health, unitHealthBar);
        }

        public void Remove(Health health)
        {
            if (!_unitHealthMap.TryGetValue(health, out var unitHealthBar))
            {
                return;
            }

            Object.Destroy(unitHealthBar.healthBar.gameObject);
            unitHealthBar.disposables.ForEach(x => x.Dispose());
        }
    }
}
