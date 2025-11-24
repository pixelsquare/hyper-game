using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class IdalmunonMoveLegacy : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _attackModifier;
            public float _receivedDamage;
        }

        [SerializeField] private uint _maxLevel = 5;
        [SerializeField] private Stats[] _statsTable = new Stats[5];
        [SerializeField] private ParticleSystem _vfx;

        public override string LegacyId => "IdalmunonMove";
        public override uint MaxLevel => _maxLevel;

        private bool _isOn;
        private Stats _stats;
        private UnitPlayer _unitPlayer;
        private IMoveEvent _moveEvent;

        public override void LevelUp()
        {
            var previousAttModifier = _stats._attackModifier;
            var previousDMModifier = _stats._receivedDamage;

            ++_currentLevel;
            _stats = _statsTable[_currentLevel - 1];

            //re-apply augments if active
            if (_isOn)
            {
                var modStats = new PlayerStats
                {
                    _damageAmplify = -previousAttModifier + _stats._attackModifier,
                    _damageMitigation = -previousDMModifier + _stats._receivedDamage
                };

                _unitPlayer.ModifyStats(modStats);
            }
        }

        private void ActivateLegacy()
        {
            if (!_isOn)
            {
                var modStats = new PlayerStats
                {
                    _damageAmplify = _stats._attackModifier,
                    _damageMitigation = _stats._receivedDamage
                };

                _unitPlayer.ModifyStats(modStats);
                _isOn = true;
                _vfx.Play();
            }
        }

        private void DeactivateLegacy()
        {
            if (_isOn)
            {
                var modStats = new PlayerStats
                {
                    _damageAmplify = -_stats._attackModifier,
                    _damageMitigation = -_stats._receivedDamage
                };

                _unitPlayer.ModifyStats(modStats);
                _isOn = false;

                _vfx.Stop(true);
                _vfx.Clear(true);
            }
        }

        private void Awake()
        {
            _unitPlayer = transform.GetComponentInParent<UnitPlayer>();
            _moveEvent = transform.GetComponentInParent<IMoveEvent>();
            _currentLevel = 1;
        }

        private void OnEnable()
        {
            _moveEvent.OnMoveStart += DeactivateLegacy;
            _moveEvent.OnMoveStop += ActivateLegacy;
        }

        private void OnDisable()
        {
            _moveEvent.OnMoveStart -= DeactivateLegacy;
            _moveEvent.OnMoveStop -= ActivateLegacy;
        }

        private void Start()
        {
            _stats = _statsTable[_currentLevel - 1];

            //player starts at idle
            ActivateLegacy();
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
