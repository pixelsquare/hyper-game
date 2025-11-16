using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Tracking;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitPurchaseItems : Unit
    {
        [DoNotSerialize]
        public ValueInput itemSelectionController;

        [DoNotSerialize]
        public ValueOutput fullySuccessful;

        [DoNotSerialize]
        public ValueOutput error;

        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize]
        public ControlOutput errorTrigger;

        protected override void Definition()
        {
            itemSelectionController = ValueInput<ItemSelectionController>(nameof(itemSelectionController));
            fullySuccessful = ValueOutput<bool>(nameof(fullySuccessful));
            error = ValueOutput<ServiceError>(nameof(error));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), RunCoroutine);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            errorTrigger = ControlOutput(nameof(errorTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator RunCoroutine(Flow flow)
        {
            var selectedItems = flow.GetValue<ItemSelectionController>(itemSelectionController).SelectedItems.ToArray();
            var selectedItemStates = selectedItems.Select(i => i.State).ToArray();
            var unOwnedItems = selectedItems.Where(i => !UserInventoryData.IsItemOwned(i.Data.itemId)).ToArray();

            GetCartExpectedCost(unOwnedItems, out var expectedCost);

            var request = new BuyShopItemsRequest
            {
                equippedAndItemsToBuy = selectedItemStates,
                expectedCost = expectedCost
            };

            var task = Services.ShopService.BuyShopItemsAsync(request);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Result.HasError)
            {
                flow.SetValue(error, task.Result.Error);
                yield return errorTrigger;
                yield break;
            }

            // TODO: validate response and return error response if invalid data received from backend?
            // 1. no equipped items occupying the same slot
            // 2. currencies are distinct

            var equippedItems = task.Result.Result.equippedItems;
            var boughtItems = task.Result.Result.boughtItems;
            var updatedBalance = task.Result.Result.updatedBalance;

            SendItemPurchasedAnalytics(selectedItems);

            var newEquippedItems = new List<AvatarItemState>(equippedItems).Concat(boughtItems);
            var ownedItems = UserInventoryData.OwnedItems;

            UserInventoryData.EquippedItems = newEquippedItems.ToArray();
            UserInventoryData.OwnedItems = ownedItems.Concat(boughtItems).ToArray();
            UserInventoryData.UserWallet.SetCurrencies(updatedBalance);

            flow.SetValue(fullySuccessful, task.Result.Result.fullySuccessful);

            yield return outputTrigger;
        }

        private void GetCartExpectedCost(AvatarItemConfig[] itemConfigs, out Currency[] expectedCost)
        {
            expectedCost = new Currency[]
            {
                new() { code = Currency.UBE_COI, amount = 0 },
                new() { code = Currency.UBE_DIA, amount = 0 }
            };

            foreach (var item in itemConfigs)
            {
                var itemCost = item.Data.cost;

                if (string.IsNullOrEmpty(itemCost.code))
                {
                    continue;
                }

                switch (itemCost.code)
                {
                    case Currency.UBE_COI:
                        expectedCost[0].amount += itemCost.amount;
                        break;

                    case Currency.UBE_DIA:
                        expectedCost[1].amount += itemCost.amount;
                        break;
                }
            }
        }

        private void SendItemPurchasedAnalytics(AvatarItemConfig[] selectedItems)
        {
            var unOwnedItems = selectedItems.Where(i => !UserInventoryData.IsItemOwned(i.Data.itemId)).ToArray();
            var userProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();

            foreach (var item in unOwnedItems)
            {
                var itemCost = item.Data.cost;
                var itemCostCode = itemCost.code;
                var kCoinCost = string.CompareOrdinal(itemCostCode, Currency.UBE_COI) == 0 ? itemCost.amount : 0;
                var diamondCost = string.CompareOrdinal(itemCostCode, Currency.UBE_DIA) == 0 ? itemCost.amount : 0;
                var coinCost = 0;

                var itemPurchasedEvent = new ItemPurchasedEvent
                (
                        userProfile.accountId,
                        0,
                        item.State.itemId,
                        diamondCost,
                        kCoinCost,
                        coinCost
                );

                GlobalNotifier.Instance.Trigger(itemPurchasedEvent);
            }
        }
    }
}
