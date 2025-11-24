using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianPassive5 : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _cooldownReduction;
        }

        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "KahangianPassive5";
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

            OnModifyCooldown();
        }

        private void OnModifyCooldown()
        {
            Dispatcher.SendMessageData(GameEvents.Gameplay.ModifyCooldownReduction, _stats._cooldownReduction);
        }

        [Inject]
        private void Construct(LazyInject<LegacySlotTracker> legacySlotTracker)
        {
            _legacySlotTracker = legacySlotTracker;
        }

        private void OnEquipLegacy(ILegacy legacy)
        {
            if (legacy is IModifyCooldown modifyCooldownLegacy)
            {
                modifyCooldownLegacy.OnCooldownModified(_stats._cooldownReduction);
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
