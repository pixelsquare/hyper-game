namespace Santelmo.Rinsurv
{
    public delegate void OnSpawn(UnitSpawn unitSpawn);
    public delegate void OnDespawn(UnitSpawn unitSpawn);
    
    public interface ISpawnEvent
    {
        public event OnSpawn OnSpawnEvent;
        public event OnDespawn OnDespawnEvent;
    }
}
