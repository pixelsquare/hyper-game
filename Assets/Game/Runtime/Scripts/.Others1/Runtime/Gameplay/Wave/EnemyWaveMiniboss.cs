using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Spawning/Miniboss Wave")]
    public class EnemyWaveMiniboss : EnemyWaveConfig, IWaveInterval, IWaveInitial
    {
        [SerializeField] private EnemyGroup[] _enemyGroups;
        [SerializeField] private float _interval;
        [SerializeField] private EnemyGroup _minibossGroup;

        public float WaveInterval => _interval;

        public SpawnData[] GetInitialWave(Vector2 origin)
        {
            return _minibossGroup.ToSpawnData(origin, SpawnType.Elite);
        }

        public SpawnData[] GetIntervalWave(Vector2 origin)
        {
            var enemyGroup = _enemyGroups.Roll();
            return enemyGroup.ToSpawnData(origin, SpawnType.Mob);
        }

    }
}
