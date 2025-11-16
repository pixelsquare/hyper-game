using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Gifting
{
    public class VGComboEndEvent : Event<string>
    {
        public const string EVENT_NAME = "VGComboEndEvent";

        public VGComboEndEvent(string id) : base(EVENT_NAME)
        {
            Id = id;
        }
        
        public string Id { get; }
    }
}
