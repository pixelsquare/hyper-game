using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedShopService : IShopService
    {
        public enum ResultErrorFlags
        {
            None = 0,
            UnknownError = 0xb1 << 1,
            NetworkUnknownError = 0xb1 << 2,
            NetworkTimeoutError = 0xb1 << 3,
            NetworkUnreachableError = 0xb1 << 4,
            InsufficientWalletBalanceError = 0xb1 << 5,
            ExpectedCostsMismatchError = 0xb1 << 6,
            ItemsNotFoundError = 0xb1 << 7,
            ItemsAlreadyOwnedError = 0xb1 << 8,
            ItemsUnownedError = 0xb1 << 9,
            TransactionFailed = 0xb1 << 10,
            AppInMaintenanceError = 0xb1 << 11,
        }

        public int ResponseTimeInMilliseconds { get; set; }

        public ResultErrorFlags ErrorFlags { get; set; }

        public bool FullySuccessfulPurchase { get; set; }

        private readonly AvatarItemStateIdComparer idComparer = new();

        public async Task<ServiceResultWrapper<BuyShopItemsResult>> BuyShopItemsAsync(BuyShopItemsRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<BuyShopItemsResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<BuyShopItemsResult>(error);
            }

            // TODO: Add more error handling?

            if (ErrorFlags.HasFlag(ResultErrorFlags.InsufficientWalletBalanceError))
            {
                return new ServiceResultWrapper<BuyShopItemsResult>(MockedErrors.insufficientWalletBalanceError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.ExpectedCostsMismatchError))
            {
                return new ServiceResultWrapper<BuyShopItemsResult>(MockedErrors.expectedCostsMismatchError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.ItemsNotFoundError))
            {
                return new ServiceResultWrapper<BuyShopItemsResult>(MockedErrors.itemsNotFoundError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.ItemsAlreadyOwnedError))
            {
                return new ServiceResultWrapper<BuyShopItemsResult>(MockedErrors.itemsAlreadyOwnedError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.ItemsUnownedError))
            {
                return new ServiceResultWrapper<BuyShopItemsResult>(MockedErrors.itemsNotOwnedError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.TransactionFailed))
            {
                return new ServiceResultWrapper<BuyShopItemsResult>(MockedErrors.transactionFailedError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<BuyShopItemsResult>(MockedErrors.appInMaintenanceError);
            }

            var currentOwnedItemsJson = PlayerPrefs.GetString(MockedServicesUtil.OWNED_ITEMS_KEY, "{}");
            var currentOwnedItems = JsonHelper.FromJson<AvatarItemState>(currentOwnedItemsJson) ?? Array.Empty<AvatarItemState>();
            var currentOwnedItemsMap = currentOwnedItems.ToDictionary(a => a.itemId);

            foreach (var newItem in request.equippedAndItemsToBuy)
            {
                var key = newItem.itemId;

                if (!currentOwnedItemsMap.TryAdd(key, newItem))
                {
                    currentOwnedItemsMap[key] = newItem; // Overwrite the item when it already exists.
                }
            }

            var boughtItems = request.equippedAndItemsToBuy.Except(currentOwnedItems, idComparer);
            var equippedItems = request.equippedAndItemsToBuy.Except(boughtItems, idComparer);
            var selectedItems = equippedItems.Concat(boughtItems);
            var ownedItems = currentOwnedItemsMap.Values;

            PlayerPrefs.SetString(MockedServicesUtil.OWNED_ITEMS_KEY, JsonHelper.ToJson(ownedItems.ToArray()));
            PlayerPrefs.SetString(MockedServicesUtil.EQUIPPED_ITEMS_KEY, JsonHelper.ToJson(selectedItems.ToArray()));

            var result = new BuyShopItemsResult
            {
                fullySuccessful = FullySuccessfulPurchase,
                boughtItems = boughtItems.ToArray(),
                equippedItems = equippedItems.ToArray(),
                updatedBalance = request.expectedCost
            };

            return new ServiceResultWrapper<BuyShopItemsResult>(result);
        }

        public async Task<ServiceResultWrapper<GetShopItemsCostResult>> GetShopItemCostsAsync(GetShopItemCostsRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetShopItemsCostResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetShopItemsCostResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetShopItemsCostResult>(MockedErrors.appInMaintenanceError);
            }

            // TODO: Add more error handling?

            var sampleJson =
            "[" +
                "{ " +
                    "\"itemId\": \"UB_shirt_default_01_Karlito01\", " +
                    "\"cost\":{ \"code\": \"KCO\", \"amount\": \"49\" }," +
                    "\"markUpDownCost\": \"30\"" +
                "}," +
                "{ " +
                    "\"itemId\": \"UB_shirt_default_01_Karlito02\", " +
                    "\"cost\": { \"code\": \"KCO\", \"amount\": \"99\" }," +
                    "\"markUpDownCost\": \"99\"" +
                "}" +
            "]";

            var itemCosts = JsonHelper.FromJson<ShopItem>($"{{\"Data\": {sampleJson}}}") ?? Array.Empty<ShopItem>();

            var result = new GetShopItemsCostResult
            {
                itemsCost = itemCosts
            };

            return new ServiceResultWrapper<GetShopItemsCostResult>(result);
        }

        private bool TryGetNetworkError(out ServiceError error)
        {
            error = null;

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkUnreachableError))
            {
                error = ServiceErrors.networkUnreachableError;
                return true;
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkTimeoutError))
            {
                error = ServiceErrors.networkTimeoutError;
                return true;
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkUnknownError))
            {
                error = ServiceErrors.networkUnknownError;
                return true;
            }

            return false;
        }

        private class AvatarItemStateIdComparer : IEqualityComparer<AvatarItemState>
        {
            public bool Equals(AvatarItemState x, AvatarItemState y)
            {
                return x.itemId == y.itemId;
            }

            public int GetHashCode(AvatarItemState obj)
            {
                return obj.itemId.GetHashCode();
            }
        }
    }
}
