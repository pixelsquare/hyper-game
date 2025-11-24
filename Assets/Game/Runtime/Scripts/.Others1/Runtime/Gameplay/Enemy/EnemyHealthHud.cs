using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class EnemyHealthHud : MonoBehaviour
    {
        [SerializeField] private RinawaImage _healthBar;
        [SerializeField] private RinawaText _healthText;
        
        private uint _maxHealth;
        private uint _currentHealth;

        public void AddValue(uint value)
        {
            _maxHealth += value;
            _currentHealth += value;
        }

        public void UpdateBar(uint damage, uint currentHealth, uint maxHealth, Vector3 origin)
        {
            if (damage < _currentHealth)
            {
                _currentHealth -= damage;
            }
            else
            {
                _currentHealth = 0;
            }

            _healthBar.fillAmount = (float) _currentHealth / _maxHealth;
            _healthText.text = $"Boss Health: {_currentHealth}/{_maxHealth}";
        }

        private void OnBossSpawn(IMessage message)
        {
            if (message.Data is UnitSpawn bossSpawn
                && bossSpawn.TryGetComponent<IMaxHealth>(out var maxHealth)
                && bossSpawn.TryGetComponent<IDamageEvent>(out var damageEvent))
            {
                AddValue(maxHealth.MaxHealth);
                damageEvent.OnUnitDamage += UpdateBar;
            }
        }

        private void OnFinaleWaveStart(IMessage message)
        {
            _healthBar.gameObject.SetActive(true);
        }

        private void OnGameEnded(IMessage message)
        {
            _healthBar.gameObject.SetActive(false);            
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(GameEvents.Gameplay.OnFinaleWaveStart, OnFinaleWaveStart);
            Dispatcher.AddListener(GameEvents.AppState.OnGameEndedEvent, OnGameEnded);
            Dispatcher.AddListener(GameEvents.Gameplay.OnBossSpawn, OnBossSpawn);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnFinaleWaveStart, OnFinaleWaveStart, true);
            Dispatcher.RemoveListener(GameEvents.AppState.OnGameEndedEvent, OnGameEnded, true);
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnBossSpawn, OnBossSpawn, true);
        }
    }
}
