using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public struct HeroStats
    {
        [SerializeField] private int _health;
        [SerializeField] private int _attack;
        [SerializeField] private int _armor;
        [SerializeField] private float _moveMultiplier;

        [Range(0, 100)]
        [SerializeField] private int _evasionChance;

        [Range(0, 100)]
        [SerializeField] private int _criticalChance;
        [SerializeField] private int _criticalDamage;

        [SerializeField] private int _lootDistance;

        [Range(0, 100)]
        [SerializeField] private int _cooldownReduction;

        public int Health => _health;
        public int Attack => _attack;
        public int Level { get; set; }
        public int Armor => _armor;
        public float MoveMultiplier => _moveMultiplier;
        public float EvasionChance => _evasionChance / 100f;
        public float CriticalChance => _criticalChance / 100f;
        public float CriticalDamage => _criticalDamage;
        public int LootDistance => _lootDistance;
        public float CooldownReduction => _cooldownReduction;
    }
}
