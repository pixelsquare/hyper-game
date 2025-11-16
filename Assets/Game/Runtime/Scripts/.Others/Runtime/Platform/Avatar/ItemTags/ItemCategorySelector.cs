using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(fileName = "ItemCategorySelector", menuName = "Config/KumuKulitan/Avatars/ItemCategorySelector")]
    public class ItemCategorySelector : ScriptableObject
    {
        [SerializeField] private ItemCategoryConfig[] configs;

        private Dictionary<string, ItemCategoryConfig> configTable;

        public bool TryGetValue(AvatarItemData itemData, out ItemCategoryConfig config)
        {
            if (string.IsNullOrEmpty(itemData.itemCategory))
            {
                config = null;
                return false;
            }

            return configTable.TryGetValue(itemData.itemCategory, out config);
        }
        
        public IEnumerable<ItemCategoryConfig> GetConfigsOfItemType(AvatarItemType itemType)
        {
            return from config in configs
                   where itemType.Equals(config.ItemType)
                   select config;
        }

        private void OnEnable()
        {
            if (configTable == null)
            {
                configTable = configs.ToDictionary(config => config.ItemCategory.ToLower());
            }
        }
    }
}
