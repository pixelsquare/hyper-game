using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    internal class SendVirtualGiftRequester : Requester<SendVirtualGiftRequest, SendVirtualGiftResult>
    {
        public SendVirtualGiftRequester(SendVirtualGiftRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Post(BackendUtil.GetFullUrl("/api/v3/gift/send"), JsonConvert.SerializeObject(RequestObject));
        }

        public override SendVirtualGiftResult FormResultFromPayload(HttpResponseObject payload)
        {
            var responseObject = JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
            {
                updated_balance_timestamp = string.Empty,
                updated_balance = Array.Empty<Currency>(),
            });

            var dateTimeWasParsed = DateTime.TryParse(responseObject.updated_balance_timestamp, out var timestamp);
            if (!dateTimeWasParsed)
            {
                throw new FormatException("Invalid datetime format");
            }
            
            var result = new SendVirtualGiftResult
            {
                updatedBalanceTimestamp = timestamp,
                newWalletBalance = responseObject.updated_balance,
            };

            return result;
        }
    }

    internal class GetVirtualGiftCostsRequester : Requester<GetVirtualGiftCostsRequest, GetVirtualGiftCostsResult>
    {
        private readonly GetAllGiftItemsResultConverter getAllGiftItemsResultConverter = new();

        public GetVirtualGiftCostsRequester(GetVirtualGiftCostsRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Get(BackendUtil.GetFullUrl("/api/v3/gift"));
        }

        public override GetVirtualGiftCostsResult FormResultFromPayload(HttpResponseObject payload)
        {
            return JsonConvert.DeserializeObject<GetVirtualGiftCostsResult>(payload.data.ToString(), getAllGiftItemsResultConverter);
        }
    }
}
