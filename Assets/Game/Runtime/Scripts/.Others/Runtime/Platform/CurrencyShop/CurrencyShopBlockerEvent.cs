using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.UI
{
    public class CurrencyShopBlockerEvent : Event<string>
    { 
        public const string EVENT_NAME = "CurrencyShopBlockerEvent";

        private bool isBlocked;
        
        public CurrencyShopBlockerEvent(bool isBlocked) : base(EVENT_NAME)
        {
            this.isBlocked = isBlocked;
        }

        public bool IsBlocked => isBlocked;
    }
}
