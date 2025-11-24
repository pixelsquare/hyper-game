using System;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public struct EnemyStats
    {
        public uint _health;
        public float _attack;
        public float _movespeed;
        public float _knockbackDuration;
        public float _knockbackDistance;
        public uint _exp;
        public float _damageMitigation;
        public float _extraKnockback;
        
        public uint CalcTotalReceivedDamage(uint incomingDamage)
        {
            var value = incomingDamage + (incomingDamage * _damageMitigation);
            return (uint)value;
        }
        
        public static EnemyStats operator +(EnemyStats a, EnemyStats b)
        {
            return new EnemyStats
            {
                _health = a._health + b._health,
                _attack = a._attack + b._attack,
                _movespeed = a._movespeed + b._movespeed,
                _knockbackDuration = a._knockbackDuration + b._knockbackDuration,
                _knockbackDistance = a._knockbackDistance + b._knockbackDistance,
                _exp = a._exp + b._exp,
                _damageMitigation = a._damageMitigation + b._damageMitigation,
                _extraKnockback = a._extraKnockback + b._extraKnockback,
            };
        }
        
        // todo loot drop
        // todo guaranteed drop
    }
}
