using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// ScriptableObject containing an array of Avatar Item data.
    /// </summary>
    [CreateAssetMenu(fileName = "AvatarItemDatabase", menuName = "Config/KumuKulitan/Avatars/ItemDatabase")]
    public class ItemDatabase : ScriptableObject // TODO: should be replaced with backend database when available.
    {
        #region Static
        public static ItemDatabase Current { get; private set; }
        
        public static async Task Load(AssetReferenceT<ItemDatabase> dbRef)
        {
            var db = await dbRef.LoadAssetAsync().Task;
            db.Initialize();
            Current = db;
        }
        #endregion

        [SerializeField] private List<AvatarItemConfig> itemDataConfig;

        private Dictionary<string, AvatarItemConfig> catalog;

        public List<AvatarItemConfig> ItemConfigs => itemDataConfig;

        public void Initialize()
        {
            catalog ??= itemDataConfig.ToDictionary(itemConfig => itemConfig.Data.itemId);
        }

        public bool TryGetItem(string itemId, out AvatarItemConfig itemConfig)
        {
            return catalog.TryGetValue(itemId, out itemConfig);
        }

        public AvatarItemConfig GetItem(int index)
        {
            return itemDataConfig[index];
        }

        public AvatarItemConfig GetItem(string itemId)
        {
            return catalog[itemId];
        }

        public bool HasItem(string itemId)
        {
            return catalog.ContainsKey(itemId);
        }

        public int GetItemIndex(string itemId)
        {
            return itemDataConfig.FindIndex(itemConfig => itemId.Equals(itemConfig.Data.itemId));
        }

        public bool TryGetItemIndex(string itemId, out int index)
        {
            if (!catalog.ContainsKey(itemId))
            {
                index = -1;
                return false;
            }
            
            index = itemDataConfig.FindIndex(itemConfig => itemId.Equals(itemConfig.Data.itemId));
            
            if (index == -1)
            {
                return false;
            }

            return true;
        }
        
        [ContextMenu("Log item ids")]
        private void LogItemIds()
        {
            var itemIds = itemDataConfig.Select(i => i.Data.itemId);
            
            $"Item ids in ItemDatabase:\n{string.Join(",\n", itemIds)}".Log();
        }

        [ContextMenu("Log item data")]
        private void LogItemData()
        {
            var itemData = from config in itemDataConfig
                           select new
                           {
                               id = config.Data.itemId,
                               name = config.Data.itemName,
                               typeCode = config.GetTypeCode(),
                               colorHex = "#FFFFFF", 
                           };

            var itemLog = from data in itemData
                          select "{\n" +
                                 $"    id: {data.id},\n" +
                                 $"    name: {data.name},\n" +
                                 $"    typeCode = {data.typeCode},\n" +
                                 $"    colorHex = {data.colorHex},\n" +
                                 "},";

            var logText = $"[\n{string.Join("\n", itemLog)}\n]";
            
            $"item database json:\n{logText}".Log();
            GUIUtility.systemCopyBuffer = logText;
        }

        [ContextMenu("Generate CSV")]
        private void CsvItemData()
        {
            var itemData = from config in itemDataConfig
                where !config.Unpurchaseable
                select new
                {
                    id = config.Data.itemId,
                    name = config.Data.itemName,
                    typeCode = config.GetTypeCode(),
                    colorHex = "#FFFFFF", 
                };
            
            var itemLog = from data in itemData
                          select $"{data.id},{data.name},{data.typeCode},{data.colorHex}";

            var logText = string.Join("\n", itemLog);
            logText.Log();
            GUIUtility.systemCopyBuffer = logText;
        }
    }
}
