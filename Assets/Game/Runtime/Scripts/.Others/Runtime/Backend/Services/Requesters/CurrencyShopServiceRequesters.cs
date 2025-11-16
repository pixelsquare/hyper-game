using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    internal class ValidateReceiptRequester : Requester<ValidateReceiptRequest, ValidateReceiptResult>
    {
        public ValidateReceiptRequester(ValidateReceiptRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var reqConverter = new GenericServiceConverter<ValidateReceiptRequest>(BackendUtil.GlobalMapping);
            return BackendUtil.Post(BackendUtil.GetFullUrl("/api/v3/in-app-purchase/verify"), JsonConvert.SerializeObject(RequestObject, reqConverter));
        }

        public override ValidateReceiptResult FormResultFromPayload(HttpResponseObject payload)
        {
            var data = JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
            {
                wallet_balance = Array.Empty<Currency>()
            });

            var result = new ValidateReceiptResult
            {
                walletBalance = data.wallet_balance
            };

            return result;
        }
    }

    internal class BuyCurrencyRequester : Requester<BuyCurrencyRequest, BuyCurrencyResult>
    {
        public BuyCurrencyRequester(BuyCurrencyRequest requestObject) : base(requestObject)
        {
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            var reqConverter = new GenericServiceConverter<BuyCurrencyRequest>(BackendUtil.GlobalMapping);
            return BackendUtil.Post(BackendUtil.GetFullUrl("/api/v3/store/buy"), JsonConvert.SerializeObject(RequestObject, reqConverter));
        }

        public override BuyCurrencyResult FormResultFromPayload(HttpResponseObject payload)
        {
            var data = JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
            {
                wallet_balance = Array.Empty<Currency>()
            });

            var result = new BuyCurrencyResult
            {
                walletBalance = data.wallet_balance
            };

            return result;
        }
    }
    
    internal class GetCurrencyShopProductsRequester : Requester<GetCurrencyShopProductsRequest, GetCurrencyShopProductsResult>
    {
        private CurrencyShopProductCostData[] cache = null;
        
        public GetCurrencyShopProductsRequester(GetCurrencyShopProductsRequest requestObject) : base(requestObject)
        {
        }

        public override bool UseCachedResult(out GetCurrencyShopProductsResult result)
        {
            result = null;
            
            if (cache == null)
            {
                return false;
            }

            result = new GetCurrencyShopProductsResult
            {
                items = cache
            };
            
            return true;
        }

        public override UnityWebRequest FormUnityWebRequest()
        {
            return BackendUtil.Get(BackendUtil.GetFullUrl("/api/v3/store"));
        }

        public override GetCurrencyShopProductsResult FormResultFromPayload(HttpResponseObject payload)
        {
            var data = JsonConvert.DeserializeAnonymousType(payload.data.ToString(), new
            {
                items = Array.Empty<CurrencyShopProductCostData>()
            });

            var result = new GetCurrencyShopProductsResult
            {
                items = data.items
            };

            // Cache result
            cache = data.items;

            return result;
        }
    }
}
