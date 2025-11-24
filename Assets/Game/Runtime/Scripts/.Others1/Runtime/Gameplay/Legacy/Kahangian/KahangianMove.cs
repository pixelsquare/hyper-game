using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianMove : Legacy, IMomentumStop
    {
        [SerializeField] private GameObject _moveVfx;
        [SerializeField] private GameObject _fullSpeedMoveVfx;
        [SerializeField] private float _moveVfxTriggerPercentage;
        [SerializeField] private float _fullSpeedMoveVfxTriggerPercentage;
        [SerializeField] private Transform _vfxPivot;
        [SerializeField] private Stats[] _statsTable = new Stats[5];

        private Stats _stats;
        private UnitPlayer _unitPlayer;
        private InputMovement _inputMovement;
        private IAimDirection _aimDirection;

        public override string LegacyId => "KahangianMove";
        public override uint MaxLevel => (uint)_statsTable.Length;
        
        public event OnMomentumStop OnMomentumStop;

        private bool _isOn;
        private float _countdownToIncrementSpeed;
        private float _currentAugmentedValue;
        private IAudioManager _audioManager;
        private LazyInject<LegacySlotTracker> _legacySlotTracker;
        private bool _isPlayingSfx;
        private bool _atFullSpeed;

        public override void LevelUp()
        {
            ++_currentLevel;
            _stats = _statsTable[_currentLevel - 1];
        }
        
        [Inject]
        private void Construct(IAudioManager audioManager, LazyInject<LegacySlotTracker> legacySlotTracker)
        {
            _audioManager = audioManager;
            _legacySlotTracker = legacySlotTracker;
        }

        private void ActivateLegacy()
        {
            if (!_isOn)
            {
                _isOn = true;
                _countdownToIncrementSpeed = _stats._moveSpeedModifierIncrementInterval;
            }
        }

        private void DeactivateLegacy()
        {
            if (!_isOn)
            {
                return;
            }

            var modStats = new PlayerStats
            {
                _extraMoveSpeed = -_currentAugmentedValue
            };

            _currentAugmentedValue = 0;
            _unitPlayer.ModifyStats(modStats);

            _isOn = false;
            
            OnMomentumStop?.Invoke(_atFullSpeed);

            //activate legacy if player is still moving when hit
            if (_inputMovement.IsMoving)
            {
                ActivateLegacy();
            }
            
            _moveVfx.SetActive(false);
            _fullSpeedMoveVfx.SetActive(false);
            _audioManager.StopSound(Sfx.KahangianMove);
            _isPlayingSfx = false;
            _atFullSpeed = false;
        }

        private void OnPlayerDamage(uint damage, uint currenthealth, uint maxhealth, Vector3 origin)
        {
            DeactivateLegacy();
        }
        
        private void OnEquipLegacy(ILegacy legacy)
        {
            if (legacy is IMomentumStopEffect stopEffect)
            {
                OnMomentumStop += stopEffect.OnMomentumStopEffect;
            }
        }

        private void Update()
        {
            if (!_isOn)
            {
                return;
            }

            if (_countdownToIncrementSpeed > 0)
            {
                _countdownToIncrementSpeed -= Time.deltaTime;
            }
            else
            {
                var previousAugmentedValue = _currentAugmentedValue;
                _currentAugmentedValue += _stats._moveSpeedModifierIncrement;

                if (_currentAugmentedValue > _stats._maxMoveSpeedModifier)
                {
                    _currentAugmentedValue = _stats._maxMoveSpeedModifier;
                }
                else if (_currentAugmentedValue - previousAugmentedValue > 0)
                {
                    var modStats = new PlayerStats
                    {
                        _extraMoveSpeed = _currentAugmentedValue - previousAugmentedValue
                    };

                    _unitPlayer.ModifyStats(modStats);
                }

                if (_currentAugmentedValue >= _stats._maxMoveSpeedModifier * _fullSpeedMoveVfxTriggerPercentage)
                {
                    _atFullSpeed = true;
                    _fullSpeedMoveVfx.SetActive(true);
                    _moveVfx.SetActive(false);
                }
                else if (_currentAugmentedValue >= _stats._maxMoveSpeedModifier * _moveVfxTriggerPercentage)
                {
                    _moveVfx.SetActive(true);
                    
                    if (!_isPlayingSfx)
                    {
                        _audioManager.PlaySound(Sfx.KahangianMove);
                        _isPlayingSfx = true;
                    }
                }

                _countdownToIncrementSpeed = _stats._moveSpeedModifierIncrementInterval;
            }
        }

        private void LateUpdate()
        {
            if (!_isOn)
            {
                return;
            }
            
            var angleClamp = GameplayAnimationUtility.DirectionToIndex(_aimDirection.AimDirection);
            _vfxPivot.transform.eulerAngles = new Vector3(0, 0, (angleClamp * 45f) + 270f);
        }

        private void Awake()
        {
            _unitPlayer = transform.GetComponentInParent<UnitPlayer>();
            _inputMovement = transform.GetComponentInParent<InputMovement>();
            _aimDirection = GetComponentInParent<IAimDirection>();
        }

        private void Start()
        {
            LevelUp();
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void OnEnable()
        {
            _inputMovement.OnMoveStart += ActivateLegacy;
            _inputMovement.OnMoveStop += DeactivateLegacy;
            _unitPlayer.OnUnitDamage += OnPlayerDamage;
            
            _legacySlotTracker.Value.OnEquipLegacyEvent += OnEquipLegacy;
        }

        private void OnDisable()
        {
            _inputMovement.OnMoveStart -= ActivateLegacy;
            _inputMovement.OnMoveStop -= DeactivateLegacy;
            _unitPlayer.OnUnitDamage -= OnPlayerDamage;
            
            _legacySlotTracker.Value.OnEquipLegacyEvent -= OnEquipLegacy;
        }

        private void OnValidate()
        {
            if (_statsTable.Length != MaxLevel)
            {
                _statsTable = new Stats[MaxLevel];
            }
        }

        [Serializable]
        private struct Stats
        {
            public float _moveSpeedModifierIncrement;
            public float _moveSpeedModifierIncrementInterval;
            public float _maxMoveSpeedModifier;
        }
    }
}
