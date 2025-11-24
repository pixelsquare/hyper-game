using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class Hero1LahatLahat3 : Legacy, IWeaponHit, ILegacyLayerApply, IMonoSpawnPool<MultiframeProjectile>, IModifyCooldown
    {
        [Serializable]
        private struct Stats
        {
            public uint _damage;
            public uint _amount;
            public uint _pierce;
            public float _interval;
            public float _hitInterval;
            public float _projectileSpeed;
            public float _projectileDistance;
        }

        [SerializeField] private SerializableDictionary<LegacyLayer, MultiframeProjectile> _prefabTable;
        [SerializeField] private Stats[] _statsTable;

        public override string LegacyId => "Hero1LahatLahat3";
        public override uint MaxLevel => (uint)_statsTable.Length;
        public MultiframeProjectile Prefab { get; private set; }
        public IEnumerable<MultiframeProjectile> Pool => _projectilePool;

        public event OnWeaponHit OnWeaponHit;

        private float _interval;
        private Stats _stats;

        private IAimDirection _aimDirection;

        private DiContainer _diContainer;
        private IAudioManager _audioManager;
        private LazyInject<LegacySlotTracker> _legacySlotTracker;
        private List<MultiframeProjectile> _projectilePool;

        public override void LevelUp()
        {
            if (_currentLevel < MaxLevel)
            {
                _stats = _statsTable[_currentLevel++];
            }
        }

        public void ApplyLegacyLayer(LegacyLayer legacyLayer)
        {
            Prefab = _prefabTable[legacyLayer];
            _projectilePool.Clear();
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
            if (legacy.LegacySlot == LegacySlot.LahatLahat
                && legacy.LegacyId != LegacyId)
            {
                gameObject.SetActive(false);
            }

            if (legacy is IWeaponHitEffect hitEffect)
            {
                OnWeaponHit += hitEffect.OnHitEffect;
            }

            if (legacy is ILegacyLayerEffect layerEffect)
            {
                ApplyLegacyLayer(layerEffect.LegacyEffectLayer);
            }
        }

        public MultiframeProjectile Spawn()
        {
            var instance = Pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);

            if (!instance)
            {
                instance = _diContainer.InstantiatePrefabForComponent<MultiframeProjectile>(Prefab);
                instance.OnProjectileExpire += _ => instance.OnDespawn();
                _projectilePool.Add(instance);
            }

            return instance;
        }

        public void Despawn(MultiframeProjectile spawn)
        {

        }

        public void OnCooldownModified(float cooldownReduction)
        {
            for (int i = 0; i<_statsTable.Length; i++)
            {
                var oldStat = _statsTable[i];

                _statsTable[i] = new Stats
                {
                    _damage = oldStat._damage,
                    _amount = oldStat._amount,
                    _pierce = oldStat._pierce,
                    _interval = oldStat._interval - oldStat._interval * cooldownReduction,
                    _hitInterval = oldStat._hitInterval,
                    _projectileSpeed = oldStat._projectileSpeed,
                    _projectileDistance = oldStat._projectileDistance,
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
                projectile.Speed = _stats._projectileSpeed;
                projectile.Direction = _aimDirection.AimDirection;
                projectile.Damage = _stats._damage;
                projectile.Pierce = _stats._pierce;
                projectile.Distance = _stats._projectileDistance;
                projectile.OnWeaponHit += OnWeaponHit;
                projectile.OnSpawn();
                
                _interval = _stats._interval;
                _audioManager.PlaySound(Sfx.Hero1LahatLahat3);
            }
        }

        private void Awake()
        {
            _aimDirection = transform.parent.GetComponent<IAimDirection>();
            _projectilePool = new List<MultiframeProjectile>();
            Prefab = _prefabTable[LegacyLayer.None];
        }

        private void OnEnable()
        {
            _legacySlotTracker.Value.OnEquipLegacyEvent += OnEquipLegacy;
            Dispatcher.AddListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown);
        }

        private void OnDisable()
        {
            _legacySlotTracker.Value.OnEquipLegacyEvent -= OnEquipLegacy;
            Dispatcher.RemoveListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown, true);
        }

        private void Start()
        {
            LevelUp();
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }
    }
}
