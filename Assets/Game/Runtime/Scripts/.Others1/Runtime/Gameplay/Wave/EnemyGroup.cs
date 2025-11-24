using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{       
    [Serializable]
    public class EnemyGroup
    {
        public EnemySet[] _enemySets;
    }

    [Serializable]
    public class EnemySet
    {
        public UnitSpawn _enemyPrefab;
        public SpawnAreaConfig _spawnArea;
        public uint _amount;

        public Vector2[] RollSpawnPoints(Vector2 origin, uint amount)
        {
            return _spawnArea.RollSpawnPoints(origin, amount);
        } 
    }
}
