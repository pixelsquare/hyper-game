using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Item/Item Database", fileName = "ItemDatabase")]
    public class ItemDatabase : ScriptableObject, IGlobalBinding
    {
        [SerializeField] private ItemConfig[] _itemConfigs;

        public IEnumerable<IItem> AllItems => _itemConfigs.Select(x => x.Config);
        public IEnumerable<Weapon> AllWeapons => _itemConfigs.Where(x => x.Config is Weapon)
                                                             .Select(x => x.Config as Weapon);
        public IEnumerable<Emblem> AllEmblems => _itemConfigs.Where(x => x.Config is Emblem)
                                                             .Select(x => x.Config as Emblem);
        public IEnumerable<Relic> AllRelics => _itemConfigs.Where(x => x.Config is Relic)
                                                           .Select(x => x.Config as Relic);

        public IEnumerable<T> GetItems<T>() where T : IItem
        {
            return _itemConfigs.Where(x => x.Config.GetType() == typeof(T)).Select(x => (T)x.Config);
        }

        public IItem GetItem(string itemId)
        {
            var idx = Array.FindIndex(_itemConfigs, x => x.Config.Id.Equals(itemId));
            return idx != -1 ? _itemConfigs[idx].Config : null;
        }
    }
}
