using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Spawning/Enemy Wave")]
    public class EnemyWaveRegular : EnemyWaveConfig, IWaveInterval
    {
        [SerializeField] private EnemyGroup[] _enemyGroups;
        [SerializeField] private float _interval;

        public float WaveInterval => _interval;
        
        public SpawnData[] GetIntervalWave(Vector2 origin)
        {
            var enemyGroup = _enemyGroups.Roll();
            return enemyGroup.ToSpawnData(origin, SpawnType.Mob);
        }
    }
}
