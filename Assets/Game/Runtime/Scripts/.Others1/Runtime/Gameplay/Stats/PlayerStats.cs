using System;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public struct PlayerStats
    {
        public uint _health;
        public float _attack;
        public float _armor;
        public float _movespeed;
        public float _evasion;
        public float _critRate;
        public float _critDamage;
        public float _lootDistance;
        public float _cooldown;
        public float _damageAmplify;
        public float _incomingHeal;
        public float _damageMitigation;
        public float _extraMoveSpeed;
        public float _extraKnockback;
        public float _movementSpeedBonusDamage;

        public uint CalcTotalDamage(float modifier)
        {
            var value =_attack * (modifier + _damageAmplify);

            if (_movementSpeedBonusDamage > 0)
            {
                value += value * _extraMoveSpeed * _movementSpeedBonusDamage;
            }
            
            return (uint)value;
        }
        
        public uint CalcTotalReceivedDamage(uint incomingDamage)
        {
            var value = incomingDamage + (incomingDamage * _damageMitigation);
            return (uint)value;
        }

        public static PlayerStats operator +(PlayerStats a, PlayerStats b)
        {
            return new PlayerStats
            {
                _health = a._health + b._health,
                _attack = a._attack + b._attack,
                _armor = a._armor + b._armor,
                _movespeed = a._movespeed + b._movespeed,
                _evasion = a._evasion + b._evasion,
                _critRate = a._critRate + b._critRate,
                _critDamage = a._critDamage + b._critDamage,
                _lootDistance = a._lootDistance + b._lootDistance,
                _cooldown = a._cooldown + b._cooldown,
                _damageAmplify = a._damageAmplify + b._damageAmplify,
                _incomingHeal = a._incomingHeal + b._incomingHeal,
                _damageMitigation = a._damageMitigation + b._damageMitigation,
                _extraMoveSpeed = a._extraMoveSpeed + b._extraMoveSpeed,
                _extraKnockback = a._extraKnockback + b._extraKnockback,
                _movementSpeedBonusDamage = a._movementSpeedBonusDamage + b._movementSpeedBonusDamage,
            };
        }

        public static explicit operator PlayerStats(HeroStats stats)
        {
            return new PlayerStats
            {
                _health = (uint) stats.Health,
                _attack = (uint)stats.Attack,
                _armor = (uint)stats.Armor,
                _movespeed = stats.MoveMultiplier,
                _evasion = stats.EvasionChance,
                _critRate = stats.CriticalChance,
                _critDamage = stats.CriticalDamage,
                _lootDistance = stats.LootDistance,
                _cooldown = stats.CooldownReduction
            };
        }
    }
}
