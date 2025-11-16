using System.Linq;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Tracking;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitAnalyticsItemSelected : Unit
    {
        [DoNotSerialize] private ControlInput inputTrigger;
        [DoNotSerialize] private ControlOutput outputTrigger;

        [DoNotSerialize]
        public ValueInput itemSelectionController;

        protected override void Definition()
        {
            inputTrigger = ControlInput(nameof(inputTrigger), SendAnalytics);
            outputTrigger = ControlOutput(nameof(outputTrigger));

            itemSelectionController = ValueInput<ItemSelectionController>(nameof(itemSelectionController));

            Succession(inputTrigger, outputTrigger);
        }

        private ControlOutput SendAnalytics(Flow flow)
        {
            var selectedItems = flow.GetValue<ItemSelectionController>(itemSelectionController).SelectedItems.ToArray();
            var newEquippedItems = selectedItems.Where(i => 
                    !UserInventoryData.IsItemEquipped(i.Data.itemId)).ToArray();
            var userProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();

            foreach (var item in newEquippedItems)
            {
                var itemId = item.Data.itemId;
                var itemCost = item.Data.cost;
                var itemCostCode = itemCost.code;
                var kCoinCost = string.CompareOrdinal(itemCostCode, Currency.UBE_COI) == 0 ? itemCost.amount : 0;
                var diamondCost = string.CompareOrdinal(itemCostCode, Currency.UBE_DIA) == 0 ? itemCost.amount : 0;
                var coinCost = 0;

                var itemPurchasedEvent = new ItemSelectedEvent
                (
                        userProfile.accountId,
                        0,
                        item.State.itemId,
                        diamondCost,
                        kCoinCost,
                        coinCost,
                        UserInventoryData.IsItemOwned(itemId)
                );

                GlobalNotifier.Instance.Trigger(itemPurchasedEvent);
            }

            return outputTrigger;
        }
    }
}
