using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Kulitan.Gifting;

namespace Kumu.Kulitan.Backend
{
    /// <summary>
    /// Not testable in TestRunner due to dependency on singletons.
    /// </summary>
    public class MockedVirtualGiftService : IVirtualGiftService
    {
        private Action<VirtualGiftEventInfo> _virtualGiftReceivedEvent;

        public enum ResultErrorFlags
        {
            None = 0,
            UnknownError = 0xb1 << 1,
            NetworkUnknownError = 0xb1 << 2,
            NetworkTimeoutError = 0xb1 << 3,
            NetworkUnreachableError = 0xb1 << 4,
            InsufficientWalletBalanceError = 0xb1 << 5,
            TransactionFailed = 0xb1 << 10,
            AppInMaintenanceError = 0xb1 << 11,
        }

        public int ResponseTimeInMilliseconds { get; set; }

        public int VirtualGiftNotificationDelayInMilliseconds { get; set; }

        public ResultErrorFlags ErrorFlags { get; set; }

        public Action<VirtualGiftEventInfo> VirtualGiftReceivedEvent { get; set; }

        private string LocalAccountId => UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId;

        private VirtualGiftDatabase vgDatabase;

        private VirtualGiftDatabase VgDatabase =>
            VirtualGiftDatabase.Current; // This singleton makes it impossible to write unit tests for this class

        /// <summary>
        /// Called to get pricing information of Virtual Gift items (to sync into <see cref="VirtualGiftDatabase"/> for example).
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <returns>An array of virtual gift cost data.</returns>
        public async Task<ServiceResultWrapper<GetVirtualGiftCostsResult>> GetVirtualGiftCostsAsync(
            GetVirtualGiftCostsRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetVirtualGiftCostsResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetVirtualGiftCostsResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetVirtualGiftCostsResult>(MockedErrors.appInMaintenanceError);
            }

            // for mock, just use the pricing information in the VirtualGiftDatabase as is
            var costs = new List<VirtualGiftCostData>();
            foreach (var vgConfig in VirtualGiftDatabase.Current.GiftConfigs)
            {
                costs.Add(new VirtualGiftCostData
                {
                    id = vgConfig.Data.giftId,
                    cost = vgConfig.Data.cost,
                    markUpDownCost = 0
                });
            }

            return new ServiceResultWrapper<GetVirtualGiftCostsResult>(new GetVirtualGiftCostsResult
                { itemCosts = costs.ToArray() });
        }

        public async Task<ServiceResultWrapper<SendVirtualGiftResult>> SendVirtualGiftAsync(
            SendVirtualGiftRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<SendVirtualGiftResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<SendVirtualGiftResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InsufficientWalletBalanceError))
            {
                return new ServiceResultWrapper<SendVirtualGiftResult>(MockedErrors.insufficientWalletBalanceError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.TransactionFailed))
            {
                return new ServiceResultWrapper<SendVirtualGiftResult>(MockedErrors.transactionFailedError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<SendVirtualGiftResult>(MockedErrors.appInMaintenanceError);
            }

            // wallet balance is always 0/0 while mocked, the balance response is not used anyway
            var newBalance = new Currency[]
            {
                new() { amount = 0, code = Currency.UBE_COI },
                new() { amount = 0, code = Currency.UBE_DIA },
            };

            //notify sender
            var result = new SendVirtualGiftResult
            {
                updatedBalanceTimestamp = DateTime.UtcNow,
                newWalletBalance = newBalance
            };

            return new ServiceResultWrapper<SendVirtualGiftResult>(result);
        }

        /// <summary>
        /// Used to invoke the the VirtualGiftReceivedEvent delegate, since SendVirtualGiftAsync() will not automatically invoke it.
        /// In TestRunner, make sure to provide a gifter value or an exception will be thrown. 
        /// </summary>
        /// <param name="request">Mocked request.</param>
        /// <param name="gifter">Mocked account id of gifter. If null or whitespace, will use account id from <see cref="UserProfileLocalDataManager"/>.</param>
        public void InvokeVirtualGiftReceivedEvent(SendVirtualGiftRequest request, string gifter = default)
        {
            if (string.IsNullOrWhiteSpace(gifter))
            {
                gifter = LocalAccountId;
            }

            var newVGEventInfo = new VirtualGiftEventInfo
            {
                gifter = gifter,
                giftees = request.giftees.ToArray(),
                gifts = request.gifts,
                category = request.category
            };

            VirtualGiftReceivedEvent?.Invoke(newVGEventInfo);
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
