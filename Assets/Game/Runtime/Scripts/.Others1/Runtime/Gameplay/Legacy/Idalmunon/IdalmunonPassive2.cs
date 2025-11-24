using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class IdalmunonPassive2 : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _cutoffHealthPct;
            public float _additionalDamage;
        }

        [SerializeField] private uint _maxLevel = 5;
        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "IdalmunonPassive2";
        public override uint MaxLevel => _maxLevel;

        private Stats _stats;
        private bool _isAugmented;
        private UnitPlayer _unitPlayer;

        public override void LevelUp()
        {
            if (_currentLevel == _maxLevel)
            {
                return;
            }

            _stats = _statsTable[_currentLevel];
            ++_currentLevel;
        }

        private void HealthCheck(uint damage, uint currentHealth, uint maxHealth, Vector3 origin)
        {
            var healthPct = currentHealth * 100f / (maxHealth * 100f);

            if (healthPct <= _stats._cutoffHealthPct && !_isAugmented)
            {
                var modStats = new PlayerStats
                {
                    _damageAmplify = _stats._additionalDamage
                };

                _unitPlayer.ModifyStats(modStats);
                _isAugmented = true;
            }

            if (healthPct > _stats._cutoffHealthPct && _isAugmented)
            {
                var modStats = new PlayerStats
                {
                    _damageAmplify = -_stats._additionalDamage
                };

                _unitPlayer.ModifyStats(modStats);
                _isAugmented = false;
            }
        }

        private void Awake()
        {
            _unitPlayer = transform.GetComponentInParent<UnitPlayer>();
        }

        private void OnEnable()
        {
            _unitPlayer.OnUnitDamage += HealthCheck;
        }

        private void OnDisable()
        {
            _unitPlayer.OnUnitDamage -= HealthCheck;
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
