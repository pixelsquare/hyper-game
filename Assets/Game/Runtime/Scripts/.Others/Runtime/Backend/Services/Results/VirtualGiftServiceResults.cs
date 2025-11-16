using System;

namespace Kumu.Kulitan.Backend
{
    public class GetVirtualGiftCostsResult : ResultBase
    {
        public VirtualGiftCostData[] itemCosts;
    }
    
    [Serializable]
    public struct VirtualGiftCostData
    {
        public string id;
        public Currency cost;
        public int markUpDownCost;
    }
    
    public class SendVirtualGiftResult : ResultBase
    {
        public DateTime updatedBalanceTimestamp;
        public Currency[] newWalletBalance;
    }
}
