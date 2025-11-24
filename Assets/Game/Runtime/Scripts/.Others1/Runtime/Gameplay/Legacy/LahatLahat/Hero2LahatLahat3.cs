using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class Hero2LahatLahat3 : Legacy, IWeaponHit
    {
        [Serializable]
        private struct Stats
        {
            public float _damage;
            public float _distance;
            public float _radius;
        }

        [SerializeField] private SpriteRenderer _vfx; // todo: replace with integrated visuals when available 
        [SerializeField] private uint _maxLevel;
        [SerializeField] private Stats[] _statsTable;
        [SerializeField] private ContactFilter2D _contactFilter;

        public override string LegacyId => "Hero2LahatLahat3";
        public override string LegacyName => "Stomp";

        public override LegacySlot LegacySlot => LegacySlot.LahatLahat;
        public override uint MaxLevel => _maxLevel;

        public event OnWeaponHit OnWeaponHit;

        private Stats _stats;
        private float _distance;
        private Vector2 _prevPosition;
        private Collider2D[] _colliders;
        private uint _totalDamage;

        private DiContainer _diContainer;
        private LazyInject<UnitPlayer> _unitPlayer;
        private LazyInject<LegacySlotTracker> _legacySlotTracker;

        public override void LevelUp()
        {
            if (_currentLevel == MaxLevel)
            {
                return;
            }

            _stats = _statsTable[_currentLevel];
            ++_currentLevel;
        }

        [Inject]
        private void Construct(DiContainer diContainer, LazyInject<UnitPlayer> unitPlayer, LazyInject<LegacySlotTracker> legacySlotTracker)
        {
            _diContainer = diContainer;
            _unitPlayer = unitPlayer;
            _legacySlotTracker = legacySlotTracker;
        }

        private void Pulse()
        {
            var position = transform.position;
            var indicator = _diContainer.InstantiatePrefabForComponent<SpriteRenderer>(_vfx, position, Quaternion.identity, null) 
                 ?? Instantiate(_vfx, position, Quaternion.identity);
            indicator.gameObject.SetActive(true);
            Destroy(indicator.gameObject, .5f); // todo: this is just a placeholder implementation to indicate stomp position

            var amount = Physics2D.OverlapCircle(transform.position, _stats._radius, _contactFilter, _colliders);

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
            if (legacy.LegacySlot == LegacySlot.Strike
                && legacy is IWeaponHitEffect hitEffect)
            {
                OnWeaponHit += hitEffect.OnHitEffect;
            }
        }

        private void Update()
        {
            if ((Vector2)transform.position == _prevPosition)
            {
                return;
            }

            if (_distance > 0f)
            {
                var delta = Vector2.Distance(transform.position, _prevPosition);
                _distance -= delta;
            }
            else
            {
                Pulse();
                _distance = _stats._distance;
            }

            _prevPosition = transform.position;
        }

        private void OnEnable()
        {
            _legacySlotTracker.Value.OnEquipLegacyEvent += OnEquipLegacy;
            _unitPlayer.Value.OnModifyStatsEvent += OnModifyStats;
        }

        private void OnDisable()
        {
            _legacySlotTracker.Value.OnEquipLegacyEvent -= OnEquipLegacy;
            _unitPlayer.Value.OnModifyStatsEvent -= OnModifyStats;
        }

        private void Start()
        {
            LevelUp();
            _prevPosition = transform.position;
            _colliders = new Collider2D[50];
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;

            foreach (var stats in _statsTable)
            {
                Gizmos.DrawWireSphere(transform.position, stats._radius);
            }
        }
    }
}
