using System.Threading.Tasks;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitGetShopItemCosts : UnitServiceBase<GetShopItemsCostResult>
    {
        protected override Task<ServiceResultWrapper<GetShopItemsCostResult>> GetServiceOperation()
        {
            return Services.ShopService.GetShopItemCostsAsync(new GetShopItemCostsRequest());
        }

        protected override void BeforeExit()
        {
            var itemDatabase = ItemDatabase.Current;

            var currency = new Currency();

#if USES_MOCKS
            currency.code = Currency.UBE_COI;
            currency.amount = 0;
#endif
            // Resets item cost for all items on the database.
            itemDatabase.ItemConfigs.ForEach(item => item.SetItemCost(currency, 0));

            foreach (var itemCost in task.Result.Result.itemsCost)
            {
                var key = itemCost.itemId;

                if (itemDatabase.TryGetItem(key, out var config))
                {
                    config.SetItemCost(itemCost.cost, itemCost.markUpDownCost);
                }
            }
        }
    }
}
