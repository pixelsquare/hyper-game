using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class IdalmunonPassive3 : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _additionalDamage;
        }

        [SerializeField] private uint _maxLevel = 5;
        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "IdalmunonPassive3";
        public override uint MaxLevel => _maxLevel;

        private Stats _stats;
        private bool _isAugmented;
        private UnitPlayer _unitPlayer;

        public override void LevelUp()
        {
            _stats = _statsTable[_currentLevel];
            ++_currentLevel;

            var modifiedStats = new PlayerStats
            {
                _damageAmplify = _stats._additionalDamage
            };

            _unitPlayer.ModifyStats(modifiedStats);
        }

        private void Awake()
        {
            _unitPlayer = transform.GetComponentInParent<UnitPlayer>();
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
