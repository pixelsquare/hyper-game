using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Gifting
{
    public class VGSelectedEvent :  Event<string>
    {
        public const string EVENT_NAME = "VGSelectedEvent";
        
        public VGSelectedEvent(VirtualGiftGiftsData[] vgSendData) : base(EVENT_NAME)
        {
            VgSendData = vgSendData;
        }
        
        public VirtualGiftGiftsData[] VgSendData { get; }
    }
}
