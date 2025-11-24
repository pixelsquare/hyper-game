using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Hero/Hero Database", fileName = "HeroDatabase")]
    public class HeroDatabase : ScriptableObject
    {
        [SerializeField] private HeroConfig[] _heroConfigs;

        public IEnumerable<Hero> Heroes => _heroConfigs.Select(x => x.Config);

        public Hero GetHero(string id)
        {
            return !string.IsNullOrEmpty(id) ? Heroes.First(x => x.Id.Equals(id)) : null;
        }
    }
}
