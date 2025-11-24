using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class UnitPlayer : MonoBehaviour, IMaxHealth, IMoveSpeed, IDamageBroadcaster, IDeathBroadcaster,
        IHexDamageBroadcaster, IDamageEvent
    {
        [BoxGroup("Stats", centerLabel: true)][HideLabel]
        [SerializeField] private PlayerStats _stats;
        
        public uint MaxHealth => _stats._health;
        public float MoveSpeed => _stats._movespeed + _stats._extraMoveSpeed;
        public PlayerStats Stats => _stats;

        public delegate void OnModifyStats(PlayerStats stats);

        public event OnModifyStats OnModifyStatsEvent;
        public event OnUnitDamage OnUnitDamage;
        public event OnUnitDeath OnPlayerDeathEvent;
        public event OnHexDamage OnPlayerHexDamageEvent;

        [Inject] private IHeroLoadoutManager _heroLoadoutManager;
        [Inject] private ItemDatabase _itemDatabase;
        [Inject] private IAudioManager _audioManager;

        public void ModifyStats(PlayerStats stats)
        {
            _stats += stats;
            OnModifyStatsEvent?.Invoke(_stats);
        }

        public uint BroadcastDamage(uint damage, uint currentHealth, uint maxHealth, Vector3 origin)
        {
            _audioManager.PlaySound(Sfx.PlayerHit);
            OnUnitDamage?.Invoke(damage, currentHealth, maxHealth, origin);
            
            return _stats.CalcTotalReceivedDamage(damage);
        }

        public void BroadcastHexDamage(uint damage)
        {
            OnPlayerHexDamageEvent?.Invoke(damage);
        }

        public void BroadcastDeath()
        {
            OnPlayerDeathEvent?.Invoke();
        }

        private IEnumerator Start() // todo replace with proper sequence 
        {
            _stats = (PlayerStats) _heroLoadoutManager.ActiveHero.HeroStats;
            
            var equipmentBonus = new PlayerStats();
            equipmentBonus = _heroLoadoutManager.ActiveHero.ItemEquipped
                .Select(item => _itemDatabase.GetItem(item))
                .OfType<IEquipment>()
                .Aggregate(equipmentBonus, (current, equipment) => current + (PlayerStats) equipment.HeroStats);
            
            _stats += equipmentBonus;
            yield return null;
            OnModifyStatsEvent?.Invoke(_stats);
        }
    }
}
