using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Item/Relic/Change Config", fileName = "RelicChangeConfig")]
    public class RelicChangeConfig : ItemConfig
    {
        [HideLabel]
        [SerializeField] private RelicChange _relicConfig = new();

        public override IItem Config => _relicConfig;
    }
}
