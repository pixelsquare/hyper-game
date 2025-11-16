using System;

namespace Kumu.Kulitan.Backend
{
    public class BuyCurrencyResult : ResultBase
    {
        public Currency[] walletBalance;
    }
    
    public class ValidateReceiptResult : ResultBase
    {
        public Currency[] walletBalance;
    }

    public class GetCurrencyShopProductsResult : ResultBase
    {
        public CurrencyShopProductCostData[] items;
    }

    [Serializable]
    public class CurrencyShopProductCostData
    {
        public string id;
        public Currency cost;
    }
}
