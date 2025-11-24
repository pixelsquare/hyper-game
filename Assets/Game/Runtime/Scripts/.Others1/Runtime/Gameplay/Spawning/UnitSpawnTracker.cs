using System.Collections.Generic;

namespace Santelmo.Rinsurv
{
    public abstract class UnitSpawnTracker 
    {
        protected readonly List<UnitSpawn> _spawnList;
        protected abstract SpawnType SpawnType { get; }

        protected UnitSpawnTracker()
        {
            _spawnList = new List<UnitSpawn>();
        }

        public abstract bool CanSpawn();

        public virtual void OnDespawn(UnitSpawn unitSpawn)
        {
            _spawnList.Remove(unitSpawn);
        }

        public virtual void OnSpawn(UnitSpawn unitSpawn)
        {
            _spawnList.Add(unitSpawn);
        }
    }
}
