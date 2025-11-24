using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Santelmo.Rinsurv
{
    public class IdalmunonCast : Legacy, IMonoSpawnPool<SelfDespawn>
    {
        [Serializable]
        private struct Stats
        {
            public float _damage;
            public float _cooldown;
        }

        [SerializeField] private ParticleSystem _vfx;
        [SerializeField] private Transform _vfxPivot;
        [SerializeField] private SelfDespawn _hitVfx; //todo: replace with vfx when available
        [SerializeField] private uint _maxLevel;
        [SerializeField] private Stats[] _statsTable;
        [SerializeField] private ContactFilter2D _contactFilter;
        [SerializeField] private float _nearCheckRadius;

        public override string LegacyId => "IdalmunonCast";
        public override uint MaxLevel => _maxLevel;
        public SelfDespawn Prefab { get; private set; }
        public IEnumerable<SelfDespawn> Pool => _selfDespawnerPool;

        private Stats _stats;
        private float _cooldown;
        private Collider2D[] _colliders;
        private uint _totalDamage;
        private List<SelfDespawn> _selfDespawnerPool;

        private DiContainer _diContainer;
        private IAudioManager _audioManager;
        private LazyInject<UnitPlayer> _unitPlayer;

        public override void LevelUp()
        {
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
            var amount = Physics2D.OverlapCircle(transform.position, _nearCheckRadius, _contactFilter, _colliders);

            if (amount < 1)
            {
                return;
            }

            var targetEnemyIndex = Random.Range(0, amount);
            var targetEnemy = _colliders[targetEnemyIndex].gameObject;

            if (!targetEnemy.TryGetComponent<IHittable>(out var contactHit))
            {
                return;
            }

            contactHit.Hit(_totalDamage, transform.position);

            var enemyDirection = new Vector2(targetEnemy.transform.position.x - transform.position.x,
                targetEnemy.transform.position.y - transform.position.y);

            var angle = Vector2.Angle(Vector2.up, enemyDirection);
            if (enemyDirection.x > 0)
            {
                angle = 360 - angle;
            }

            _vfxPivot.transform.localEulerAngles = new Vector3(0, 0, angle);
            _vfx.Stop();
            _vfx.Play();

            //leaving this in as an option in case more VFX is requested to better identify which was the target
            if (_hitVfx != null)
            {
                var position = targetEnemy.transform.position;
                var hitIndicator = Spawn();
                
                var hitIndicatorTransform = hitIndicator.transform;
                hitIndicatorTransform.position = position;
                hitIndicatorTransform.rotation = Quaternion.identity;
                hitIndicatorTransform.parent = null;

                hitIndicator.SetReturnParent(transform);
                hitIndicator.gameObject.SetActive(true);
            }

            _audioManager.PlaySound(Sfx.IdalmunonCast);
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
            if (_cooldown < 0f)
            {
                _cooldown = _stats._cooldown;
                Pulse();
            }
            else
            {
                _cooldown -= Time.deltaTime;
            }
        }

        private void Awake()
        {
            _selfDespawnerPool = new List<SelfDespawn>();
            Prefab = _hitVfx;
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
            _colliders = new Collider2D[200];
            LevelUp();
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _nearCheckRadius);
        }
    }
}
