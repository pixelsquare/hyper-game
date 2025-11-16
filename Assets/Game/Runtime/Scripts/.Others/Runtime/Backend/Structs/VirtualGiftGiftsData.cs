using System;

namespace Kumu.Kulitan.Gifting
{
    [Serializable]
    public struct VirtualGiftGiftsData : IVirtualGiftIdQuantified
    {
        public string id;
        public int quantity;
        
        public string GetId()
        {
            return id;
        }

        public int GetQuantity()
        {
            return quantity;
        }
    }
}
