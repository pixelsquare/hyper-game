using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class SampleLegacy : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _radius;
            public float _interval;
            public uint _damage;
            public GameObject _indicator;
        }

        [SerializeField] private uint _maxLevel = 5;
        [SerializeField] private Stats[] _statsTable = new Stats[5];
        [SerializeField] private ContactFilter2D _contactFilter;

        public override string LegacyId => "SampleLegacy";
        public override string LegacyName => "Sample Legacy";

        public override LegacySlot LegacySlot => LegacySlot.Smite;
        public override uint MaxLevel => _maxLevel;

        private float _duration;
        private Collider2D[] _colliders;
        private Stats _stats;
        private uint _totalDamage;

        private LazyInject<UnitPlayer> _unitPlayer;

        private const int HIT_MAX = 200; // todo [jef] : use max enemy amount

        public override void LevelUp()
        {
            ++_currentLevel;
            _stats._indicator.SetActive(false);
            _stats = _statsTable[_currentLevel - 1];
            _stats._indicator.SetActive(true);
        }

        [Inject]
        private void Construct(LazyInject<UnitPlayer> unitPlayer)
        {
            _unitPlayer = unitPlayer;
        }

        private void Pulse()
        {
            var hits = Physics2D.OverlapCircle(transform.position, _stats._radius, _contactFilter, _colliders);

            for (var i = 0; i < hits; ++i)
            {
                var col = _colliders[i];

                if (!col.TryGetComponent<IHittable>(out var hittable)
                    || !hittable.IsHittable)
                {
                    continue;
                }

                hittable.Hit(_totalDamage, transform.position);
            }
        }

        private void OnModifyStats(PlayerStats stats)
        {
            _totalDamage = stats.CalcTotalDamage(_stats._damage);
        }

        private void Update()
        {
            if (_duration < 0f)
            {
                _duration = _stats._interval;
                Pulse();
            }
            else
            {
                _duration -= Time.deltaTime;
            }
        }

        private void Awake()
        {
            _currentLevel = 1;
        }

        private void OnEnable()
        {
            _unitPlayer.Value.OnModifyStatsEvent += OnModifyStats;
        }

        private void OnDisable()
        {
            _unitPlayer.Value.OnModifyStatsEvent -= OnModifyStats;
        }

        private void Start()
        {
            _colliders = new Collider2D[HIT_MAX];
            _stats = _statsTable[_currentLevel - 1];
            _stats._indicator.SetActive(true);
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void OnValidate()
        {
            if (_statsTable.Length != MaxLevel)
            {
                _statsTable = new Stats[MaxLevel];
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            foreach (var stats in _statsTable)
            {
                Gizmos.DrawWireSphere(transform.position, stats._radius);
            }
        }
    }
}
