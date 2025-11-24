using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianStrike : Legacy, ILegacyLayerEffect
    {
        [Serializable]
        private struct Stats
        {
            public float _additionalDamage;
            public float _extraKnockback;
        }

        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public LegacyLayer LegacyEffectLayer => LegacyLayer.Kahangian;

        public override string LegacyId => "KahangianStrike";
        public override uint MaxLevel => (uint)_statsTable.Length;

        private Stats _stats;
        private UnitPlayer _unitPlayer;

        private LazyInject<LegacySlotTracker> _legacySlotTracker;

        public override void LevelUp()
        {
            if (_currentLevel == MaxLevel)
            {
                return;
            }

            _unitPlayer.ModifyStats(new PlayerStats
            {
                _damageAmplify = -_stats._additionalDamage,
                _extraKnockback = -_stats._extraKnockback
            });

            _stats = _statsTable[_currentLevel++];

            _unitPlayer.ModifyStats(new PlayerStats
            {
                _damageAmplify = _stats._additionalDamage,
                _extraKnockback = _stats._extraKnockback
            });
        }

        [Inject]
        private void Construct(LazyInject<LegacySlotTracker> legacySlotTracker)
        {
            _legacySlotTracker = legacySlotTracker;
        }

        private void OnEquipLegacy(ILegacy legacy)
        {
            if (legacy is ILegacyLayerApply legacyLayerApply)
            {
                legacyLayerApply.ApplyLegacyLayer(LegacyEffectLayer);
            }
        }

        private void Awake()
        {
            _unitPlayer = transform.GetComponentInParent<UnitPlayer>();
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
