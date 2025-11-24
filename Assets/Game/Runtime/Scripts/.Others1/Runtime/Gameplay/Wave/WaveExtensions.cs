using UnityEngine;

namespace Santelmo.Rinsurv
{
    public static class WaveExtensions
    {
        public static EnemyGroup Roll(this EnemyGroup[] enemyGroups)
        {
            var idx = Random.Range(0, enemyGroups.Length);
            return enemyGroups[idx];
        }

        public static SpawnData[] ToSpawnData(this EnemyGroup enemyGroup, Vector2 origin, SpawnType spawnType)
        {
            var amount = enemyGroup._enemySets.Length;
            var spawnData = new SpawnData[amount];

            for (var i = 0; i < amount; ++i)
            {
                var enemySet = enemyGroup._enemySets[i];
                var spawnDatum = (SpawnData)enemyGroup._enemySets[i];
                spawnDatum.spawnPoints = enemySet.RollSpawnPoints(origin, enemySet._amount);
                spawnDatum.spawnType = spawnType;
                spawnData[i] = spawnDatum;
            }

            return spawnData;
        }
    }
}
