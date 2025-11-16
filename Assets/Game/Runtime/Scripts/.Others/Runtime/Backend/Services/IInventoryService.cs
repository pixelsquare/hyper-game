using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public interface IInventoryService
    {
        public Task<ServiceResultWrapper<GetWalletBalanceResult>> GetWalletBalanceAsync(GetWalletBalanceRequest request);

        public Task<ServiceResultWrapper<GetInventoryResult>> GetInventoryAsync(GetInventoryRequest request);

        public Task<ServiceResultWrapper<EquipItemsResult>> EquipItemsAsync(EquipItemsRequest request);
    }
}
