using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Gifting
{
    public class VGPlayerSelected : Event<string>
    {
        public const string EVENT_NAME = "VGPlayerSelected";
        
        public VGPlayerSelected(VirtualGiftGifteeData[] gifteesData) : base(EVENT_NAME)
        {
            VGGifteeData = gifteesData;
        }
        
        public VirtualGiftGifteeData[] VGGifteeData { get; }
    }
}
