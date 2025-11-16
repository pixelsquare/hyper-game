using System;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftEventInfo : IVirtualGiftShareComputable
    {
        public VirtualGiftData.VGType category;
        public string[] giftees;
        public VirtualGiftGiftsData[] gifts;
        public string gifter;

        public int GetTotalValue()
        {
            var result = 0;

            foreach (var g in gifts)
            {
                var giftConfigExists = VirtualGiftDatabase.Current.TryGetGift(g.id, out var giftConfig);

                if (!giftConfigExists)
                {
                    throw new Exception($"Gift id does not exist: {g.id}");
                }

                // Assumption is that virtual gifts are always received as diamonds 
                result += giftConfig.Data.cost.amount * g.quantity;
            }

            return result;
        }

        public int ComputeOwnShare()
        {
            var totalValue = GetTotalValue();

            if (category == VirtualGiftData.VGType.Individual)
            {
                return totalValue;
            }

            if (category == VirtualGiftData.VGType.Shared)
            {
                return totalValue / giftees.Length;
            }

            return -1;
        }
    }
}
