using Kumu.Kulitan.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// Scriptable object containing a map for default Avatar items ids.
    /// </summary>
    [CreateAssetMenu(fileName = "AvatarDefaultItems", menuName = "Config/KumuKulitan/Avatars/DefaultItems")]
    public class DefaultItemData : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<AvatarItemType, AvatarItemConfig> defaultItemMap;

        public KeyValuePair<AvatarItemType, AvatarItemConfig>[] AllItemPairs()
        {
            return defaultItemMap.ToArray();
        }

        public IEnumerable<AvatarItemType> GetKeys()
        {
            return defaultItemMap.Keys;
        }

        public IEnumerable<AvatarItemConfig> AllItems()
        {
            return defaultItemMap.Values;
        }

        public bool TryGetItem(AvatarItemType itemType, out AvatarItemConfig itemData)
        {
            return defaultItemMap.TryGetValue(itemType, out itemData);
        }

        public bool TryGetItem(string itemTypeCode, out AvatarItemConfig itemData)
        {
            var itemType = AvatarItemUtil.ToAvatarItemType(itemTypeCode);
            return TryGetItem(itemType, out itemData);
        }
    }
}
