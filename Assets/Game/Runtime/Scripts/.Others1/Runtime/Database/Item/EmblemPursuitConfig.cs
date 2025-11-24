using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Item/Emblem/Pursuit Config", fileName = "EmblemPursuitConfig")]
    public class EmblemPursuitConfig : ItemConfig
    {
        [HideLabel]
        [SerializeField] private EmblemPursuit _emblemConfig = new();

        public override IItem Config => _emblemConfig;
    }
}
