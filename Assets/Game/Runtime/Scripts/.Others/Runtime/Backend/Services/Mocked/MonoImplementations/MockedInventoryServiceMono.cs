using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedInventoryServiceMono : MockedServiceMono, IInventoryService
    {
        [SerializeField] private MockedInventoryService.ResultErrorFlags errorFlags;

        [SerializeField] private int responseTimeInMilliseconds = 1;

        [SerializeField] private Currency[] walletBalance =
        {
            new() { code = Currency.UBE_DIA, amount = 100 },
            new() { code = Currency.UBE_COI, amount = 100 }
        };

        private readonly MockedInventoryService service = new();

        public async Task<ServiceResultWrapper<GetWalletBalanceResult>> GetWalletBalanceAsync(GetWalletBalanceRequest request)
        {
            // TODO: add error responses
            ConfigService();
            return await service.GetWalletBalanceAsync(request);
        }

        public async Task<ServiceResultWrapper<GetInventoryResult>> GetInventoryAsync(GetInventoryRequest request)
        {
            // TODO: add error responses
            ConfigService();
            return await service.GetInventoryAsync(request);
        }

        public async Task<ServiceResultWrapper<EquipItemsResult>> EquipItemsAsync(EquipItemsRequest request)
        {
            // TODO: add error responses
            ConfigService();
            return await service.EquipItemsAsync(request);
        }

        private void Start()
        {
            InitializeUserItems();
        }

        private void InitializeUserItems()
        {
            if (PlayerPrefs.HasKey(MockedServicesUtil.EQUIPPED_ITEMS_KEY)
             && PlayerPrefs.HasKey(MockedServicesUtil.OWNED_ITEMS_KEY))
            {
                return;
            }

            var defaultItemData = Resources.Load<DefaultItemData>("Avatars/DefaultItems/AvatarDefaultItems");
            var swatchTable = Resources.Load<SwatchTable>("Avatars/ColorSwatches/SwatchTable");

            var equippedItemsMap = new Dictionary<AvatarItemType, AvatarItemState>();

            foreach (var pair in defaultItemData.AllItemPairs())
            {
                if (equippedItemsMap.TryGetValue(pair.Key, out _))
                {
                    continue;
                }

                var defaultItem = pair.Value;

                if (swatchTable.TryGetDefaultColor(pair.Key, out var color))
                {
                    defaultItem.SetStateColor(color);
                }

                equippedItemsMap.Add(pair.Key, defaultItem.State);
            }

            var items = equippedItemsMap.Values.ToArray();
            PlayerPrefs.SetString(MockedServicesUtil.OWNED_ITEMS_KEY, JsonHelper.ToJson(items));
            PlayerPrefs.SetString(MockedServicesUtil.EQUIPPED_ITEMS_KEY, JsonHelper.ToJson(items));
        }

        private void ConfigService()
        {
            service.ErrorFlags = errorFlags;
            service.ResponseTimeInMilliseconds = responseTimeInMilliseconds;
            service.WalletBalance = walletBalance;
        }
    }
}
