using System;
using System.Collections.Generic;
using Kumu.Kulitan.Avatar;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    internal class GetShopItemsCostsRequester : Requester<GetShopItemCostsRequest, GetShopItemsCostResult>
    {
        public GetShopItemsCostsRequester(GetShopItemCostsRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Get(BackendUtil.GetFullUrl("/api/v3/shop"));
        }

        public override GetShopItemsCostResult FormResultFromPayload(HttpResponseObject payload)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(payload.data.ToString());

            var items = JsonConvert.DeserializeObject<IDictionary<string, object>[]>(dataMap["items"].ToString());
            var itemsLen = items.Length;

            var shopItems = new ShopItem[itemsLen];

            for (var i = 0; i < itemsLen; i++)
            {
                var item = items[i];
                shopItems[i] = new ShopItem
                {
                    itemId = item["id"].ToString(),
                    cost = JsonConvert.DeserializeObject<Currency>(item["cost"].ToString()),
                    markUpDownCost = int.Parse(item["mark_up_down_cost"].ToString())
                };
            }

            var result = new GetShopItemsCostResult
            {
                itemsCost = shopItems
            };

            return result;
        }
    }

    internal class BuyShopItemsRequester : Requester<BuyShopItemsRequest, BuyShopItemsResult>
    {
        public BuyShopItemsRequester(BuyShopItemsRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var reqConverter = new GenericServiceConverter<BuyShopItemsRequest>(BackendUtil.GlobalMapping);
            return BackendUtil.Post(BackendUtil.GetFullUrl("/api/v3/shop/buy"), JsonConvert.SerializeObject(RequestObject, reqConverter));
        }

        public override BuyShopItemsResult FormResultFromPayload(HttpResponseObject payload)
        {
            var data = JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
            {
                fully_successful = false,
                equipped_items = new object(),
                bought_items = new object(),
                updated_balance = Array.Empty<Currency>()
            });

            var resConverter = new GenericServiceConverter<AvatarItemState[]>(BackendUtil.GlobalMapping);
            var equippedItems = JsonConvert.DeserializeObject<AvatarItemState[]>(data.equipped_items.ToString(), resConverter);
            var boughtItems = JsonConvert.DeserializeObject<AvatarItemState[]>(data.bought_items.ToString(), resConverter);

            var result = new BuyShopItemsResult
            {
                fullySuccessful = data.fully_successful,
                boughtItems = boughtItems,
                equippedItems = equippedItems,
                updatedBalance = data.updated_balance
            };

            return result;
        }
    }
}
