using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public interface IShopService
    {
        public Task<ServiceResultWrapper<BuyShopItemsResult>> BuyShopItemsAsync(BuyShopItemsRequest request);

        public Task<ServiceResultWrapper<GetShopItemsCostResult>> GetShopItemCostsAsync(GetShopItemCostsRequest request);
    }
}
