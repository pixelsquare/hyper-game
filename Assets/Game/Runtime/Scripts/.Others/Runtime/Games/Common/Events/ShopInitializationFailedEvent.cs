using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Common
{
    public class ShopInitializationFailedEvent : Event<string>
    {
        public const string EVENT_NAME = "ShopInitializationFailedEvent";
        
        public ShopInitializationFailedEvent() : base(EVENT_NAME)
        {
        }
    }
}
