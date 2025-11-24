using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Spawning/Spawn Area/Spread")]
    public class SpawnAreaSpread : SpawnAreaConfig
    {
        [SerializeField] private float _minRange = 4f;
        [SerializeField] private float _maxRange = 6f;
        
        public override Vector2[] RollSpawnPoints(Vector2 origin, uint amount)
        {
            var spawnPoints = new Vector2[amount];

            for (var i = 0; i < amount; ++i)
            {
                var range = Random.Range(_minRange, _maxRange);
                spawnPoints[i] = origin + Random.insideUnitCircle.normalized * range;
            }

            return spawnPoints;
        }
    }
}
