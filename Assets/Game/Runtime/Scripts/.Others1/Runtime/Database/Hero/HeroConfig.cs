using Sirenix.OdinInspector;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Hero/Hero Config", fileName = "HeroConfig")]
    public class HeroConfig : BaseConfig<Hero>
    {
        [HideLabel]
        [SerializeField] private Hero _hero = new();

        public override Hero Config => _hero;
    }
}
