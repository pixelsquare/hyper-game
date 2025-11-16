using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public class MockedCurrencyShopService : ICurrencyShopService
    {
        public enum ResultErrorFlags
        {
            None = 0,
            UnknownError = 0xb1 << 0,
            NetworkUnknownError = 0xb1 << 1,
            NetworkTimeoutError = 0xb1 << 2,
            NetworkUnreachableError = 0xb1 << 3,
            AppInMaintenanceError = 0xb1 << 4,
            InvalidReceiptError = 0xb1 << 5,
            FakeStoreInProductionError = 0xb1 << 6,
            ProductIdMismatchError = 0xb1 << 7,
            ItemNotFoundError = 0xb1 << 8,
            DuplicateTransactionIdError = 0xb1 << 9,
            InvalidStoreError = 0xb1 << 10,
            FailedToIncrementWalletBalanceError = 0xb1 << 11,
        }

        public int ResponseTimeInMilliseconds { get; set; }

        public ResultErrorFlags ErrorFlags { get; set; }
        
        public async Task<ServiceResultWrapper<GetCurrencyShopProductsResult>> GetCurrencyShopProductsAsync(GetCurrencyShopProductsRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetCurrencyShopProductsResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetCurrencyShopProductsResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetCurrencyShopProductsResult>(MockedErrors.appInMaintenanceError);
            }

            var products = new CurrencyShopProductCostData[]
            {
                new()
                {
                    id = "kumu.ph.kulitan_stg.coins1k",
                    cost = new Currency(2000, Currency.UBE_DIA)
                },
            };
            
            var result = new GetCurrencyShopProductsResult
            {
                items = products
            };

            return new ServiceResultWrapper<GetCurrencyShopProductsResult>(result);
        }

        public async Task<ServiceResultWrapper<ValidateReceiptResult>> ValidateReceiptAsync(ValidateReceiptRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<ValidateReceiptResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<ValidateReceiptResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<ValidateReceiptResult>(MockedErrors.appInMaintenanceError);
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.FakeStoreInProductionError))
            {
                return new ServiceResultWrapper<ValidateReceiptResult>(new ServiceError(ServiceErrorCodes.FAKE_STORE_IN_PRODUCTION, "Test store transactions is not allowed."));
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.ProductIdMismatchError))
            {
                return new ServiceResultWrapper<ValidateReceiptResult>(new ServiceError(ServiceErrorCodes.PRODUCT_ID_MISMATCH, "Product ID mismatched."));
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.ItemNotFoundError))
            {
                return new ServiceResultWrapper<ValidateReceiptResult>(new ServiceError(ServiceErrorCodes.ITEM_NOT_FOUND, "Item not found."));
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidReceiptError))
            {
                return new ServiceResultWrapper<ValidateReceiptResult>(new ServiceError(ServiceErrorCodes.FAILED_TO_VERIFY_RECEIPT, "Failed to verify receipt."));
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.DuplicateTransactionIdError))
            {
                return new ServiceResultWrapper<ValidateReceiptResult>(new ServiceError(ServiceErrorCodes.DUPLICATE_TRANSACTION_ID, "Transaction ID of receipt has been already processed."));
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.DuplicateTransactionIdError))
            {
                return new ServiceResultWrapper<ValidateReceiptResult>(new ServiceError(ServiceErrorCodes.DUPLICATE_TRANSACTION_ID, "Transaction ID of receipt has been already processed."));
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidStoreError))
            {
                var store = "StoreName";
                return new ServiceResultWrapper<ValidateReceiptResult>(new ServiceError(ServiceErrorCodes.INTERNAL_SERVER_ERROR, $"Invalid store: {store}"));
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.FailedToIncrementWalletBalanceError))
            {
                return new ServiceResultWrapper<ValidateReceiptResult>(new ServiceError(ServiceErrorCodes.FAILED_TO_INCREMENT_WALLET_BALANCE, "Failed to increment wallet balance."));
            }

            var result = new ValidateReceiptResult
            {
                walletBalance = new []
                {
                    new Currency(0, Currency.UBE_COI),
                    new Currency(0, Currency.UBE_DIA)
                }
            };

            return new ServiceResultWrapper<ValidateReceiptResult>(result);
        }

        public async Task<ServiceResultWrapper<BuyCurrencyResult>> BuyCurrencyAsync(BuyCurrencyRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<BuyCurrencyResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<BuyCurrencyResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<BuyCurrencyResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new BuyCurrencyResult
            {
                walletBalance = new []
                {
                    new Currency(0, Currency.UBE_COI),
                    new Currency(0, Currency.UBE_DIA)
                }
            };

            return new ServiceResultWrapper<BuyCurrencyResult>(result);
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
