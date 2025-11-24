using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class SpawnData
    {
        public UnitSpawn prefab;
        public Vector2[] spawnPoints;
        public SpawnType spawnType;

        public static explicit operator SpawnData(EnemySet enemySet)
        {
            return new SpawnData
            {
                prefab = enemySet._enemyPrefab,
            };
        }
    }
}
