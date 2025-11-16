using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public class InventoryService : IInventoryService
    {
        public async Task<ServiceResultWrapper<GetWalletBalanceResult>> GetWalletBalanceAsync(GetWalletBalanceRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetWalletBalanceRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            // Serialize request

            #region SerializeRequest

            var fullUrl = BackendUtil.GetFullUrl("/api/v3/inventory/wallet");
            using var req = BackendUtil.Get(fullUrl);

            #endregion

            try
            {
                #region TrySendRequest

                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetWalletBalanceResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }
                
                #endregion

                #region TryGetPayload

                if (!BackendUtil.TryGetPayload<GetWalletBalanceResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                #endregion

                #region RefreshTokenWhenNeeded

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetWalletBalanceAsync(request));
                }

                #endregion

                #region DeserializeResult

                var data = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
                {
                    wallet = System.Array.Empty<Currency>()
                });

                var result = new GetWalletBalanceResult
                {
                    walletBalance = data.wallet
                };
                
                // return new ServiceResultWrapper<GetWalletBalanceResult>(result);

                #endregion

                return new ServiceResultWrapper<GetWalletBalanceResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetWalletBalanceResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<GetInventoryResult>> GetInventoryAsync(GetInventoryRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetInventoryRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/inventory");
            using var req = BackendUtil.Get(fullUrl);

            try
            {
                #region Test

                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetInventoryResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                #endregion

                if (!BackendUtil.TryGetPayload<GetInventoryResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetInventoryAsync(request));
                }

                var data = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
                {
                    wallet = System.Array.Empty<Currency>(),
                    items = System.Array.Empty<string>(),
                    equipped_items = new object()
                });

                var resConverter = new GenericServiceConverter<Avatar.AvatarItemState[]>(BackendUtil.GlobalMapping);
                var equippedItems = Newtonsoft.Json.JsonConvert.DeserializeObject<Avatar.AvatarItemState[]>(data.equipped_items.ToString(), resConverter);

                var result = new GetInventoryResult
                {
                    ownedItemIds = data.items,
                    walletBalance = data.wallet,
                    equippedItems = equippedItems
                };

                return new ServiceResultWrapper<GetInventoryResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetInventoryResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Get Inventory Failed", e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<EquipItemsResult>> EquipItemsAsync(EquipItemsRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new EquipItemsRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/inventory");

            var reqConverter = new GenericServiceConverter<EquipItemsRequest>(BackendUtil.GlobalMapping);
            var reqBody = Newtonsoft.Json.JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Put(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<EquipItemsResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<EquipItemsResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => EquipItemsAsync(request));
                }

                var dataMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(payload.data.ToString());

                var resConverter = new GenericServiceConverter<Avatar.AvatarItemState[]>(BackendUtil.GlobalMapping);
                var equippedItems = Newtonsoft.Json.JsonConvert.DeserializeObject<Avatar.AvatarItemState[]>(dataMap["equipped_items"].ToString(), resConverter);

                var result = new EquipItemsResult
                {
                    equippedItems = equippedItems
                };

                return new ServiceResultWrapper<EquipItemsResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<EquipItemsResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Item Equip Failed", e.Message));
            }
#endif
        }
    }
}
