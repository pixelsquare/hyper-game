using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Extensions;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Gifting
{
    /// <summary>
    /// ScriptableObject containing an array of virtual gift data.
    /// </summary>
    [CreateAssetMenu(fileName = "VirtualGiftDatabase", menuName = "Config/KumuKulitan/Gifting/VirtualGiftDatabase")]
    public class VirtualGiftDatabase : ScriptableObject // TODO: should be replaced with backend database when available.
    {
        #region Static
        public static VirtualGiftDatabase Current { get; private set; }
        
        public static async Task Load(AssetReferenceT<VirtualGiftDatabase> dbRef)
        {
            var db = await dbRef.LoadAssetAsync().Task;
            db.Initialize();
            Current = db;
        }
        #endregion

        [SerializeField] private List<VirtualGiftConfig> virtualGiftConfig;

        private Dictionary<string, VirtualGiftConfig> catalog;

        public bool IsSynced { get; set; } = false;

        public List<VirtualGiftConfig> GiftConfigs => virtualGiftConfig;

        public void Initialize()
        {
            catalog ??= virtualGiftConfig.ToDictionary(giftConfig => giftConfig.Data.giftId);
        }

        public bool TryGetGift(string giftId, out VirtualGiftConfig giftConfig)
        {
            return catalog.TryGetValue(giftId, out giftConfig);
        }

        public VirtualGiftConfig GetGiftByIndex(int index)
        {
            return virtualGiftConfig[index];
        }

        public VirtualGiftConfig GetGift(string giftId)
        {
            return catalog[giftId];
        }

        public bool HasGift(string giftId)
        {
            return catalog.ContainsKey(giftId);
        }

        public int GetGiftIndex(string giftId)
        {
            return virtualGiftConfig.FindIndex(giftConfig => giftId.Equals(giftConfig.Data.giftId));
        }

        public bool TryGetGiftIndex(string giftId, out int index)
        {
            if (!catalog.ContainsKey(giftId))
            {
                index = -1;
                return false;
            }
            
            index = virtualGiftConfig.FindIndex(giftConfig => giftId.Equals(giftConfig.Data.giftId));
            
            if (index == -1)
            {
                return false;
            }

            return true;
        }

        public Currency[] ComputeCosts<T>(IEnumerable<T> vgQuantities) where T : IVirtualGiftIdQuantified
        {
            var wallet = new Wallet();

            foreach (var q in vgQuantities)
            {
                var giftExists = TryGetGift(q.GetId(), out var vgConfig);
                if (!giftExists)
                {
                    throw new Exception($"Gift id does not exist in database: {q.GetId()}");
                }
                
                wallet.AccumulateCurrencies(q.GetQuantity() * vgConfig.Data.cost);
            }

            return wallet.GetCurrencies().ToArray();
        }
        
        [ContextMenu("Log gift ids")]
        public void LogGiftIds()
        {
            var giftIds = virtualGiftConfig.Select(i => i.Data.giftId);
            
            $"Gift IDs in GiftDatabase:\n{string.Join(",\n", giftIds)}".Log();
        }
    }
}
