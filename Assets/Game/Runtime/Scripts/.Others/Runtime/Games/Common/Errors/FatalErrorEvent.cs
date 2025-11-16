using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Common
{
    public class FatalErrorEvent : Event<string>
    {
        public const string EVENT_NAME = "FatalErrorEvent";
        
        public FatalErrorEvent() : base(EVENT_NAME)
        {
        }
    }
}
