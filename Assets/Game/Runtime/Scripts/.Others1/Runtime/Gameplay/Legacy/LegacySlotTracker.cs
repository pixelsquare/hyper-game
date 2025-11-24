using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public delegate void OnEquipLegacy(ILegacy legacy);

    public class LegacySlotTracker : MonoBehaviour
    {
        public LegacySlot OccupiedSlots => _occupiedSlot;
        public ILegacy WeaponLegacy => _weaponLegacy;

        public ReadOnlyDictionary<LegacySlot, ILegacy> LegacyActives => new(_legacyActives); // todo [optimize]
        public ReadOnlyDictionary<string, ILegacy> LegacyPassives => new(_legacyPassives); // todo [optimize]

        public event OnEquipLegacy OnEquipLegacyEvent; // todo [clarify]: rename this

        private ILegacy _weaponLegacy;
        private LegacySlot _occupiedSlot;
        private Dictionary<LegacySlot, ILegacy> _legacyActives;
        private Dictionary<string, ILegacy> _legacyPassives;

        public void Track(ILegacy legacy)
        {
            if (legacy.LegacySlot == LegacySlot.LahatLahat)
            {
                _weaponLegacy = legacy;
            }
            else if (legacy.LegacySlot == LegacySlot.Passive)
            {
                _legacyPassives.Add(legacy.LegacyId, legacy);
            }
            else
            {
                _legacyActives.Add(legacy.LegacySlot, legacy);
                _occupiedSlot |= legacy.LegacySlot;
            }

            OnEquipLegacyEvent?.Invoke(legacy);
        }

        private void OnEquipLegacy(IMessage message)
        {
            if (message.Data is ILegacy legacy)
            {
                Track(legacy);
            }
        }

        private void Awake()
        {
            _legacyActives = new Dictionary<LegacySlot, ILegacy>();
            _legacyPassives = new Dictionary<string, ILegacy>();
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(GameEvents.Gameplay.OnEquipLegacy, OnEquipLegacy, true);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnEquipLegacy, OnEquipLegacy, true);
        }
    }

    [Flags]
    public enum LegacySlot
    {
        None = 0b0000_0000,
        Strike = 0b0000_0001,
        Move = 0b0000_0010,
        Deflect = 0b0000_0100,
        Cast = 0b0000_1000,
        Smite = 0b0001_0000,
        LahatLahat = 0b0010_0000,
        Passive = 0b0100_0000
    }
}
