using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class IdalmunonPassive5 : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _hexRadiusModifier;
        }

        [SerializeField] private uint _maxLevel = 5;
        [SerializeField] private Stats[] _statsTable = new Stats[5];

        private Stats _stats;
        private IdalmunonStrike _idalmunonStrike;

        public override string LegacyId => "IdalmunonPassive5";
        public override uint MaxLevel => _maxLevel;

        public override void LevelUp()
        {
            _stats = _statsTable[_currentLevel];
            ++_currentLevel;

            _idalmunonStrike.UpdateRadiusAugmentation(_stats._hexRadiusModifier);
        }

        private void Start()
        {
            LevelUp();
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void Awake()
        {
            _idalmunonStrike = transform.parent.GetComponentInChildren<IdalmunonStrike>();
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
