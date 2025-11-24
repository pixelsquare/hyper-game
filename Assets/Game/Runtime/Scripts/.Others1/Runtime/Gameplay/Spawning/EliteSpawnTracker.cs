namespace Santelmo.Rinsurv
{
    public class EliteSpawnTracker : UnitSpawnTracker
    {
        protected override SpawnType SpawnType => SpawnType.Elite;

        public override bool CanSpawn()
        {
            return true;
        }

        public override void OnDespawn(UnitSpawn unitSpawn)
        {
            base.OnDespawn(unitSpawn);

            if (_spawnList.Count < 1)
            {
                Dispatcher.SendMessage(GameEvents.Gameplay.OnMinibossWaveFinish);
            }
        }
    }
}
