using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Avatar
{
    public class UserWalletUpdatedEvent : Event<string>
    { 
        public const string EVENT_NAME = "UserWalletUpdatedEvent";

        public UserWalletUpdatedEvent(Currency[] currencies) : base(EVENT_NAME)
        {
            Currencies = currencies;
        }
        
        public Currency[] Currencies { get; }
    }
}
