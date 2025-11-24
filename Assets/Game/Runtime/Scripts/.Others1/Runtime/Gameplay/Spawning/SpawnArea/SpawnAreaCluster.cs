using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Spawning/Spawn Area/Cluster")]
    public class SpawnAreaCluster : SpawnAreaConfig
    {
        [SerializeField] private float _radius = 1f;
        [SerializeField] private float _minRange = 4f;
        [SerializeField] private float _maxRange = 6f;
        
        public override Vector2[] RollSpawnPoints(Vector2 origin, uint amount)
        {
            var range = Random.Range(_minRange, _maxRange);
            var clusterCenter = origin + Random.insideUnitCircle * range;

            var spawnPoints = new Vector2[amount];

            for (var i = 0; i < amount; ++i)
            {
                spawnPoints[i] = clusterCenter + Random.insideUnitCircle.normalized * (_radius * Random.value);
            }

            return spawnPoints;
        }
    }
}
