using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianCast : Legacy, IMonoSpawnPool<KahangianProjectile>, IModifyCooldown, IModifyThunderLegacies
    {
        [Serializable]
        private struct Stats
        {
            public uint _damage;
            public float _range;
            public float _interval;
        }

        [SerializeField] private KahangianProjectile _projectilePrefab;
        [SerializeField] private float _projectileSpeed;
        [SerializeField] private float _projectileRange;
        [SerializeField] private HexIndicator _hexIndicatorPrefab;
        [SerializeField] private float _hexDuration;
        [SerializeField] private float _extraKnockback;
        [SerializeField] private ContactFilter2D _contactFilter;
        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "KahangianCast";
        public override uint MaxLevel => (uint)_statsTable.Length;
        public KahangianProjectile Prefab { get; private set; }
        public IEnumerable<KahangianProjectile> Pool => _projectilePool;

        private Stats _stats;
        private float _interval;
        private List<KahangianProjectile> _projectilePool;
        private float _additionalDamageToHexed;
        private uint _totalDamage;
        private float _additionalSize;

        private IAimDirection _aimDirection;
        private DiContainer _diContainer;
        private IAudioManager _audioManager;
        private UnitPlayer _unitPlayer;

        public override void LevelUp()
        {
            if (_currentLevel == MaxLevel)
            {
                return;
            }

            _stats = _statsTable[_currentLevel++];
        }

        [Inject]
        private void Construct(DiContainer diContainer, IAudioManager audioManager, LazyInject<UnitPlayer> unitPlayer)
        {
            _diContainer = diContainer;
            _audioManager = audioManager;
            _unitPlayer = unitPlayer.Value;
        }
        
        public KahangianProjectile Spawn()
        {
            var instance = Pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);

            if (instance)
            {
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = _diContainer.InstantiatePrefabForComponent<KahangianProjectile>(Prefab);
                instance.OnProjectileExpire += projectile => Despawn((KahangianProjectile) projectile);
                _projectilePool.Add(instance);
            }

            return instance;
        }
        
        public void Despawn(KahangianProjectile spawn)
        {
            spawn.gameObject.SetActive(false);
            spawn.transform.parent = transform;
        }

        public void OnCooldownModified(float cooldownReduction)
        {
            for (int i = 0; i<_statsTable.Length; i++)
            {
                var oldStat = _statsTable[i];

                _statsTable[i] = new Stats
                {
                    _damage = oldStat._damage,
                    _interval = oldStat._interval - oldStat._interval * cooldownReduction,
                };
            }
            
            _stats = _statsTable[_currentLevel - 1];
        }

        private void OnModifyCooldown(IMessage message)
        {
            var cooldownReduction = (float)message.Data;
            OnCooldownModified(cooldownReduction);
        }
        
        public void OnIncreaseDamageToHexed(float additionalDamagePct)
        {
            _additionalDamageToHexed = additionalDamagePct;
        }
        
        private void OnModifyThunderDamage(IMessage message)
        {
            var additionalDamagePct = (float)message.Data;
            OnIncreaseDamageToHexed(additionalDamagePct);
        }

        public void OnModifyEnlargement(float additionalSize)
        {
            _additionalSize = additionalSize;
        }
        
        private void OnReceivedModifyEnlargement(IMessage message)
        {
            var additionalSizePct = (float)message.Data;
            OnModifyEnlargement(additionalSizePct);
        }

        private void OnModifyStats(PlayerStats stats)
        {
            _totalDamage = stats.CalcTotalDamage(_stats._damage);
        }

        private Vector3 GetNearestTarget() // todo: expensive operation, optimize
        {
            var hitColliders = new Collider2D[50];
            var hitAmount = Physics2D.OverlapCircle(transform.position, _stats._range, _contactFilter, hitColliders);
            
            var direction = _aimDirection.AimDirection;
            var minDistance = float.MaxValue;

            for (var i = hitAmount - 1; i > -1; --i)
            {
                var targetPosition = hitColliders[i].transform.position;
                var distance = Vector2.Distance(transform.position, targetPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    direction = (targetPosition - transform.position).normalized;
                }
            }

            return direction;
        }
        
        private void Update()
        {
            if (_interval > 0)
            {
                _interval -= Time.deltaTime;
            }
            else
            {
                var position = transform.position;
                var projectile = Spawn();
                
                var projectileTransform = projectile.transform;
                projectileTransform.position = position;
                projectileTransform.rotation = Quaternion.identity;
                projectileTransform.parent = null;
                
                projectile.Speed = _projectileSpeed;
                projectile.Direction = GetNearestTarget();
                projectile.Damage = _totalDamage;
                projectile.Distance = _projectileRange;
                projectile.ReturnTarget = transform.parent;
                projectile.HexDuration = _hexDuration;
                projectile.KnockbackValue = _extraKnockback;
                projectile.IndicatorPrefab = _hexIndicatorPrefab;
                projectile.AdditionalDamageToHexed = _additionalDamageToHexed;
                projectile.AdditionalSize = _additionalSize;
                
                _audioManager.PlaySound(Sfx.KahangianCast);
                
                _interval = _stats._interval;
            }
        }

        private void Awake()
        {
            Prefab = _projectilePrefab;
            _aimDirection = transform.parent.GetComponent<IAimDirection>();
            _projectilePool = new List<KahangianProjectile>();
        }

        private void Start()
        {
            LevelUp();
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void OnEnable()
        {
            _unitPlayer.OnModifyStatsEvent += OnModifyStats;
            Dispatcher.AddListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown);
            Dispatcher.AddListener(GameEvents.Gameplay.ModifyThunderLegacyDamage, OnModifyThunderDamage);
            Dispatcher.AddListener(GameEvents.Gameplay.ModifyBaklawEnlargement, OnReceivedModifyEnlargement);
        }
        
        private void OnDisable()
        {
            _unitPlayer.OnModifyStatsEvent -= OnModifyStats;
            Dispatcher.RemoveListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown, true);
            Dispatcher.RemoveListener(GameEvents.Gameplay.ModifyThunderLegacyDamage, OnModifyThunderDamage, true);
            Dispatcher.RemoveListener(GameEvents.Gameplay.ModifyBaklawEnlargement, OnReceivedModifyEnlargement, true);
        }
    }
}
