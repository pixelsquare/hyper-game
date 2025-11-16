using System;
using System.Collections.Generic;
using Kumu.Kulitan.Avatar;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    internal class EquipItemsRequester : Requester<EquipItemsRequest, EquipItemsResult>
    {
        public EquipItemsRequester(EquipItemsRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var reqConverter = new GenericServiceConverter<EquipItemsRequest>(BackendUtil.GlobalMapping);
            return BackendUtil.Put(BackendUtil.GetFullUrl("/api/v3/inventory"), JsonConvert.SerializeObject(RequestObject, reqConverter));
        }

        public override EquipItemsResult FormResultFromPayload(HttpResponseObject payload)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(payload.data.ToString());
            var resConverter = new GenericServiceConverter<AvatarItemState[]>(BackendUtil.GlobalMapping);
            var equippedItems = JsonConvert.DeserializeObject<AvatarItemState[]>(dataMap["equipped_items"].ToString(), resConverter);

            var result = new EquipItemsResult
            {
                equippedItems = equippedItems
            };

            return result;
        }
    }

    internal class GetInventoryRequester : Requester<GetInventoryRequest, GetInventoryResult>
    {
        public GetInventoryRequester(GetInventoryRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Get(BackendUtil.GetFullUrl("/api/v3/inventory"));
        }

        public override GetInventoryResult FormResultFromPayload(HttpResponseObject payload)
        {
            var data = JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
            {
                wallet = Array.Empty<Currency>(),
                items = Array.Empty<string>(),
                equipped_items = new object()
            });

            var resConverter = new GenericServiceConverter<AvatarItemState[]>(BackendUtil.GlobalMapping);
            var equippedItems = JsonConvert.DeserializeObject<AvatarItemState[]>(data.equipped_items.ToString(), resConverter);

            var result = new GetInventoryResult
            {
                ownedItemIds = data.items,
                walletBalance = data.wallet,
                equippedItems = equippedItems
            };

            return result;
        }
    }

    internal class GetWalletBalanceRequester : Requester<GetWalletBalanceRequest, GetWalletBalanceResult>
    {
        public GetWalletBalanceRequester(GetWalletBalanceRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Get(BackendUtil.GetFullUrl("/api/v3/inventory/wallet"));
        }

        public override GetWalletBalanceResult FormResultFromPayload(HttpResponseObject payload)
        {
            var data = JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
            {
                wallet = Array.Empty<Currency>()
            });

            var result = new GetWalletBalanceResult
            {
                walletBalance = data.wallet
            };

            return result;
        }
    }
}
