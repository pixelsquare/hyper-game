using System.Linq;

namespace Santelmo.Rinsurv
{
    public class MobSpawnTracker : UnitSpawnTracker
    {
        private readonly int _max;
        
        protected override SpawnType SpawnType => SpawnType.Mob;
        
        public override bool CanSpawn()
        {
            if (_spawnList.Count == _max)
            {
                DespawnOldest();
            }
            
            return true;
        }
        
        private void DespawnOldest()
        {
            var oldest = _spawnList.FirstOrDefault();
            if (oldest)
            {
                oldest.Despawn();
            }
        }

        public MobSpawnTracker(int max)
        {
            _max = max;            
        }
    }
}
