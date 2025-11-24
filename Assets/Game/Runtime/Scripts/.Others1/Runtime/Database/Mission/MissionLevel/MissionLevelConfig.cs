using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Missions/Mission Level Config", fileName = "MissionLevelConfig")]
    public class MissionLevelConfig : BaseConfig<MissionLevel>
    {
        [HideLabel]
        [SerializeField] private MissionLevel _missionLevel = new();

        public override MissionLevel Config => _missionLevel;
    }
}
