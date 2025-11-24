using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianPassive3 : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _sizeIncrease;
        }

        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "KahangianPassive3";

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

            Dispatcher.SendMessageData(GameEvents.Gameplay.ModifyBaklawEnlargement, _stats._sizeIncrease);
        }

        [Inject]
        private void Construct(LazyInject<LegacySlotTracker> legacySlotTracker)
        {
            _legacySlotTracker = legacySlotTracker;
        }
        
        private void OnEquipLegacy(ILegacy legacy)
        {
            if (legacy is KahangianCast kahangianCast)
            {
                kahangianCast.OnModifyEnlargement(_stats._sizeIncrease);
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
            Dispatcher.SendMessageData(GameEvents.Gameplay.ModifyBaklawEnlargement, _stats._sizeIncrease);
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
