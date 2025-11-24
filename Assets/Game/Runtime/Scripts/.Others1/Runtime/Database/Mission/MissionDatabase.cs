using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Missions/Mission Database", fileName = "MissionDatabase")]
    public class MissionDatabase : ScriptableObject
    {
        [SerializeField] private MissionConfig[] _missionConfigs;

        public IEnumerable<Mission> Missions => _missionConfigs.Select(x => x.Config);
    }
}
