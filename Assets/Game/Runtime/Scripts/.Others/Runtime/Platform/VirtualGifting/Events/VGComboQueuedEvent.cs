using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Gifting
{
    public class VGComboQueuedEvent : Event<string>
    {
        public const string EVENT_NAME = "VGComboQueuedEvent";

        public VGComboQueuedEvent(string id) : base(EVENT_NAME)
        {
            Id = id;
        }
        
        public string Id { get; }
    }
}
