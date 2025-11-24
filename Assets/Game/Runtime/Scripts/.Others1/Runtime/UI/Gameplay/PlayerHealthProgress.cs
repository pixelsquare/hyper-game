using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class PlayerHealthProgress : MonoBehaviour
    {
        [SerializeField] private RinawaImage _progressImage;

        [Inject] private UnitPlayer _unitPlayer;

        private Health _playerHealth;
        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<float> _progressProp = new(1.0f);

        private void HandlePlayerDamagedEvent(uint damage, uint currentHealth, uint maxHealth, Vector3 origin)
        {
            _progressProp.Value = _playerHealth.PercentProgress;
        }

        private void Awake()
        {
            if (!_unitPlayer.TryGetComponent(out _playerHealth))
            {
                throw new NullReferenceException("Health component is missing from `UnitPlayer`.");
            }
        }

        private void OnEnable()
        {
            _unitPlayer.OnUnitDamage += HandlePlayerDamagedEvent;
            _progressProp.Subscribe(x => _progressImage.fillAmount = 1f - x)
                         .AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            _unitPlayer.OnUnitDamage -= HandlePlayerDamagedEvent;
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
