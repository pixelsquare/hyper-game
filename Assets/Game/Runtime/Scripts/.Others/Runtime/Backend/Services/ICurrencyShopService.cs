using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public interface ICurrencyShopService
    {
        public Task<ServiceResultWrapper<GetCurrencyShopProductsResult>> GetCurrencyShopProductsAsync(GetCurrencyShopProductsRequest request);

        public Task<ServiceResultWrapper<ValidateReceiptResult>> ValidateReceiptAsync(ValidateReceiptRequest request);

        public Task<ServiceResultWrapper<BuyCurrencyResult>> BuyCurrencyAsync(BuyCurrencyRequest request);
    }
}
