using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Item/Relic/Victory Config", fileName = "RelicVictoryConfig")]
    public class RelicVictoryConfig : ItemConfig
    {
        [HideLabel]
        [SerializeField] private RelicVictory _relicConfig = new();

        public override IItem Config => _relicConfig;
    }
}
