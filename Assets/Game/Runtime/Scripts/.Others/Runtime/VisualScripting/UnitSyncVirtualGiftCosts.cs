using System.Linq;
using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Gifting;

namespace Kumu.Kulitan.VisualScripting
{
    /// <summary>
    /// Called by FSM
    /// </summary>
    public class UnitSyncVirtualGiftCosts : UnitServiceBase<GetVirtualGiftCostsResult>
    {
        protected override Task<ServiceResultWrapper<GetVirtualGiftCostsResult>> GetServiceOperation()
        {
            return Services.VirtualGiftService.GetVirtualGiftCostsAsync(new GetVirtualGiftCostsRequest());
        }

        protected override void BeforeExit()
        {
            var itemCosts = task.Result.Result.itemCosts;
            
            // Resolve mismatched item ids by removing VirtualGiftConfigs from VGDatabase if backend does not respond
            // with data for those ids
            VirtualGiftDatabase.Current.GiftConfigs.RemoveAll(vg => itemCosts.All(i => vg.Data.giftId != i.id));
            
            // Update costs for items in VGDatabase based on info from backend
            foreach (var vg in VirtualGiftDatabase.Current.GiftConfigs)
            {
                var itemCost = itemCosts.First(i => i.id == vg.Data.giftId).cost;
                
                vg.SetGiftCost(new Currency
                {
                    amount = itemCost.amount,
                    code = itemCost.code,
                } );
            }

            VirtualGiftDatabase.Current.IsSynced = true;
        }
    }
}
