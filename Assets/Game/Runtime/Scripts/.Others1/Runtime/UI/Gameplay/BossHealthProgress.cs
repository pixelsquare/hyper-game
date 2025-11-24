using System;
using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class BossHealthProgress : MonoBehaviour
    {
        [SerializeField] private RinawaText _bossNameText;
        [SerializeField] private RinawaSlider _bossHealthSlider;

        private CompositeDisposable _compositeDisposable = new();
        private IDamageEvent _damageEvent;

        private readonly ReactiveProperty<string> _bossNameProp = new(null);
        private readonly ReactiveProperty<float> _bossHealthProp = new(0);

        public void Setup(UnitSpawn unitSpawn)
        {
            if (!unitSpawn.TryGetComponent<IMaxHealth>(out var maxHealth) 
             || !unitSpawn.TryGetComponent(out _damageEvent)
             || !unitSpawn.TryGetComponent<UnitEnemy>(out var unitEnemy))
            {
                throw new NullReferenceException("Unit spawn does not contain required references.");
            }

            _bossNameProp.Value = unitEnemy.Info.Name;
            _bossHealthProp.Value = (int)maxHealth.MaxHealth;
            _damageEvent.OnUnitDamage += OnBossHealthUpdated;
        }

        private void OnBossHealthUpdated(uint damage, uint currentHealth, uint maxHealth, Vector3 origin)
        {
            var healthPercentage = (float)currentHealth / maxHealth;
            _bossHealthProp.Value = healthPercentage;
        }

        private void OnEnable()
        {
            _bossNameProp.Subscribe(x => _bossNameText.text = x)
                         .AddTo(_compositeDisposable);

            _bossHealthProp.Subscribe(x => _bossHealthSlider.value = x)
                           .AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            _damageEvent.OnUnitDamage -= OnBossHealthUpdated;
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
