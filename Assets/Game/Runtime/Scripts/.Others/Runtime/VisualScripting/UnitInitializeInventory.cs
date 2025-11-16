using System.Threading.Tasks;
using Kumu.Extensions;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitInitializeInventory : UnitServiceBase<GetInventoryResult>
    {
        protected override Task<ServiceResultWrapper<GetInventoryResult>> GetServiceOperation()
        {
            "Initializing inventory...".Log();
            return Services.InventoryService.GetInventoryAsync(new GetInventoryRequest());
        }

        protected override void BeforeExit()
        {
            UserInventoryData.EquippedItems = task.Result.Result.equippedItems;
            UserInventoryData.UserWallet = new UserWallet(task.Result.Result.walletBalance);

            if (AvatarSaveLoadUtil.TryConvertToAvatarItemState(task.Result.Result.ownedItemIds, out var ownedItems))
            {
                UserInventoryData.OwnedItems = ownedItems;
            }
        }
    }
}
