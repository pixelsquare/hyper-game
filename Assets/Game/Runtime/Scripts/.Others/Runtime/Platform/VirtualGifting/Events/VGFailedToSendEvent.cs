using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Gifting
{
    public class VGFailedToSendEvent : Event<string>
    {
        public const string EVENT_NAME = "VGFailedToSendEvent";

        public VGFailedToSendEvent() : base(EVENT_NAME)
        {
        }
    }
}
