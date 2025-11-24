using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Santelmo.Rinsurv
{
    public class IdalmunonSmite : Legacy, IMonoSpawnPool<SelfDespawn>
    {
        [Serializable]
        private struct Stats
        {
            public float _damage;
            public float _radius;
            public float _cooldown;
        }

        [SerializeField] private uint _maxLevel;
        [SerializeField] private Stats[] _statsTable;
        [SerializeField] private Vector2 _viewBoxDims;
        [SerializeField] private ContactFilter2D _contactFilter;
        [SerializeField] private SelfDespawn _vfx;

        public override string LegacyId => "IdalmunonSmite";
        public override uint MaxLevel => _maxLevel;
        public SelfDespawn Prefab { get; private set; }
        public IEnumerable<SelfDespawn> Pool => _selfDespawnerPool;

        private Stats _stats;
        private Collider2D[] _colliders;
        private uint _totalDamage;
        private float _duration;
        private List<SelfDespawn> _selfDespawnerPool;

        private DiContainer _diContainer;
        private IAudioManager _audioManager;
        private LazyInject<UnitPlayer> _unitPlayer;

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
        private void Construct(DiContainer diContainer, IAudioManager audioManager, LazyInject<UnitPlayer> unitPlayer)
        {
            _diContainer = diContainer;
            _audioManager = audioManager;
            _unitPlayer = unitPlayer;
        }

        private void Pulse()
        {
            var enemyCountInBox = Physics2D.OverlapBox(transform.position, _viewBoxDims, 0, _contactFilter, _colliders);

            var smoteEnemyIndex = Random.Range(0, enemyCountInBox);
            var smoteEnemy = _colliders[smoteEnemyIndex];

            var amount = Physics2D.OverlapCircle(smoteEnemy.transform.position, _stats._radius, _contactFilter, _colliders);
            for (var i = 0; i < amount; ++i)
            {
                var hit = _colliders[i];
                if (!hit.TryGetComponent<IHittable>(out var hittable))
                {
                    continue;
                }

                hittable.Hit(_totalDamage, transform.position);
            }

            var position = smoteEnemy.transform.position;
            var indicatorVFX = Spawn();
            
            var indicatorVFXTransform = indicatorVFX.transform;
            indicatorVFXTransform.position = position;
            indicatorVFXTransform.rotation = _vfx.transform.rotation;
            indicatorVFXTransform.parent = null;
            
            indicatorVFX.SetReturnParent(transform);
            indicatorVFX.gameObject.SetActive(true);
            
            _audioManager.PlaySound(Sfx.IdalmunonSmite);
        }

        private void OnModifyStats(PlayerStats stats)
        {
            _totalDamage = stats.CalcTotalDamage(_stats._damage);
        }
        
        public SelfDespawn Spawn()
        {
            var instance = Pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);

            if (instance)
            {
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = _diContainer.InstantiatePrefabForComponent<SelfDespawn>(Prefab);
                _selfDespawnerPool.Add(instance);
            }

            return instance;
        }
        
        public void Despawn(SelfDespawn spawn)
        {
            spawn.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_duration < 0f)
            {
                _duration = _stats._cooldown;
                Pulse();
            }
            else
            {
                _duration -= Time.deltaTime;
            }
        }
        
        private void Awake()
        {
            _selfDespawnerPool = new List<SelfDespawn>();
            Prefab = _vfx;
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
            _colliders = new Collider2D[50];
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(transform.position, new Vector3(_viewBoxDims.x, _viewBoxDims.y, 0));
        }
    }
}
