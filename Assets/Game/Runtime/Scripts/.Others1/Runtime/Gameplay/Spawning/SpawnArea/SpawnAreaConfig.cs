using UnityEngine;

namespace Santelmo.Rinsurv
{
    public abstract class SpawnAreaConfig : ScriptableObject
    {
        public abstract Vector2[] RollSpawnPoints(Vector2 origin, uint amount);
    }
}
