using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class UnitSpawn : MonoBehaviour, ISpawnEvent
    {
        public event OnSpawn OnSpawnEvent;
        public event OnDespawn OnDespawnEvent;

        public SpawnType SpawnType => _spawnType;
        
        private SpawnType _spawnType;

        public void Init(SpawnType spawnType)
        {
            _spawnType = spawnType;
            OnSpawnEvent?.Invoke(this);
        }

        public void Despawn()
        {
            OnDespawnEvent?.Invoke(this);
        }
    }
}
