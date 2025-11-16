namespace Kumu.Kulitan.Backend
{
    public class BuyCurrencyRequest : RequestCommon
    {
        public string id;
    }
    
    public class ValidateReceiptRequest : RequestCommon
    {
        public string id;
        public string receipt;
    }

    public class GetCurrencyShopProductsRequest : RequestCommon
    {
    }
}
