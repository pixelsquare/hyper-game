namespace Santelmo.Rinsurv
{
    public class BossSpawnTracker : UnitSpawnTracker
    {
        protected override SpawnType SpawnType => SpawnType.Boss;

        public override bool CanSpawn()
        {
            return true;
        }

        public override void OnDespawn(UnitSpawn unitSpawn)
        {
            base.OnDespawn(unitSpawn);

            if (_spawnList.Count < 1)
            {
                Dispatcher.SendMessage(GameEvents.Gameplay.OnGameWin);
            }
        }

        public override void OnSpawn(UnitSpawn unitSpawn)
        {
            base.OnSpawn(unitSpawn);
            
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnBossSpawn, unitSpawn);
        }
    }
}
