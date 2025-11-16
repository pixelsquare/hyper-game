using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Common
{
    public class TokenExpiredErrorEvent : Event<string>
    {
        public const string EVENT_NAME = "TokenExpiredErrorEvent";
        
        public TokenExpiredErrorEvent() : base(EVENT_NAME)
        {
        }
    }
}
