using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public class CurrencyShopService : ICurrencyShopService
    {
        public async Task<ServiceResultWrapper<GetCurrencyShopProductsResult>> GetCurrencyShopProductsAsync(GetCurrencyShopProductsRequest request)
        {
            var requester = new GetCurrencyShopProductsRequester(request);
            return await requester.ExecuteRequestAsync();
        }

        public async Task<ServiceResultWrapper<ValidateReceiptResult>> ValidateReceiptAsync(ValidateReceiptRequest request)
        {
            var requester = new ValidateReceiptRequester(request);
            return await requester.ExecuteRequestAsync();
        }

        public async Task<ServiceResultWrapper<BuyCurrencyResult>> BuyCurrencyAsync(BuyCurrencyRequest request)
        {
            var requester = new BuyCurrencyRequester(request);
            return await requester.ExecuteRequestAsync();
        }
    }
}
