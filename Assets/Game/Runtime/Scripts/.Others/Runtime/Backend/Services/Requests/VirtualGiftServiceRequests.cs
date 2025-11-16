using System;
using Kumu.Kulitan.Gifting;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class GetVirtualGiftCostsRequest : RequestCommon
    {
    }

    [Serializable]
    public class SendVirtualGiftRequest : RequestCommon
    {
        public VirtualGiftData.VGType category; // TODO (cj): convert to string 
        public string[] giftees;
        public VirtualGiftGiftsData[] gifts;
        public string[] spectators; // account ids of users in the room, but aren't a gifter or giftee
    }
}
