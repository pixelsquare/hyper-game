using System.Threading.Tasks;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitGetWalletBalance : UnitServiceBase<GetWalletBalanceResult>
    {
        protected override Task<ServiceResultWrapper<GetWalletBalanceResult>> GetServiceOperation()
        {
            return Services.InventoryService.GetWalletBalanceAsync(new GetWalletBalanceRequest());
        }

        protected override void BeforeExit()
        {
            var result = task.Result.Result.walletBalance;
            UserInventoryData.UserWallet.SetCurrencies(result);
        }
    }
}
