using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public class ShopService : IShopService
    {
        public async Task<ServiceResultWrapper<BuyShopItemsResult>> BuyShopItemsAsync(BuyShopItemsRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new BuyShopItemsRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/shop/buy");

            var reqConverter = new GenericServiceConverter<BuyShopItemsRequest>(BackendUtil.GlobalMapping);
            var reqBody = Newtonsoft.Json.JsonConvert.SerializeObject(request, reqConverter);
            using var req = BackendUtil.Post(fullUrl, reqBody);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<BuyShopItemsResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<BuyShopItemsResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => BuyShopItemsAsync(request));
                }

                var data = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
                {
                    fully_successful = false,
                    equipped_items = new object(),
                    bought_items = new object(),
                    updated_balance = System.Array.Empty<Currency>()
                });

                var resConverter = new GenericServiceConverter<Avatar.AvatarItemState[]>(BackendUtil.GlobalMapping);
                var equippedItems = Newtonsoft.Json.JsonConvert.DeserializeObject<Avatar.AvatarItemState[]>(data.equipped_items.ToString(), resConverter);
                var boughtItems = Newtonsoft.Json.JsonConvert.DeserializeObject<Avatar.AvatarItemState[]>(data.bought_items.ToString(), resConverter);

                var result = new BuyShopItemsResult
                {
                    fullySuccessful = data.fully_successful,
                    boughtItems = boughtItems,
                    equippedItems = equippedItems,
                    updatedBalance = data.updated_balance
                };

                return new ServiceResultWrapper<BuyShopItemsResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<BuyShopItemsResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Buy Items Failed", e.Message));
            }
#endif
        }

        public async Task<ServiceResultWrapper<GetShopItemsCostResult>> GetShopItemCostsAsync(GetShopItemCostsRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new GetShopItemsCostsRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            var fullUrl = BackendUtil.GetFullUrl("/api/v3/shop");
            using var req = BackendUtil.Get(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<GetShopItemsCostResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<GetShopItemsCostResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                if (payload.error_code == ServiceErrorCodes.INVALID_TOKEN)
                {
                    return await TokenRefresh.RefreshTokensAsync(() => GetShopItemCostsAsync(request));
                }

                var dataMap = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>>(payload.data.ToString());

                var items = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, object>[]>(dataMap["items"].ToString());
                var itemsLen = items.Length;

                var shopItems = new ShopItem[itemsLen];

                for (var i = 0; i < itemsLen; i++)
                {
                    var item = items[i];
                    shopItems[i] = new ShopItem
                    {
                        itemId = item["id"].ToString(),
                        cost = Newtonsoft.Json.JsonConvert.DeserializeObject<Currency>(item["cost"].ToString()),
                        markUpDownCost = int.Parse(item["mark_up_down_cost"].ToString())
                    };
                }

                var result = new GetShopItemsCostResult
                {
                    itemsCost = shopItems
                };

                return new ServiceResultWrapper<GetShopItemsCostResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<GetShopItemsCostResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }
    }
}
