using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class IdalmunonPassive1 : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _additionalDamage;
        }

        [SerializeField] private uint _maxLevel = 5;
        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "IdalmunonPassive1";
        public override uint MaxLevel => _maxLevel;

        private Stats _stats;
        private IdalmunonStrike _idalmunonStrike;

        public override void LevelUp()
        {
            if (_currentLevel == _maxLevel)
            {
                return;
            }

            _stats = _statsTable[_currentLevel];
            ++_currentLevel;

            _idalmunonStrike.UpdateEnemyDamageMitigation(_stats._additionalDamage);
        }

        private void Awake()
        {
            _idalmunonStrike = transform.parent.GetComponentInChildren<IdalmunonStrike>();
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
