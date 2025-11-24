using System.Linq;

namespace Santelmo.Rinsurv
{
    public class MissionManager : IMissionManager
    {
        private readonly MissionDatabase _missionDatabase;

        public MissionManager(MissionDatabase missionDatabase)
        {
            _missionDatabase = missionDatabase;
            ActiveMission = _missionDatabase.Missions.First(x => x != null);
            ActiveMissionLevel = ActiveMission.MissionLevels.First(x => x != null);
        }

        public Mission[] Missions => _missionDatabase.Missions.ToArray();

        public Mission ActiveMission { get; private set; }
        public MissionLevel ActiveMissionLevel { get; private set; }

        public void SetActiveMission(Mission mission)
        {
            ActiveMission = mission;
        }

        public void SetActiveMissionLevel(MissionLevel missionLevel)
        {
            ActiveMissionLevel = missionLevel;
        }
    }
}
