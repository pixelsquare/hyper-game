using System.Threading.Tasks;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedCurrencyShopServiceMono : MockedServiceMono, ICurrencyShopService
    {
        [SerializeField] private MockedCurrencyShopService.ResultErrorFlags errorFlags;

        [SerializeField] private int responseTimeInMilliseconds = 1;

        private readonly MockedCurrencyShopService service = new();
        
        public Task<ServiceResultWrapper<GetCurrencyShopProductsResult>> GetCurrencyShopProductsAsync(GetCurrencyShopProductsRequest request)
        {
            ConfigService();
            return service.GetCurrencyShopProductsAsync(request);
        }

        public Task<ServiceResultWrapper<ValidateReceiptResult>> ValidateReceiptAsync(ValidateReceiptRequest request)
        {
            ConfigService();
            return service.ValidateReceiptAsync(request);
        }

        public Task<ServiceResultWrapper<BuyCurrencyResult>> BuyCurrencyAsync(BuyCurrencyRequest request)
        {
            ConfigService();
            return service.BuyCurrencyAsync(request);
        }
        
        private void ConfigService()
        {
            service.ErrorFlags = errorFlags;
            service.ResponseTimeInMilliseconds = responseTimeInMilliseconds;
        }
    }
}
