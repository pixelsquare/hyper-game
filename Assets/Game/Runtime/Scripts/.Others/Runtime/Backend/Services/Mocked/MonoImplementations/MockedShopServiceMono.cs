using System.Threading.Tasks;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedShopServiceMono : MockedServiceMono, IShopService
    {
        [SerializeField] private MockedShopService.ResultErrorFlags errorFlags;

        [SerializeField] private int responseTimeInMilliseconds = 1;

        [SerializeField] private bool fullySuccessful = true;

        private readonly MockedShopService service = new();

        public async Task<ServiceResultWrapper<BuyShopItemsResult>> BuyShopItemsAsync(BuyShopItemsRequest request)
        {
            // TODO: add error responses
            ConfigService();
            return await service.BuyShopItemsAsync(request);
        }

        public async Task<ServiceResultWrapper<GetShopItemsCostResult>> GetShopItemCostsAsync(GetShopItemCostsRequest request)
        {
            // TODO: add error responses
            ConfigService();
            return await service.GetShopItemCostsAsync(request);
        }

        private void ConfigService()
        {
            service.ErrorFlags = errorFlags;
            service.ResponseTimeInMilliseconds = responseTimeInMilliseconds;
            service.FullySuccessfulPurchase = fullySuccessful;
        }
    }
}
