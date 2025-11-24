using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class IdalmunonStrike : Legacy, IWeaponHitEffect, ILegacyLayerEffect, IMonoSpawnPool<SelfDespawn>
    {
        [Serializable]
        private struct Stats
        {
            public float _additionalDamage;
        }

        [SerializeField] private Stats[] _statsTable = new Stats[5];
        [SerializeField] private HexIndicator _indicatorPrefab;
        [SerializeField] private SelfDespawn _radiusDisplayPrefab;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _hexDuration;
        [SerializeField] private float _hexRadius;
        [SerializeField] private uint _hexDamage;

        public override string LegacyId => "IdalmunonStrike";

        public LegacyLayer LegacyEffectLayer => LegacyLayer.Idalmunon;
        public override uint MaxLevel => (uint)_statsTable.Length;
        public SelfDespawn Prefab { get; private set; }
        public IEnumerable<SelfDespawn> Pool => _selfDespawnerPool;

        private Stats _stats;
        private bool _isAugmented;
        private UnitPlayer _unitPlayer;
        private List<IWeaponHit> _registeredWeapons;
        private float _enemyDamageMitigation;
        private float _newRadiusAugmentation;
        private HexIndicatorSpawnPoolTracker _hexIndicatorSpawnPoolTracker;
        private List<SelfDespawn> _selfDespawnerPool;

        private DiContainer _diContainer;
        private IAudioManager _audioManager;
        private LazyInject<HexSystem> _hexSystem;
        private LazyInject<LegacySlotTracker> _legacySlotTracker;

        public override void LevelUp()
        {
            if (_currentLevel == MaxLevel)
            {
                return;
            }

            _unitPlayer.ModifyStats(new PlayerStats
            {
                _damageAmplify = -_stats._additionalDamage
            });

            _stats = _statsTable[_currentLevel++];

            _unitPlayer.ModifyStats(new PlayerStats
            {
                _damageAmplify = _stats._additionalDamage
            });
        }

        public void OnHitEffect(Transform hitTransform)
        {
            var hex = _diContainer.Instantiate<IdalmunonHex>(new object[]
            {
                _hexDuration, _hexRadius + _newRadiusAugmentation, _hexDamage, _layerMask,
                hitTransform, _indicatorPrefab, _hexIndicatorSpawnPoolTracker, this, transform, 
                _enemyDamageMitigation, _radiusDisplayPrefab
            });
            _hexSystem.Value.OnHexApplyEvent(hex);
            // _audioManager.PlaySound(Sfx.IdalmunonStrike);
        }

        public void UpdateEnemyDamageMitigation(float newDamageMitigation)
        {
            _enemyDamageMitigation = newDamageMitigation;
        }

        public void UpdateRadiusAugmentation(float newRadiusAugmentation)
        {
            _newRadiusAugmentation = newRadiusAugmentation;
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

        [Inject]
        private void Construct(DiContainer diContainer, IAudioManager audioManager,
                               LazyInject<HexSystem> hexSystem, LazyInject<LegacySlotTracker> legacySlotTracker)
        {
            _diContainer = diContainer;
            _audioManager = audioManager;
            _hexSystem = hexSystem;
            _legacySlotTracker = legacySlotTracker;
            _hexIndicatorSpawnPoolTracker = hexSystem.Value.GetComponent<HexIndicatorSpawnPoolTracker>();
        }

        private void OnEquipLegacy(ILegacy legacy)
        {
            if (legacy is IWeaponHit weaponLegacy)
            {
                weaponLegacy.OnWeaponHit += OnHitEffect;
            }

            if (legacy is ILegacyLayerApply legacyLayerApply)
            {
                legacyLayerApply.ApplyLegacyLayer(LegacyEffectLayer);
            }
        }

        private void Awake()
        {
            _unitPlayer = transform.GetComponentInParent<UnitPlayer>();
            _selfDespawnerPool = new List<SelfDespawn>();
            Prefab = _radiusDisplayPrefab;
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
    }
}
