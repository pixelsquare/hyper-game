using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Missions/Mission Config", fileName = "MissionConfig")]
    public class MissionConfig : BaseConfig<Mission>
    {
        [HideLabel]
        [SerializeField] private Mission _mission = new();

        public override Mission Config => _mission;
    }
}
