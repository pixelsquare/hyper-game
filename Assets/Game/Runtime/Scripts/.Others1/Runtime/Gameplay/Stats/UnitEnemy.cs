using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class UnitEnemy : MonoBehaviour, IMoveSpeed, IMaxHealth, IDamageBroadcaster, IDeathBroadcaster, IDeathEvent, IDamageEvent, IKnockback, IHexDamageBroadcaster
    {
        [BoxGroup("Information", centerLabel: true)][HideLabel]
        [SerializeField] private UnitInfo _info;
        
        [BoxGroup("Stats", centerLabel: true)][HideLabel]
        [SerializeField] private EnemyStats _stats;

        public UnitInfo Info => _info;
        public EnemyStats Stats => _stats;

        public float MoveSpeed => _stats._movespeed;
        public uint MaxHealth => _stats._health;

        public float KnockbackDistance => _stats._knockbackDistance * (1 + _stats._extraKnockback + _stats._extraKnockback);
        public float KnockbackDuration => _stats._knockbackDuration;
        
        public static event OnUnitDeathGlobal _onUnitDeathGlobal;
        public event OnUnitDamage OnUnitDamage;
        public event OnUnitDeath OnUnitDeath;
        public event OnHexDamage _onEnemyHexDamage;

        public uint BroadcastDamage(uint damage, uint currentHealth, uint maxHealth, Vector3 origin)
        {
            OnUnitDamage?.Invoke(damage,currentHealth, maxHealth, origin);
            return _stats.CalcTotalReceivedDamage(damage);
        }
        
        public void BroadcastHexDamage(uint damage)
        {
            _onEnemyHexDamage?.Invoke(damage);
        }

        public void BroadcastDeath()
        {
            OnUnitDeath?.Invoke();
            _onUnitDeathGlobal?.Invoke(transform);
        }
        
        public void ModifyStats(EnemyStats stats)
        {
            _stats += stats;
        }
    }
}
