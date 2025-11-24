using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class Health : MonoBehaviour, IHittable
    {
        [SerializeField] private uint _max;
        [SerializeField] private Vector3 _healthBarOffset;
        [SerializeField] private bool _showHealthBar;

        public float PercentProgress => (float)_current / _max;

        public bool IsAlive => _current > 0;

        public bool IsHittable
        {
            get => IsAlive && _isHittable;
            set => _isHittable = value;
        }

        [Inject] private IHealthBarManager _healthBarManager;

        private uint _current;
        private IDamageBroadcaster _damageBroadcaster;
        private IDeathBroadcaster _deathBroadcaster;

        private bool _isHittable = true;

        public void OnUnitSpawn(UnitSpawn revive)
        {
            _current = _max;
        }

        public void Hit(uint damage, Vector3 origin)
        {
            if (!IsHittable)
            {
                return;
            }

            damage = _damageBroadcaster.BroadcastDamage(damage, _current, _max, origin);

            if (damage >= _current)
            {
                _current = 0;
                _deathBroadcaster.BroadcastDeath();
            }
            else
            {
                _current -= damage;
            }
        }

        public void RestorePercentage(float percentage)
        {
            _current = (uint)(_max * percentage);
        }

        public void RestoreAmount(uint healAmount)
        {
            _current += healAmount;

            if (_current > _max)
            {
                _current = _max;
            }
        }

        private void Awake()
        {
            _damageBroadcaster = GetComponent<IDamageBroadcaster>();
            _deathBroadcaster = GetComponent<IDeathBroadcaster>();
        }

        private void Start()
        {
            if (TryGetComponent<IMaxHealth>(out var maxHealth))
            {
                _max = maxHealth.MaxHealth;
            }

            _current = _max;
        }

        private void OnEnable()
        {
            if (_showHealthBar)
            {
                _healthBarManager?.Add(this, _healthBarOffset);
            }

            if (TryGetComponent<ISpawnEvent>(out var spawnEvent))
            {
                spawnEvent.OnSpawnEvent += OnUnitSpawn;
            }
        }

        private void OnDisable()
        {
            _healthBarManager?.Remove(this);

            if (TryGetComponent<ISpawnEvent>(out var spawnEvent))
            {
                spawnEvent.OnSpawnEvent -= OnUnitSpawn;
            }
        }
    }
}
