using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Item/Emblem/Departure Config", fileName = "EmblemDepartureConfig")]
    public class EmblemDepartureConfig : ItemConfig
    {
        [HideLabel]
        [SerializeField] private EmblemDeparture _emblemConfig = new();

        public override IItem Config => _emblemConfig;
    }
}
