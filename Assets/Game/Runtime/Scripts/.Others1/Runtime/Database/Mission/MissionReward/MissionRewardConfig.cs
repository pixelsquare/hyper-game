using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Missions/Mission Reward Config", fileName = "MissionRewardConfig")]
    public class MissionRewardConfig : ItemConfig
    {
        [HideLabel]
        [SerializeField] private MissionReward _missionReward;

        public override IItem Config => _missionReward;
    }
}
