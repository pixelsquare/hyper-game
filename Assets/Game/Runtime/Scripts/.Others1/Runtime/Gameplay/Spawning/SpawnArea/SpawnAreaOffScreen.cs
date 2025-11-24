using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Spawning/Spawn Area/Off Screen")]
    public class SpawnAreaOffScreen : SpawnAreaConfig
    {
        [SerializeField] private Vector2 _maxOffset;
        
        /// <summary>
        /// Returns a randomized spawn points outside the screen but clamped within a <c>maxOffset</c>. 
        /// </summary>
        public override Vector2[] RollSpawnPoints(Vector2 origin, uint amount)
        {
            var mainCamera = Camera.main;
            var edge = mainCamera.ScreenSizeToWorldSpace2D() / 2f;
            var bounds = edge + _maxOffset;
            var spawnPoints = new Vector2[amount];

            for (var i = 0; i < amount; ++i)
            {
                var axis = Random.value > .5f;
                var bias = new Vector2(
                    Random.value > .5f ? -1 : 1,
                    Random.value > .5f ? -1 : 1
                ); 
                
                var x = axis ?
                    Random.Range(-bounds.x, bounds.x) :
                    Random.Range(edge.x, bounds.x);
                
                var y = !axis ?
                    Random.Range(-bounds.y, bounds.y) :
                    Random.Range(edge.y, bounds.y);

                var magnitude = new Vector2(x, y);
                spawnPoints[i] = Vector2.Scale(magnitude, bias);
            }


            return spawnPoints;
        }
    }
}
