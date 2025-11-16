using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Gifting
{
    public class VGTypeSelectedEvent : Event<string>
    {
        public const string EVENT_NAME = "VGTypeSelectedEvent";

        public VGTypeSelectedEvent(VirtualGiftData.VGType type) : base(EVENT_NAME)
        {
            Type = type;
        }
        
        public VirtualGiftData.VGType Type { get; }
    }
}
