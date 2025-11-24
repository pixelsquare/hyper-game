using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianPassive2 : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _movementBonusDamage;
        }

        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "KahangianPassive2";

        public override uint MaxLevel => (uint)_statsTable.Length;

        private Stats _stats;
        private LazyInject<UnitPlayer> _unitPlayer;

        public override void LevelUp()
        {
            if (_currentLevel == MaxLevel)
            {
                return;
            }

            _stats = _statsTable[_currentLevel++];

            UpdateBonusMovementDamage();
        }

        private void UpdateBonusMovementDamage()
        {
            var modStats = new PlayerStats
            {
                _movementSpeedBonusDamage = _stats._movementBonusDamage,
            };
            
            _unitPlayer.Value.ModifyStats(modStats);
        }

        [Inject]
        private void Construct(LazyInject<UnitPlayer> unitPlayer)
        {
            _unitPlayer = unitPlayer;
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
