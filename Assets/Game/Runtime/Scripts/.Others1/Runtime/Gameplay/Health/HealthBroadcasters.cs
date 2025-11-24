using UnityEngine;

namespace Santelmo.Rinsurv
{
    public delegate void OnUnitDamage(uint damage, uint currentHealth, uint maxHealth, Vector3 origin);
    public delegate void OnUnitDeath();
    public delegate void OnUnitDeathGlobal(Transform transform);
    public delegate void OnHexDamage(uint damage);

    public interface IDamageBroadcaster
    {
        public uint BroadcastDamage(uint damage, uint currentHealth, uint maxHealth, Vector3 origin);
    }

    public interface IHexDamageBroadcaster
    {
        public void BroadcastHexDamage(uint damage);
    }

    public interface IDeathBroadcaster
    {
        public void BroadcastDeath();
    }

    public interface IDeathEvent
    {
        public event OnUnitDeath OnUnitDeath;
    }

    public interface IDamageEvent
    {
        public event OnUnitDamage OnUnitDamage;
    }
}
