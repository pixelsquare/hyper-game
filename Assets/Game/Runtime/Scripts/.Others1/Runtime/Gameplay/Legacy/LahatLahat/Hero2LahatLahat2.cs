using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv.LahatLahat.Kick
{
    public class Hero2LahatLahat2 : Legacy, IWeaponHit, IModifyCooldown
    {
        [Serializable]
        private struct Stats
        {
            public float _damage;
            public float _hitRate;
            public float _interval;
            public float _duration;
            public float _radius;
        }

        [SerializeField] private SpriteRenderer _spriteRenderer; //todo: replace with vfx when available
        [SerializeField] private uint _maxLevel;
        [SerializeField] private Stats[] _statsTable;
        [SerializeField] private ContactFilter2D _contactFilter;

        public override string LegacyId => "Hero2LahatLahat2";
        public override string LegacyName => "Spinning Kick";
        public override LegacySlot LegacySlot => LegacySlot.LahatLahat;
        public override uint MaxLevel => _maxLevel;

        public event OnWeaponHit OnWeaponHit;

        private Stats _stats;
        private float _hitRate;
        private float _duration;
        private float _interval;
        private Collider2D[] _colliders;
        private uint _totalDamage;

        private LazyInject<UnitPlayer> _unitPlayer;
        private LazyInject<LegacySlotTracker> _legacySlotTracker;

        public override void LevelUp()
        {
            _stats = _statsTable[_currentLevel];
            ++_currentLevel;
        }

        [Inject]
        private void Construct(LazyInject<UnitPlayer> unitPlayer, LazyInject<LegacySlotTracker> legacySlotTracker)
        {
            _unitPlayer = unitPlayer;
            _legacySlotTracker = legacySlotTracker;
        }

        private void Pulse()
        {
            var amount = Physics2D.OverlapCircle(transform.position, _stats._radius, _contactFilter, _colliders);

            if (amount < 1)
            {
                return;
            }

            for (var i = 0; i < amount; ++i)
            {
                var hit = _colliders[i];
                if (!hit.TryGetComponent<IHittable>(out var hittable))
                {
                    continue;
                }

                hittable.Hit(_totalDamage, transform.position);
                OnWeaponHit?.Invoke(hit.transform);
            }
        }

        private void OnModifyStats(PlayerStats stats)
        {
            _totalDamage = stats.CalcTotalDamage(_stats._damage);
        }

        private void OnEquipLegacy(ILegacy legacy)
        {
            if (legacy is IWeaponHitEffect hitEffect)
            {
                OnWeaponHit += hitEffect.OnHitEffect;
            }
        }
        
        public void OnCooldownModified(float cooldownReduction)
        {
            for (int i = 0; i<_statsTable.Length; i++)
            {
                var oldStat = _statsTable[i];

                _statsTable[i] = new Stats
                {
                    _damage = oldStat._damage,
                    _hitRate = oldStat._hitRate,
                    _interval = oldStat._interval - oldStat._interval * cooldownReduction,
                    _duration = oldStat._duration,
                    _radius = oldStat._radius,
                };
            }
            
            _stats = _statsTable[_currentLevel - 1];
        }

        private void OnModifyCooldown(IMessage message)
        {
            var cooldownReduction = (float)message.Data;
            OnCooldownModified(cooldownReduction);
        }

        private void Update()
        {
            if (_interval > 0f)
            {
                _interval -= Time.deltaTime;
            }
            else
            {
                _interval = _stats._interval;
                _duration = _stats._duration;
                _spriteRenderer.enabled = true;
            }

            if (_duration > 0f)
            {
                _duration -= Time.deltaTime;
            }
            else
            {
                _spriteRenderer.enabled = false;
                return;
            }

            if (_hitRate > 0f)
            {
                _hitRate -= Time.deltaTime;
            }
            else
            {
                _hitRate = _stats._hitRate;
                Pulse();
            }
        }

        private void OnEnable()
        {
            _legacySlotTracker.Value.OnEquipLegacyEvent += OnEquipLegacy;
            _unitPlayer.Value.OnModifyStatsEvent += OnModifyStats;
            Dispatcher.AddListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown);
        }

        private void OnDisable()
        {
            _legacySlotTracker.Value.OnEquipLegacyEvent -= OnEquipLegacy;
            _unitPlayer.Value.OnModifyStatsEvent -= OnModifyStats;
            Dispatcher.RemoveListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown, true);
        }

        private void Start()
        {
            _colliders = new Collider2D[200];
            LevelUp();
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;

            foreach (var stats in _statsTable)
            {
                Gizmos.DrawWireSphere(transform.position, stats._radius);
            }

            if (Application.isPlaying && _duration > 0f)
            {
                Gizmos.DrawSphere(transform.position, _stats._radius);
            }
        }
    }
}
