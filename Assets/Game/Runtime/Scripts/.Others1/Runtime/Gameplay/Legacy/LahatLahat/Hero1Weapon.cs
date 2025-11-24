using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class Hero1Weapon : Legacy, IWeaponHit, ILegacyLayerApply, IMonoSpawnPool<TipProjectile>, IModifyCooldown
    {
        [Serializable]
        private struct Stats
        {
            public float _damageModifier;
            public uint _amount;
            public uint _pierce;
            public float _interval;
            public float _projectileSpeed;
            public float _projectileDistance;
        }

        [SerializeField] private SerializableDictionary<LegacyLayer, TipProjectile> _prefabTable;
        [SerializeField] private Stats[] _statsTable;

        public override string LegacyId => "Hero1Weapon";

        public override uint MaxLevel => (uint)_statsTable.Length;
        public TipProjectile Prefab { get; private set; }
        public IEnumerable<TipProjectile> Pool => _projectilePool;

        public event OnWeaponHit OnWeaponHit;

        private uint _totalDamage;
        private float _interval;
        private uint _amount;
        private Stats _stats;

        private IAimDirection _aimDirection;
        private IAudioManager _audioManager;

        private DiContainer _diContainer;
        private LazyInject<LegacySlotTracker> _legacySlotTracker;
        private List<TipProjectile> _projectilePool;

        public override void LevelUp()
        {
            if (_currentLevel < MaxLevel)
            {
                _stats = _statsTable[_currentLevel++];
                var playerStats = GetComponentInParent<UnitPlayer>().Stats; // todo [optimize]
                _totalDamage = playerStats.CalcTotalDamage(_stats._damageModifier);
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

        public TipProjectile Spawn()
        {
            var instance = Pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);

            if (!instance)
            {
                instance = _diContainer.InstantiatePrefabForComponent<TipProjectile>(Prefab);
                instance.OnProjectileExpire += _ => instance.OnDespawn();
                _projectilePool.Add(instance);
            }

            return instance;
        }

        public void Despawn(TipProjectile spawn)
        {
        }

        public void OnCooldownModified(float cooldownReduction)
        {
            for (int i = 0; i<_statsTable.Length; i++)
            {
                var oldStat = _statsTable[i];

                _statsTable[i] = new Stats
                {
                    _damageModifier = oldStat._damageModifier,
                    _amount = oldStat._amount,
                    _pierce = oldStat._pierce,
                    _interval = oldStat._interval - oldStat._interval * cooldownReduction,
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

        private void OnModifyStats(PlayerStats stats)
        {
            _totalDamage = stats.CalcTotalDamage(_stats._damageModifier);
        }

        private void Update()
        {
            if (_interval > 0)
            {
                _interval -= Time.deltaTime;
            }
            else if (_amount > 0)
            {
                var position = transform.position;
                var projectile = Spawn();
                var projectileTransform = projectile.transform;
                projectileTransform.position = position;
                projectileTransform.rotation = Quaternion.identity;
                projectile.Speed = _stats._projectileSpeed;
                projectile.Direction = _aimDirection.AimDirection;
                projectile.Damage = _totalDamage;
                projectile.Pierce = _stats._pierce;
                projectile.Distance = _stats._projectileDistance;
                projectile.OnWeaponHit += OnWeaponHit;
                projectile.OnSpawn();
                --_amount;
                _audioManager.PlaySound(Sfx.Hero1Weapon);
            }
            else
            {
                _interval = _stats._interval;
                _amount = _stats._amount;
            }
        }

        private void Awake()
        {
            _aimDirection = transform.parent.GetComponent<IAimDirection>();
            _projectilePool = new List<TipProjectile>();
            Prefab = _prefabTable[Rinsurv.LegacyLayer.None];
        }

        private void OnEnable()
        {
            _legacySlotTracker.Value.OnEquipLegacyEvent += OnEquipLegacy;
            Dispatcher.AddListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown);
            GetComponentInParent<UnitPlayer>().OnModifyStatsEvent += OnModifyStats;
        }

        private void OnDisable()
        {
            _legacySlotTracker.Value.OnEquipLegacyEvent -= OnEquipLegacy;
            Dispatcher.RemoveListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown, true);

            if (transform.parent.TryGetComponent<UnitPlayer>(out var unitPlayer))
            {
                unitPlayer.OnModifyStatsEvent -= OnModifyStats; 
            }
        }

        private void Start()
        {
            LevelUp();
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }
    }
}
