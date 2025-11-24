using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianPassive1 : Legacy, IMomentumStopEffect, IMonoSpawnPool<SelfDespawn>
    {
        [Serializable]
        private struct Stats
        {
            public float _damageMultiplier;
            public float _knockbackDistance;
        }

        [SerializeField] private float _damageRadius;
        [SerializeField] private float _baseDamage;
        [SerializeField] private SelfDespawn _vfx;
        [SerializeField] private ContactFilter2D _contactFilter;
        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "KahangianPassive1";

        public override uint MaxLevel => (uint)_statsTable.Length;
        public SelfDespawn Prefab { get; private set; }
        public IEnumerable<SelfDespawn> Pool => _selfDespawnerPool;
        
        private Collider2D[] _colliders;
        private Stats _stats;
        private List<SelfDespawn> _selfDespawnerPool;
        private DiContainer _diContainer;
        private LazyInject<LegacySlotTracker> _legacySlotTracker;
        private IAudioManager _audioManager;

        public override void LevelUp()
        {
            if (_currentLevel == MaxLevel)
            {
                return;
            }

            _stats = _statsTable[_currentLevel++];
        }

        [Inject]
        private void Construct(DiContainer diContainer, LazyInject<LegacySlotTracker> legacySlotTracker, IAudioManager audioManager)
        {
            _diContainer = diContainer;
            _legacySlotTracker = legacySlotTracker;
            _audioManager = audioManager;
        }
        
        private void OnEquipLegacy(ILegacy legacy)
        {
            if (legacy is IMomentumStop momentumLegacy)
            {
                momentumLegacy.OnMomentumStop += OnMomentumStopEffect;
            }
        }

        public void OnMomentumStopEffect(bool isAtFullSpeed)
        {
            if (!isAtFullSpeed)
            {
                return;
            }
            
            //vfx to affected enemy area
            if (_vfx != null)
            {
                var position = transform.position;
                var indicator = Spawn();
                    
                var indicatorTransform = indicator.transform;
                indicatorTransform.position = position;
                indicatorTransform.rotation = _vfx.transform.rotation;
                indicatorTransform.parent = null;
                    
                indicator.SetReturnParent(transform);
                indicator.gameObject.SetActive(true);
            }

            var hits = Physics2D.OverlapCircle(transform.position, _damageRadius, _contactFilter, _colliders);

            for (var i = 0; i < hits; ++i)
            {
                var col = _colliders[i];

                if (!col.TryGetComponent<Health>(out var contactHealth))
                {
                    continue;
                }
                
                //add extra knockback temporarily
                var unitEnemy = col.GetComponent<UnitEnemy>();
                
                var newStats = new EnemyStats
                {
                    _extraKnockback = _stats._knockbackDistance
                };

                unitEnemy.ModifyStats(newStats);

                //hit
                contactHealth.Hit((uint)(_baseDamage * _stats._damageMultiplier), transform.position);
                
                //remove extra knockback
                var removeStats = new EnemyStats
                {
                    _extraKnockback = -_stats._knockbackDistance
                };

                unitEnemy.ModifyStats(removeStats);
            }
            
            _audioManager.StopSound(Sfx.KahangianPassive1);
            _audioManager.PlaySound(Sfx.KahangianPassive1);
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
        
        private void Awake()
        {
            _selfDespawnerPool = new List<SelfDespawn>();
            Prefab = _vfx;
        }
        
        private void OnEnable()
        {
            _legacySlotTracker.Value.OnEquipLegacyEvent += OnEquipLegacy;
        }

        private void OnDisable()
        {
            _legacySlotTracker.Value.OnEquipLegacyEvent -= OnEquipLegacy;
        }

        private void Start()
        {
            _colliders = new Collider2D[200];
            LevelUp();
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
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _damageRadius);
        }
    }
}
