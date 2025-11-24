using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianDeflect : Legacy, IMonoSpawnPool<SelfDespawn>, IModifyCooldown
    {
        [Serializable]
        private struct Stats
        {
            public float _cooldown;
            public float _moveSpeedAugment;
            public float _augmentationDuration;
        }

        [SerializeField] private SelfDespawn _areaEffectIndicator;
        [SerializeField] private float _hexApplicationRadius;
        [SerializeField] private float _hexDuration;
        [SerializeField] private HexIndicator _hexIndicator;
        [SerializeField] private float _extraKnockback;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "KahangianDeflect";
        public override uint MaxLevel => (uint)_statsTable.Length;
        public SelfDespawn Prefab { get; private set; }
        public IEnumerable<SelfDespawn> Pool => _selfDespawnerPool;

        private Stats _stats;
        private UnitPlayer _unitPlayer;

        private bool _isOn;
        private bool _isLegacyReady;
        private float _moveTimer;
        private float _cooldownTimer;
        private Collider2D[] _colliders;
        private HexIndicatorSpawnPoolTracker _hexIndicatorSpawnPoolTracker;
        private List<SelfDespawn> _selfDespawnerPool;

        private DiContainer _diContainer;
        private LazyInject<HexSystem> _hexSystem;
        private IAudioManager _audioManager;

        public override void LevelUp()
        {
            if (_currentLevel == MaxLevel)
            {
                return;
            }
            
            var previousSpeedModifier = _stats._moveSpeedAugment;

            ++_currentLevel;
            _stats = _statsTable[_currentLevel - 1];

            //re-apply augments if active
            if (_isOn)
            {
                var modStats = new PlayerStats
                {
                    _extraMoveSpeed = -previousSpeedModifier + _stats._moveSpeedAugment
                };

                _unitPlayer.ModifyStats(modStats);
            }
        }

        [Inject]
        private void Construct(DiContainer diContainer, IAudioManager audioManager, LazyInject<HexSystem> hexSystem)
        {
            _diContainer = diContainer;
            _hexSystem = hexSystem;
            _hexIndicatorSpawnPoolTracker = hexSystem.Value.GetComponent<HexIndicatorSpawnPoolTracker>();
            _audioManager = audioManager;
        }

        private void ActivateLegacy()
        {
            if (!_isOn)
            {
                var modStats = new PlayerStats
                {
                    _extraMoveSpeed = _stats._moveSpeedAugment
                };

                _unitPlayer.ModifyStats(modStats);
                _moveTimer = 0;
                _cooldownTimer = 0;

                _isOn = true;

                //placeholder to show affected enemy area
                if (_areaEffectIndicator != null)
                {
                    var position = transform.position;
                    var indicator = Spawn();
                    
                    var indicatorTransform = indicator.transform;
                    indicatorTransform.position = position;
                    indicatorTransform.rotation = _areaEffectIndicator.transform.rotation;
                    indicatorTransform.parent = null;
                    
                    indicator.SetReturnParent(transform);
                    indicator.gameObject.SetActive(true);
                }

                var hits = Physics2D.OverlapCircleNonAlloc(transform.position, _hexApplicationRadius, _colliders, _layerMask);

                for (var i = 0; i < hits; ++i)
                {
                    var col = _colliders[i];

                    if (!col.TryGetComponent<Health>(out var contactHealth))
                    {
                        continue;
                    }

                    var hex = _diContainer.Instantiate<KahangianHex>(new object[]
                            { col.transform, _hexDuration, _extraKnockback, _hexIndicator, _hexIndicatorSpawnPoolTracker });
                    _hexSystem.Value.OnHexApplyEvent(hex);
                }
                
                _audioManager.PlaySound(Sfx.KahangianDeflect);
            }
        }

        private void DeactivateLegacy()
        {
            if (_isOn)
            {
                var modStats = new PlayerStats
                {
                    _extraMoveSpeed = -_stats._moveSpeedAugment
                };

                _unitPlayer.ModifyStats(modStats);
                _isOn = false;
            }
        }

        private void OnDamageEvent(uint damage, uint currenthealth, uint maxhealth, Vector3 origin)
        {
            if (_isLegacyReady)
            {
                ActivateLegacy();
                _isLegacyReady = false;
            }
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
        
        public void OnCooldownModified(float cooldownReduction)
        {
            for (int i = 0; i<_statsTable.Length; i++)
            {
                var oldStat = _statsTable[i];

                _statsTable[i] = new Stats
                {
                    _augmentationDuration = oldStat._augmentationDuration,
                    _cooldown = oldStat._cooldown - oldStat._cooldown * cooldownReduction,
                    _moveSpeedAugment = oldStat._moveSpeedAugment,
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
            if (_isOn)
            {
                _moveTimer += Time.deltaTime;

                if (_moveTimer >= _stats._augmentationDuration)
                {
                    DeactivateLegacy();
                }
            }

            if (!_isLegacyReady)
            {
                _cooldownTimer += Time.deltaTime;

                if (_cooldownTimer >= _stats._cooldown)
                {
                    _isLegacyReady = true;
                }
            }
        }

        private void Awake()
        {
            _unitPlayer = transform.GetComponentInParent<UnitPlayer>();
            _selfDespawnerPool = new List<SelfDespawn>();
            Prefab = _areaEffectIndicator;
        }
        
        private void OnEnable()
        {
            _unitPlayer.OnUnitDamage += OnDamageEvent;
            Dispatcher.AddListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown);
        }

        private void OnDisable()
        {
            _unitPlayer.OnUnitDamage -= OnDamageEvent;
            Dispatcher.RemoveListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown, true);
        }

        private void Start()
        {
            LevelUp();
            _isLegacyReady = true;
            _colliders = new Collider2D[GameConstants.GameValues.HitCollisions];
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
            Gizmos.DrawWireSphere(transform.position, _hexApplicationRadius);
        }
    }
}
