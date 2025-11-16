using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Common
{
    public class AccountBannedEvent : Event<string>
    {
        public const string EVENT_NAME = "BannedAccountEvent";

        public AccountBannedEvent(BanType banType, BannedObject bannedObject) : base(EVENT_NAME)
        {
            BanType = banType;
            BannedObject = bannedObject;
        }

        public BanType BanType { get; }

        public BannedObject BannedObject { get; }
    }
}
