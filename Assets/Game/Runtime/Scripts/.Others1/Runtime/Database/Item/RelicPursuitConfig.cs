using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Item/Relic/Pursuit Config", fileName = "RelicPursuit")]
    public class RelicPursuitConfig : ItemConfig
    {
        [HideLabel]
        [SerializeField] private RelicPursuit _relicConfig = new();

        public override IItem Config => _relicConfig;
    }
}
