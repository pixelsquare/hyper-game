using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Gifting
{
    public class OnVirtualGiftPlayEffectEvent: Event<string>
    {
        public const string EVENT_NAME = "OnVirtualGiftPlayEffect";

        public OnVirtualGiftPlayEffectEvent(VgEffectData vgEffectData) : base(EVENT_NAME)
        {

            VgEffectData = vgEffectData;
        }
        
        public VgEffectData VgEffectData { get; }
    }
}
