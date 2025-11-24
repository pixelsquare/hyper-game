using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Spawning/Boss Wave")]
    public class EnemyWaveBoss : EnemyWaveConfig, IWaveFinale, IWaveInitial
    {
        [SerializeField] private EnemyGroup _bossGroup;
        [SerializeField] private bool _clearEnemies;
        
        public SpawnData[] GetInitialWave(Vector2 origin)
        {
            return _bossGroup.ToSpawnData(origin, SpawnType.Boss);
        }

        public void OnWaveFinale()
        {
            Dispatcher.SendMessage(GameEvents.Gameplay.OnFinaleWaveStart);
        }
    }
}
