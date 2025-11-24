using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianPassive4 : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _additionalDamagePct;
        }

        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "KahangianPassive4";
        
        public override uint MaxLevel => (uint)_statsTable.Length;

        private Stats _stats;

        private LazyInject<LegacySlotTracker> _legacySlotTracker;

        public override void LevelUp()
        {
            if (_currentLevel == MaxLevel)
            {
                return;
            }

            _stats = _statsTable[_currentLevel++];

            OnModifyThunderDamage(_stats._additionalDamagePct);
        }

        public void OnModifyThunderDamage(float additionalDamage)
        {
            Dispatcher.SendMessageData(GameEvents.Gameplay.ModifyThunderLegacyDamage, additionalDamage);
        }

        [Inject]
        private void Construct(LazyInject<LegacySlotTracker> legacySlotTracker)
        {
            _legacySlotTracker = legacySlotTracker;
        }

        private void OnEquipLegacy(ILegacy legacy)
        {
            if (legacy is IModifyThunderLegacies modifyThunderLegacy)
            {
                modifyThunderLegacy.OnIncreaseDamageToHexed(_stats._additionalDamagePct);
            }
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
