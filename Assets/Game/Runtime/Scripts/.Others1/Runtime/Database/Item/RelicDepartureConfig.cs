using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Item/Relic/Departure Config", fileName = "RelicDepartureConfig")]
    public class RelicDepartureConfig : ItemConfig
    {
        [HideLabel]
        [SerializeField] private RelicDeparture _relicConfig = new();

        public override IItem Config => _relicConfig;
    }
}
