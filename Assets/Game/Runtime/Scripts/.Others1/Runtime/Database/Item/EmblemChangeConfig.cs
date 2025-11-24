using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Item/Emblem/Change Config", fileName = "EmblemChangeConfig")]
    public class EmblemChangeConfig : ItemConfig
    {
        [HideLabel]
        [SerializeField] private EmblemChange _emblemConfig = new();

        public override IItem Config => _emblemConfig;
    }
}
