namespace Santelmo.Rinsurv
{
    public interface IMissionManager : IGlobalBinding
    {
        public Mission[] Missions { get; }

        public Mission ActiveMission { get; }

        public MissionLevel ActiveMissionLevel { get; }

        public void SetActiveMission(Mission mission);

        public void SetActiveMissionLevel(MissionLevel missionLevel);
    }
}
