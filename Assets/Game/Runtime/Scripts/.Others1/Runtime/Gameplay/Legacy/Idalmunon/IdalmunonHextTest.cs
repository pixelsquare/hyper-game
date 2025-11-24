using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class IdalmunonHexTest : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _damage;
            public float _distance;
            public float _radius;
        }

        [SerializeField] private uint _maxLevel;
        [SerializeField] private Stats[] _statsTable;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private SpriteRenderer _vfx; // todo: replace with integrated visuals when available
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private ContactFilter2D _contactFilter2D;

        public override LegacyLayer LegacyLayer => LegacyLayer.Idalmunon;
        public override string LegacyId => "HexTest";
        public override string LegacyName => "Hex Test";

        public override LegacySlot LegacySlot => LegacySlot.LahatLahat;
        public override uint MaxLevel => _maxLevel;

        private Stats _stats;
        private float _distance;
        private Vector2 _prevPosition;
        private Collider2D[] _colliders;
        private uint _totalDamage;

        private DiContainer _diContainer;
        private LazyInject<UnitPlayer> _unitPlayer;
        private LazyInject<HexSystem> _hexSystem;

        [Inject]
        private void Construct(DiContainer diContainer, LazyInject<UnitPlayer> unitPlayer, LazyInject<HexSystem> hexSystem)
        {
            _diContainer = diContainer;
            _unitPlayer = unitPlayer;
            _hexSystem = hexSystem;
        }

        public override void LevelUp()
        {
            if (_currentLevel == MaxLevel)
            {
                return;
            }

            _stats = _statsTable[_currentLevel];
            ++_currentLevel;
        }

        private void Pulse()
        {
            var position = transform.position;
            var indicator = _diContainer.InstantiatePrefabForComponent<SpriteRenderer>(_vfx, position, Quaternion.identity, null) 
                 ?? Instantiate(_vfx, position, Quaternion.identity);
            indicator.gameObject.SetActive(true);
            Destroy(indicator.gameObject, .5f); // todo: this is just a placeholder implementation to indicate stomp position

            var amount = Physics2D.OverlapCircle(transform.position, _stats._radius, _contactFilter2D, _colliders);

            for (var i = 0; i < amount; ++i)
            {
                var hit = _colliders[i];
                if (!hit.TryGetComponent<IHittable>(out var hittable))
                {
                    continue;
                }

                var hex = _diContainer.Instantiate<IdalmunonHex>(new object[]
                {
                    3, _stats._radius, _totalDamage,
                    _layerMask, hit.transform, _indicatorPrefab
                });
                _hexSystem.Value.OnHexApplyEvent(hex);
                hittable.Hit(_totalDamage, transform.position);
            }
        }

        private void OnModifyStats(PlayerStats stats)
        {
            _totalDamage = stats.CalcTotalDamage(_stats._damage);
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
            _unitPlayer.Value.OnModifyStatsEvent += OnModifyStats;
        }

        private void OnDisable()
        {
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
