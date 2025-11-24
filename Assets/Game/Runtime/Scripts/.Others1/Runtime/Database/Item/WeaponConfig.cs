using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Item/Weapon Config", fileName = "WeaponConfig")]
    public class WeaponConfig : ItemConfig
    {
        [HideLabel]
        [SerializeField] private Weapon _weapon = new();

        public override IItem Config => _weapon;
    }
}
