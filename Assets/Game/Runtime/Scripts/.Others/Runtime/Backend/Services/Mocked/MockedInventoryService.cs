using System;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedInventoryService : IInventoryService
    {
        public enum ResultErrorFlags
        {
            None = 0,
            UnknownError = 0xb1 << 1,
            NetworkUnknownError = 0xb1 << 2,
            NetworkTimeoutError = 0xb1 << 3,
            NetworkUnreachableError = 0xb1 << 4,
            ItemsNotFoundError = 0xb1 << 5,
            ItemsUnownedError = 0xb1 << 6,
            AppInMaintenanceError = 0xb1 << 7,
            GetWalletBalanceError = 0xb1 << 8,
            GetInventoryError = 0xb1 << 9,
        }

        public int ResponseTimeInMilliseconds { get; set; }

        public ResultErrorFlags ErrorFlags { get; set; }

        public Currency[] WalletBalance { get; set; }

        public async Task<ServiceResultWrapper<GetWalletBalanceResult>> GetWalletBalanceAsync(GetWalletBalanceRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetWalletBalanceResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetWalletBalanceResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetWalletBalanceResult>(MockedErrors.appInMaintenanceError);
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.GetWalletBalanceError))
            {
                return new ServiceResultWrapper<GetWalletBalanceResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Get wallet balance error"));
            }

            var result = new GetWalletBalanceResult();
            result.walletBalance = WalletBalance;

            return new ServiceResultWrapper<GetWalletBalanceResult>(result);
        }

        public async Task<ServiceResultWrapper<GetInventoryResult>> GetInventoryAsync(GetInventoryRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetInventoryResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetInventoryResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetInventoryResult>(MockedErrors.appInMaintenanceError);
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.GetInventoryError))
            {
                return new ServiceResultWrapper<GetInventoryResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Get Inventory error"));
            }

            // TODO: Add more error handling?

            var ownedItemsJson = PlayerPrefs.GetString(MockedServicesUtil.OWNED_ITEMS_KEY, "{}");
            var ownedItems = JsonHelper.FromJson<AvatarItemState>(ownedItemsJson) ?? Array.Empty<AvatarItemState>();
            var ownedItemIds = ownedItems.Select(a => a.itemId).ToArray();

            var equippedItemsJson = PlayerPrefs.GetString(MockedServicesUtil.EQUIPPED_ITEMS_KEY, "{}");
            var equippedItems = JsonHelper.FromJson<AvatarItemState>(equippedItemsJson) ?? Array.Empty<AvatarItemState>();

            var result = new GetInventoryResult
            {
                ownedItemIds = ownedItemIds,
                equippedItems = equippedItems,
                walletBalance = new Currency[]
                {
                    new() { code = Currency.UBE_DIA, amount = 0 },
                    new() { code = Currency.UBE_COI, amount = 0 },
                }
            };

            return new ServiceResultWrapper<GetInventoryResult>(result);
        }

        public async Task<ServiceResultWrapper<EquipItemsResult>> EquipItemsAsync(EquipItemsRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<EquipItemsResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<EquipItemsResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.ItemsNotFoundError))
            {
                return new ServiceResultWrapper<EquipItemsResult>(MockedErrors.itemsNotFoundError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.ItemsUnownedError))
            {
                return new ServiceResultWrapper<EquipItemsResult>(MockedErrors.itemsNotOwnedError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<EquipItemsResult>(MockedErrors.appInMaintenanceError);
            }

            // TODO: Add more error handling?

            var equippedItems = request.equippedItems;
            var currentItemsJson = JsonHelper.ToJson(request.equippedItems.ToArray());
            PlayerPrefs.SetString(MockedServicesUtil.EQUIPPED_ITEMS_KEY, currentItemsJson);

            var result = new EquipItemsResult
            {
                equippedItems = equippedItems.ToArray()
            };

            return new ServiceResultWrapper<EquipItemsResult>(result);
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
    }
}
