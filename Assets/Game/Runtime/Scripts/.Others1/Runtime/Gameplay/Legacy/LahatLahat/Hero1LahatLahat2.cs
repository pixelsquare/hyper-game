using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class Hero1LahatLahat2 : Legacy, IWeaponHit, ILegacyLayerApply, IModifyCooldown
    {
        [Serializable]
        private struct Stats
        {
            public float _damageModifier;
            public int _amount;
            public uint _pierce;
            public float _interval;
            public float _projectileSpeed;
            public float _projectileDistance;
            public float _spread;
        }

        [SerializeField] private Stats[] _statsTable;
        [SerializeField] private SerializableDictionary<LegacyLayer, TipProjectile> _prefabTable;

        public override string LegacyId => "Hero1LahatLahat2";
        public override uint MaxLevel => (uint)_statsTable.Length;

        public event OnWeaponHit OnWeaponHit;

        private Stats _stats;
        private IAimDirection _aimDirection;
        private float _interval;
        private float _amount;
        private TipProjectile _projectilePrefab;
        private uint _totalDamage;

        private DiContainer _diContainer;
        private LazyInject<LegacySlotTracker> _legacySlotTracker;
        private IAudioManager _audioManager;

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
            _projectilePrefab = _prefabTable[legacyLayer];
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
            if (legacy is IWeaponHitEffect hitEffect)
            {
                OnWeaponHit += hitEffect.OnHitEffect;
            }

            if (legacy is ILegacyLayerEffect layerEffect)
            {
                ApplyLegacyLayer(layerEffect.LegacyEffectLayer);
            }
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
                    _spread = oldStat._spread,
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
            else
            {
                var coneDirections = ProjectileUtility.GetConeDirections(_aimDirection.AimDirection, _stats._amount, _stats._spread);
                foreach (var direction in coneDirections)
                {
                    var position = transform.position;
                    var projectile = _diContainer.InstantiatePrefabForComponent<TipProjectile>(_projectilePrefab, position, Quaternion.identity, null) 
                         ?? Instantiate(_projectilePrefab, position, Quaternion.identity);
                    projectile.Speed = _stats._projectileSpeed;
                    projectile.Direction = direction;
                    projectile.Damage = _totalDamage;
                    projectile.Pierce = _stats._pierce;
                    projectile.Distance = _stats._projectileDistance;
                    projectile.OnWeaponHit += OnWeaponHit;
                    projectile.OnSpawn();
                }

                _interval = _stats._interval;
                _audioManager.PlaySound(Sfx.Hero1LahatLahat2);
            }
        }

        private void Awake()
        {
            _aimDirection = transform.parent.GetComponent<IAimDirection>();
            _projectilePrefab = _prefabTable[LegacyLayer.None];
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

        private void OnDrawGizmos()
        {
            if (_statsTable.Length == 0)
            {
                return;
            }

            var position = transform.position;
            var up = transform.up;
            var stats = _statsTable[0];

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(position, up * stats._projectileDistance);

            var coneDirections = ProjectileUtility.GetConeDirections(up, stats._amount, stats._spread);
            foreach (var direction in coneDirections)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(position, direction * stats._projectileDistance);
                Gizmos.color = Color.green;
                Gizmos.DrawRay(position, direction);
            }
        }
    }
}
